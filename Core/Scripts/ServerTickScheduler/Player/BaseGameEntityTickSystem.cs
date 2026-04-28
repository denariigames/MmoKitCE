using MultiplayerARPG.Server.Scheduling;

namespace MultiplayerARPG.Server.Runtime
{
    /// <summary>
    /// Drives BaseGameEntity.ManagedUpdate() / ManagedLateUpdate() for registered server entities.
    /// </summary>
    public sealed class BaseGameEntityTickSystem : ITickSystem, IOrderedTickSystem, ILoadSheddingTickSystem
    {
        public string Name { get { return nameof(BaseGameEntityTickSystem); } }
        public int Order { get { return 0; } }

        public bool ShouldRun(in TickContext ctx, SchedulerPressureLevel pressureLevel)
        {
            // This is core entity simulation. Do not globally shed it.
            return true;
        }

        public void Prepare(in TickContext ctx) { }

        public void Execute(in TickContext ctx)
        {
            var list = MultiplayerARPG.BaseGameEntityTickDriver.Entities;

            for (int i = 0; i < list.Count; ++i)
            {
                BaseGameEntity ent = list[i];
                if (!ent || !ent.IsServer)
                    continue;

                ent.ManagedUpdate();
            }
        }

        public void Commit(in TickContext ctx)
        {
            var list = MultiplayerARPG.BaseGameEntityTickDriver.Entities;

            for (int i = 0; i < list.Count; ++i)
            {
                BaseGameEntity ent = list[i];
                if (!ent || !ent.IsServer)
                    continue;

                ent.ManagedLateUpdate();
            }
        }
    }
}
