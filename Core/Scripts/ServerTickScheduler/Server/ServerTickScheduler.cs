using System.Collections.Generic;
using System.Diagnostics;
using MultiplayerARPG.Server.Time;

namespace MultiplayerARPG.Server.Scheduling
{
    public enum TickExecutionMode
    {
        SingleThreaded,
        ThreadPrepared
    }

    public sealed class ServerTickSettings
    {
        public TickExecutionMode ExecutionMode = TickExecutionMode.SingleThreaded;

        // max ticks a channel can run per frame
        public int MaxTicksPerFrame = 5;

        // Per-channel policies
        public readonly List<TickChannelConfig> Channels = new();
    }

    public sealed class TickChannelConfig
    {
        public string Name;

        // Higher priority runs first
        public int Priority;

        // Normal tick rate
        public double BaseHz;

        // Lowest allowed tick rate under throttling
        public double MinHz;

        // Time budget per frame (ms)
        public double MaxMsPerFrame;

        // Optional stability caps (per system per tick).
        // Systems may opt-in by implementing IIncrementalTickSystem.
        // - MaxItemsPerTick: maximum items a system should process per tick.
        // - MaxSystemMsPerTick: maximum milliseconds a system should spend per tick.
        public int MaxItemsPerTick = int.MaxValue;
        public double MaxSystemMsPerTick = double.MaxValue;

        // Phase offset in seconds (0..interval). Example: 0.025 for 25ms.
        public double PhaseOffsetSeconds;

        // Can this channel slow down under load?
        public bool AllowThrottling;
    }

    public readonly struct TickContext
    {
        public readonly long TickIndex;     // monotonically increasing per-channel
        public readonly double Now;         // absolute time for this tick
        public readonly float FixedDelta;   // dt for this tick (usually 1/Hz)

        public TickContext(long tickIndex, double now, float fixedDelta)
        {
            TickIndex = tickIndex;
            Now = now;
            FixedDelta = fixedDelta;
        }
    }

    public interface ITickSystem
    {
        string Name { get; }

        void Prepare(in TickContext ctx);
        void Execute(in TickContext ctx);
        void Commit(in TickContext ctx);
    }

    /// <summary>
    /// Optional opt-in interface for systems that can time-slice their work to improve stability at high load.
    /// Implementors should do bounded work and return quickly.
    /// </summary>
    public interface IIncrementalTickSystem : ITickSystem
    {
        void ExecuteSlice(in TickContext ctx, int maxItems, double maxMs);
    }

    internal sealed class TickChannel
    {
        public readonly string Name;

        private readonly int maxTicksPerFrame;
        private readonly double baseInterval;
        private readonly double maxBacklogSeconds;

        private double currentInterval;
        private double accumulator;

        // Staggers channel start so multiple channels don't align on the same frames.
        private double phaseOffsetRemaining;

        public long TickIndex { get; private set; }
        public double CurrentHz => 1.0 / currentInterval;
        public float FixedDelta => (float)currentInterval;

        public TickChannel(string name, double baseHz, int maxTicksPerFrame, double phaseOffsetSeconds, double maxBacklogSeconds = 0.25)
        {
            Name = name;
            this.maxTicksPerFrame = maxTicksPerFrame;
            this.maxBacklogSeconds = maxBacklogSeconds;

            baseInterval = 1.0 / baseHz;
            currentInterval = baseInterval;
            accumulator = 0;
            TickIndex = 0;

            phaseOffsetRemaining = phaseOffsetSeconds > 0 ? phaseOffsetSeconds : 0;
        }

        public void SetHz(double hz)
        {
            if (hz <= 0)
                return;
            currentInterval = 1.0 / hz;
        }

        public void RestoreBaseHz() => currentInterval = baseInterval;

        public void AddTime(double deltaTime)
        {
            // Consume phase offset first (delays first tick firing)
            if (phaseOffsetRemaining > 0)
            {
                double consume = System.Math.Min(phaseOffsetRemaining, deltaTime);
                phaseOffsetRemaining -= consume;
                deltaTime -= consume;
                if (deltaTime <= 0)
                    return;
            }

            accumulator += deltaTime;

            // IMPORTANT:
            // If the backlog cap is lower than the current tick interval, low-Hz channels can
            // never reach accumulator/currentInterval >= 1, meaning they will never produce a tick.
            // Example: 1 Hz channel => interval=1s. If maxBacklogSeconds=0.25s, ticks=(int)(0.25/1)=0 forever.
            // Ensure the cap is at least one interval (or more) so low-frequency channels can execute.
            double cap = System.Math.Max(maxBacklogSeconds, currentInterval);
            if (accumulator > cap)
                accumulator = cap;
        }

