// File: ServerTickScheduler/Server/ServerRuntimeSimulation.cs
#if UNITY_SERVER || UNITY_EDITOR
using Cysharp.Threading.Tasks;
using Insthync.ManagedUpdating;
using MultiplayerARPG.Server.Scheduling;
using UnityEngine;
using UnityEngine.Rendering;

namespace MultiplayerARPG
{
    // Server simulation bootstrap + driver as a partial of GameInstance
    public partial class GameInstance
    {
        private ServerTickScheduler _serverScheduler;
        private bool _serverSimulationInitialized;
        private float _nextInitAttemptTime;

        [System.Serializable]
        private struct InspectorTickChannel
        {
            public string name;
            public int priority;
            public double baseHz;
            public double minHz;
            public double maxMsPerFrame;

            [Tooltip("Optional: max items a time-sliced (IIncrementalTickSystem) system should process per tick in this channel. 0 or less = unlimited.")]
            public int maxItemsPerTick;

            [Tooltip("Optional: max milliseconds a time-sliced (IIncrementalTickSystem) system may spend per tick in this channel. 0 or less = unlimited.")]
            public double maxSystemMsPerTick;

            public bool allowThrottling;
        }

        [System.Serializable]
        private class AdvancedTickOverrides
        {
            [Tooltip("When enabled, these values are used as-is (no special 0 behavior).")]
            public bool enabled = false;

            public double monsterAISystemHz = 5;
            public double monsterActivityMoveIntentHz = 20;
            public double monsterActivityCombatHz = 20;
            public double delayedTasksHz = 1;
        }

        [Header("Server Simulation")]
        [SerializeField] private bool driveServerSimulationFromUnityUpdate = true;

        [Header("Legacy UpdateManager Bridge")]
        [Tooltip("When enabled on authoritative/headless server, UpdateManager's Update/LateUpdate/FixedUpdate loops are disabled and driven by ServerTickScheduler instead.")]
        [SerializeField] private bool tickDriveUpdateManager = true;

        [Tooltip("Tick rate for the UpdateManager bridge. <= 0 uses the Movement channel BaseHz.")]
        [SerializeField] private double managedUpdateManagerHz = 0;

        [Tooltip("If true, server simulation will run when the current process is authoritative (dedicated server or listen/host).")]
        [SerializeField] private bool runWhenNetworkIsServer = true;

        [Tooltip("If true, allow server simulation to run when not connected/started as client/server (offline/local authority testing).")]
        [SerializeField] private bool runWhenOffline = false;

        [Header("Tick Channels")]
        [SerializeField] private InspectorTickChannel[] tickChannels = new InspectorTickChannel[]
        {
            // NOTE: maxItemsPerTick/maxSystemMsPerTick only apply to systems that implement IIncrementalTickSystem.
            // These defaults are conservative and aim for stability at high entity counts.
            new InspectorTickChannel { name = "Movement", priority = 100, baseHz = 20, minHz = 15, maxMsPerFrame = 5.0, maxItemsPerTick = 2048, maxSystemMsPerTick = 0.5, allowThrottling = true },
            new InspectorTickChannel { name = "Combat",   priority =  80, baseHz = 20, minHz = 10, maxMsPerFrame = 2.0, maxItemsPerTick = 2048, maxSystemMsPerTick = 0.75, allowThrottling = true  },
            new InspectorTickChannel { name = "AI",       priority =  40, baseHz =  10, minHz =  5, maxMsPerFrame = 3.0, maxItemsPerTick = 1024, maxSystemMsPerTick = 1.0, allowThrottling = true  },
            new InspectorTickChannel { name = "LowFreq",  priority =  10, baseHz =  1, minHz = 0.5, maxMsPerFrame = 1, maxItemsPerTick = 4096, maxSystemMsPerTick = 1, allowThrottling = true },
        };

        // Collapsed foldout in default inspector
        [SerializeField] private AdvancedTickOverrides advancedOverrides = new AdvancedTickOverrides();

        // Tracks whether we've auto-filled override values from channels for the current "enabled" session
        [SerializeField, HideInInspector] private bool advancedOverridesSeededFromChannels;

        private void OnEnable()
        {
            TryInitServerSimulationDeferred().Forget();
        }

        private void Update()
        {
            if (!driveServerSimulationFromUnityUpdate)
                return;

            // If host/server starts after scene load, keep trying to init.
            if (!_serverSimulationInitialized && Time.unscaledTime >= _nextInitAttemptTime)
            {
                _nextInitAttemptTime = Time.unscaledTime + 0.5f;
                if (ShouldRunServerSimulation())
                    InitServerSimulation();
            }

            TickServerSimulation();
        }

        private void OnDisable()
        {
            ShutdownServerSimulation();
        }

        /// <summary>
        /// Call this from your heartbeat if you disable driveServerSimulationFromUnityUpdate.
        /// </summary>
        public void TickServerSimulation()
        {
            if (!_serverSimulationInitialized)
                return;

            _serverScheduler?.Update();
        }

        private async UniTaskVoid TryInitServerSimulationDeferred()
        {
            // Ensure GameInstance.Awake() has completed (Singleton, settings, etc.)
            await UniTask.Yield();

            if (!this || !gameObject)
                return;

            if (_serverSimulationInitialized)
                return;

            if (!ShouldRunServerSimulation())
                return;

            InitServerSimulation();
        }

        private bool ShouldRunServerSimulation()
        {
            // Dedicated/headless server (fast path)
            if (Application.isBatchMode)
                return true;

            if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null)
                return true;

#if UNITY_SERVER
            return true;
#endif

            // Listen server / LAN host / any authoritative server process
            if (runWhenNetworkIsServer)
            {
                var nm = BaseGameNetworkManager.Singleton;
                if (nm != null)
                    return nm.IsServer;
            }

