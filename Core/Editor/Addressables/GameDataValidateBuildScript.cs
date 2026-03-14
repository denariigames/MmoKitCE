#if !DISABLE_ADDRESSABLES
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Build.DataBuilders;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;

namespace MultiplayerARPG
{
    [CreateAssetMenu(fileName = "GameDataValidateBuildScript.asset", menuName = "Addressables/Content Builders/GameDataValidateBuildScript")]
    public class GameDataValidateBuildScript : BuildScriptPackedMode
    {
        public override string Name
        {
            get { return "GameDataValidateBuildScript"; }
        }

        protected override TResult BuildDataImplementation<TResult>(AddressablesDataBuilderInput builderInput)
        {
            // Preprocess Addressable assets here
            AddressableAssetSettings settings = AddressableAssetSettingsDefaultObject.Settings;
            if (settings != null)
            {
                Debug.Log("Validating game data's hash asset ID from addressable assets");
                foreach (AddressableAssetGroup group in settings.groups)
                {
                    foreach (AddressableAssetEntry entry in group.entries)
                    {
                        if (entry.MainAsset is BaseGameData gameData)
                        {
                            if (gameData.ValidateHashAssetID())
                                EditorUtility.SetDirty(gameData);
                        }
                    }
                }
            }
            // Continue with the normal build process
            return base.BuildDataImplementation<TResult>(builderInput);
        }
    }
}
#endif