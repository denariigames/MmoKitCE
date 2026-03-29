using MultiplayerARPG;
using MultiplayerARPG.Server.AI;
using MultiplayerARPG.Server.Scheduling;

namespace MultiplayerARPG.Server.Runtime
{
    public sealed class MonsterActivityMoveIntentSystem : ITickSystem
    {
        public string Name => nameof(MonsterActivityMoveIntentSystem);

        public void Prepare(in TickContext ctx) { }

public void Execute(in TickContext ctx)
{
    int processed = 0;

    var list = MonsterActivityTickRegistry.Snapshot();
    for (int i = 0; i < list.Count; i++)
    {
        var comp = list[i];
        if (comp == null || comp.Entity == null)
            continue;

        var identity = comp.Entity.Identity;
        if (identity != null && identity.CountSubscribers() == 0)
            continue;

        comp.TickMoveIntent(ctx.FixedDelta);
        processed++;
    }
}


        public void Commit(in TickContext ctx) { }
    }
}
