using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

namespace Insthync.UnityEditorUtils.Editor
{
    public abstract class BaseCustomEditor : UnityEditor.Editor
    {
        /*
         * ShowOnEnum() - Made by JWolf
         * Modified by Insthync
         */

        /// <summary>
        /// Field names in this list will be hidden
        /// </summary>
        protected List<string> hiddenFields;

        /// <summary>
        /// Dictionary<ShowingFieldName(string), List<FieldCondition>>
        /// </summary>
        protected Dictionary<string, List<FieldCondition>> fieldConditions;

        protected virtual void OnEnable()
        {
            hiddenFields = new List<string>();
            fieldConditions = new Dictionary<string, List<FieldCondition>>();
            SetFieldCondition();
        }

        /// <summary>
        /// Implement fields condition here
        /// </summary>
        protected virtual void SetFieldCondition()
        {

        }

        /// <summary>
        /// Use this function to set when witch fields should be visible.
        /// </summary>
        /// <param name='conditionMemberName'>
        /// The name of the Enum field. (in your case that is "type")
        /// </param>
        /// <param name='conditionValue'>
        /// When the Enum value is this in the editor, the field is visible.
        /// </param>
        /// <param name='showingFieldName'>
        /// The Field name that should only be visible when the chosen enum value is set.
        /// </param>
        protected void ShowOnEnum(string conditionMemberName, string conditionValue, string showingFieldName)
        {
            EnumFieldCondition newFieldCondition = new EnumFieldCondition(conditionMemberName, conditionValue, showingFieldName);
            if (!newFieldCondition.Validate(target, ToString()))
            {
                Debug.LogError(newFieldCondition.Error);
                return;
            }
            AddFieldCondition(showingFieldName, newFieldCondition);
        }

        /// <summary>
        /// Use this function to set when witch fields should be visible.
        /// </summary>
        /// <param name='conditionMemberName'>
        /// The name of the Bool field.
        /// </param>
        /// <param name='conditionValue'>
        /// When the Bool value is this in the editor, the field is visible.
        /// </param>
        /// <param name='showingFieldName'>
        /// The Field name that should only be visible when the chosen bool value is set.
        /// </param>
        protected void ShowOnBool(string conditionMemberName, bool conditionValue, string showingFieldName)
        {
            BoolFieldCondition newFieldCondition = new BoolFieldCondition(conditionMemberName, conditionValue, showingFieldName);
            if (!newFieldCondition.Validate(target, ToString()))
            {
                Debug.LogError(newFieldCondition.Error);
                return;
            }
            AddFieldCondition(showingFieldName, newFieldCondition);
        }

        /// <summary>
        /// Use this function to set when witch fields should be visible.
        /// </summary>
        /// <param name='conditionMemberName'>
        /// The name of the Int field.
        /// </param>
        /// <param name='conditionValue'>
        /// When the Int value is this in the editor, the field is visible.
        /// </param>
        /// <param name='showingFieldName'>
        /// The Field name that should only be visible when the chosen int value is set.
        /// </param>
        protected void ShowOnInt(string conditionMemberName, int conditionValue, string showingFieldName)
        {
            IntFieldCondition newFieldCondition = new IntFieldCondition(conditionMemberName, conditionValue, showingFieldName);
            if (!newFieldCondition.Validate(target, ToString()))
            {
                Debug.LogError(newFieldCondition.Error);
                return;
            }
            AddFieldCondition(showingFieldName, newFieldCondition);
        }

        private void AddFieldCondition(string showingFieldName, FieldCondition condition)
        {
            if (!fieldConditions.ContainsKey(showingFieldName))
                fieldConditions.Add(showingFieldName, new List<FieldCondition>());
            fieldConditions[showingFieldName].Add(condition);
        }

        public override void OnInspectorGUI()
        {
            // Update the serializedProperty - always do this in the beginning of OnInspectorGUI.
            serializedObject.Update();
            // Render fields
            RenderFields();
            // Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.
            serializedObject.ApplyModifiedProperties();
        }

        protected virtual void RenderFields()
        {
            SerializedProperty obj = serializedObject.GetIterator();
            if (obj.NextVisible(true))
            {
                // Loops through all visible fields
                do
                {
                    RenderField(obj);
                } while (obj.NextVisible(false));
            }
        }

        protected virtual void RenderField(SerializedProperty obj)
        {
            if (FieldShouldVisible(obj))
                EditorGUILayout.PropertyField(obj, true);
        }

        protected virtual bool FieldShouldVisible(SerializedProperty obj)
        {
            if (hiddenFields.Contains(obj.name))
            {
                // The field is being hidden
                return false;
            }
            if (fieldConditions.ContainsKey(obj.name))
            {
                bool shouldVisible = false;
                foreach (var condition in fieldConditions[obj.name])
                {
                    if (condition.ShouldVisible(target, obj))
                    {
                        // The field should visible
                        shouldVisible = true;
                        break;
                    }
                }
                if (!shouldVisible)
                    return false;
            }
            return true;
        }

        protected static FieldInfo GetField(Object target, string name)
        {
            System.Type lookupType = target.GetType();
            do
            {
                FieldInfo field = lookupType.GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (field != null)
                    return field;
                lookupType = lookupType.BaseType;
            } while (lookupType != typeof(MonoBehaviour));
            return null;
        }

        protected static PropertyInfo GetProperty(Object target, string name)
        {
            System.Type lookupType = target.GetType();
            do
            {
                PropertyInfo field = lookupType.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (field != null)
                    return field;
                lookupType = lookupType.BaseType;
            } while (lookupType != typeof(MonoBehaviour));
            return null;
        }

