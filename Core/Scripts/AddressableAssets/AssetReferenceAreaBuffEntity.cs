#if !DISABLE_ADDRESSABLES
using Insthync.AddressableAssetTools;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MultiplayerARPG
{
    [System.Serializable]
    public class AssetReferenceAreaBuffEntity : AssetReferenceComponent<AreaBuffEntity>
    {
        [SerializeField]
        protected int hashAssetId;

        public int HashAssetId
        {
            get { return hashAssetId; }
        }

        public AssetReferenceAreaBuffEntity(string guid) : base(guid)
        {
        }

#if UNITY_EDITOR
        public AssetReferenceAreaBuffEntity(AreaBuffEntity entity) : base(AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(entity)))
        {
            if (entity != null && entity.Identity != null)
            {
                hashAssetId = entity.Identity.HashAssetId;
                Debug.Log($"[AssetReferenceAreaBuffEntity] Set `hashAssetId` to `{hashAssetId}`, name: {entity.name}");
            }
            else
            {
                hashAssetId = 0;
                Debug.LogWarning($"[AssetReferenceAreaBuffEntity] Cannot find entity, so set `hashAssetId` to `0`");
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

            if ((value is GameObject gameObject) && gameObject.TryGetComponent(out AreaBuffEntity entity) && entity.Identity != null)
            {
                hashAssetId = entity.Identity.HashAssetId;
                Debug.Log($"[AssetReferenceAreaBuffEntity] Set `hashAssetId` to `{hashAssetId}` when set editor asset: `{value.name}`");
                return true;
            }
            else
            {
                hashAssetId = 0;
                Debug.LogWarning($"[AssetReferenceAreaBuffEntity] Cannot find entity or not proper object's type, so set `hashAssetId` to `0`");
                return false;
            }
        }
#endif
    }
}
#endif