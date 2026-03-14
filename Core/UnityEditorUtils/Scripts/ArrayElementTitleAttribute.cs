using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Insthync.UnityEditorUtils
{
    public class ArrayElementTitleAttribute : PropertyAttribute
    {
        public string variableName { get; private set; }
#if UNITY_EDITOR
        public float[] nullColorValue { get; private set; }
        public float[] notNullColorValue { get; private set; }
        public float[] proNullColorValue { get; private set; }
        public float[] proNotNullColorValue { get; private set; }
#endif

        public Color nullColor
        {
            get
            {
#if UNITY_EDITOR
                return EditorGUIUtility.isProSkin ? GetColor(proNullColorValue) : GetColor(nullColorValue);
#else
                return Color.black;
#endif
            }
        }

        public Color notNullColor
        {
            get
            {
#if UNITY_EDITOR
                return EditorGUIUtility.isProSkin ? GetColor(proNotNullColorValue) : GetColor(notNullColorValue);
#else
                return Color.black;
#endif
            }
        }

        public ArrayElementTitleAttribute(string variableName) :
            this(variableName, new float[] { 1, 0, 0, 1 }, new float[] { 0, 0, 1, 1 }, new float[] { 1, 0, 0, 1 }, new float[] { 0, 1, 0, 1 })
        {
        }

        public ArrayElementTitleAttribute(string variableName, float[] nullColorValue, float[] notNullColorValue) :
            this(variableName, nullColorValue, notNullColorValue, nullColorValue, notNullColorValue)
        {
        }

        public ArrayElementTitleAttribute(string variableName, float[] nullColorValue, float[] notNullColorValue, float[] proNullColorValue, float[] proNotNullColorValue)
        {
            this.variableName = variableName;
#if UNITY_EDITOR
            this.nullColorValue = nullColorValue;
            this.notNullColorValue = notNullColorValue;
            this.proNullColorValue = proNullColorValue;
            this.proNotNullColorValue = proNotNullColorValue;
#endif
        }

        private Color GetColor(float[] arr)
        {
            if (arr.Length > 3)
                return new Color(arr[0], arr[1], arr[2], arr[3]);
            else if (arr.Length == 3)
                return new Color(arr[0], arr[1], arr[2]);
            return Color.black;
        }
    }
}
