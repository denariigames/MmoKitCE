using MultiplayerARPG.Server.Scheduling;

namespace MultiplayerARPG.Server.Runtime
{
    /// <summary>
    /// Scheduler-driven movement update. This intentionally does not shed under load because
    /// skipping authoritative movement creates visible rubberbanding and desync risk.
    /// </summary>
    public sealed class MovementTickSystem : ITickSystem, IOrderedTickSystem, ILoadSheddingTickSystem
    {
        public string Name => nameof(MovementTickSystem);
        public int Order => 100;

        public bool ShouldRun(in TickContext ctx, SchedulerPressureLevel pressureLevel)
        {
            return true;
        }

        public void Prepare(in TickContext ctx) { }

        public void Execute(in TickContext ctx)
        {
            float dt = ctx.FixedDelta;

            var nav = MultiplayerARPG.MovementTickDriver.NavMesh;
            for (int i = nav.Count - 1; i >= 0; --i)
            {
                var comp = nav[i];
                if (comp == null || !comp.IsServer)
                    continue;

                comp.TickUpdate(dt);
            }
        }

        public void Commit(in TickContext ctx) { }
    }
}
