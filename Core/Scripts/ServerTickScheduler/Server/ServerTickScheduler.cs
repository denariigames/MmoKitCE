//Updated
using System;
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

    public enum SchedulerPressureLevel
    {
        Normal = 0,
        Elevated = 1,
        Critical = 2
    }

    public sealed class ServerTickSettings
    {
        public TickExecutionMode ExecutionMode = TickExecutionMode.SingleThreaded;
        public int MaxTicksPerFrame = 5;
        public double MaxBacklogSeconds = 0.25d;
        public bool LogBudgetWarnings = true;
        public bool LogExceptionDetails = true;
        public bool ThrowOnInvalidConfiguration = true;
        public readonly List<TickChannelConfig> Channels = new List<TickChannelConfig>();

        public void Validate()
        {
            if (MaxTicksPerFrame <= 0)
                Fail("MaxTicksPerFrame must be > 0.");
            if (MaxBacklogSeconds <= 0d)
                Fail("MaxBacklogSeconds must be > 0.");

            HashSet<string> names = new HashSet<string>();
            for (int i = 0; i < Channels.Count; i++)
            {
                TickChannelConfig c = Channels[i];
                if (c == null)
                    Fail("Channels contains a null config at index " + i + ".");
                if (string.IsNullOrEmpty(c.Name))
                    Fail("Channel config at index " + i + " has no Name.");
                if (!names.Add(c.Name))
                    Fail("Duplicate channel config: " + c.Name);
                c.Validate(ThrowOnInvalidConfiguration);
            }
        }

        private void Fail(string message)
        {
            if (ThrowOnInvalidConfiguration)
                throw new ArgumentException("[ServerTickScheduler] " + message);
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            UnityEngine.Debug.LogError("[ServerTickScheduler] " + message);
#endif
        }
    }

    public sealed class TickChannelConfig
    {
        public string Name;
        public int Priority;
        public double BaseHz;
        public double MinHz;
        public double MaxMsPerFrame;
        public double PhaseOffsetSeconds;
        public bool AllowThrottling;
        public int MaxTicksPerFrameOverride;
        public int BudgetHitThreshold = 3;
        public int HealthyFrameRestoreThreshold = 5;
        public double EmergencyDropBacklogAboveSeconds = 1.0d;

        public void Validate(bool throwOnInvalid)
        {
            if (BaseHz <= 0d)
                Fail("Channel '" + Name + "' BaseHz must be > 0.", throwOnInvalid);
            if (MinHz <= 0d)
                Fail("Channel '" + Name + "' MinHz must be > 0.", throwOnInvalid);
            if (MinHz > BaseHz)
                Fail("Channel '" + Name + "' MinHz cannot be greater than BaseHz.", throwOnInvalid);
            if (MaxMsPerFrame <= 0d)
                Fail("Channel '" + Name + "' MaxMsPerFrame must be > 0.", throwOnInvalid);
            if (PhaseOffsetSeconds < 0d)
                Fail("Channel '" + Name + "' PhaseOffsetSeconds cannot be negative.", throwOnInvalid);
            if (MaxTicksPerFrameOverride < 0)
                Fail("Channel '" + Name + "' MaxTicksPerFrameOverride cannot be negative.", throwOnInvalid);
            if (BudgetHitThreshold <= 0)
                BudgetHitThreshold = 1;
            if (HealthyFrameRestoreThreshold <= 0)
                HealthyFrameRestoreThreshold = 1;
        }

        private static void Fail(string message, bool throwOnInvalid)
        {
            if (throwOnInvalid)
                throw new ArgumentException("[ServerTickScheduler] " + message);
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            UnityEngine.Debug.LogError("[ServerTickScheduler] " + message);
#endif
        }
    }

    public readonly struct TickContext
    {
        public readonly long TickIndex;
        public readonly double Now;
        public readonly float FixedDelta;
        public readonly string ChannelName;
        public readonly SchedulerPressureLevel PressureLevel;

        public TickContext(long tickIndex, double now, float fixedDelta)
            : this(tickIndex, now, fixedDelta, null, SchedulerPressureLevel.Normal)
        {
        }

        public TickContext(long tickIndex, double now, float fixedDelta, string channelName, SchedulerPressureLevel pressureLevel)
        {
            TickIndex = tickIndex;
            Now = now;
            FixedDelta = fixedDelta;
            ChannelName = channelName;
            PressureLevel = pressureLevel;
        }
    }

    public interface ITickSystem
    {
        string Name { get; }
        void Prepare(in TickContext ctx);
        void Execute(in TickContext ctx);
        void Commit(in TickContext ctx);
    }

    public interface IOrderedTickSystem
    {
        int Order { get; }
    }

    public interface ILoadSheddingTickSystem
    {
        bool ShouldRun(in TickContext ctx, SchedulerPressureLevel pressureLevel);
    }

    public sealed class TickSystemMetrics
    {
        public string ChannelName;
        public string SystemName;
        public int Order;
        public long PrepareCount;
        public long ExecuteCount;
        public long CommitCount;
        public long SkippedByLoadShedding;
        public long PrepareExceptionCount;
        public long ExecuteExceptionCount;
        public long CommitExceptionCount;
        public double LastPrepareMs;
        public double LastExecuteMs;
        public double LastCommitMs;
        public double LastTotalMs;
        public double MaxTotalMs;
        public double AverageTotalMs;
        public long LastTickIndex;
    }

    public sealed class TickChannelMetrics
    {
        public string Name;
        public int Priority;
        public double CurrentHz;
        public double BaseHz;
        public double MinHz;
        public double LastFrameMs;
        public double MaxFrameMs;
        public double AccumulatorSeconds;
        public int ExecutedTicksLastFrame;
        public int PendingTicksLastFrame;
        public long TotalExecutedTicks;
        public long BudgetHitCount;
        public long ThrottleCount;
        public bool IsThrottled;
        public SchedulerPressureLevel PressureLevel;
    }

    public sealed class SchedulerSnapshot
    {
        public double ServerTime;
        public double RealTime;
        public double DeltaTime;
        public double RawDeltaTime;
        public bool IsRunning;
        public readonly List<TickChannelMetrics> Channels = new List<TickChannelMetrics>();
        public readonly List<TickSystemMetrics> Systems = new List<TickSystemMetrics>();
    }

    internal sealed class TickChannel
    {
        public readonly string Name;
        private readonly int maxTicksPerFrame;
        private readonly double baseInterval;
        private readonly double maxBacklogSeconds;
        private double currentInterval;
        private double accumulator;
        private double phaseOffsetRemaining;

        public long TickIndex { get; private set; }
        public double CurrentHz { get { return 1.0d / currentInterval; } }
        public double BaseHz { get { return 1.0d / baseInterval; } }
        public float FixedDelta { get { return (float)currentInterval; } }
        public double AccumulatorSeconds { get { return accumulator; } }
        public bool IsThrottled { get { return Math.Abs(currentInterval - baseInterval) > 0.0000001d; } }

        public TickChannel(string name, double baseHz, int maxTicksPerFrame, double phaseOffsetSeconds, double maxBacklogSeconds)
        {
            Name = name;
            this.maxTicksPerFrame = maxTicksPerFrame;
            this.maxBacklogSeconds = maxBacklogSeconds;
            baseInterval = 1.0d / baseHz;
            currentInterval = baseInterval;
            accumulator = 0d;
            TickIndex = 0L;
            phaseOffsetRemaining = phaseOffsetSeconds > 0d ? phaseOffsetSeconds : 0d;
        }

        public void SetHz(double hz)
        {
            if (hz <= 0d)
                return;
            currentInterval = 1.0d / hz;
        }

        public void RestoreBaseHz()
        {
            currentInterval = baseInterval;
        }

        public void AddTime(double deltaTime)
        {
            if (deltaTime <= 0d)
                return;

            if (phaseOffsetRemaining > 0d)
            {
                double consume = Math.Min(phaseOffsetRemaining, deltaTime);
                phaseOffsetRemaining -= consume;
                deltaTime -= consume;
                if (deltaTime <= 0d)
                    return;
            }

            accumulator += deltaTime;
            if (accumulator > maxBacklogSeconds)
                accumulator = maxBacklogSeconds;
        }

        public int PeekTicks()
        {
            int ticks = (int)(accumulator / currentInterval);
            if (ticks > maxTicksPerFrame)
                ticks = maxTicksPerFrame;
            return ticks;
        }

        public double GetTickNow(double serverNow, int tickOffset)
        {
            double oldestNow = serverNow - accumulator;
            return oldestNow + currentInterval * (tickOffset + 1);
        }

        public void CommitTicks(int executedTicks)
        {
            for (int i = 0; i < executedTicks; i++)
            {
                accumulator -= currentInterval;
                if (accumulator < 0d)
                    accumulator = 0d;
                TickIndex++;
            }
        }

        public void DropBacklogToSingleTick()
        {
            if (accumulator > currentInterval)
                accumulator = currentInterval;
        }
    }

    internal sealed class ScheduledTickSystem
    {
        public readonly ITickSystem System;
        public readonly int Order;
        public readonly long RegistrationIndex;
        public readonly TickSystemMetrics Metrics = new TickSystemMetrics();

        private readonly Stopwatch phaseWatch = new Stopwatch();

        public ScheduledTickSystem(ITickSystem system, int order, long registrationIndex, string channelName)
        {
            System = system;
            Order = order;
            RegistrationIndex = registrationIndex;
            Metrics.ChannelName = channelName;
            Metrics.SystemName = system != null ? system.Name : "<null>";
            Metrics.Order = order;
        }

        public double TimePrepare(in TickContext ctx)
        {
            phaseWatch.Restart();
            System.Prepare(ctx);
            phaseWatch.Stop();
            Metrics.PrepareCount++;
            Metrics.LastPrepareMs = phaseWatch.Elapsed.TotalMilliseconds;
            return Metrics.LastPrepareMs;
        }

        public double TimeExecute(in TickContext ctx)
        {
            phaseWatch.Restart();
            System.Execute(ctx);
            phaseWatch.Stop();
            Metrics.ExecuteCount++;
            Metrics.LastExecuteMs = phaseWatch.Elapsed.TotalMilliseconds;
            return Metrics.LastExecuteMs;
        }

        public double TimeCommit(in TickContext ctx)
        {
            phaseWatch.Restart();
            System.Commit(ctx);
            phaseWatch.Stop();
            Metrics.CommitCount++;
            Metrics.LastCommitMs = phaseWatch.Elapsed.TotalMilliseconds;
            return Metrics.LastCommitMs;
        }

        public void CompleteTick(long tickIndex)
        {
            Metrics.LastTickIndex = tickIndex;
            Metrics.LastTotalMs = Metrics.LastPrepareMs + Metrics.LastExecuteMs + Metrics.LastCommitMs;
            if (Metrics.LastTotalMs > Metrics.MaxTotalMs)
                Metrics.MaxTotalMs = Metrics.LastTotalMs;

            long samples = Metrics.CommitCount > 0L ? Metrics.CommitCount : Metrics.ExecuteCount;
            if (samples <= 1L)
                Metrics.AverageTotalMs = Metrics.LastTotalMs;
            else
                Metrics.AverageTotalMs += (Metrics.LastTotalMs - Metrics.AverageTotalMs) / samples;
        }
    }

    internal sealed class ScheduledChannel
    {
        public readonly TickChannel Channel;
        public readonly TickChannelConfig Config;
        public readonly List<ScheduledTickSystem> Systems = new List<ScheduledTickSystem>();
        public readonly TickChannelMetrics Metrics = new TickChannelMetrics();
        public readonly List<ScheduledTickSystem> PreparedThisTick = new List<ScheduledTickSystem>(32);
        public readonly List<ScheduledTickSystem> ExecutedThisTick = new List<ScheduledTickSystem>(32);
        public int ConsecutiveBudgetHits;
        public int ConsecutiveHealthyFrames;
        private readonly Stopwatch frameWatch = new Stopwatch();

        public ScheduledChannel(TickChannel channel, TickChannelConfig config)
        {
            Channel = channel;
            Config = config;
            Metrics.Name = config.Name;
            Metrics.Priority = config.Priority;
            Metrics.BaseHz = config.BaseHz;
            Metrics.MinHz = config.MinHz;
        }

        public void SortSystems()
        {
            Systems.Sort(delegate (ScheduledTickSystem a, ScheduledTickSystem b)
            {
                int orderCompare = a.Order.CompareTo(b.Order);
                if (orderCompare != 0)
                    return orderCompare;
                return a.RegistrationIndex.CompareTo(b.RegistrationIndex);
            });
        }

        public void BeginFrame(int pendingTicks)
        {
            Metrics.ExecutedTicksLastFrame = 0;
            Metrics.PendingTicksLastFrame = pendingTicks;
            frameWatch.Restart();
        }

        public bool CanExecuteMore()
        {
            return frameWatch.Elapsed.TotalMilliseconds < Config.MaxMsPerFrame;
        }

        public void RecordExecutedTick()
        {
            Metrics.ExecutedTicksLastFrame++;
            Metrics.TotalExecutedTicks++;
        }

        public void EndFrame()
        {
            frameWatch.Stop();
            Metrics.LastFrameMs = frameWatch.Elapsed.TotalMilliseconds;
            if (Metrics.LastFrameMs > Metrics.MaxFrameMs)
                Metrics.MaxFrameMs = Metrics.LastFrameMs;
            Metrics.CurrentHz = Channel.CurrentHz;
            Metrics.AccumulatorSeconds = Channel.AccumulatorSeconds;
            Metrics.IsThrottled = Channel.IsThrottled;
        }
    }

    public sealed class ServerTickScheduler
    {
        private readonly List<ScheduledChannel> scheduled = new List<ScheduledChannel>();
        private readonly Dictionary<string, ScheduledChannel> scheduledByName = new Dictionary<string, ScheduledChannel>();
        private readonly ServerTickSettings settings;
        private long registrationCounter;
        private bool shutdown;

        public bool IsRunning { get { return !shutdown; } }

        public ServerTickScheduler(ServerTickSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException("settings");

            this.settings = settings;
            this.settings.Validate();

            if (settings.ExecutionMode == TickExecutionMode.ThreadPrepared)
                throw new NotSupportedException("TickExecutionMode.ThreadPrepared is not implemented in the core scheduler yet. Use SingleThreaded until a real thread-safe prepare/commit pipeline exists.");
        }

        public void RegisterSystem(ITickSystem system, string channelName, double tickRate)
        {
            RegisterSystem(system, channelName, tickRate, ResolveOrder(system));
        }

        public void RegisterSystem(ITickSystem system, string channelName, double tickRate, int order)
        {
            if (shutdown)
                throw new ObjectDisposedException("ServerTickScheduler");
            if (system == null)
                throw new ArgumentNullException("system");
            if (string.IsNullOrEmpty(channelName))
                throw new ArgumentException("channelName is required.", "channelName");
            if (tickRate <= 0d)
                throw new ArgumentOutOfRangeException("tickRate", "tickRate must be > 0.");

            ScheduledChannel entry;
            if (!scheduledByName.TryGetValue(channelName, out entry))
            {
                TickChannelConfig config = settings.Channels.Find(c => c.Name == channelName);
                if (config == null)
                {
                    config = new TickChannelConfig
                    {
                        Name = channelName,
                        Priority = 0,
                        BaseHz = tickRate,
                        MinHz = tickRate,
                        MaxMsPerFrame = double.MaxValue,
                        PhaseOffsetSeconds = 0d,
                        AllowThrottling = false
                    };
                    config.Validate(settings.ThrowOnInvalidConfiguration);
                }

                int maxTicks = config.MaxTicksPerFrameOverride > 0 ? config.MaxTicksPerFrameOverride : settings.MaxTicksPerFrame;
                entry = new ScheduledChannel(
                    new TickChannel(channelName, config.BaseHz, maxTicks, config.PhaseOffsetSeconds, settings.MaxBacklogSeconds),
                    config);
                scheduled.Add(entry);
                scheduledByName[channelName] = entry;
                SortChannels();
            }

            entry.Systems.Add(new ScheduledTickSystem(system, order, registrationCounter++, channelName));
            entry.SortSystems();
        }

        public bool UnregisterSystem(ITickSystem system)
        {
            if (system == null)
                return false;

            bool removed = false;
            for (int i = 0; i < scheduled.Count; i++)
            {
                ScheduledChannel channel = scheduled[i];
                for (int s = channel.Systems.Count - 1; s >= 0; s--)
                {
                    if (object.ReferenceEquals(channel.Systems[s].System, system))
                    {
                        channel.Systems.RemoveAt(s);
                        removed = true;
                    }
                }
            }
            return removed;
        }

        public void Clear()
        {
            scheduled.Clear();
            scheduledByName.Clear();
            registrationCounter = 0L;
        }

        public void Shutdown()
        {
            if (shutdown)
                return;
            Clear();
            shutdown = true;
        }

        public void Update()
        {
            if (shutdown)
                return;

            ServerClock.Update();
            double dt = ServerClock.DeltaTime;
            double serverNow = ServerClock.Time;

            for (int e = 0; e < scheduled.Count; e++)
            {
                ScheduledChannel entry = scheduled[e];
                entry.Channel.AddTime(dt);

                int ticksAvailable = entry.Channel.PeekTicks();
                if (ticksAvailable <= 0)
                {
                    entry.Metrics.CurrentHz = entry.Channel.CurrentHz;
                    entry.Metrics.AccumulatorSeconds = entry.Channel.AccumulatorSeconds;
                    continue;
                }

                entry.BeginFrame(ticksAvailable);
                SchedulerPressureLevel pressure = ComputePressure(entry, ticksAvailable);
                entry.Metrics.PressureLevel = pressure;

                for (int i = 0; i < ticksAvailable; i++)
                {
                    if (!entry.CanExecuteMore())
                        break;

                    double tickNow = entry.Channel.GetTickNow(serverNow, i);
                    TickContext ctx = new TickContext(entry.Channel.TickIndex + i, tickNow, entry.Channel.FixedDelta, entry.Channel.Name, pressure);
                    ExecuteTick(entry, ctx, pressure);
                    entry.RecordExecutedTick();
                }

                entry.Channel.CommitTicks(entry.Metrics.ExecutedTicksLastFrame);
                entry.EndFrame();
                ApplyBudgetPolicy(entry);
            }
        }

        public SchedulerSnapshot CaptureSnapshot()
        {
            SchedulerSnapshot snapshot = new SchedulerSnapshot();
            snapshot.ServerTime = ServerClock.Time;
            snapshot.RealTime = ServerClock.RealTime;
            snapshot.DeltaTime = ServerClock.DeltaTime;
            snapshot.RawDeltaTime = ServerClock.RawDeltaTime;
            snapshot.IsRunning = IsRunning;

            for (int i = 0; i < scheduled.Count; i++)
            {
                ScheduledChannel c = scheduled[i];
                TickChannelMetrics cm = new TickChannelMetrics();
                CopyChannelMetrics(c.Metrics, cm);
                snapshot.Channels.Add(cm);

                for (int s = 0; s < c.Systems.Count; s++)
                {
                    TickSystemMetrics sm = new TickSystemMetrics();
                    CopySystemMetrics(c.Systems[s].Metrics, sm);
                    snapshot.Systems.Add(sm);
                }
            }
            return snapshot;
        }

        public string BuildDiagnosticsReport(int worstSystemCount)
        {
            SchedulerSnapshot snapshot = CaptureSnapshot();
            System.Text.StringBuilder sb = new System.Text.StringBuilder(1024);
            sb.Append("[Scheduler] time=").Append(snapshot.ServerTime.ToString("F2"))
              .Append(" dt=").Append(snapshot.DeltaTime.ToString("F4"))
              .Append(" rawDt=").Append(snapshot.RawDeltaTime.ToString("F4"))
              .Append(" running=").Append(snapshot.IsRunning).Append('\n');

            for (int i = 0; i < snapshot.Channels.Count; i++)
            {
                TickChannelMetrics c = snapshot.Channels[i];
                sb.Append("  Channel ").Append(c.Name)
                  .Append(" hz=").Append(c.CurrentHz.ToString("F2"))
                  .Append(" ms=").Append(c.LastFrameMs.ToString("F3"))
                  .Append(" ticks=").Append(c.ExecutedTicksLastFrame).Append('/').Append(c.PendingTicksLastFrame)
                  .Append(" backlog=").Append(c.AccumulatorSeconds.ToString("F3"))
                  .Append(" pressure=").Append(c.PressureLevel)
                  .Append(" throttled=").Append(c.IsThrottled)
                  .Append('\n');
            }

            List<TickSystemMetrics> systems = snapshot.Systems;
            systems.Sort(delegate (TickSystemMetrics a, TickSystemMetrics b)
            {
                return b.LastTotalMs.CompareTo(a.LastTotalMs);
            });

            int count = worstSystemCount <= 0 ? systems.Count : Math.Min(worstSystemCount, systems.Count);
            for (int i = 0; i < count; i++)
            {
                TickSystemMetrics s = systems[i];
                sb.Append("  System ").Append(s.ChannelName).Append('/').Append(s.SystemName)
                  .Append(" total=").Append(s.LastTotalMs.ToString("F3"))
                  .Append(" avg=").Append(s.AverageTotalMs.ToString("F3"))
                  .Append(" max=").Append(s.MaxTotalMs.ToString("F3"))
                  .Append(" ex=").Append(s.PrepareExceptionCount + s.ExecuteExceptionCount + s.CommitExceptionCount)
                  .Append(" skipped=").Append(s.SkippedByLoadShedding)
                  .Append('\n');
            }

            return sb.ToString();
        }

        private void ExecuteTick(ScheduledChannel entry, in TickContext ctx, SchedulerPressureLevel pressure)
        {
            List<ScheduledTickSystem> prepared = entry.PreparedThisTick;
            List<ScheduledTickSystem> executed = entry.ExecutedThisTick;
            prepared.Clear();
            executed.Clear();

            for (int i = 0; i < entry.Systems.Count; i++)
            {
                ScheduledTickSystem system = entry.Systems[i];
                if (!ShouldRun(system, ctx, pressure))
                {
                    system.Metrics.SkippedByLoadShedding++;
                    continue;
                }

                try
                {
                    system.TimePrepare(ctx);
                    prepared.Add(system);
                }
                catch (Exception ex)
                {
                    system.Metrics.PrepareExceptionCount++;
                    HandleException(system, entry, "Prepare", ex);
                }
            }

            for (int i = 0; i < prepared.Count; i++)
            {
                ScheduledTickSystem system = prepared[i];
                try
                {
                    system.TimeExecute(ctx);
                    executed.Add(system);
                }
                catch (Exception ex)
                {
                    system.Metrics.ExecuteExceptionCount++;
                    HandleException(system, entry, "Execute", ex);
                }
            }

            for (int i = 0; i < executed.Count; i++)
            {
                ScheduledTickSystem system = executed[i];
                try
                {
                    system.TimeCommit(ctx);
                    system.CompleteTick(ctx.TickIndex);
                }
                catch (Exception ex)
                {
                    system.Metrics.CommitExceptionCount++;
                    HandleException(system, entry, "Commit", ex);
                }
            }
        }

        private bool ShouldRun(ScheduledTickSystem system, in TickContext ctx, SchedulerPressureLevel pressure)
        {
            ILoadSheddingTickSystem loadAware = system.System as ILoadSheddingTickSystem;
            if (loadAware == null)
                return true;
            try
            {
                return loadAware.ShouldRun(ctx, pressure);
            }
            catch (Exception ex)
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                UnityEngine.Debug.LogError("[Scheduler] ShouldRun failed for '" + system.Metrics.SystemName + "':\n" + ex);
#endif
                return true;
            }
        }

        private SchedulerPressureLevel ComputePressure(ScheduledChannel entry, int ticksAvailable)
        {
            if (entry.Channel.AccumulatorSeconds >= entry.Config.EmergencyDropBacklogAboveSeconds)
                return SchedulerPressureLevel.Critical;
            if (ticksAvailable >= 2 || entry.ConsecutiveBudgetHits > 0)
                return SchedulerPressureLevel.Elevated;
            return SchedulerPressureLevel.Normal;
        }

        private void ApplyBudgetPolicy(ScheduledChannel entry)
        {
            bool budgetHit = entry.Metrics.LastFrameMs >= entry.Config.MaxMsPerFrame || entry.Metrics.ExecutedTicksLastFrame < entry.Metrics.PendingTicksLastFrame;
            if (budgetHit)
            {
                entry.Metrics.BudgetHitCount++;
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                if (settings.LogBudgetWarnings)
                    UnityEngine.Debug.LogWarning("[Scheduler] Channel '" + entry.Config.Name + "' hit budget (" + entry.Metrics.LastFrameMs.ToString("F2") + " ms, " + entry.Metrics.ExecutedTicksLastFrame + "/" + entry.Metrics.PendingTicksLastFrame + " ticks).");
#endif
                entry.ConsecutiveBudgetHits++;
                entry.ConsecutiveHealthyFrames = 0;
            }
            else
            {
                entry.ConsecutiveHealthyFrames++;
                entry.ConsecutiveBudgetHits = 0;
            }

            if (!entry.Config.AllowThrottling)
                return;

            if (entry.ConsecutiveBudgetHits >= entry.Config.BudgetHitThreshold)
            {
                double newHz = Math.Max(entry.Config.MinHz, entry.Channel.CurrentHz * 0.5d);
                if (newHz < entry.Channel.CurrentHz - 0.001d)
                {
                    entry.Channel.SetHz(newHz);
                    entry.Metrics.ThrottleCount++;
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                    UnityEngine.Debug.LogWarning("[Scheduler] Throttling channel '" + entry.Config.Name + "' to " + newHz.ToString("F2") + " Hz.");
#endif
                }
                entry.ConsecutiveBudgetHits = 0;
            }

            if (entry.ConsecutiveHealthyFrames >= entry.Config.HealthyFrameRestoreThreshold)
            {
                entry.Channel.RestoreBaseHz();
                entry.ConsecutiveHealthyFrames = 0;
            }
        }

        private static int ResolveOrder(ITickSystem system)
        {
            IOrderedTickSystem ordered = system as IOrderedTickSystem;
            return ordered != null ? ordered.Order : 0;
        }

        private void SortChannels()
        {
            scheduled.Sort(delegate (ScheduledChannel a, ScheduledChannel b)
            {
                int priorityCompare = b.Config.Priority.CompareTo(a.Config.Priority);
                if (priorityCompare != 0)
                    return priorityCompare;
                return string.CompareOrdinal(a.Config.Name, b.Config.Name);
            });
        }

        private void HandleException(ScheduledTickSystem system, ScheduledChannel entry, string phase, Exception ex)
        {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (settings.LogExceptionDetails)
                UnityEngine.Debug.LogError("[Scheduler] " + phase + " failed for system '" + system.Metrics.SystemName + "' in channel '" + entry.Config.Name + "':\n" + ex);
            else
                UnityEngine.Debug.LogError("[Scheduler] " + phase + " failed for system '" + system.Metrics.SystemName + "'.");
#else
            UnityEngine.Debug.LogError("[Scheduler] " + phase + " failed for system '" + system.Metrics.SystemName + "'.");
#endif
        }

        private static void CopyChannelMetrics(TickChannelMetrics source, TickChannelMetrics dest)
        {
            dest.Name = source.Name;
            dest.Priority = source.Priority;
            dest.CurrentHz = source.CurrentHz;
            dest.BaseHz = source.BaseHz;
            dest.MinHz = source.MinHz;
            dest.LastFrameMs = source.LastFrameMs;
            dest.MaxFrameMs = source.MaxFrameMs;
            dest.AccumulatorSeconds = source.AccumulatorSeconds;
            dest.ExecutedTicksLastFrame = source.ExecutedTicksLastFrame;
            dest.PendingTicksLastFrame = source.PendingTicksLastFrame;
            dest.TotalExecutedTicks = source.TotalExecutedTicks;
            dest.BudgetHitCount = source.BudgetHitCount;
            dest.ThrottleCount = source.ThrottleCount;
            dest.IsThrottled = source.IsThrottled;
            dest.PressureLevel = source.PressureLevel;
        }

        private static void CopySystemMetrics(TickSystemMetrics source, TickSystemMetrics dest)
        {
            dest.ChannelName = source.ChannelName;
            dest.SystemName = source.SystemName;
            dest.Order = source.Order;
            dest.PrepareCount = source.PrepareCount;
            dest.ExecuteCount = source.ExecuteCount;
            dest.CommitCount = source.CommitCount;
            dest.SkippedByLoadShedding = source.SkippedByLoadShedding;
            dest.PrepareExceptionCount = source.PrepareExceptionCount;
            dest.ExecuteExceptionCount = source.ExecuteExceptionCount;
            dest.CommitExceptionCount = source.CommitExceptionCount;
            dest.LastPrepareMs = source.LastPrepareMs;
            dest.LastExecuteMs = source.LastExecuteMs;
            dest.LastCommitMs = source.LastCommitMs;
            dest.LastTotalMs = source.LastTotalMs;
            dest.MaxTotalMs = source.MaxTotalMs;
            dest.AverageTotalMs = source.AverageTotalMs;
            dest.LastTickIndex = source.LastTickIndex;
        }
    }
}
