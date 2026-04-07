using UnityEditor;
using UnityEngine;

public class ColoredHeaderObject : MonoBehaviour
{
    [Header("Preset")] 
    [SerializeField] private ColoredHeaderPreset preset;

    [SerializeField] private ColoredHeaderSettings settings;

    public ColoredHeaderSettings GetSettings()
    {
        return settings;
    }

    private void SetPresetSettings()
    {
        settings = preset.GetHeaderPreset().settings;
        gameObject.name = preset.GetHeaderPreset().namePreset;
    }

    public string GetName()
    {
        if (preset == null)
        {
            return gameObject.name;
        }
        return preset.GetHeaderPreset().namePreset;
    }
    
#if UNITY_EDITOR
    private void OnValidate()
    {
        EditorApplication.RepaintAnimationWindow();

        if (preset != null)
        {
            SetPresetSettings();
        }
    }
    
#endif
}
