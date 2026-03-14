using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    public static class PhysicFunctionsExtensions
    {
        public static bool IsGameEntityInDistance<T>(this IPhysicFunctions functions, T targetEntity, Vector3 position, float distance, bool includeUnHittable)
            where T : class, ITargetableEntity
        {
            int tempOverlapSize = functions.OverlapObjects(position, distance, 1 << targetEntity.EntityGameObject.layer, hitTriggers: QueryTriggerInteraction.Collide);
            if (tempOverlapSize == 0)
                return false;
            ITargetableEntity tempTargetableEntity;
            for (int tempLoopCounter = 0; tempLoopCounter < tempOverlapSize; ++tempLoopCounter)
            {
                if (!includeUnHittable && !functions.GetOverlapObject(tempLoopCounter).GetComponent<IUnHittable>().IsNull())
                    continue;
                tempTargetableEntity = functions.GetOverlapObject(tempLoopCounter).GetComponent<ITargetableEntity>();
                if (!tempTargetableEntity.IsNull() && tempTargetableEntity.EntityGameObject == targetEntity.EntityGameObject)
                    return true;
            }
            return false;
        }

        public static bool IsGameEntityHitBoxInDistance<T>(this IPhysicFunctions functions, T targetEntity, Vector3 position, float distance, bool includeUnHittable)
            where T : class, IGameEntity
        {
            int tempOverlapSize = functions.OverlapObjects(position, distance, 1 << targetEntity.GetGameObject().layer, hitTriggers: QueryTriggerInteraction.Collide);
            if (tempOverlapSize == 0)
                return false;
            DamageableHitBox tempBaseEntity;
            for (int tempLoopCounter = 0; tempLoopCounter < tempOverlapSize; ++tempLoopCounter)
            {
                if (!includeUnHittable && !functions.GetOverlapObject(tempLoopCounter).GetComponent<IUnHittable>().IsNull())
                    continue;
                tempBaseEntity = functions.GetOverlapObject(tempLoopCounter).GetComponent<DamageableHitBox>();
                if (tempBaseEntity != null && tempBaseEntity.Entity == targetEntity.Entity)
                    return true;
            }
            return false;
        }

        public static List<T> FindGameEntitiesInDistance<T>(this IPhysicFunctions functions, Vector3 position, float distance, int layerMask)
            where T : class, IGameEntity
        {
            List<T> result = new List<T>();
            int tempOverlapSize = functions.OverlapObjects(position, distance, layerMask, hitTriggers: QueryTriggerInteraction.Collide);
            if (tempOverlapSize == 0)
                return result;
            IGameEntity tempBaseEntity;
            T tempEntity;
            for (int tempLoopCounter = 0; tempLoopCounter < tempOverlapSize; ++tempLoopCounter)
            {
                tempBaseEntity = functions.GetOverlapObject(tempLoopCounter).GetComponent<IGameEntity>();
                if (tempBaseEntity.IsNull())
                    continue;
                tempEntity = tempBaseEntity.Entity as T;
                if (tempEntity == null)
                    continue;
                if (result.Contains(tempEntity))
                    continue;
                result.Add(tempEntity);
            }
            return result;
        }
    }
}
