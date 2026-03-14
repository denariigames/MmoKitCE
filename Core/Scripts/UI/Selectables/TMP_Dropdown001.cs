using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
using TMPro.EditorUtilities;
#endif
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace MultiplayerARPG
{
    /// <summary>
    /// Made this component for navigation demo, because built-in Unity's components not good enough
    /// </summary>
    public class TMP_Dropdown001 : TMPro.TMP_Dropdown
    {
        public bool selectDisabledSelectable = false;
        public float selectedScaleDuration = 1f;
        public float selectedScaling = 0.05f;
        public Transform scalingTransform;
        public List<Selectable> upSelectables = new List<Selectable>();
        public List<Selectable> downSelectables = new List<Selectable>();
        public List<Selectable> leftSelectables = new List<Selectable>();
        public List<Selectable> rightSelectables = new List<Selectable>();
        public ScrollRect scrollRect;
        private SelectionState _currentSelectionState;
        private Vector3 _defaultLocalScale = Vector3.one;

        protected override void Awake()
        {
            base.Awake();
            if (scalingTransform == null)
                scalingTransform = transform;
            _defaultLocalScale = scalingTransform.localScale;
        }

        protected override void Start()
        {
            base.Start();
            if (scrollRect == null)
                scrollRect = GetComponentInParent<ScrollRect>();
        }

        protected Selectable GetFirstSelectable(List<Selectable> list, Selectable defaultExplicit)
        {
            if (list == null)
                return defaultExplicit;

            for (int i = 0; i < list.Count; i++)
            {
                if (list[i] != null && list[i].IsInteractable() && list[i].gameObject.activeInHierarchy)
                {
                    return list[i];
                }
            }

            return defaultExplicit;
        }

        public void AddUpSelectable(Selectable selectable, bool prioritize = false)
        {
            AddSelectable(upSelectables, selectable, prioritize);
        }

        public void AddDownSelectable(Selectable selectable, bool prioritize = false)
        {
            AddSelectable(downSelectables, selectable, prioritize);
        }

        public void AddLeftSelectable(Selectable selectable, bool prioritize = false)
        {
            AddSelectable(leftSelectables, selectable, prioritize);
        }

        public void AddRightSelectable(Selectable selectable, bool prioritize = false)
        {
            AddSelectable(rightSelectables, selectable, prioritize);
        }

        protected void AddSelectable(List<Selectable> list, Selectable selectable, bool prioritize)
        {
            if (list == null)
            {
                list = new List<Selectable>();
            }

            if (prioritize)
            {
                list.Insert(0, selectable);
            }
            else
            {
                list.Add(selectable);
            }
        }

        public void Clear()
        {
            upSelectables.Clear();
            downSelectables.Clear();
            leftSelectables.Clear();
            rightSelectables.Clear();
        }

        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            base.DoStateTransition(state, instant);
            _currentSelectionState = state;
        }

        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);
            this.ScrollSnap(scrollRect);
        }

        void Update()
        {
            if (Application.isPlaying)
            {
                if (_currentSelectionState == SelectionState.Selected)
                    scalingTransform.localScale = new Vector3(_defaultLocalScale.x + Mathf.PingPong(Time.time, selectedScaleDuration) * selectedScaling, _defaultLocalScale.y + Mathf.PingPong(Time.time, selectedScaleDuration) * selectedScaling, scalingTransform.localScale.z);
                else
                    scalingTransform.localScale = _defaultLocalScale;
            }
        }

        public override Selectable FindSelectableOnLeft()
        {
            Selectable selectable = GetFirstSelectable(leftSelectables, base.FindSelectableOnLeft());
            if (selectable != null && !selectable.interactable && !selectDisabledSelectable)
                selectable = null;
            return selectable;
        }

        public override Selectable FindSelectableOnRight()
        {
            Selectable selectable = GetFirstSelectable(rightSelectables, base.FindSelectableOnRight());
            if (selectable != null && !selectable.interactable && !selectDisabledSelectable)
                selectable = null;
            return selectable;
        }

        public override Selectable FindSelectableOnUp()
        {
            Selectable selectable = GetFirstSelectable(upSelectables, base.FindSelectableOnUp());
            if (selectable != null && !selectable.interactable && !selectDisabledSelectable)
                selectable = null;
            return selectable;
        }

        public override Selectable FindSelectableOnDown()
        {
            Selectable selectable = GetFirstSelectable(downSelectables, base.FindSelectableOnDown());
            if (selectable != null && !selectable.interactable && !selectDisabledSelectable)
                selectable = null;
            return selectable;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(TMP_Dropdown001), true)]
    [CanEditMultipleObjects]
    public class TMP_Dropdown001Editor : DropdownEditor
    {
        protected SerializedProperty selectedScaleDuration;
        protected SerializedProperty selectedScaling;
        protected SerializedProperty selectDisabledSelectable;
        protected SerializedProperty scalingTransform;
        protected SerializedProperty upSelectables;
        protected SerializedProperty downSelectables;
        protected SerializedProperty leftSelectables;
        protected SerializedProperty rightSelectables;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            TMP_Dropdown001 target = this.target as TMP_Dropdown001;

            if (selectedScaleDuration == null)
                selectedScaleDuration = serializedObject.FindProperty(nameof(target.selectedScaleDuration));
            if (selectedScaling == null)
                selectedScaling = serializedObject.FindProperty(nameof(target.selectedScaling));
            if (selectDisabledSelectable == null)
                selectDisabledSelectable = serializedObject.FindProperty(nameof(target.selectDisabledSelectable));
            if (scalingTransform == null)
                scalingTransform = serializedObject.FindProperty(nameof(target.scalingTransform));
            if (upSelectables == null)
                upSelectables = serializedObject.FindProperty(nameof(target.upSelectables));
            if (downSelectables == null)
                downSelectables = serializedObject.FindProperty(nameof(target.downSelectables));
            if (leftSelectables == null)
                leftSelectables = serializedObject.FindProperty(nameof(target.leftSelectables));
            if (rightSelectables == null)
                rightSelectables = serializedObject.FindProperty(nameof(target.rightSelectables));

            EditorGUILayout.LabelField("Smart Navigation", EditorStyles.boldLabel);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Up"))
            {
                target.upSelectables.Clear();
                target.AddUpSelectable(target.FindSelectableOnUp());
                EditorUtility.SetDirty(target);
            }
            if (GUILayout.Button("Down"))
            {
                target.downSelectables.Clear();
                target.AddDownSelectable(target.FindSelectableOnDown());
                EditorUtility.SetDirty(target);
            }
            if (GUILayout.Button("Left"))
            {
                target.leftSelectables.Clear();
                target.AddLeftSelectable(target.FindSelectableOnLeft());
                EditorUtility.SetDirty(target);
            }
            if (GUILayout.Button("Right"))
            {
                target.rightSelectables.Clear();
                target.AddRightSelectable(target.FindSelectableOnRight());
                EditorUtility.SetDirty(target);
            }
            if (GUILayout.Button("Clear"))
            {
                target.Clear();
                EditorUtility.SetDirty(target);
            }
            GUILayout.EndHorizontal();

            serializedObject.Update();
            EditorGUILayout.PropertyField(upSelectables, new GUIContent("Up Selectables"));
            EditorGUILayout.PropertyField(downSelectables, new GUIContent("Down Selectables"));
            EditorGUILayout.PropertyField(leftSelectables, new GUIContent("Left Selectables"));
            EditorGUILayout.PropertyField(rightSelectables, new GUIContent("Right Selectables"));
            EditorGUILayout.PropertyField(selectDisabledSelectable, new GUIContent("Select Disabled Selectable"));
            EditorGUILayout.PropertyField(selectedScaleDuration, new GUIContent("Selected Scale Duration"));
            EditorGUILayout.PropertyField(selectedScaling, new GUIContent("Selected Scaling"));
            EditorGUILayout.PropertyField(scalingTransform, new GUIContent("Scaling Transform"));
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}