        public int PeekTicks()
        {
            int ticks = (int)(accumulator / currentInterval);
            if (ticks > maxTicksPerFrame) ticks = maxTicksPerFrame;
            return ticks;
        }

        public double GetTickNow(double serverNow, int tickOffset)
        {
            // Oldest simulated time represented by the accumulator.
            // Each tick advances by currentInterval.
            double oldestNow = serverNow - accumulator;
            return oldestNow + currentInterval * (tickOffset + 1);
        }

        public void CommitOneTick()
        {
            accumulator -= currentInterval;
            if (accumulator < 0)
                accumulator = 0;
            TickIndex++;
        }

        public void CommitTicks(int executedTicks)
        {
            for (int i = 0; i < executedTicks; i++)
                CommitOneTick();
        }
    }

    internal sealed class ScheduledSystem
    {
        public readonly TickChannel Channel;
        public readonly TickChannelConfig Config;
        public readonly List<ITickSystem> Systems = new();

        // Runtime metrics
        public double LastFrameMs;
        public int ExecutedTicks;

        // Throttling state
        public int ConsecutiveBudgetHits;
        public int ConsecutiveHealthyFrames;

        private readonly Stopwatch stopwatch = new Stopwatch();

        // Budget warning throttling (avoid spamming logs which also costs CPU)
        private double _lastBudgetWarnTime;

        public ScheduledSystem(TickChannel channel, TickChannelConfig config)
        {
            Channel = channel;
            Config = config;
        }

        public void BeginTiming()
        {
            ExecutedTicks = 0;
            stopwatch.Restart();
        }

        public bool CanExecuteMore()
        {
            return stopwatch.Elapsed.TotalMilliseconds < Config.MaxMsPerFrame;
        }

        public double RemainingMs
        {
            get
            {
                if (double.IsPositiveInfinity(Config.MaxMsPerFrame) || Config.MaxMsPerFrame == double.MaxValue)
                    return double.MaxValue;
                double remain = Config.MaxMsPerFrame - stopwatch.Elapsed.TotalMilliseconds;
                return remain > 0 ? remain : 0;
            }
        }

        public bool ShouldWarnBudget(double serverNow, double minIntervalSeconds = 1.0)
        {
            if (serverNow - _lastBudgetWarnTime < minIntervalSeconds)
                return false;
            _lastBudgetWarnTime = serverNow;
            return true;
        }

        public void RecordTick()
        {
            ExecutedTicks++;
        }

        public void EndTiming()
        {
            stopwatch.Stop();
            LastFrameMs = stopwatch.Elapsed.TotalMilliseconds;
        }
    }

    internal interface ITickExecutionStrategy
    {
        void ExecuteTick(ScheduledSystem entry, in TickContext ctx);
    }

    internal sealed class SingleThreadedExecution : ITickExecutionStrategy
    {
        public void ExecuteTick(ScheduledSystem entry, in TickContext ctx)
        {
            // Allow early-out when budget is exhausted.
            int preparedCount = 0;
            for (int i = 0; i < entry.Systems.Count; ++i)
            {
                if (!entry.CanExecuteMore())
                    break;
                var system = entry.Systems[i];
                try { system.Prepare(ctx); }
                catch (System.Exception ex) { Handle(system, entry, ex); }
                preparedCount++;
            }

            int executedCount = 0;
            for (int i = 0; i < preparedCount; ++i)
            {
                if (!entry.CanExecuteMore())
                    break;

                var system = entry.Systems[i];
                try
                {
                    // Opt-in time-slicing path.
                    if (system is IIncrementalTickSystem inc)
                    {
                        double sysBudget = entry.RemainingMs;
                        if (sysBudget > entry.Config.MaxSystemMsPerTick)
                            sysBudget = entry.Config.MaxSystemMsPerTick;
                        inc.ExecuteSlice(ctx, entry.Config.MaxItemsPerTick, sysBudget);
                    }
                    else
                    {
                        system.Execute(ctx);
                    }
                }
                catch (System.Exception ex) { Handle(system, entry, ex); }

                executedCount++;
            }

            for (int i = 0; i < executedCount; ++i)
            {
                var system = entry.Systems[i];
                try { system.Commit(ctx); }
                catch (System.Exception ex) { Handle(system, entry, ex); }
            }
        }

