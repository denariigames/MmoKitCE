using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace MultiplayerARPG
{
    public class GameDataValidatePreprocessBuild : IPreprocessBuildWithReport
    {
        // This determines the order in which this callback is called relative to others.
        public int callbackOrder => 0;

        // This method is called before the build process starts.
        public void OnPreprocessBuild(BuildReport report)
        {
            Debug.Log("Validating game data's hash asset ID");
            string[] allAssets = AssetDatabase.GetAllAssetPaths();
            foreach (string assetPath in allAssets)
            {
                // Check if the asset is included based on your criteria, e.g., labels, specific folders
                if (AssetDatabase.IsValidFolder(assetPath) || !assetPath.StartsWith("Assets/"))
                    continue;
                // Validate only game data, so if it is not game data, skip it
                System.Type assetType = AssetDatabase.GetMainAssetTypeAtPath(assetPath);
                if (assetType == null || !assetType.IsSubclassOf(typeof(BaseGameData)))
                    continue;
                BaseGameData gameData = AssetDatabase.LoadAssetAtPath(assetPath, assetType) as BaseGameData;
                if (gameData.ValidateHashAssetID())
                    EditorUtility.SetDirty(gameData);
                Debug.Log($"Validated game data's hash asset ID for {gameData}");
            }
        }
    }
}
