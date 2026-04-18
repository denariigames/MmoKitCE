using UnityEngine;

[CreateAssetMenu(fileName = "ColoredHeaderPreset", menuName = "Colored Header/Create Preset", order = 1)]
public class ColoredHeaderPreset : ScriptableObject
{
    [SerializeField] private HeaderPreset headerPreset;
    public HeaderPreset GetHeaderPreset()
    {
        return headerPreset;
    }
    
}
[System.Serializable]
public class HeaderPreset
{
    public string namePreset;
    public ColoredHeaderSettings settings;
}

[System.Serializable]
public class ColoredHeaderSettings
{
    [Header("Background")]
    [Tooltip("Header background color.")]
    [SerializeField] private Color backgroundColor = Color.black;

    [Header("Text")] 
    
    [Tooltip("Header text color.")]
    [SerializeField] private Color fontColor = Color.white;
    [Tooltip("Header text alignment.")]
    [SerializeField] private TextAlignmentOptions textAlignment = TextAlignmentOptions.Center;
    [Tooltip("Header text style.")]
    [SerializeField] private FontStyleOptions fontStyle = FontStyleOptions.Normal;
    [Tooltip("Header text uppercase.")]
    [SerializeField] private bool uppercase;

    public Color GetBackgroundColor => backgroundColor;
    public TextAlignmentOptions GetTextAlignment => textAlignment;
    public FontStyleOptions GetFontStyle => fontStyle;
    public bool IsUppercase => uppercase;
    public Color GetTextColor => fontColor;
}
