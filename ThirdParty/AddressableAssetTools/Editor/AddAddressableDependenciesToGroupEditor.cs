#if !DISABLE_ADDRESSABLES
using UnityEditor;

namespace Insthync.AddressableAssetTools
{
    public class AddAddressableDependenciesToGroupEditor : BaseAddAddressableToGroupEditor
    {
        [MenuItem("Tools/Addressables/Add Dependencies to Group")]
        public static void ShowWindow()
        {
            GetWindow<AddAddressableDependenciesToGroupEditor>("Add Dependencies to Group");
        }

        protected override bool IsTargetAsset(string dependencyPath)
        {
            return true;
        }
    }
}
#endif