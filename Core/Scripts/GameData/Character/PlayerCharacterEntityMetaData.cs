using Insthync.AddressableAssetTools;
using Insthync.DevExtension;
using UnityEngine;

namespace MultiplayerARPG
{
    [CreateAssetMenu(fileName = GameDataMenuConsts.PLAYER_CHARACTER_ENTITY_METADATA_FILE, menuName = GameDataMenuConsts.PLAYER_CHARACTER_ENTITY_METADATA_MENU, order = GameDataMenuConsts.PLAYER_CHARACTER_ENTITY_METADATA_ORDER)]
    public partial class PlayerCharacterEntityMetaData : BaseGameData
    {
#if UNITY_EDITOR || !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
        [Header("Character Prefabs And Data")]
        [SerializeField]
#if !DISABLE_ADDRESSABLES
        [AddressableAssetConversion(nameof(addressableEntityPrefab))]
#endif
        protected BasePlayerCharacterEntity entityPrefab;
#endif
        public BasePlayerCharacterEntity EntityPrefab
        {
            get
            {
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
                return entityPrefab;
#else
                return null;
#endif
            }
            set
            {
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
                entityPrefab = value;
#endif
            }
        }

#if !DISABLE_ADDRESSABLES
        [SerializeField]
        protected AssetReferenceBasePlayerCharacterEntity addressableEntityPrefab;
        public AssetReferenceBasePlayerCharacterEntity AddressableEntityPrefab
        {
            get { return addressableEntityPrefab; }
            set { addressableEntityPrefab = value; }
        }
#endif

        [Tooltip("This is list which used as choice of character classes when create character")]
        [SerializeField]
        protected PlayerCharacter[] characterDatabases = new PlayerCharacter[0];
        public PlayerCharacter[] CharacterDatabases
        {
            get { return characterDatabases; }
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
            get { return addressableControllerPrefab; }
            set { addressableControllerPrefab = value; }
        }
#endif

        [SerializeField]
        protected CharacterRace race;
        public CharacterRace Race
        {
            get { return race; }
            set { race = value; }
        }

        [Header("FPS Model Settings")]
        [SerializeField]
        protected bool overrideFpsModel;
        public bool OverrideFpsModel
        {
            get { return overrideFpsModel; }
            set { overrideFpsModel = value; }
        }

#if UNITY_EDITOR || !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
        [SerializeField]
#if !DISABLE_ADDRESSABLES
        [AddressableAssetConversion(nameof(addressableFpsModelPrefab))]
#endif
        protected BaseCharacterModel fpsModelPrefab;
#endif
        public BaseCharacterModel FpsModelPrefab
        {
            get
            {
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
                return fpsModelPrefab;
#else
                return null;
#endif
            }
            set
            {
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
                fpsModelPrefab = value;
#endif
            }
        }

#if !DISABLE_ADDRESSABLES
        [SerializeField]
        protected AssetReferenceBaseCharacterModel addressableFpsModelPrefab;
        public AssetReferenceBaseCharacterModel AddressableFpsModelPrefab
        {
            get { return addressableFpsModelPrefab; }
            set { addressableFpsModelPrefab = value; }
        }
#endif

        [SerializeField]
        [Tooltip("Position offsets from fps model container (Camera's transform)")]
        private Vector3 fpsModelPositionOffsets = Vector3.zero;
        public Vector3 FpsModelPositionOffsets
        {
            get { return fpsModelPositionOffsets; }
            set { fpsModelPositionOffsets = value; }
        }

        [SerializeField]
        [Tooltip("Rotation offsets from fps model container (Camera's transform)")]
        private Vector3 fpsModelRotationOffsets = Vector3.zero;
        public Vector3 FpsModelRotationOffsets
        {
            get { return fpsModelRotationOffsets; }
            set { fpsModelRotationOffsets = value; }
        }

        public int GetPlayerCharacterEntityHashAssetId()
        {
            if (EntityPrefab != null)
                return EntityPrefab.HashAssetId;
#if !DISABLE_ADDRESSABLES
            if (AddressableEntityPrefab.IsDataValid())
                return AddressableEntityPrefab.HashAssetId;
#endif
            return 0;
        }

        public void Setup(BasePlayerCharacterEntity entity)
        {
            entity.MetaDataId = DataId;
            this.InvokeInstanceDevExtMethods("Setup", entity);
        }

        public override void PrepareRelatesData()
        {
            base.PrepareRelatesData();
            GameInstance.AddCharacters(CharacterDatabases);
        }
    }
}
