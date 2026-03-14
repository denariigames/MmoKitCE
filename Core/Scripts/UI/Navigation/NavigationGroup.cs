using Cysharp.Threading.Tasks;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MultiplayerARPG
{
    public class NavigationGroup : Selectable
    {
        public bool selectDisabledSelectable = false;
        public List<Selectable> upSelectables = new List<Selectable>();
        public List<Selectable> downSelectables = new List<Selectable>();
        public List<Selectable> leftSelectables = new List<Selectable>();
        public List<Selectable> rightSelectables = new List<Selectable>();
        public List<NavigationChild> childs = new List<NavigationChild>();
        public bool forceUpdateChildNavigation = false;
        protected bool _selectedOnNoChild;
        protected NavigationChild _lastSelectedChild;
        protected int _dirtyChildCount = -1;
        protected static NavigationGroup s_lastSelectedGroupWhileDisabled;

        protected override void Awake()
        {
            base.Awake();
            childs.Clear();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            upSelectables.Nullify();
            upSelectables?.Clear();
            downSelectables.Nullify();
            downSelectables?.Clear();
            leftSelectables.Nullify();
            leftSelectables?.Clear();
            rightSelectables.Nullify();
            rightSelectables?.Clear();
            childs.Nullify();
            childs?.Clear();
            _lastSelectedChild = null;
        }

        public void SetLastSelectedChild(NavigationChild child)
        {
            if (!childs.Contains(child))
                return;
            _lastSelectedChild = child;
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

        public override void Select()
        {
            if (!isActiveAndEnabled)
            {
                s_lastSelectedGroupWhileDisabled = this;
                return;
            }
            base.Select();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            if (s_lastSelectedGroupWhileDisabled == this)
            {
                s_lastSelectedGroupWhileDisabled = null;
                Select();
            }
        }

        public override void OnSelect(BaseEventData eventData)
        {
            base.OnSelect(eventData);
            if (childs.Count > 0)
            {
                _selectedOnNoChild = false;
                if (_lastSelectedChild != null && _lastSelectedChild.isActiveAndEnabled)
                {
                    DelaySelect(_lastSelectedChild.Selectable);
                }
                else
                {
                    for (int i = 0; i < childs.Count; ++i)
                    {
                        if (childs[i].isActiveAndEnabled)
                        {
                            DelaySelect(childs[i].Selectable);
                            break;
                        }
                    }
                }
            }
            else
            {
                _selectedOnNoChild = true;
            }
        }

        private async void DelaySelect(Selectable selectable, float delay = 0.1f)
        {
            while (delay > 0)
            {
                delay -= Time.deltaTime;
                await UniTask.Yield();
            }
            selectable.Select();
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

        public void AddChild(NavigationChild child)
        {
            if (childs.Contains(child))
                return;
            if (child.shouldBeFirstSelected)
                childs.Insert(0, child);
            else
                childs.Add(child);
            if (_selectedOnNoChild)
            {
                _selectedOnNoChild = false;
                for (int i = 0; i < childs.Count; ++i)
                {
                    if (childs[i].isActiveAndEnabled)
                    {
                        DelaySelect(childs[i].Selectable);
                        break;
                    }
                }
            }
        }

        public void RemoveChild(NavigationChild child)
        {
            bool selected = EventSystem.current != null && child.Selectable.gameObject == EventSystem.current.currentSelectedGameObject;
            int index = childs.IndexOf(child);
            childs.Remove(child);
            if (!selected)
                return;
            if (childs.Count <= 0)
            {
                Select();
            }
            else
            {
                if (index < childs.Count)
                {
                    for (int i = index; i >= 0; --i)
                    {
                        if (childs[i].isActiveAndEnabled)
                        {
                            DelaySelect(childs[i].Selectable);
                            break;
                        }
                    }
                }
                else
                {
                    for (int i = childs.Count - 1; i >= 0; --i)
                    {
                        if (childs[i].isActiveAndEnabled)
                        {
                            DelaySelect(childs[i].Selectable);
                            break;
                        }
                    }
                }
            }
        }

        private void LateUpdate()
        {
            UpdateChildNavigation();
        }

        public void UpdateChildNavigation()
        {
            if (_dirtyChildCount == childs.Count && !forceUpdateChildNavigation)
                return;
            forceUpdateChildNavigation = false;
            _dirtyChildCount = childs.Count;
            for (int i = 0; i < childs.Count; ++i)
            {
                Navigation nav = new Navigation();
                nav.mode = Navigation.Mode.Explicit;
                Navigation.Mode findMode = Navigation.Mode.None;
                if (childs[i].vertical)
                    findMode |= Navigation.Mode.Vertical;
                if (childs[i].horizontal)
                    findMode |= Navigation.Mode.Horizontal;
                if (childs[i].vertical)
                {
                    nav.selectOnUp = FindChildSelectable(findMode, childs[i], childs[i].transform.rotation * Vector3.up);
                    nav.selectOnDown = FindChildSelectable(findMode, childs[i], childs[i].transform.rotation * Vector3.down);
                }
                if (childs[i].horizontal)
                {
                    nav.selectOnLeft = FindChildSelectable(findMode, childs[i], childs[i].transform.rotation * Vector3.left);
                    nav.selectOnRight = FindChildSelectable(findMode, childs[i], childs[i].transform.rotation * Vector3.right);
                }
                if (childs[i].vertical)
                {
                    if (nav.selectOnUp == null)
                        nav.selectOnUp = FindSelectableOnUp();
                    if (nav.selectOnDown == null)
                        nav.selectOnDown = FindSelectableOnDown();
                }
                if (childs[i].horizontal)
                {
                    if (nav.selectOnLeft == null)
                        nav.selectOnLeft = FindSelectableOnLeft();
                    if (nav.selectOnRight == null)
                        nav.selectOnRight = FindSelectableOnRight();
                }
                childs[i].Selectable.navigation = nav;
            }
        }

        public Selectable FindChildSelectable(Navigation.Mode mode, NavigationChild child, Vector3 dir)
        {
            dir = dir.normalized;
            Vector3 vector = Quaternion.Inverse(child.transform.rotation) * dir;
            Vector3 vector2 = child.transform.TransformPoint(GetPointOnRectEdge(child.transform as RectTransform, vector));
            float num = float.NegativeInfinity;
            float num2 = float.NegativeInfinity;
            float num3 = 0f;
            bool flag = navigation.wrapAround && (mode == Navigation.Mode.Vertical || mode == Navigation.Mode.Horizontal);
            Selectable selectable = null;
            Selectable result = null;
            for (int i = 0; i < childs.Count; i++)
            {
                if (child == childs[i])
                    continue;
                Selectable selectable2 = childs[i].Selectable;
                if (selectable2 == this || !selectable2.IsInteractable() || selectable2.navigation.mode == Navigation.Mode.None)
                    continue;

                RectTransform rectTransform = selectable2.transform as RectTransform;
                Vector3 position = (rectTransform != null) ? ((Vector3)rectTransform.rect.center) : Vector3.zero;
                Vector3 rhs = selectable2.transform.TransformPoint(position) - vector2;
                float num4 = Vector3.Dot(dir, rhs);
                if (flag && num4 < 0f)
                {
                    num3 = (0f - num4) * rhs.sqrMagnitude;
                    if (num3 > num2)
                    {
                        num2 = num3;
                        result = selectable2;
                    }
                }
                else if (!(num4 <= 0f))
                {
                    num3 = num4 / rhs.sqrMagnitude;
                    if (num3 > num)
                    {
                        num = num3;
                        selectable = selectable2;
                    }
                }
            }

            if (flag && null == selectable)
            {
                return result;
            }

            return selectable;
        }

        private static Vector3 GetPointOnRectEdge(RectTransform rect, Vector2 dir)
        {
            if (rect == null)
            {
                return Vector3.zero;
            }

            if (dir != Vector2.zero)
            {
                dir /= Mathf.Max(Mathf.Abs(dir.x), Mathf.Abs(dir.y));
            }

            dir = rect.rect.center + Vector2.Scale(rect.rect.size, dir * 0.5f);
            return dir;
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(NavigationGroup), true)]
    [CanEditMultipleObjects]
    public class NavigationGroupEditor : UnityEditor.UI.SelectableEditor
    {
        protected SerializedProperty selectDisabledSelectable;
        protected SerializedProperty upSelectables;
        protected SerializedProperty downSelectables;
        protected SerializedProperty leftSelectables;
        protected SerializedProperty rightSelectables;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            NavigationGroup target = this.target as NavigationGroup;

            if (selectDisabledSelectable == null)
                selectDisabledSelectable = serializedObject.FindProperty(nameof(target.selectDisabledSelectable));
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
            serializedObject.ApplyModifiedProperties();
        }
    }
#endif
}
