#if !DISABLE_ADDRESSABLES
using UnityEngine;
using UnityEngine.AddressableAssets;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Insthync.AddressableAssetTools
{
    [System.Serializable]
    public class AssetReferenceAvatarMask : AssetReference
    {
#if UNITY_EDITOR
        public AssetReferenceAvatarMask(AvatarMask scene) : base(AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(scene)))
        {
        }

        public override bool ValidateAsset(string path)
        {
            return ValidateAsset(AssetDatabase.LoadAssetAtPath<AvatarMask>(path));
        }

        public override bool ValidateAsset(Object obj)
        {
            return (obj != null) && (obj is AvatarMask);
        }

        public override bool SetEditorAsset(Object value)
        {
            if (!base.SetEditorAsset(value))
            {
                return false;
            }
            return value is AvatarMask;
        }
#endif
    }
}
#endif