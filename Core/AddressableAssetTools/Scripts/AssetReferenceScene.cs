#if !DISABLE_ADDRESSABLES
using UnityEngine;
using UnityEngine.AddressableAssets;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Insthync.AddressableAssetTools
{
    [System.Serializable]
    public class AssetReferenceScene : AssetReference
    {
        public string SceneName
        {
            get { return this.GetSceneName(); }
        }

#if UNITY_EDITOR
        public AssetReferenceScene(SceneAsset scene) : base(AssetDatabase.AssetPathToGUID(AssetDatabase.GetAssetPath(scene)))
        {

        }

        public override bool ValidateAsset(string path)
        {
            return ValidateAsset(AssetDatabase.LoadAssetAtPath<SceneAsset>(path));
        }

        public override bool ValidateAsset(Object obj)
        {
            return (obj != null) && (obj is SceneAsset);
        }

        public override bool SetEditorAsset(Object value)
        {
            if (!base.SetEditorAsset(value))
            {
                return false;
            }
            if (value is SceneAsset scene)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
#endif
    }
}
#endif