using System.Collections.Generic;

namespace MultiplayerARPG
{
    public interface IEntityMovementForceUpdateListener
    {
        void OnPreUpdateForces(IList<EntityMovementForceApplier> forceAppliers);
        void OnPostUpdateForces(IList<EntityMovementForceApplier> forceAppliers);
    }

    public static class EntityMovementForceUpdateListenerExtensions
    {
        public static void OnPreUpdateForces(this IList<IEntityMovementForceUpdateListener> listeners, IList<EntityMovementForceApplier> forceAppliers)
        {
            if (forceAppliers == null || forceAppliers.Count == 0)
                return;
            for (int i = 0; i < listeners.Count; ++i)
            {
                listeners[i].OnPreUpdateForces(forceAppliers);
            }
        }

        public static void OnPostUpdateForces(this IList<IEntityMovementForceUpdateListener> listeners, IList<EntityMovementForceApplier> forceAppliers)
        {
            if (forceAppliers == null || forceAppliers.Count == 0)
                return;
            for (int i = 0; i < listeners.Count; ++i)
            {
                listeners[i].OnPostUpdateForces(forceAppliers);
            }
        }
    }
}
