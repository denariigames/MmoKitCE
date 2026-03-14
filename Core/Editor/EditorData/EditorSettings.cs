using UnityEngine;

namespace MultiplayerARPG
{
    public class EditorSettings : ScriptableObject
    {
        public GameDatabase workingDatabase;
        public string[] socketEnhancerTypeTitles = new string[0];
        [Header("Addressable Assets")]
        public string clientBuildProfileName;
        public string serverBuildProfileName;
    }
}