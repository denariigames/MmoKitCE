using LiteNetLibManager;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    public partial class DefaultHitRegistrationManager : MonoBehaviour, IHitRegistrationManager
    {
        public const int MAX_VALIDATE_QUEUE_SIZE = 16;
        public const int MAX_GLOBAL_VALIDATE_SIZE = 4096;
        public const int MAX_REGISTERING_HITS_SIZE = 4096;
        public const long MAX_VALIDATE_AGE_MS = 15000;

        public float hitValidationBuffer = 2f;
        protected GameObject _hitBoxObject;
        protected Transform _hitBoxTransform;

        protected static readonly Dictionary<string, HitValidateData> s_validatingHits = new Dictionary<string, HitValidateData>();
        protected static readonly Dictionary<uint, Queue<string>> s_removingQueues = new Dictionary<uint, Queue<string>>();
        protected static readonly List<HitRegisterData> s_registeringHits = new List<HitRegisterData>();

        private static readonly List<string> s_tempRemovingValidateIds = new List<string>();
        private static readonly List<uint> s_tempRemovingObjectIds = new List<uint>();

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

            ClearData();
        }

        private static long GetServerTimestamp()
        {
            if (BaseGameNetworkManager.Singleton != null)
                return BaseGameNetworkManager.Singleton.ServerTimestamp;
            return (long)(Time.unscaledTime * 1000f);
        }

        private void AppendValidatingData(uint objectId, string id, HitValidateData hitValidateData)
        {
            if (!s_removingQueues.TryGetValue(objectId, out Queue<string> removingQueue))
                s_removingQueues[objectId] = removingQueue = new Queue<string>();

            while (removingQueue.Count >= MAX_VALIDATE_QUEUE_SIZE)
                RemoveValidateData(removingQueue.Dequeue());

            removingQueue.Enqueue(id);
            s_validatingHits[id] = hitValidateData;
        }

        private static void RemoveValidateData(string id)
        {
            if (string.IsNullOrEmpty(id))
                return;

            if (s_validatingHits.TryGetValue(id, out HitValidateData data) && data != null)
                data.ClearReferences();

            s_validatingHits.Remove(id);
        }

        private static void RemoveValidateDataForObject(uint objectId)
        {
            if (!s_removingQueues.TryGetValue(objectId, out Queue<string> queue))
                return;

            while (queue.Count > 0)
                RemoveValidateData(queue.Dequeue());

            s_removingQueues.Remove(objectId);
        }

        private static void PruneValidationCaches()
        {
            long timestamp = GetServerTimestamp();
            s_tempRemovingValidateIds.Clear();
            s_tempRemovingObjectIds.Clear();

            foreach (KeyValuePair<string, HitValidateData> entry in s_validatingHits)
            {
                HitValidateData data = entry.Value;
                if (data == null || timestamp - data.PreparedServerTimestamp > MAX_VALIDATE_AGE_MS || data.Attacker == null || !data.Attacker)
                    s_tempRemovingValidateIds.Add(entry.Key);
            }

            for (int i = 0; i < s_tempRemovingValidateIds.Count; ++i)
                RemoveValidateData(s_tempRemovingValidateIds[i]);

            foreach (KeyValuePair<uint, Queue<string>> entry in s_removingQueues)
            {
                if (entry.Value == null || entry.Value.Count == 0)
                {
                    s_tempRemovingObjectIds.Add(entry.Key);
                    continue;
                }

                if (BaseGameNetworkManager.Singleton != null &&
                    !BaseGameNetworkManager.Singleton.TryGetEntityByObjectId<BaseGameEntity>(entry.Key, out _))
                {
                    s_tempRemovingObjectIds.Add(entry.Key);
                }
            }

            for (int i = 0; i < s_tempRemovingObjectIds.Count; ++i)
                RemoveValidateDataForObject(s_tempRemovingObjectIds[i]);

            while (s_validatingHits.Count > MAX_GLOBAL_VALIDATE_SIZE)
            {
                string oldestId = null;
                long oldestTimestamp = long.MaxValue;
                foreach (KeyValuePair<string, HitValidateData> entry in s_validatingHits)
                {
                    long prepared = entry.Value != null ? entry.Value.PreparedServerTimestamp : long.MinValue;
                    if (prepared < oldestTimestamp)
                    {
                        oldestTimestamp = prepared;
                        oldestId = entry.Key;
                    }
                }

                if (string.IsNullOrEmpty(oldestId))
                    break;

                RemoveValidateData(oldestId);
            }

            s_tempRemovingValidateIds.Clear();
            s_tempRemovingObjectIds.Clear();
        }

        public HitValidateData GetHitValidateData(BaseGameEntity attacker, int simulateSeed)
        {
            if (attacker == null || !attacker)
                return null;

            string id = HitRegistrationUtils.MakeValidateId(attacker.ObjectId, simulateSeed);
            if (s_validatingHits.TryGetValue(id, out HitValidateData hitValidateData))
            {
                if (hitValidateData == null || hitValidateData.Attacker == null || !hitValidateData.Attacker)
                {
                    RemoveValidateData(id);
                    return null;
                }
                return hitValidateData;
            }
            return null;
        }

        public void PrepareHitRegValidation(BaseGameEntity attacker, int simulateSeed, float[] triggerDurations, byte fireSpread, DamageInfo damageInfo, List<Dictionary<DamageElement, MinMaxFloat>> damageAmounts, WeaponHandlingState weaponHandlingState, CharacterItem weapon, BaseSkill skill, int skillLevel)
        {
            if (attacker == null || !attacker)
                return;

            PruneValidationCaches();

            string id = HitRegistrationUtils.MakeValidateId(attacker.ObjectId, simulateSeed);
            bool appending = false;
            if (!s_validatingHits.TryGetValue(id, out HitValidateData hitValidateData) || hitValidateData == null)
            {
                hitValidateData = new HitValidateData();
                appending = true;
            }
            else
            {
                // Validate IDs can collide/reuse. Never carry old hit objects/counts/validator payload into a new attack.
                hitValidateData.ResetForReuse();
            }

            hitValidateData.Attacker = attacker;
            hitValidateData.AttackerObjectId = attacker.ObjectId;
            hitValidateData.PreparedServerTimestamp = GetServerTimestamp();
            hitValidateData.TriggerDurations = triggerDurations;
            hitValidateData.FireSpread = fireSpread;
            hitValidateData.DamageInfo = damageInfo;
            hitValidateData.DamageAmounts = damageAmounts;
            hitValidateData.WeaponHandlingState = weaponHandlingState;
            hitValidateData.Weapon = weapon;
            hitValidateData.Skill = skill;
            hitValidateData.SkillLevel = skillLevel;

            if (!appending)
                s_validatingHits[id] = hitValidateData;
            else
                AppendValidatingData(attacker.ObjectId, id, hitValidateData);
        }

        public void PrepareHitRegData(HitRegisterData hitRegisterData)
        {
            // Stock implementation does not consume this list. Bound it so combat cannot grow it forever.
            if (s_registeringHits.Count >= MAX_REGISTERING_HITS_SIZE)
                s_registeringHits.RemoveRange(0, s_registeringHits.Count - MAX_REGISTERING_HITS_SIZE + 1);

            s_registeringHits.Add(hitRegisterData);
        }

        public bool PerformValidation(BaseGameEntity attacker, HitRegisterData hitData)
        {
            if (attacker == null || !attacker)
                return false;

            string id = HitRegistrationUtils.MakeValidateId(attacker.ObjectId, hitData.SimulateSeed);
            if (!s_validatingHits.TryGetValue(id, out HitValidateData hitValidateData) || hitValidateData == null)
            {
                Logging.LogError($"Cannot find hit validating data, it must be prepared (then confirm damages, and perform validation later)");
                return false;
            }

            if (hitValidateData.Attacker == null || !hitValidateData.Attacker || hitValidateData.DamageAmounts == null)
            {
                RemoveValidateData(id);
                return false;
            }

            if (hitData.TriggerIndex >= hitValidateData.DamageAmounts.Count)
                return false;

            uint objectId = hitData.HitObjectId;
            string hitObjectId = HitRegistrationUtils.MakeHitObjectId(hitData.TriggerIndex, hitData.SpreadIndex, hitData.HitObjectId);
            if (hitValidateData.HitObjects.Contains(hitObjectId))
                return false;

            int hitBoxIndex = hitData.HitBoxIndex;
            if (BaseGameNetworkManager.Singleton == null ||
                !BaseGameNetworkManager.Singleton.TryGetEntityByObjectId(objectId, out DamageableEntity damageableEntity) ||
                damageableEntity == null || !damageableEntity ||
                damageableEntity.HitBoxes == null ||
                hitBoxIndex < 0 || hitBoxIndex >= damageableEntity.HitBoxes.Length)
            {
                return false;
            }

            DamageableHitBox hitBox = damageableEntity.HitBoxes[hitBoxIndex];
            if (hitBox == null || !hitBox)
                return false;

            if (!hitValidateData.DamageInfo?.IsHitValid(hitValidateData, hitData, hitBox) ?? false)
                return false;

            if (!IsHit(attacker, hitValidateData, hitData, hitBox))
                return false;

            string hitId = HitRegistrationUtils.MakeHitRegId(hitData.TriggerIndex, hitData.SpreadIndex);
            if (!hitValidateData.HitsCount.TryGetValue(hitId, out int hitCount))
                hitCount = 0;
            hitValidateData.HitsCount[hitId] = ++hitCount;

            hitBox.ReceiveDamage(attacker.EntityTransform.position, attacker.GetInfo(), hitValidateData.DamageAmounts[hitData.TriggerIndex], hitValidateData.Weapon, hitValidateData.Skill, hitValidateData.SkillLevel, hitData.SimulateSeed);
            hitValidateData.HitObjects.Add(hitObjectId);
            return true;
        }

        private bool IsHit(BaseGameEntity attacker, HitValidateData hitValidateData, HitRegisterData hitData, DamageableHitBox hitBox)
        {
            if (BaseGameNetworkManager.Singleton == null || hitBox == null || !hitBox || _hitBoxTransform == null)
                return false;

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
            foreach (KeyValuePair<string, HitValidateData> entry in s_validatingHits)
            {
                if (entry.Value != null)
                    entry.Value.ClearReferences();
            }
            s_validatingHits.Clear();
            s_removingQueues.Clear();
            s_registeringHits.Clear();
            s_tempRemovingValidateIds.Clear();
            s_tempRemovingObjectIds.Clear();
        }

        public static int DebugValidatingHitsCount => s_validatingHits.Count;
        public static int DebugRemovingQueuesCount => s_removingQueues.Count;
        public static int DebugRegisteringHitsCount => s_registeringHits.Count;
    }
}
