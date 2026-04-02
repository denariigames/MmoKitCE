#if !DISABLE_ADDRESSABLES
using UnityEngine;
using UnityEngine.AddressableAssets;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Insthync.AddressableAssetTools
{
    [System.Serializable]
    public class AssetReferenceAnimationClip : AssetReference
    {
#if UNITY_EDITOR
        public AssetReferenceAnimationClip(AnimationClip scene) : base(AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(scene)))
        {
        }

        public override bool ValidateAsset(string path)
        {
            return ValidateAsset(AssetDatabase.LoadAssetAtPath<AnimationClip>(path));
        }

        public override bool ValidateAsset(Object obj)
        {
            return (obj != null) && (obj is AnimationClip);
        }

        public override bool SetEditorAsset(Object value)
        {
            if (!base.SetEditorAsset(value))
            {
                return false;
            }
            return value is AnimationClip;
        }
#endif
    }
}
#endif