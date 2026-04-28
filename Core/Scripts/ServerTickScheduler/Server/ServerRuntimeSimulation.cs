using System;
using UnityEngine;
using MultiplayerARPG.Server.Scheduling;

namespace MultiplayerARPG.Server.Runtime
{
    /// <summary>
    /// Project adapter for the reusable scheduler core.
    /// Keep kit/game-specific registrations here; keep the scheduler core reusable.
    /// </summary>
    public sealed class ServerRuntimeSimulation : MonoBehaviour
    {
        [Header("Lifecycle")]
        [SerializeField] private bool startOnAwake = true;
        [SerializeField] private bool resetClockOnStart = true;

        [Header("Diagnostics")]
        [SerializeField] private bool logDiagnostics;
        [SerializeField] private float diagnosticsIntervalSeconds = 10f;
        [SerializeField] private int diagnosticsWorstSystemCount = 8;

        private ServerTickScheduler scheduler;
        private ServerDelayedTaskSystem gameplayDelayedTasks;
        private ServerDelayedTaskSystem backgroundDelayedTasks;
        private float nextDiagnosticsTime;
        private bool started;

        public ServerTickScheduler Scheduler { get { return scheduler; } }

        private void Awake()
        {
            if (startOnAwake)
                StartServerSimulation();
        }

        public void StartServerSimulation()
        {
            if (started)
                return;

#if !UNITY_SERVER && !UNITY_EDITOR
            enabled = false;
            return;
#endif
            if (resetClockOnStart)
                MultiplayerARPG.Server.Time.ServerClock.Reset();

            ServerTickSettings settings = BuildDefaultSettings();
            scheduler = new ServerTickScheduler(settings);

            RegisterOptionalProjectSystems(scheduler);

            gameplayDelayedTasks = new ServerDelayedTaskSystem
            {
                MaxCallbacksPerTick = 512,
                MaxMsPerTick = 1.0d
            };
            scheduler.RegisterSystem(gameplayDelayedTasks, "GameplayTimers", 20, 0);
            ServerDelayedTasks.Bind(gameplayDelayedTasks);

            backgroundDelayedTasks = new ServerDelayedTaskSystem
            {
                MaxCallbacksPerTick = 128,
                MaxMsPerTick = 0.5d
            };
            scheduler.RegisterSystem(backgroundDelayedTasks, "LowFreq", 1, 0);

            started = true;
            nextDiagnosticsTime = UnityEngine.Time.unscaledTime + diagnosticsIntervalSeconds;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            Debug.Log("[ServerRuntimeSimulation] Started server scheduler.");
#endif
        }

        public void StopServerSimulation()
        {
            if (!started)
                return;

            ServerDelayedTasks.Unbind();

            if (gameplayDelayedTasks != null)
                gameplayDelayedTasks.Clear();
            if (backgroundDelayedTasks != null)
                backgroundDelayedTasks.Clear();
            if (scheduler != null)
                scheduler.Shutdown();

            gameplayDelayedTasks = null;
            backgroundDelayedTasks = null;
            scheduler = null;
            started = false;
        }

        private void Update()
        {
            if (!started || scheduler == null)
                return;

            scheduler.Update();

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            if (logDiagnostics && diagnosticsIntervalSeconds > 0f && UnityEngine.Time.unscaledTime >= nextDiagnosticsTime)
            {
                nextDiagnosticsTime = UnityEngine.Time.unscaledTime + diagnosticsIntervalSeconds;
                Debug.Log(scheduler.BuildDiagnosticsReport(diagnosticsWorstSystemCount));
            }
#endif
        }

        private void OnDisable()
        {
            StopServerSimulation();
        }

        private void OnDestroy()
        {
            StopServerSimulation();
        }

