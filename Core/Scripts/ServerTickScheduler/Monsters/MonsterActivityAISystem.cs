using MultiplayerARPG.Server.Scheduling;

namespace MultiplayerARPG.Server.AI
{
    /// <summary>
    /// AI decision tick for MonsterActivityComponent.
    /// Runs on the AI channel and degrades optional idle AI work under scheduler pressure.
    /// </summary>
    public sealed class MonsterActivityAISystem : ITickSystem, ILoadSheddingTickSystem, IOrderedTickSystem
    {
        private SchedulerPressureLevel pressureLevel;

        public string Name { get { return "MonsterActivityAI"; } }
        public int Order { get { return 0; } }

        public MonsterActivityAISystem()
        {
            MonsterActivityTickRegistry.TickDrivenEnabled = true;
        }

        public bool ShouldRun(in TickContext ctx, SchedulerPressureLevel pressureLevel)
        {
            // AI is allowed to degrade internally, but the system should still run so
            // active combat/leash decisions can continue under pressure.
            this.pressureLevel = pressureLevel;
            return true;
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
                if (comp == null || comp.Entity == null)
                    continue;

                if (!ShouldProcess(comp))
                    continue;

                comp.TickAI(ctx.FixedDelta);
            }
        }

        public void Commit(in TickContext ctx)
        {
            // no-op
        }

        private bool ShouldProcess(MultiplayerARPG.MonsterActivityComponent comp)
        {
            var entity = comp.Entity;
            if (entity == null)
                return false;

            var identity = entity.Identity;
            bool hasSubscribers = identity == null || identity.CountSubscribers() > 0;
            bool hasTarget = entity.TryGetTargetEntity(out IDamageableEntity target) &&
                             target != null &&
                             target.GetObjectId() != entity.ObjectId &&
                             !target.IsDeadOrHideFrom(entity) &&
                             target.CanReceiveDamageFrom(entity.GetInfo());

            // Normal: preserve legacy behavior, but still skip entities outside AOI.
            if (pressureLevel == SchedulerPressureLevel.Normal)
                return hasSubscribers;

            // Elevated: stop spending AI time on idle wandering. Keep combat/leash/aggro AI.
            if (pressureLevel == SchedulerPressureLevel.Elevated)
                return hasSubscribers && hasTarget;

            // Critical: only active combat AI remains. This is the survival mode.
            return hasSubscribers && hasTarget;
        }
    }
}
