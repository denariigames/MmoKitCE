using Insthync.AddressableAssetTools;
using UnityEngine;

namespace MultiplayerARPG
{
    [System.Serializable]
    public class BuffMount : IAddressableAssetConversable
    {
#if UNITY_EDITOR || !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
        [Tooltip("Leave `Mount Entity` to NULL to not summon mount entity")]
        [SerializeField]
        private VehicleEntity mountEntity;
#endif
        public VehicleEntity MountEntity
        {
            get
            {
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
                return mountEntity;
#else
                return null;
#endif
            }
        }

#if !DISABLE_ADDRESSABLES
        [SerializeField]
        private AssetReferenceVehicleEntity addressableMountEntity;
        public AssetReferenceVehicleEntity AddressableMountEntity
        {
            get
            {
                return addressableMountEntity;
            }
        }
#endif

        [SerializeField]
        private IncrementalInt level;
        public IncrementalInt Level { get { return level; } }

        public void ProceedAddressableAssetConversion(string groupName)
        {
#if UNITY_EDITOR && !DISABLE_ADDRESSABLES
            AddressableEditorUtils.ConvertObjectRefToAddressable(ref mountEntity, ref addressableMountEntity, groupName);
#endif
        }
    }
}