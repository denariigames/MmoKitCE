using UnityEditor;
using UnityEngine;

namespace Insthync.UnityEditorUtils.Editor
{
    [CustomPropertyDrawer(typeof(UnityTag))]
    public class UnityTagPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, GUIContent.none, property);
            SerializedProperty tag = property.FindPropertyRelative("tag");
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            if (tag != null)
            {
                tag.stringValue = EditorGUI.TagField(position, tag.stringValue);
            }
            EditorGUI.EndProperty();
        }
    }
}
