using Cysharp.Threading.Tasks;
using Insthync.AddressableAssetTools;
using Insthync.UnityEditorUtils;
using UnityEngine;
#if !DISABLE_ADDRESSABLES
using UnityEngine.AddressableAssets;
#endif
using UnityEngine.Serialization;

namespace MultiplayerARPG
{
    [System.Serializable]
    public class EquipmentModel : IAddressableAssetConversable
    {
        [Header("Generic Settings")]
        public string equipSocket;

        [Header("Prefab Settings")]
        [Tooltip("Turn it on to use instantiated object which is a child of character model")]
        [FormerlySerializedAs("useInstantiatedObject")]
        public bool useInstantiatedObject;

#if UNITY_EDITOR || !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
        [BoolShowConditional(nameof(useInstantiatedObject), false)]
        [SerializeField]
        [FormerlySerializedAs("model")]
        protected GameObject meshPrefab = null;
#endif
        public GameObject MeshPrefab
        {
            get
            {
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
                return meshPrefab;
#else
                return null;
#endif
            }
        }

#if !DISABLE_ADDRESSABLES
        [BoolShowConditional(nameof(useInstantiatedObject), false)]
        [SerializeField]
        protected AssetReferenceGameObject addressableMeshPrefab = null;
        public AssetReferenceGameObject AddressableMeshPrefab
        {
            get { return addressableMeshPrefab; }
        }
#endif

        [BoolShowConditional(nameof(useInstantiatedObject), true)]
        public int instantiatedObjectIndex;
        public byte priority;

        [Header("Skinned Mesh Settings")]
        [Tooltip("Turn it on to not use bones from entities if this mesh is skinned mesh")]
        [FormerlySerializedAs("doNotUseEntityBones")]
        public bool doNotSetupBones;
        [Tooltip("Leave this empty, to use `EquipmentModelBonesSetupManager` from `GameInstance`")]
        [BoolShowConditional(nameof(doNotSetupBones), false)]
        public BaseEquipmentModelBonesSetupManager equipmentModelBonesSetupManager;

        [Header("Transform Settings")]
        public Vector3 localPosition = Vector3.zero;
        public Vector3 localEulerAngles = Vector3.zero;
        [Tooltip("Turn it on to not change object scale when it is instantiated to character's hands (or other part of body)")]
        public bool doNotChangeScale;
        [BoolShowConditional(nameof(doNotChangeScale), false)]
        public Vector3 localScale = Vector3.one;

        [Header("Weapon Sheath Settings")]
        public bool useSpecificSheathEquipWeaponSet;
        public byte specificSheathEquipWeaponSet;

        #region These variables will be used at runtime, do not make changes in editor
        [System.NonSerialized]
        public int indexOfModel = -1;
        [System.NonSerialized]
        public string equipPosition;
        [System.NonSerialized]
        public CharacterItem item;
        [System.NonSerialized]
        public EquipmentModelDelegate onInstantiated;
        #endregion

        public UniTask<GameObject> GetMeshPrefab()
        {
#if !DISABLE_ADDRESSABLES
            return AddressableMeshPrefab.GetOrLoadAssetAsyncOrUsePrefab(MeshPrefab);
#else
            return UniTask.FromResult(MeshPrefab);
#endif
        }

        public EquipmentModel SetMeshPrefab(GameObject meshPrefab)
        {
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
            this.meshPrefab = meshPrefab;
#endif
            return this;
        }

        public EquipmentModel Clone()
        {
            return new EquipmentModel()
            {
                // Generic Settings
                equipSocket = equipSocket,
                // Prefab Settings
                useInstantiatedObject = useInstantiatedObject,
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
                meshPrefab = meshPrefab,
#endif
#if !DISABLE_ADDRESSABLES
                addressableMeshPrefab = addressableMeshPrefab,
#endif
                instantiatedObjectIndex = instantiatedObjectIndex,
                priority = priority,
                // Skinned Mesh Settings
                doNotSetupBones = doNotSetupBones,
                equipmentModelBonesSetupManager = equipmentModelBonesSetupManager,
                // Transform Settings
                localPosition = localPosition,
                localEulerAngles = localEulerAngles,
                doNotChangeScale = doNotChangeScale,
                localScale = localScale,
                // Weapon Sheath Settings
                useSpecificSheathEquipWeaponSet = useSpecificSheathEquipWeaponSet,
                specificSheathEquipWeaponSet = specificSheathEquipWeaponSet,
                // Runtime only data
                indexOfModel = indexOfModel,
                equipPosition = equipPosition,
                item = item,
                onInstantiated = onInstantiated,
            };
        }

        public void InvokeOnInstantiated(GameObject modelObject, BaseEquipmentEntity equipmentEntity, EquipmentInstantiatedObjectGroup instantiatedObjectGroup, EquipmentContainer equipmentContainer)
        {
            if (onInstantiated != null)
                onInstantiated.Invoke(this, modelObject, equipmentEntity, instantiatedObjectGroup, equipmentContainer);
        }

        public void ProceedAddressableAssetConversion(string groupName)
        {
#if UNITY_EDITOR && !DISABLE_ADDRESSABLES
            AddressableEditorUtils.ConvertObjectRefToAddressable(ref meshPrefab, ref addressableMeshPrefab, groupName);
#endif
        }
    }
}
