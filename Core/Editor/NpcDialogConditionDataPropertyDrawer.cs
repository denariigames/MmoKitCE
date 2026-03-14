using Siccity.SerializableCallback.Editor;
using UnityEditor;
using UnityEngine;

namespace MultiplayerARPG
{
    [CustomPropertyDrawer(typeof(NpcDialogConditionDataAttribute))]
    public class NpcDialogConditionDataPropertyDrawer : SerializableCallbackDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (IsShow(property))
                return base.GetPropertyHeight(property, label);
            else
                return -EditorGUIUtility.standardVerticalSpacing;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (IsShow(property))
                base.OnGUI(position, property, label);
        }

        private bool IsShow(SerializedProperty property)
        {
            var propertyPath = property.propertyPath;
            var conditionPath = propertyPath.Replace(property.name, "conditionType");
            var sourcePropertyValue = property.serializedObject.FindProperty(conditionPath);
            return sourcePropertyValue.enumNames[sourcePropertyValue.enumValueIndex].Equals("CustomByCallback");
        }
    }
}
