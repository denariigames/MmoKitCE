// These codes are come from: https://raw.githubusercontent.com/zaikman/UnityPublic/master/InspectorButton.cs
using System.Reflection;
using UnityEngine;
using UnityEditor;
using System;

namespace Insthync.UnityEditorUtils.Editor
{
    [CustomPropertyDrawer(typeof(InspectorButtonAttribute))]
    public class InspectorButtonPropertyDrawer : PropertyDrawer
    {
        private MethodInfo _eventMethodInfo = null;

        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
        {
            InspectorButtonAttribute inspectorButtonAttribute = (InspectorButtonAttribute)attribute;
            Rect buttonRect = new Rect(position.x, position.y, position.width, position.height);
            string labelText = label.text;
            if (!string.IsNullOrWhiteSpace(inspectorButtonAttribute.labelText))
                labelText = inspectorButtonAttribute.labelText;
            if (GUI.Button(buttonRect, labelText))
            {
                Type eventOwnerType = fieldInfo.DeclaringType;
                string eventName = inspectorButtonAttribute.methodName;
                do
                {
                    _eventMethodInfo = eventOwnerType.GetMethod(eventName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
                    eventOwnerType = eventOwnerType.BaseType;
                } while (_eventMethodInfo == null);

                if (_eventMethodInfo != null)
                {
                    _eventMethodInfo.Invoke(GetDeclaringObject(prop), null);
                }
                else
                {
                    Debug.LogWarning(string.Format("InspectorButton: Unable to find method {0} in {1}", eventName, eventOwnerType));
                }
                if (inspectorButtonAttribute.dirtyAfterPressed)
                    EditorUtility.SetDirty(prop.serializedObject.targetObject);
            }
        }

        /// <summary>
        /// Gets the object that directly declares this SerializedProperty (its parent object).
        /// </summary>
        public static object GetDeclaringObject(SerializedProperty property)
        {
            if (property == null) return null;

            object obj = property.serializedObject.targetObject;
            string path = property.propertyPath.Replace(".Array.data[", "[");
            string[] elements = path.Split('.');

            // Walk down to the parent of the final field
            for (int i = 0; i < elements.Length - 1; i++)
            {
                obj = GetValue(obj, elements[i]);
            }

            return obj;
        }

        private static object GetValue(object source, string name)
        {
            if (source == null) return null;

            if (name.Contains("["))
            {
                // Handle array or list element
                int start = name.IndexOf("[");
                string elementName = name.Substring(0, start);
                int index = Convert.ToInt32(name.Substring(start).Replace("[", "").Replace("]", ""));

                var enumerable = GetFieldValue(source, elementName) as System.Collections.IEnumerable;
                if (enumerable == null) return null;

                var enm = enumerable.GetEnumerator();
                for (int i = 0; i <= index; i++) enm.MoveNext();
                return enm.Current;
            }
            else
            {
                return GetFieldValue(source, name);
            }
        }

        private static object GetFieldValue(object source, string name)
        {
            if (source == null) return null;

            Type type = source.GetType();
            FieldInfo f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (f != null)
                return f.GetValue(source);

            PropertyInfo p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (p != null)
                return p.GetValue(source, null);

            return null;
        }
    }
}
