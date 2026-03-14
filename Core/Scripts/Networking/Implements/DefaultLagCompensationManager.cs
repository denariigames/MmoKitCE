using LiteNetLibManager;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    public class DefaultLagCompensationManager : MonoBehaviour, ILagCompensationManager
    {
        [SerializeField]
        private bool isDisabled = false;
        public bool IsDisabled => isDisabled;


        [SerializeField]
        private float snapShotInterval = 0.06f;
        public float SnapShotInterval => snapShotInterval;

        [SerializeField]
        private int maxHistorySize = 16;
        public int MaxHistorySize => maxHistorySize;

        public bool ShouldStoreHitBoxesTransformHistory
        {
            get
            {
                if (isDisabled)
                    return false;
                float time = Time.unscaledTime;
                bool should = time - _lastHistoryStoreTime < SnapShotInterval;
                if (should)
                    _lastHistoryStoreTime = time;
                return should;
            }
        }

        private Dictionary<uint, DamageableEntity> _damageableEntities = new Dictionary<uint, DamageableEntity>();
        private List<DamageableEntity> _simulatedDamageableEntities = new List<DamageableEntity>();
        private float _lastHistoryStoreTime;

        public bool SimulateHitBoxes(long connectionId, long targetTime, Action action)
        {
            if (action == null || !BeginSimlateHitBoxes(connectionId, targetTime))
                return false;
            action.Invoke();
            EndSimulateHitBoxes();
            return true;
        }

        public bool SimulateHitBoxesByHalfRtt(long connectionId, Action action)
        {
            if (action == null || !BeginSimlateHitBoxesByHalfRtt(connectionId))
                return false;
            action.Invoke();
            EndSimulateHitBoxes();
            return true;
        }

        public bool BeginSimlateHitBoxes(long connectionId, long targetTime)
        {
            if (!BaseGameNetworkManager.Singleton.IsServer || !BaseGameNetworkManager.Singleton.ContainsPlayer(connectionId))
                return false;
            LiteNetLibPlayer player = BaseGameNetworkManager.Singleton.GetPlayer(connectionId);
            return InternalBeginSimlateHitBoxes(player, targetTime);
        }

        public bool BeginSimlateHitBoxesByHalfRtt(long connectionId)
        {
            if (!BaseGameNetworkManager.Singleton.IsServer || !BaseGameNetworkManager.Singleton.ContainsPlayer(connectionId))
                return false;
            LiteNetLibPlayer player = BaseGameNetworkManager.Singleton.GetPlayer(connectionId);
            long targetTime = BaseGameNetworkManager.Singleton.ServerTimestamp - (player.Rtt / 2);
            return InternalBeginSimlateHitBoxes(player, targetTime);
        }

        private bool InternalBeginSimlateHitBoxes(LiteNetLibPlayer player, long targetTime)
        {
            foreach (uint subscribingObjectId in player.GetSubscribingObjectIds())
            {
                if (_damageableEntities.ContainsKey(subscribingObjectId))
                {
                    _damageableEntities[subscribingObjectId].RewindHitBoxes(targetTime);
                    if (!_simulatedDamageableEntities.Contains(_damageableEntities[subscribingObjectId]))
                        _simulatedDamageableEntities.Add(_damageableEntities[subscribingObjectId]);
                }
            }
            Physics.SyncTransforms();
            return true;
        }

        public void EndSimulateHitBoxes()
        {
            for (int i = 0; i < _simulatedDamageableEntities.Count; ++i)
            {
                if (_simulatedDamageableEntities[i] != null)
                    _simulatedDamageableEntities[i].RestoreHitBoxes();
            }
            _simulatedDamageableEntities.Clear();
            Physics.SyncTransforms();
        }

        public void AddDamageableEntity(DamageableEntity entity)
        {
            _damageableEntities[entity.ObjectId] = entity;
        }

        public void RemoveDamageableEntity(DamageableEntity entity)
        {
            _damageableEntities.Remove(entity.ObjectId);
        }
    }
}