        private static ServerTickSettings BuildDefaultSettings()
        {
            ServerTickSettings settings = new ServerTickSettings
            {
                ExecutionMode = TickExecutionMode.SingleThreaded,
                MaxTicksPerFrame = 5,
                MaxBacklogSeconds = 0.25d,
                LogBudgetWarnings = true,
                LogExceptionDetails = true
            };

            settings.Channels.Add(new TickChannelConfig
            {
                Name = "Movement",
                Priority = 100,
                BaseHz = 20,
                MinHz = 20,
                MaxMsPerFrame = 2.0d,
                PhaseOffsetSeconds = 0.000d,
                AllowThrottling = false
            });

            settings.Channels.Add(new TickChannelConfig
            {
                Name = "Entities",
                Priority = 90,
                BaseHz = 20,
                MinHz = 20,
                MaxMsPerFrame = 2.0d,
                PhaseOffsetSeconds = 0.005d,
                AllowThrottling = false
            });

            settings.Channels.Add(new TickChannelConfig
            {
                Name = "Combat",
                Priority = 80,
                BaseHz = 20,
                MinHz = 10,
                MaxMsPerFrame = 2.0d,
                PhaseOffsetSeconds = 0.010d,
                AllowThrottling = true
            });

            settings.Channels.Add(new TickChannelConfig
            {
                Name = "GameplayTimers",
                Priority = 70,
                BaseHz = 20,
                MinHz = 10,
                MaxMsPerFrame = 1.0d,
                PhaseOffsetSeconds = 0.020d,
                AllowThrottling = true
            });

            settings.Channels.Add(new TickChannelConfig
            {
                Name = "AI",
                Priority = 40,
                BaseHz = 5,
                MinHz = 2,
                MaxMsPerFrame = 3.0d,
                PhaseOffsetSeconds = 0.040d,
                AllowThrottling = true
            });

            settings.Channels.Add(new TickChannelConfig
            {
                Name = "LowFreq",
                Priority = 10,
                BaseHz = 1,
                MinHz = 0.5d,
                MaxMsPerFrame = 0.5d,
                PhaseOffsetSeconds = 0.125d,
                AllowThrottling = true
            });

            return settings;
        }

        private static void RegisterOptionalProjectSystems(ServerTickScheduler scheduler)
        {
            TryRegisterByTypeName(scheduler, "MultiplayerARPG.Server.Runtime.BaseGameEntityTickSystem", "Entities", 20, 0);
            TryRegisterByTypeName(scheduler, "MultiplayerARPG.Server.Runtime.MovementTickSystem", "Movement", 20, 100);
            TryRegisterByTypeName(scheduler, "MultiplayerARPG.Server.Runtime.CharacterSkillAndBuffTickSystem", "LowFreq", 1, 100);
            TryRegisterByTypeName(scheduler, "MultiplayerARPG.Server.Runtime.CharacterRecoveryTickSystem", "LowFreq", 1, 200);

            TryRegisterByTypeName(scheduler, "MultiplayerARPG.Server.AI.MonsterActivityAISystem", "AI", 5, 0);
            TryRegisterByTypeName(scheduler, "MultiplayerARPG.Server.Runtime.MonsterActivityMoveIntentSystem", "Movement", 20, 0);
            TryRegisterByTypeName(scheduler, "MultiplayerARPG.Server.Runtime.MonsterActivityCombatSystem", "Combat", 20, 0);
        }

        private static void TryRegisterByTypeName(ServerTickScheduler scheduler, string typeName, string channelName, double tickRate, int order)
        {
            Type type = FindType(typeName);
            if (type == null)
                return;

            if (!typeof(ITickSystem).IsAssignableFrom(type))
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                Debug.LogWarning("[ServerRuntimeSimulation] Type exists but does not implement ITickSystem: " + typeName);
#endif
                return;
            }

            try
            {
                ITickSystem system = (ITickSystem)Activator.CreateInstance(type);
                scheduler.RegisterSystem(system, channelName, tickRate, order);
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                Debug.Log("[ServerRuntimeSimulation] Registered optional tick system: " + typeName + " -> " + channelName);
#endif
            }
            catch (Exception ex)
            {
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                Debug.LogError("[ServerRuntimeSimulation] Failed to register optional tick system '" + typeName + "':\n" + ex);
#endif
            }
        }

        private static Type FindType(string typeName)
        {
            Type type = Type.GetType(typeName);
            if (type != null)
                return type;

            System.Reflection.Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assemblies.Length; i++)
            {
                type = assemblies[i].GetType(typeName);
                if (type != null)
                    return type;
            }
            return null;
        }
    }
}