        private static void Handle(ITickSystem system, ScheduledSystem entry, System.Exception ex)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            UnityEngine.Debug.LogError($"[Scheduler] System '{system.Name}' in channel '{entry.Config.Name}' failed:\n{ex}");
#else
            UnityEngine.Debug.LogError($"[Scheduler] System '{system.Name}' failed.");
#endif
        }
    }

    // ServerTickScheduler
    public sealed class ServerTickScheduler
    {
        private readonly List<ScheduledSystem> scheduled = new();
        private readonly ITickExecutionStrategy executionStrategy;
        private readonly ServerTickSettings settings;

        public ServerTickScheduler(ServerTickSettings settings)
        {
            this.settings = settings;
            executionStrategy = settings.ExecutionMode == TickExecutionMode.ThreadPrepared ? new SingleThreadedExecution() : new SingleThreadedExecution();
        }

        // Register a system into a named tick channel
        public void RegisterSystem(ITickSystem system, string channelName, double tickRate)
        {
            var entry = scheduled.Find(s => s.Channel.Name == channelName);

            if (entry == null)
            {
                var config = settings.Channels.Find(c => c.Name == channelName)
                             ?? new TickChannelConfig
                             {
                                 Name = channelName,
                                 Priority = 0,
                                 BaseHz = tickRate,
                                 MinHz = tickRate,
                                 MaxMsPerFrame = double.MaxValue,
                                 PhaseOffsetSeconds = 0,
                                 AllowThrottling = false
                             };

                // Channel Hz is owned by the channel config; tickRate is only a legacy fallback.
                double channelHz = config.BaseHz > 0 ? config.BaseHz : tickRate;
                entry = new ScheduledSystem(
                    new TickChannel(channelName, channelHz, settings.MaxTicksPerFrame, config.PhaseOffsetSeconds),
                    config);

                scheduled.Add(entry);
                // Ensure higher priority channels run first
                scheduled.Sort((a, b) => b.Config.Priority.CompareTo(a.Config.Priority));
            }

            entry.Systems.Add(system);
        }

        public void Update()
        {
            ServerClock.Update();

            double dt = ServerClock.DeltaTime;
            double serverNow = ServerClock.Time;

            foreach (var entry in scheduled)
            {
                entry.Channel.AddTime(dt);

                int ticksAvailable = entry.Channel.PeekTicks();
                if (ticksAvailable <= 0)
                    continue;

                entry.BeginTiming();

                for (int i = 0; i < ticksAvailable; i++)
                {
                    if (!entry.CanExecuteMore())
                        break;

                    double tickNow = entry.Channel.GetTickNow(serverNow, i);
                    var ctx = new TickContext(entry.Channel.TickIndex + i, tickNow, entry.Channel.FixedDelta);
                    executionStrategy.ExecuteTick(entry, ctx);
                    entry.RecordTick();
                }

                // Only consume backlog for ticks we actually executed.
                entry.Channel.CommitTicks(entry.ExecutedTicks);

                entry.EndTiming();

#if UNITY_EDITOR || DEVELOPMENT_BUILD
                // Log when a channel exceeds its budget (throttled)
                if (entry.LastFrameMs >= entry.Config.MaxMsPerFrame && entry.ShouldWarnBudget(serverNow))
                {
                    UnityEngine.Debug.LogWarning($"[Scheduler] Channel {entry.Config.Name} hit budget " + $"({entry.LastFrameMs:F2} ms, {entry.ExecutedTicks} ticks)");
                }
#endif

                // Dynamic Tick Throttling
                if (entry.Config.AllowThrottling)
                {
                    if (entry.LastFrameMs >= entry.Config.MaxMsPerFrame)
                    {
                        entry.ConsecutiveBudgetHits++;
                        entry.ConsecutiveHealthyFrames = 0;
                    }
                    else
                    {
                        entry.ConsecutiveHealthyFrames++;
                        entry.ConsecutiveBudgetHits = 0;
                    }

                    // Reduce tick rate after sustained pressure
                    if (entry.ConsecutiveBudgetHits >= 3)
                    {
                        double newHz = System.Math.Max(entry.Config.MinHz, entry.Channel.CurrentHz * 0.5);
                        entry.Channel.SetHz(newHz);
                        entry.ConsecutiveBudgetHits = 0;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
                        UnityEngine.Debug.LogWarning($"[Scheduler] Throttling channel {entry.Config.Name} to {newHz:F2} Hz");
#endif
                    }

                    // Restore tick rate after recovery
                    if (entry.ConsecutiveHealthyFrames >= 5)
                    {
                        entry.Channel.RestoreBaseHz();
                        entry.ConsecutiveHealthyFrames = 0;
                    }
                }
            }
        }
    }
}
