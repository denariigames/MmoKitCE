using UnityEditor;
using UnityEngine;

namespace Insthync.UnityEditorUtils.Editor
{
    [CustomPropertyDrawer(typeof(UnityHelpBox))]
    public class UnityHelpBoxPropertyDrawer : PropertyDrawer
    {
        public const float ExtraSpace = 12f;
        public const float MinHeight = 40f + ExtraSpace;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            UnityHelpBox target = fieldInfo.GetValue(property.serializedObject.targetObject) as UnityHelpBox;
            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.HelpBox(position, target.text, (MessageType)target.type);
            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            UnityHelpBox target = fieldInfo.GetValue(property.serializedObject.targetObject) as UnityHelpBox;
            GUIContent content = new GUIContent(target.text);
            GUIStyle style = GUI.skin.GetStyle("helpbox");
            float height = style.CalcHeight(content, EditorGUIUtility.currentViewWidth) + ExtraSpace;
            return height > MinHeight ? height : MinHeight;
        }
    }
}
