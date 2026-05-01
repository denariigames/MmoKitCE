using System;
using System.Globalization;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class ColorHierarchy
{
    static ColorHierarchy()
    {
        EditorApplication.hierarchyWindowItemOnGUI += RenderObjects;
    }

    private static void RenderObjects(int instanceID, Rect selectionRect)
    {
        GameObject gameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
        
        if (gameObject == null) return;

        if (gameObject.TryGetComponent<ColoredHeaderObject> (out var colorHeaderObject))
        {
            ColoredHeaderSettings settings = colorHeaderObject.GetSettings();
            string name = colorHeaderObject.GetName();

            if (settings == null)
            {
                settings = new ColoredHeaderSettings();
            }
            
            EditorGUI.DrawRect(selectionRect, settings.GetBackgroundColor);
            EditorGUI.LabelField(selectionRect, GetName(name, settings.IsUppercase), new GUIStyle()
            {
                alignment = GetTextAnchor(settings.GetTextAlignment),
                fontStyle = GetFontStyle(settings.GetFontStyle),
                normal = new GUIStyleState() { textColor = settings.GetTextColor}
            });   
        }
    }

    private static TextAnchor GetTextAnchor(TextAlignmentOptions textAlignmentOption)
    {
        switch (textAlignmentOption)
        {
            case TextAlignmentOptions.Center:
                return TextAnchor.MiddleCenter;
            case TextAlignmentOptions.Left:
                return TextAnchor.MiddleLeft;
            case TextAlignmentOptions.Right:
                return TextAnchor.MiddleRight;
        }
        return TextAnchor.MiddleCenter;
    }

    private static FontStyle GetFontStyle(FontStyleOptions fontStyleOption)
    {
        switch (fontStyleOption)
        {
            case FontStyleOptions.Bold:
                return FontStyle.Bold;
            case FontStyleOptions.Normal:
                return FontStyle.Normal;
            case FontStyleOptions.Italic:
                return FontStyle.Italic;
            case FontStyleOptions.BoldAndItalic:
                return FontStyle.BoldAndItalic;
            }
        return FontStyle.Normal;
    }

    private static string GetName(string nameObject, bool uppercase)
    {
        string tempName = nameObject;
        TextInfo textInfo = new CultureInfo("en-US",false).TextInfo;
        
        if (uppercase)
        {
            return tempName.ToUpper();
        }

        return textInfo.ToTitleCase(tempName);
    }
}
