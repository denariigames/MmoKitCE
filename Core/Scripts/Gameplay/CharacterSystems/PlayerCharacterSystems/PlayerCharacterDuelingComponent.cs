using Insthync.ManagedUpdating;
using LiteNetLib;
using LiteNetLibManager;
using UnityEngine;

namespace MultiplayerARPG
{
    [DisallowMultipleComponent]
    public class PlayerCharacterDuelingComponent : BaseNetworkedGameEntityComponent<BasePlayerCharacterEntity>, IManagedUpdate
    {
        [SerializeField]
        private SyncFieldByte duelingState = new SyncFieldByte();

        protected float _duelingStartTime = -1f;
        public float DuelingStartTime
        {
            get
            {
                return _duelingStartTime;
            }
            set
            {
                _duelingStartTime = value;
                if (IsServer && _duelingStartTime <= 0f)
                    duelingState.Value = (byte)DuelingState.None;
            }
        }
        public bool DuelingStarting
        {
            get
            {
                return (DuelingState)duelingState.Value == DuelingState.Starting;
            }
        }
        public bool DuelingStarted
        {
            get
            {
                return (DuelingState)duelingState.Value == DuelingState.Started;
            }
        }
        public bool DuelingTimeout
        {
            get
            {
                return (DuelingState)duelingState.Value == DuelingState.Timeout;
            }
        }
        public bool DuelingStartingOrStarted
        {
            get
            {
                return DuelingStarting || DuelingStarted;
            }
        }

        public uint DuelingCharacterObjectId { get; private set; }
        private BasePlayerCharacterEntity _duelingCharacter;
        public BasePlayerCharacterEntity DuelingCharacter
        {
            get
            {
                if (!DuelingStarted && Time.unscaledTime - DuelingRequestTime >= CurrentGameInstance.duelingRequestDuration)
                {
                    _duelingCharacter = null;
                    DuelingCharacterObjectId = 0;
                }
                return _duelingCharacter;
            }
            set
            {
                _duelingCharacter = value;
                if (_duelingCharacter == null)
                    DuelingCharacterObjectId = 0;
                else
                    DuelingCharacterObjectId = _duelingCharacter.ObjectId;
                DuelingRequestTime = Time.unscaledTime;
            }
        }


        /// <summary>
        /// Action: BasePlayerCharacterEntity anotherCharacter
        /// </summary>
        public event System.Action<BasePlayerCharacterEntity> onRequestDueling;
        /// <summary>
        /// Action: BasePlayerCharacterEntity anotherCharacter
        /// </summary>
        public event System.Action<BasePlayerCharacterEntity, float, float> onStartDueling;
        /// <summary>
        /// Action: BasePlayerCharacterEntity loserCharacter
        /// </summary>
        public event System.Action<BasePlayerCharacterEntity> onEndDueling;

        public float DuelingRequestTime { get; private set; }

        public bool DisableDueling
        {
            get
            {
                return Entity.IsDead() || CurrentGameInstance.disableDueling || BaseGameNetworkManager.CurrentMapInfo.DisableDueling;
            }
        }

        protected float _countDownDuration;
        protected float _duelDuration;

        private void OnEnable()
        {
            UpdateManager.Register(this);
        }

        private void OnDisable()
        {
            UpdateManager.Unregister(this);
        }

        public void ManagedUpdate()
        {
            if (IsServer && DuelingStartTime > 0)
            {
                float time = Time.unscaledTime;
                if (time - DuelingStartTime > _countDownDuration + _duelDuration)
                {
                    duelingState.Value = (byte)DuelingState.Timeout;
                }
                else if (time - DuelingStartTime > _countDownDuration)
                {
                    duelingState.Value = (byte)DuelingState.Started;
                }
                else
                {
                    duelingState.Value = (byte)DuelingState.Starting;
                }
            }
            if (IsOwnerClient)
            {
                if (DuelingStartingOrStarted && !DuelingTimeout && DuelingCharacterObjectId > 0 && _duelingCharacter == null && Manager.TryGetEntityByObjectId(DuelingCharacterObjectId, out _duelingCharacter))
                {
                    _duelingCharacter.DuelingComponent.DuelingCharacter = Entity;
                    _duelingCharacter.DuelingComponent.DuelingStartTime = DuelingStartTime;
                    _duelingCharacter.DuelingComponent._countDownDuration = _countDownDuration;
                    _duelingCharacter.DuelingComponent._duelDuration = _duelDuration;
                }
            }
        }