        protected class FieldCondition
        {
            public string ConditionMemberName { get; protected set; }
            public string ShowingFieldName { get; protected set; }
            public string Error { get; protected set; }
            protected MemberInfo conditionMember;
            protected FieldInfo showingField;

            public FieldCondition(string conditionMemberName, string showingFieldName)
            {
                ConditionMemberName = conditionMemberName;
                ShowingFieldName = showingFieldName;
                Error = $"Field condition is not validated yet: '{conditionMemberName}', '{showingFieldName}'";
            }

            public new string ToString()
            {
                return $"'{ConditionMemberName}', '{ShowingFieldName}'.";
            }

            public bool Validate(Object target, string scriptName = "")
            {
                return Validate(target, out _, out _, scriptName);
            }

            public virtual bool Validate(Object target, out MemberInfo conditionMember, out FieldInfo showingField, string scriptName = "")
            {
                showingField = null;

                // Valildating the "conditionMemberName"
                conditionMember = GetField(target, ConditionMemberName);
                if (conditionMember == null)
                    conditionMember = GetProperty(target, ConditionMemberName);
                if (conditionMember == null)
                {
                    Error = $"Could not find a field named: '{ConditionMemberName}' in '{target}'. Make sure you have spelled the field name for `conditionMemberName` correct in the script '{scriptName}', '{ToString()}'";
                    return false;
                }

                // Valildating the "showingFieldName"
                showingField = GetField(target, ShowingFieldName);
                if (showingField == null)
                {
                    Error = $"Could not find a field named: '{ShowingFieldName}' in '{target}'. Make sure you have spelled the field name for `showingFieldName` correct in the script '{scriptName}', '{ToString()}'";
                    return false;
                }

                this.conditionMember = conditionMember;
                this.showingField = showingField;
                Error = string.Empty;
                return true;
            }

            public virtual bool IsConditionField(Object target, SerializedProperty obj)
            {
                return ConditionMemberName.Equals(obj.name);
            }

            public virtual bool IsShowingField(Object target, SerializedProperty obj)
            {
                return ShowingFieldName.Equals(obj.name);
            }

            public virtual bool ShouldVisible(Object target, SerializedProperty obj)
            {
                return IsShowingField(target, obj);
            }
        }

        protected class FieldCondition<T> : FieldCondition
        {
            public T ConditionValue { get; protected set; }

            public FieldCondition(string conditionMemberName, T conditionValue, string showingFieldName) : base(conditionMemberName, showingFieldName)
            {
                ConditionValue = conditionValue;
                Error = $"Field condition is not validated yet: '{conditionMemberName}', '{conditionValue}', '{showingFieldName}'";
            }

            public new string ToString()
            {
                return $"'{ConditionMemberName}', '{ConditionValue}', '{ShowingFieldName}'.";
            }

            public override bool ShouldVisible(Object target, SerializedProperty obj)
            {
                if (base.ShouldVisible(target, obj))
                {
                    object currentConditionValue = null;

                    if (conditionMember is FieldInfo)
                        currentConditionValue = (conditionMember as FieldInfo).GetValue(target);

                    if (conditionMember is PropertyInfo)
                        currentConditionValue = (conditionMember as PropertyInfo).GetValue(target, null);

                    // If the `conditionValue` value isn't equal to the wanted value the field will be set not to show
                    return currentConditionValue.ToString().Equals(ConditionValue.ToString());
                }
                return false;
            }
        }

        protected class EnumFieldCondition : FieldCondition<string>
        {
            public EnumFieldCondition(string conditionMemberName, string conditionValue, string showingFieldName) : base(conditionMemberName, conditionValue, showingFieldName) { }

            public override bool Validate(Object target, out MemberInfo conditionMember, out FieldInfo showingField, string scriptName = "")
            {
                if (!base.Validate(target, out conditionMember, out showingField, scriptName))
                    return false;

                // Valildating the "conditionValue"
                bool found = false;
                object currentConditionValue = null;

                if (conditionMember is FieldInfo)
                    currentConditionValue = (conditionMember as FieldInfo).GetValue(target);

                if (conditionMember is PropertyInfo)
                    currentConditionValue = (conditionMember as PropertyInfo).GetValue(target, null);

                if (currentConditionValue != null)
                {
                    // Finding enum value
                    FieldInfo[] enumNames = currentConditionValue.GetType().GetFields();
                    foreach (FieldInfo enumName in enumNames)
                    {
                        if (enumName.Name == ConditionValue)
                        {
                            found = true;
                            break;
                        }
                    }
                }

                // If cannot find enum value
                if (!found)
                {
                    Error = $"Could not find the enum value: '{ConditionValue}' in the enum '{currentConditionValue.GetType().ToString()}'. Make sure you have spelled the field name for `conditionValue` correct in the script '{scriptName}', '{ToString()}'";
                    return false;
                }

                Error = string.Empty;
                return true;
            }
        }

        protected class BoolFieldCondition : FieldCondition<bool>
        {
            public BoolFieldCondition(string conditionMemberName, bool conditionValue, string showingFieldName) : base(conditionMemberName, conditionValue, showingFieldName) { }
        }

        protected class IntFieldCondition : FieldCondition<int>
        {
            public IntFieldCondition(string conditionMemberName, int conditionValue, string showingFieldName) : base(conditionMemberName, conditionValue, showingFieldName) { }
        }
    }
}
