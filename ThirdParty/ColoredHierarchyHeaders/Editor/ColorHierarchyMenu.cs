using UnityEditor;
using UnityEngine;

public class ColorHierarchyMenu : EditorWindow
{
    [MenuItem("GameObject/Color Header Object", false, 0)]
    static void CreateColorHeader(MenuCommand menuCommand)
    {
        GameObject obj = new GameObject("Header");
        Undo.RegisterCreatedObjectUndo(obj, "Create Color Header");
        GameObjectUtility.SetParentAndAlign(obj, menuCommand.context as GameObject);
        obj.AddComponent<ColoredHeaderObject>();
        Selection.activeObject = obj;
    }
}