        public void ClearDuelingData()
        {
            DuelingStartTime = -1f;
            CancelInvoke(nameof(UpdateDueling));
        }

        public void StopDueling()
        {
            if (DuelingCharacter == null)
            {
                ClearDuelingData();
                return;
            }
            // Set dueling state/data for co player character entity
            DuelingCharacter.DuelingComponent.ClearDuelingData();
            DuelingCharacter.DuelingComponent.DuelingCharacter = null;
            // Set dueling state/data for player character entity
            ClearDuelingData();
            DuelingCharacter = null;
        }

        public bool CallCmdSendDuelingRequest(uint objectId)
        {
            if (DisableDueling)
            {
                ClientGenericActions.ClientReceiveGameMessage(UITextKeys.UI_ERROR_FEATURE_IS_DISABLED);
                return false;
            }
            RPC(CmdSendDuelingRequest, Identity.DefaultRpcChannelId, DeliveryMethod.ReliableUnordered, objectId);
            return true;
        }

        [ServerRpc]
        protected void CmdSendDuelingRequest(uint objectId)
        {
#if UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES
            if (DisableDueling)
            {
                GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, UITextKeys.UI_ERROR_FEATURE_IS_DISABLED);
                return;
            }
            if (!Manager.TryGetEntityByObjectId(objectId, out BasePlayerCharacterEntity targetCharacterEntity))
            {
                GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, UITextKeys.UI_ERROR_CHARACTER_NOT_FOUND);
                return;
            }
            if (targetCharacterEntity.DuelingComponent.DuelingCharacter != null)
            {
                GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, UITextKeys.UI_ERROR_CHARACTER_IS_DUELING);
                return;
            }
            if (!Entity.IsGameEntityInDistance(targetCharacterEntity))
            {
                GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, UITextKeys.UI_ERROR_CHARACTER_IS_TOO_FAR);
                return;
            }
            if (targetCharacterEntity.IsInSafeArea)
            {
                GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, UITextKeys.UI_ERROR_CHARACTER_IS_IN_SAFE_AREA);
                return;
            }
            DuelingCharacter = targetCharacterEntity;
            targetCharacterEntity.DuelingComponent.DuelingCharacter = Entity;
            // Send receive dueling request to player
            DuelingCharacter.DuelingComponent.CallOwnerReceiveDuelingRequest(ObjectId);
#endif
        }

        public bool CallOwnerReceiveDuelingRequest(uint objectId)
        {
            RPC(TargetReceiveDuelingRequest, Identity.DefaultRpcChannelId, DeliveryMethod.ReliableUnordered, ConnectionId, objectId);
            return true;
        }

        [TargetRpc]
        protected void TargetReceiveDuelingRequest(uint objectId)
        {
            BasePlayerCharacterEntity playerCharacterEntity;
            if (!Manager.TryGetEntityByObjectId(objectId, out playerCharacterEntity))
                return;
            if (onRequestDueling != null)
                onRequestDueling.Invoke(playerCharacterEntity);
        }

        public bool CallCmdAcceptDuelingRequest()
        {
            RPC(CmdAcceptDuelingRequest, Identity.DefaultRpcChannelId, DeliveryMethod.ReliableUnordered);
            return true;
        }

        [ServerRpc]
        protected void CmdAcceptDuelingRequest()
        {
#if UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES
            if (DuelingCharacter == null)
            {
                GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, UITextKeys.UI_ERROR_CANNOT_ACCEPT_DUELING_REQUEST);
                StopDueling();
                return;
            }
            if (!Entity.IsGameEntityInDistance(DuelingCharacter))
            {
                GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, UITextKeys.UI_ERROR_CHARACTER_IS_TOO_FAR);
                StopDueling();
                return;
            }
            if (Entity.IsInSafeArea)
            {
                GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, UITextKeys.UI_ERROR_CHARACTER_IS_IN_SAFE_AREA);
                StopDueling();
                return;
            }
            float countDownDuration = CurrentGameInstance.duelingCountDownDuration;
            float duelDuration = CurrentGameInstance.duelingDuration;
            // Set dueling state/data for co player character entity
            DuelingCharacter.DuelingComponent.ClearDuelingData();
            DuelingCharacter.DuelingComponent.StartDueling(countDownDuration, duelDuration);
            DuelingCharacter.DuelingComponent.CallOwnerAcceptedDuelingRequest(ObjectId, countDownDuration, duelDuration);
            // Set dueling state/data for player character entity
            ClearDuelingData();
            StartDueling(countDownDuration, duelDuration);
            CallOwnerAcceptedDuelingRequest(DuelingCharacter.ObjectId, countDownDuration, duelDuration);
