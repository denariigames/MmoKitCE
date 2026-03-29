using MultiplayerARPG.Server.Scheduling;

namespace MultiplayerARPG.Server.Runtime
{
    /// <summary>
    /// Drives BaseGameEntity.ManagedUpdate() / ManagedLateUpdate() for all entities on headless server,
    /// replacing UpdateManager on server.
    /// </summary>
    public sealed class BaseGameEntityTickSystem : ITickSystem
    {
        public string Name => nameof(BaseGameEntityTickSystem);

        public void Prepare(in TickContext ctx) { }

        public void Execute(in TickContext ctx)
        {
            var list = MultiplayerARPG.BaseGameEntityTickDriver.Entities;

            for (int i = 0; i < list.Count; ++i)
            {
                var ent = list[i];
                if (ent == null)
                    continue;

                // Server-only driving
                if (!ent.IsServer)
                    continue;

                ent.ManagedUpdate();
            }
        }

        public void Commit(in TickContext ctx)
        {
            // LateUpdate phase (kept separate to preserve semantics)
            var list = MultiplayerARPG.BaseGameEntityTickDriver.Entities;

            for (int i = 0; i < list.Count; ++i)
            {
                var ent = list[i];
                if (ent == null)
                    continue;

                if (!ent.IsServer)
                    continue;

                ent.ManagedLateUpdate();
            }
        }
    }
}
