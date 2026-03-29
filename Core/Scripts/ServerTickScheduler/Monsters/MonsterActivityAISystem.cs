using MultiplayerARPG.Server.Scheduling;

namespace MultiplayerARPG.Server.AI
{
    /// <summary>
    /// AI decision tick for MonsterActivityComponent.
    /// Runs on the AI channel (low Hz) and performs target selection and decision timers.
    /// </summary>
    public sealed class MonsterActivityAISystem : ITickSystem
    {
        public string Name => "MonsterActivityAI";

        public MonsterActivityAISystem()
        {
            MonsterActivityTickRegistry.TickDrivenEnabled = true;
        }

        public void Prepare(in TickContext ctx)
        {
            // no-op
        }

public void Execute(in TickContext ctx)
{
    var list = MonsterActivityTickRegistry.Snapshot();
    for (int i = 0; i < list.Count; i++)
    {
        var comp = list[i];
        if (comp == null)
            continue;

        var identity = comp.Entity != null ? comp.Entity.Identity : null;
        if (identity != null && identity.CountSubscribers() == 0)
            continue;

        comp.TickAI(ctx.FixedDelta); 
    }
}


        public void Commit(in TickContext ctx)
        {
            // no-op
        }
    }
}
