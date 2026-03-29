using MultiplayerARPG.Server.Scheduling;
using UnityEngine;

namespace MultiplayerARPG.Server.Runtime
{
    /// <summary>
    /// Ticks CharacterRecoveryComponent for all registered characters.
    /// Intended to run on LowFreq (usually ~1 Hz), but works at higher Hz too.
    /// </summary>
    public sealed class CharacterRecoveryTickSystem : ITickSystem
    {
        public string Name => nameof(CharacterRecoveryTickSystem);
        private bool _loggedFirstExecute;
        public void Prepare(in TickContext ctx) { }

        public void Execute(in TickContext ctx)
        {
            var list = MultiplayerARPG.CharacterRecoveryTickDriver.Components;
            for (int i = 0; i < list.Count; ++i)
            {
                var comp = list[i];
                if (comp == null)
                    continue;

                var ent = comp.Entity;
                if (ent == null || !ent.IsServer)
                    continue;

                // Optional perf gate (same as skill/buff). If you don't want behavior change, remove this.
                var identity = ent.Identity;
                if (identity != null && identity.CountSubscribers() == 0 && !(ent is BasePlayerCharacterEntity))
                    continue;

                comp.TickRecovery(ctx.FixedDelta);
            }
        }

        public void Commit(in TickContext ctx) { }
    }
}