using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class VersionDisplay : MonoBehaviour
{
    [SerializeField] 
    private BuildInfo buildInfo;

    private void Start()
    {
        TextMeshProUGUI tmp = GetComponent<TextMeshProUGUI>();

        if (buildInfo != null)
            tmp.text = buildInfo.Version;
        else
            tmp.text = $"v{Application.version}";
    }
}