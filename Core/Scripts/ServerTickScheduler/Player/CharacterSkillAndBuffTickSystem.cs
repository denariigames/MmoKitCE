using MultiplayerARPG.Server.Scheduling;

namespace MultiplayerARPG.Server.Runtime
{
    /// <summary>
    /// Ticks CharacterSkillAndBuffComponent for all registered characters.
    /// Intended to run on LowFreq (usually 1 Hz), but works at higher Hz too.
    /// </summary>
    public sealed class CharacterSkillAndBuffTickSystem : ITickSystem
    {
        public string Name => nameof(CharacterSkillAndBuffTickSystem);

        public void Prepare(in TickContext ctx) { }

        public void Execute(in TickContext ctx)
        {
            var list = CharacterSkillAndBuffTickDriver.Components;

            for (int i = 0; i < list.Count; ++i)
            {
                var comp = list[i];
                if (comp == null)
                    continue;

                var ent = comp.Entity;
                if (ent == null || !ent.IsServer)
                    continue;

                // Optional perf gate: if entity is totally unobserved, skip non-player entities.
                // Players should keep ticking regardless (buff timers, cooldowns, etc.).
                var identity = ent.Identity;
                if (identity != null && identity.CountSubscribers() == 0 && !(ent is BasePlayerCharacterEntity))
                    continue;

                comp.TickSkillAndBuff(ctx.FixedDelta);
            }
        }

        public void Commit(in TickContext ctx) { }
    }
}
