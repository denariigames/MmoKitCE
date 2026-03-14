#if !DISABLE_ADDRESSABLES
using UnityEditor;
using UnityEngine;

namespace Insthync.AddressableAssetTools
{
    public class AddAddressableScriptableObjectDependenciesToGroupEditor : BaseAddAddressableToGroupEditor
    {
        [MenuItem("Tools/Addressables/Add Scriptable Object Dependencies to Group By Assets")]
        public static void ShowWindow()
        {
            GetWindow<AddAddressableScriptableObjectDependenciesToGroupEditor>("Add Scriptable Object Dependencies to Group By Assets");
        }

        protected override bool IsTargetAsset(string dependencyPath)
        {
            Object obj = AssetDatabase.LoadAssetAtPath<Object>(dependencyPath);
            bool isScriptableObjectDependencies = obj is ScriptableObject;
            return isScriptableObjectDependencies;
        }
    }
}
#endif