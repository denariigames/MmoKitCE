#if !DISABLE_ADDRESSABLES
using Insthync.AddressableAssetTools;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MultiplayerARPG
{
    [System.Serializable]
    public class AssetReferenceAreaDamageEntity : AssetReferenceComponent<AreaDamageEntity>
    {
        [SerializeField]
        protected int hashAssetId;

        public int HashAssetId
        {
            get { return hashAssetId; }
        }

        public AssetReferenceAreaDamageEntity(string guid) : base(guid)
        {
        }

#if UNITY_EDITOR
        public AssetReferenceAreaDamageEntity(AreaDamageEntity entity) : base(AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(entity)))
        {
            if (entity != null && entity.Identity != null)
            {
                hashAssetId = entity.Identity.HashAssetId;
                Debug.Log($"[AssetReferenceAreaDamageEntity] Set `hashAssetId` to `{hashAssetId}`, name: {entity.name}");
            }
            else
            {
                hashAssetId = 0;
                Debug.LogWarning($"[AssetReferenceAreaDamageEntity] Cannot find entity, so set `hashAssetId` to `0`");
            }
        }
#endif

#if UNITY_EDITOR
        public override bool SetEditorAsset(Object value)
        {
            if (!base.SetEditorAsset(value))
            {
                return false;
            }

            if ((value is GameObject gameObject) && gameObject.TryGetComponent(out AreaDamageEntity entity) && entity.Identity != null)
            {
                hashAssetId = entity.Identity.HashAssetId;
                Debug.Log($"[AssetReferenceAreaDamageEntity] Set `hashAssetId` to `{hashAssetId}` when set editor asset: `{value.name}`");
                return true;
            }
            else
            {
                hashAssetId = 0;
                Debug.LogWarning($"[AssetReferenceAreaDamageEntity] Cannot find entity or not proper object's type, so set `hashAssetId` to `0`");
                return false;
            }
        }
#endif
    }
}
#endif