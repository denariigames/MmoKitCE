using LiteNetLibManager;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    public partial class DefaultHitRegistrationManager : MonoBehaviour, IHitRegistrationManager
    {
        public const int MAX_VALIDATE_QUEUE_SIZE = 16;

        public float hitValidationBuffer = 2f;
        protected GameObject _hitBoxObject;
        protected Transform _hitBoxTransform;

        protected static readonly Dictionary<string, HitValidateData> s_validatingHits = new Dictionary<string, HitValidateData>();
        protected static readonly Dictionary<uint, Queue<string>> s_removingQueues = new Dictionary<uint, Queue<string>>();
        protected static readonly List<HitRegisterData> s_registeringHits = new List<HitRegisterData>();

        void Start()
        {
            _hitBoxObject = new GameObject("_testHitBox");
            _hitBoxTransform = _hitBoxObject.transform;
            _hitBoxTransform.parent = transform;
        }

        void OnDestroy()
        {
            if (_hitBoxObject != null)
                Destroy(_hitBoxObject);
        }

        private void AppendValidatingData(uint objectId, string id, HitValidateData hitValidateData)
        {
            Queue<string> removingQueue;
            if (!s_removingQueues.TryGetValue(objectId, out removingQueue))
                s_removingQueues[objectId] = removingQueue = new Queue<string>();
            while (removingQueue.Count >= MAX_VALIDATE_QUEUE_SIZE)
            {
                s_validatingHits.Remove(removingQueue.Dequeue());
            }
            removingQueue.Enqueue(id);
            s_validatingHits[id] = hitValidateData;
        }

        public HitValidateData GetHitValidateData(BaseGameEntity attacker, int simulateSeed)
        {
            string id = HitRegistrationUtils.MakeValidateId(attacker.ObjectId, simulateSeed);
            if (s_validatingHits.TryGetValue(id, out HitValidateData hitValidateData))
                return hitValidateData;
            return null;
        }

        public void PrepareHitRegValidation(BaseGameEntity attacker, int simulateSeed, float[] triggerDurations, byte fireSpread, DamageInfo damageInfo, List<Dictionary<DamageElement, MinMaxFloat>> damageAmounts, WeaponHandlingState weaponHandlingState, CharacterItem weapon, BaseSkill skill, int skillLevel)
        {
            string id = HitRegistrationUtils.MakeValidateId(attacker.ObjectId, simulateSeed);
            bool appending = false;
            if (!s_validatingHits.TryGetValue(id, out HitValidateData hitValidateData))
            {
                hitValidateData = new HitValidateData();
                appending = true;
            }

            hitValidateData.Attacker = attacker;
            hitValidateData.TriggerDurations = triggerDurations;
            hitValidateData.FireSpread = fireSpread;
            hitValidateData.DamageInfo = damageInfo;
            hitValidateData.DamageAmounts = damageAmounts;
            hitValidateData.WeaponHandlingState = weaponHandlingState;
            hitValidateData.Weapon = weapon;
            hitValidateData.Skill = skill;
            hitValidateData.SkillLevel = skillLevel;
            if (!appending)
            {
                // Just update the data
                s_validatingHits[id] = hitValidateData;
            }
            else
            {
                // Addpend new data
                AppendValidatingData(attacker.ObjectId, id, hitValidateData);
            }
        }

        public void PrepareHitRegData(HitRegisterData hitRegisterData)
        {
            s_registeringHits.Add(hitRegisterData);
        }

        public bool PerformValidation(BaseGameEntity attacker, HitRegisterData hitData)
        {
            if (attacker == null)
                return false;

            string id = HitRegistrationUtils.MakeValidateId(attacker.ObjectId, hitData.SimulateSeed);
            if (!s_validatingHits.TryGetValue(id, out HitValidateData hitValidateData))
            {
                // No validating data
                Logging.LogError($"Cannot find hit validating data, it must be prepared (then confirm damages, and perform validation later)");
                return false;
            }

            if (hitData.TriggerIndex >= hitValidateData.DamageAmounts.Count)
            {
                // No damage applied (may not have enough ammo)
                return false;
            }

            uint objectId = hitData.HitObjectId;
            string hitObjectId = HitRegistrationUtils.MakeHitObjectId(hitData.TriggerIndex, hitData.SpreadIndex, hitData.HitObjectId);
            if (hitValidateData.HitObjects.Contains(hitObjectId))
            {
                // Already hit
                return false;
            }

            int hitBoxIndex = hitData.HitBoxIndex;
            if (!BaseGameNetworkManager.Singleton.TryGetEntityByObjectId(objectId, out DamageableEntity damageableEntity) ||
                hitBoxIndex < 0 || hitBoxIndex >= damageableEntity.HitBoxes.Length)
            {
                // Can't find target or invalid hitbox
                return false;
            }

            DamageableHitBox hitBox = SelectValidatedHitBox(attacker, hitValidateData, hitData, damageableEntity, hitBoxIndex);
            if (hitBox == null)
                return false;

            string hitId = HitRegistrationUtils.MakeHitRegId(hitData.TriggerIndex, hitData.SpreadIndex);
            if (!hitValidateData.HitsCount.TryGetValue(hitId, out int hitCount))
            {
                // Set hit count to 0, if it is not in collection
                hitCount = 0;
            }
            hitValidateData.HitsCount[hitId] = ++hitCount;

            // Yes, it is hit
            hitBox.ReceiveDamage(attacker.EntityTransform.position, attacker.GetInfo(), hitValidateData.DamageAmounts[hitData.TriggerIndex], hitValidateData.Weapon, hitValidateData.Skill, hitValidateData.SkillLevel, hitData.SimulateSeed);
            hitValidateData.HitObjects.Add(hitObjectId);
            return true;
        }

        private DamageableHitBox SelectValidatedHitBox(BaseGameEntity attacker, HitValidateData hitValidateData, HitRegisterData hitData, DamageableEntity damageableEntity, int requestedHitBoxIndex)
        {
            EntityInfo attackerInfo = attacker.GetInfo();

            bool IsHitBoxValidAndHittable(DamageableHitBox candidate)
            {
                if (candidate == null)
                    return false;
                if (!candidate.CanReceiveDamageFrom(attackerInfo))
                    return false;
                if (!hitValidateData.DamageInfo?.IsHitValid(hitValidateData, hitData, candidate) ?? false)
                    return false;
                return IsHit(attacker, hitValidateData, hitData, candidate);
            }

            // Use requested index when valid and hittable.
            if (requestedHitBoxIndex >= 0 && requestedHitBoxIndex < damageableEntity.HitBoxes.Length)
            {
                DamageableHitBox indexedHitBox = damageableEntity.HitBoxes[requestedHitBoxIndex];
                if (IsHitBoxValidAndHittable(indexedHitBox))
                    return indexedHitBox;
            }

            // Fallback: pick nearest hittable+valid hitbox by reported hit point.
            Vector3 referencePoint = hitData.HitDestination ?? hitData.HitOrigin;
            DamageableHitBox bestHitBox = null;
            float bestDistanceSqr = float.MaxValue;
            for (int i = 0; i < damageableEntity.HitBoxes.Length; ++i)
            {
                DamageableHitBox candidate = damageableEntity.HitBoxes[i];
                if (!IsHitBoxValidAndHittable(candidate))
                    continue;
                Vector3 candidatePosition = candidate.CacheTransform != null ? candidate.CacheTransform.position : candidate.transform.position;
                float distanceSqr = (candidatePosition - referencePoint).sqrMagnitude;
                if (distanceSqr >= bestDistanceSqr)
                    continue;
                bestDistanceSqr = distanceSqr;
                bestHitBox = candidate;
            }
            return bestHitBox;
        }

        private bool IsHit(BaseGameEntity attacker, HitValidateData hitValidateData, HitRegisterData hitData, DamageableHitBox hitBox)
        {
            long timestamp = BaseGameNetworkManager.Singleton.ServerTimestamp;
            long halfRtt = attacker.Player != null ? (attacker.Player.Rtt / 2) : 0;
            long targetTime = timestamp - halfRtt;
            DamageableHitBox.TransformHistory transformHistory = hitBox.GetTransformHistory(timestamp, targetTime);
            _hitBoxTransform.position = transformHistory.Bounds.center;
            _hitBoxTransform.rotation = transformHistory.Rotation;
            Vector3 alignedHitPoint = _hitBoxTransform.InverseTransformPoint(hitData.HitOrigin);
            float maxExtents = Mathf.Max(transformHistory.Bounds.extents.x, transformHistory.Bounds.extents.y, transformHistory.Bounds.extents.z);
            return Vector3.Distance(Vector3.zero, alignedHitPoint) <= maxExtents + hitValidationBuffer;
        }

        public void ClearData()
        {
            s_validatingHits.Clear();
            s_removingQueues.Clear();
            s_registeringHits.Clear();
        }
    }
}
