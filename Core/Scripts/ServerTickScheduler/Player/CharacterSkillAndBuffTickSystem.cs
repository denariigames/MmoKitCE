using MultiplayerARPG.Server.Scheduling;

namespace MultiplayerARPG.Server.Runtime
{
    /// <summary>
    /// Ticks CharacterSkillAndBuffComponent for registered characters.
    /// </summary>
    public sealed class CharacterSkillAndBuffTickSystem : ITickSystem, IOrderedTickSystem, ILoadSheddingTickSystem
    {
        public string Name { get { return nameof(CharacterSkillAndBuffTickSystem); } }
        public int Order { get { return 100; } }

        public bool ShouldRun(in TickContext ctx, SchedulerPressureLevel pressureLevel)
        {
            // Buff/cooldown simulation should remain authoritative. Do not globally shed.
            return true;
        }

        public void Prepare(in TickContext ctx) { }

        public void Execute(in TickContext ctx)
        {
            var list = MultiplayerARPG.CharacterSkillAndBuffTickDriver.Components;

            for (int i = 0; i < list.Count; ++i)
            {
                CharacterSkillAndBuffComponent comp = list[i];
                if (!comp)
                    continue;

                BaseGameEntity ent = comp.Entity;
                if (!ent || !ent.IsServer)
                    continue;

                var identity = ent.Identity;
                if (identity != null && identity.CountSubscribers() == 0 && !(ent is BasePlayerCharacterEntity))
                    continue;

                comp.TickSkillAndBuff(ctx.FixedDelta);
            }
        }

        public void Commit(in TickContext ctx) { }
    }
}
