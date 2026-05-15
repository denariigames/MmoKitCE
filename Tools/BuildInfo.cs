using UnityEngine;

[CreateAssetMenu(menuName = "MmoKitCE/Tools/BuildInfo", fileName = "BuildInfo")]
public class BuildInfo : ScriptableObject
{
    [Tooltip("Current version shown to players")]
    public string Version = "v1.0.0";
}