            // Optional: offline/local authority testing
            if (runWhenOffline)
            {
                var nm = BaseGameNetworkManager.Singleton;
                if (nm == null)
                    return true;

                return !nm.IsClient && !nm.IsServer;
            }

            return false;
        }

        private void InitServerSimulation()
        {
            _serverSimulationInitialized = true;
            BaseGameEntityTickDriver.Enabled = true;
            MovementTickDriver.Enabled = true;
            CharacterRecoveryTickDriver.Enabled = true;
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            Debug.Log("[GameInstance] Initializing Server Simulation...");
#endif

            // If overrides were disabled, allow reseeding next time they are enabled
            if (advancedOverrides == null)
                advancedOverrides = new AdvancedTickOverrides();

            if (!advancedOverrides.enabled)
                advancedOverridesSeededFromChannels = false;

            // When overrides are enabled, seed them ONCE from channel BaseHz (no "0 means inherit" behavior)
            if (advancedOverrides.enabled && !advancedOverridesSeededFromChannels)
            {
                SeedAdvancedOverridesFromChannels();
                advancedOverridesSeededFromChannels = true;
            }

            var settings = new ServerTickSettings();

            // Channels editable from inspector
            if (tickChannels != null)
            {
                for (int i = 0; i < tickChannels.Length; ++i)
                {
                    var c = tickChannels[i];
                    if (string.IsNullOrWhiteSpace(c.name))
                        continue;

                    settings.Channels.Add(new TickChannelConfig
                    {
                        Name = c.name,
                        Priority = c.priority,
                        BaseHz = c.baseHz,
                        MinHz = c.minHz,
                        MaxMsPerFrame = c.maxMsPerFrame,
                        MaxItemsPerTick = c.maxItemsPerTick > 0 ? c.maxItemsPerTick : int.MaxValue,
                        MaxSystemMsPerTick = c.maxSystemMsPerTick > 0 ? c.maxSystemMsPerTick : double.MaxValue,
                        AllowThrottling = c.allowThrottling,
                    });
                }
            }

            _serverScheduler = new ServerTickScheduler(settings);

            // Register systems (default: channel BaseHz, optional: advanced override values)
            _serverScheduler.RegisterSystem(new MultiplayerARPG.Server.AI.MonsterActivityAISystem(), "AI", (advancedOverrides != null && advancedOverrides.enabled) ? advancedOverrides.monsterAISystemHz : GetChannelBaseHz("AI", 5));
            _serverScheduler.RegisterSystem(new MultiplayerARPG.Server.Runtime.MonsterActivityMoveIntentSystem(), "Movement", (advancedOverrides != null && advancedOverrides.enabled) ? advancedOverrides.monsterActivityMoveIntentHz : GetChannelBaseHz("Movement", 20));
            _serverScheduler.RegisterSystem(new MultiplayerARPG.Server.Runtime.MovementTickSystem(), "Movement", GetChannelBaseHz("Movement", 20));
            _serverScheduler.RegisterSystem(new MultiplayerARPG.Server.Runtime.MonsterActivityCombatSystem(), "Combat", (advancedOverrides != null && advancedOverrides.enabled) ? advancedOverrides.monsterActivityCombatHz : GetChannelBaseHz("Combat", 20));
            var delayedTasks = new ServerDelayedTaskSystem();
            _serverScheduler.RegisterSystem(delayedTasks, "LowFreq", (advancedOverrides != null && advancedOverrides.enabled) ? advancedOverrides.delayedTasksHz : GetChannelBaseHz("LowFreq", 1));
            _serverScheduler.RegisterSystem(new MultiplayerARPG.Server.Runtime.BaseGameEntityTickSystem(), "Movement", GetChannelBaseHz("Movement", 20));
            ServerDelayedTasks.Unbind();
            ServerDelayedTasks.Bind(delayedTasks);
            _serverScheduler.RegisterSystem(new MultiplayerARPG.Server.Runtime.CharacterRecoveryTickSystem(), "LowFreq", GetChannelBaseHz("LowFreq", 1));
        }

        private void SeedAdvancedOverridesFromChannels()
        {
            if (tickChannels == null)
                return;

            advancedOverrides.monsterAISystemHz = GetChannelBaseHz("AI", advancedOverrides.monsterAISystemHz);
            advancedOverrides.monsterActivityMoveIntentHz = GetChannelBaseHz("Movement", advancedOverrides.monsterActivityMoveIntentHz);
            advancedOverrides.monsterActivityCombatHz = GetChannelBaseHz("Combat", advancedOverrides.monsterActivityCombatHz);
            advancedOverrides.delayedTasksHz = GetChannelBaseHz("LowFreq", advancedOverrides.delayedTasksHz);
        }

        private double GetChannelBaseHz(string channel, double fallback)
        {
            if (tickChannels == null)
                return fallback;

            for (int i = 0; i < tickChannels.Length; ++i)
            {
                if (tickChannels[i].name == channel)
                    return tickChannels[i].baseHz > 0 ? tickChannels[i].baseHz : fallback;
            }
            return fallback;
        }

        private void ShutdownServerSimulation()
        {
            if (!_serverSimulationInitialized)
                return;

            _serverSimulationInitialized = false;
            BaseGameEntityTickDriver.Enabled = false;
            MovementTickDriver.Enabled = false;
            BaseGameEntityTickDriver.Clear();
            MovementTickDriver.Clear();
            ServerDelayedTasks.Unbind();
            ServerDelayedTasks.Unbind();
            _serverScheduler = null;
            CharacterRecoveryTickDriver.Enabled = false;
            CharacterRecoveryTickDriver.Clear();

            
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            Debug.Log("[GameInstance] Shutdown Server Simulation...");
#endif
        }
    }
}
#endif
