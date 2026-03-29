using MultiplayerARPG.Server.Scheduling;

namespace MultiplayerARPG.Server.Runtime
{
    public sealed class MovementTickSystem : ITickSystem
    {
        public string Name => nameof(MovementTickSystem);

        public void Prepare(in TickContext ctx) { }

        public void Execute(in TickContext ctx)
        {
            float dt = ctx.FixedDelta;

            var nav = MultiplayerARPG.MovementTickDriver.NavMesh;
            for (int i = 0; i < nav.Count; ++i)
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
