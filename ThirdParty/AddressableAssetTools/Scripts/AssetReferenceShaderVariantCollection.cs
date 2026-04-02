#if !DISABLE_ADDRESSABLES
using UnityEngine;
using UnityEngine.AddressableAssets;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Insthync.AddressableAssetTools
{
    [System.Serializable]
    public class AssetReferenceShaderVariantCollection : AssetReference
    {
#if UNITY_EDITOR
        public AssetReferenceShaderVariantCollection(ShaderVariantCollection scene) : base(AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(scene)))
        {
        }

        public override bool ValidateAsset(string path)
        {
            return ValidateAsset(AssetDatabase.LoadAssetAtPath<ShaderVariantCollection>(path));
        }

        public override bool ValidateAsset(Object obj)
        {
            return (obj != null) && (obj is ShaderVariantCollection);
        }

        public override bool SetEditorAsset(Object value)
        {
            if (!base.SetEditorAsset(value))
            {
                return false;
            }
            return value is ShaderVariantCollection;
        }
#endif
    }
}
#endif