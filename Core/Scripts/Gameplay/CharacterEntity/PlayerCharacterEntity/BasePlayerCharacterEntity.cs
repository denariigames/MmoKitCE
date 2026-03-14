using Insthync.UnityEditorUtils;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Serialization;

namespace MultiplayerARPG
{
    [RequireComponent(typeof(PlayerCharacterItemLockAndExpireComponent))]
    [RequireComponent(typeof(PlayerCharacterNpcActionComponent))]
    public abstract partial class BasePlayerCharacterEntity : BaseCharacterEntity, IPlayerCharacterData, IActivatableEntity
    {
        protected static readonly ProfilerMarker s_UpdateProfilerMarker = new ProfilerMarker("BasePlayerCharacterEntity - Update");

        [Category("Character Settings")]
        [Tooltip("This is list which used as choice of character classes when create character")]
        [SerializeField]
        [FormerlySerializedAs("playerCharacters")]
        protected PlayerCharacter[] characterDatabases = new PlayerCharacter[0];
        public PlayerCharacter[] CharacterDatabases
        {
            get
            {
                if (TryGetMetaData(out PlayerCharacterEntityMetaData metaData))
                    return metaData.CharacterDatabases;
                return characterDatabases;
            }
            set { characterDatabases = value; }
        }

#if UNITY_EDITOR || !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
        [Tooltip("Leave this empty to use GameInstance's controller prefab")]
        [SerializeField]
        protected BasePlayerCharacterController controllerPrefab;
#endif
        public BasePlayerCharacterController ControllerPrefab
        {
            get
            {
#if UNITY_EDITOR || !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
                if (TryGetMetaData(out PlayerCharacterEntityMetaData metaData))
                    return metaData.ControllerPrefab;
                return controllerPrefab;
#else
                return null;
#endif
            }
            set
            {
#if UNITY_EDITOR || !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
                controllerPrefab = value;
#endif
            }
        }

#if !DISABLE_ADDRESSABLES
        [Tooltip("Leave this empty to use GameInstance's controller prefab")]
        [SerializeField]
        protected AssetReferenceBasePlayerCharacterController addressableControllerPrefab;
        public AssetReferenceBasePlayerCharacterController AddressableControllerPrefab
        {
            get
            {
                if (TryGetMetaData(out PlayerCharacterEntityMetaData metaData))
                    return metaData.AddressableControllerPrefab;
                return addressableControllerPrefab;
            }
            set { addressableControllerPrefab = value; }
        }
#endif

        public PlayerCharacterItemLockAndExpireComponent ItemLockAndExpireComponent
        {
            get; private set;
        }

        public PlayerCharacterNpcActionComponent NpcActionComponent
        {
            get; private set;
        }

        public PlayerCharacterBuildingComponent BuildingComponent
        {
            get; private set;
        }

        public PlayerCharacterCraftingComponent CraftingComponent
        {
            get; private set;
        }

        public PlayerCharacterDealingComponent DealingComponent
        {
            get; private set;
        }

        public PlayerCharacterDuelingComponent DuelingComponent
        {
            get; private set;
        }

        public PlayerCharacterVendingComponent VendingComponent
        {
            get; private set;
        }

        public PlayerCharacterPkComponent PkComponent
        {
            get; private set;
        }

        public override CharacterRace Race
        {
            get
            {
                if (TryGetMetaData(out PlayerCharacterEntityMetaData metaData))
                    return metaData.Race;
                return base.Race;
            }
            set { base.Race = value; }
        }

        public bool TryGetMetaData(out PlayerCharacterEntityMetaData metaData)
        {
            metaData = null;
            if (MetaDataId == 0 || !GameInstance.PlayerCharacterEntityMetaDataList.TryGetValue(MetaDataId, out metaData))
                return false;
            return true;
        }

        public int IndexOfCharacterDatabase(int dataId)
        {
            for (int i = 0; i < CharacterDatabases.Length; ++i)
            {
                if (CharacterDatabases[i] != null && CharacterDatabases[i].DataId == dataId)
                    return i;
            }
            return -1;
        }

        public override void PrepareRelatesData()
        {
            base.PrepareRelatesData();
            GameInstance.AddCharacters(CharacterDatabases);
        }

        public override EntityInfo GetInfo()
        {
            return _info.SetEntityInfo(
                EntityTypes.Player,
                ObjectId,
                Id,
                SubChannelId,
                DataId,
                FactionId,
                PartyId,
                GuildId,
                IsInSafeArea,
                this,
                null);
        }

        protected override void EntityAwake()
        {
            base.EntityAwake();
            gameObject.tag = CurrentGameInstance.playerTag;
            gameObject.layer = CurrentGameInstance.playerLayer;
        }

        public override void OnSetOwnerClient(bool isOwnerClient)
        {
            base.OnSetOwnerClient(isOwnerClient);
            gameObject.layer = isOwnerClient ? CurrentGameInstance.playingLayer : CurrentGameInstance.playerLayer;
        }

        public override void InitialRequiredComponents()
        {
            CurrentGameInstance.EntitySetting.InitialPlayerCharacterEntityComponents(this);
            base.InitialRequiredComponents();
            ItemLockAndExpireComponent = gameObject.GetComponent<PlayerCharacterItemLockAndExpireComponent>();
            NpcActionComponent = gameObject.GetComponent<PlayerCharacterNpcActionComponent>();
            BuildingComponent = gameObject.GetComponent<PlayerCharacterBuildingComponent>();
            CraftingComponent = gameObject.GetComponent<PlayerCharacterCraftingComponent>();
            DealingComponent = gameObject.GetComponent<PlayerCharacterDealingComponent>();
            DuelingComponent = gameObject.GetComponent<PlayerCharacterDuelingComponent>();
            VendingComponent = gameObject.GetComponent<PlayerCharacterVendingComponent>();
            PkComponent = gameObject.GetComponent<PlayerCharacterPkComponent>();
        }

        protected override void EntityUpdate()
        {
            base.EntityUpdate();
            using (s_UpdateProfilerMarker.Auto())
            {
                if (this.IsDead())
                {
                    StopMove();
                    SetTargetEntity(null);
                    return;
                }
            }
        }

        public virtual float GetActivatableDistance()
        {
            return GameInstance.Singleton.conversationDistance;
        }

        public virtual bool ShouldClearTargetAfterActivated()
        {
            return false;
        }

        public virtual bool ShouldBeAttackTarget()
        {
            return !IsOwnerClient && !this.IsDeadOrHideFrom(GameInstance.PlayingCharacterEntity) && CanReceiveDamageFrom(GameInstance.PlayingCharacterEntity.GetInfo());
        }

        public virtual bool ShouldNotActivateAfterFollowed()
        {
            return true;
        }

        public virtual bool CanActivate()
        {
            return !IsOwnerClient;
        }

        public virtual void OnActivate()
        {
            BaseUISceneGameplay.Singleton.SetActivePlayerCharacter(this);
        }
    }
}
