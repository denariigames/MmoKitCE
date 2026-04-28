using MultiplayerARPG.Server.Scheduling;

namespace MultiplayerARPG.Server.Runtime
{
    /// <summary>
    /// Ticks CharacterRecoveryComponent for registered characters.
    /// Intended for LowFreq/background cadence.
    /// </summary>
    public sealed class CharacterRecoveryTickSystem : ITickSystem, IOrderedTickSystem, ILoadSheddingTickSystem
    {
        public string Name { get { return nameof(CharacterRecoveryTickSystem); } }
        public int Order { get { return 200; } }

        public bool ShouldRun(in TickContext ctx, SchedulerPressureLevel pressureLevel)
        {
            // Recovery is not transaction-critical, but globally skipping it can cause odd player-visible behavior.
            // Keep it running; Execute filters unobserved non-player entities.
            return true;
        }

        public void Prepare(in TickContext ctx) { }

        public void Execute(in TickContext ctx)
        {
            var list = MultiplayerARPG.CharacterRecoveryTickDriver.Components;
            for (int i = 0; i < list.Count; ++i)
            {
                CharacterRecoveryComponent comp = list[i];
                if (!comp)
                    continue;

                BaseGameEntity ent = comp.Entity;
                if (!ent || !ent.IsServer)
                    continue;

                var identity = ent.Identity;
                if (identity != null && identity.CountSubscribers() == 0 && !(ent is BasePlayerCharacterEntity))
                    continue;

                comp.TickRecovery(ctx.FixedDelta);
            }
        }

        public void Commit(in TickContext ctx) { }
    }
}
