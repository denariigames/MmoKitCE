using UnityEngine;
using UnityEditor;

namespace Insthync.UnityEditorUtils.Editor
{
    [CustomPropertyDrawer(typeof(ArrayElementTitleAttribute))]
    public class ArrayElementTitleDrawer : PropertyDrawer
    {
        protected ArrayElementTitleAttribute Attribute
        {
            get { return (ArrayElementTitleAttribute)attribute; }
        }

        private SerializedProperty titleProperty;
        private bool isNull;
        private Color changingColor;
        private GUIStyle style;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            string variablePropertyPath = property.propertyPath + "." + Attribute.variableName;
            titleProperty = property.serializedObject.FindProperty(variablePropertyPath);
            isNull = false;
            string newlabel = GetTitle();
            if (string.IsNullOrEmpty(newlabel))
            {
                // Element 0, Element 1, Element 2, ... Element N
                newlabel = label.text;
            }
            else
            {
                // 0: XXX
                if (!string.IsNullOrEmpty(label.text))
                    newlabel = $"{label.text.Split(' ')[1]}: {newlabel}";
                else
                    newlabel = $"      {newlabel}";
            }

            changingColor = isNull ? Attribute.nullColor : Attribute.notNullColor;
            style = SetStyleColor(new GUIStyle(), changingColor);
            EditorGUI.PropertyField(position, property, GUIContent.none, true);
            EditorGUI.LabelField(position, new GUIContent(newlabel, label.tooltip), style);
        }

        private GUIStyle SetStyleColor(GUIStyle style, Color color)
        {
            style.normal.textColor =
                style.onNormal.textColor =
                style.active.textColor =
                style.onActive.textColor =
                style.focused.textColor =
                style.onFocused.textColor =
                style.hover.textColor =
                style.onHover.textColor = color;
            return style;
        }

        private string GetTitle()
        {
            if (titleProperty == null)
                return string.Empty;
            switch (titleProperty.propertyType)
            {
                case SerializedPropertyType.Generic:
                    break;
                case SerializedPropertyType.Integer:
                    return titleProperty.intValue.ToString();
                case SerializedPropertyType.Boolean:
                    return titleProperty.boolValue.ToString();
                case SerializedPropertyType.Float:
                    return titleProperty.floatValue.ToString();
                case SerializedPropertyType.String:
                    break;
                case SerializedPropertyType.Color:
                    return titleProperty.colorValue.ToString();
                case SerializedPropertyType.ObjectReference:
                    if (titleProperty.objectReferenceValue != null)
                        return titleProperty.objectReferenceValue.name;
                    isNull = true;
                    return "None";
                case SerializedPropertyType.LayerMask:
                    break;
                case SerializedPropertyType.Enum:
                    return titleProperty.enumNames[titleProperty.enumValueIndex];
                case SerializedPropertyType.Vector2:
                    return titleProperty.vector2Value.ToString();
                case SerializedPropertyType.Vector3:
                    return titleProperty.vector3Value.ToString();
                case SerializedPropertyType.Vector4:
                    return titleProperty.vector4Value.ToString();
                case SerializedPropertyType.Rect:
                    break;
                case SerializedPropertyType.ArraySize:
                    break;
                case SerializedPropertyType.Character:
                    break;
                case SerializedPropertyType.AnimationCurve:
                    break;
                case SerializedPropertyType.Bounds:
                    break;
                case SerializedPropertyType.Gradient:
                    break;
                case SerializedPropertyType.Quaternion:
                    break;
                default:
                    break;
            }
            return string.Empty;
        }
    }
}