#endif
        }

        public bool CallCmdDeclineDuelingRequest()
        {
            RPC(CmdDeclineDuelingRequest, Identity.DefaultRpcChannelId, DeliveryMethod.ReliableUnordered);
            return true;
        }

        [ServerRpc]
        protected void CmdDeclineDuelingRequest()
        {
#if UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES
            if (DuelingCharacter != null)
                GameInstance.ServerGameMessageHandlers.SendGameMessage(DuelingCharacter.ConnectionId, UITextKeys.UI_ERROR_DUELING_REQUEST_DECLINED);
            GameInstance.ServerGameMessageHandlers.SendGameMessage(ConnectionId, UITextKeys.UI_ERROR_DUELING_REQUEST_DECLINED);
            StopDueling();
#endif
        }

        public bool CallOwnerAcceptedDuelingRequest(uint objectId, float countDownDuration, float duelDuration)
        {
            RPC(TargetAcceptedDuelingRequest, ConnectionId, objectId, countDownDuration, duelDuration);
            return true;
        }

        [TargetRpc]
        protected void TargetAcceptedDuelingRequest(uint objectId, float countDownDuration, float duelDuration)
        {
            BasePlayerCharacterEntity playerCharacterEntity;
            if (!Manager.TryGetEntityByObjectId(objectId, out playerCharacterEntity))
                return;
            if (!IsServer)
            {
                // Already setup in accept request function, so don't setup again
                DuelingCharacter = playerCharacterEntity;
                DuelingCharacter.DuelingComponent.DuelingCharacter = Entity;
                DuelingCharacter.DuelingComponent.ClearDuelingData();
                DuelingCharacter.DuelingComponent.StartDueling(countDownDuration, duelDuration);
                ClearDuelingData();
                StartDueling(countDownDuration, duelDuration);
            }
            if (onStartDueling != null)
                onStartDueling.Invoke(playerCharacterEntity, countDownDuration, duelDuration);
        }

        public bool CallOwnerEndDueling(uint loserObjectId)
        {
            RPC(TargetEndDueling, ConnectionId, loserObjectId);
            return true;
        }

        [TargetRpc]
        protected void TargetEndDueling(uint loserObjectId)
        {
            BasePlayerCharacterEntity playerCharacterEntity;
            if (!Manager.TryGetEntityByObjectId(loserObjectId, out playerCharacterEntity))
                playerCharacterEntity = null;
            StopDueling();
            if (onEndDueling != null)
                onEndDueling.Invoke(playerCharacterEntity);
        }

        protected void StartDueling(float countDownDuration, float duelDuration)
        {
            DuelingStartTime = Time.unscaledTime;
            _countDownDuration = countDownDuration;
            _duelDuration = duelDuration;
            if (IsServer)
            {
                CancelInvoke(nameof(UpdateDueling));
                InvokeRepeating(nameof(UpdateDueling), 0f, 1f);
            }
        }

        protected void UpdateDueling()
        {
            if (DuelingTimeout)
            {
                EndDueling(null);
                return;
            }

            if (Entity.IsInSafeArea)
            {
                EndDueling(Entity);
                return;
            }
        }

        public void EndDueling(BasePlayerCharacterEntity loser)
        {
            uint loserObjectId = loser != null ? loser.ObjectId : 0;
            if (DuelingCharacter != null)
                DuelingCharacter.DuelingComponent.CallOwnerEndDueling(loserObjectId);
            CallOwnerEndDueling(loserObjectId);
            StopDueling();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            // Player disconnect?
            if (IsServer && DuelingStarted)
                EndDueling(Entity);
        }
    }
}
