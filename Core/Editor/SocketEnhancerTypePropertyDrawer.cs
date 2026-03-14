using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace MultiplayerARPG
{
    [CustomPropertyDrawer(typeof(SocketEnhancerType))]
    public class SocketEnhancerTypePropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            List<string> enumNames = new List<string>(property.enumNames);
            int index = property.enumValueIndex;
            for (int i = 0; i < EditorGlobalData.SocketEnhancerTypeTitles.Length; ++i)
                enumNames[i] = EditorGlobalData.SocketEnhancerTypeTitles[i];
            property.enumValueIndex = EditorGUI.Popup(position, label.text, index, enumNames.ToArray());
        }
    }
}
