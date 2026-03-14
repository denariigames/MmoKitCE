using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Insthync.UnityEditorUtils.Editor
{
    /// <summary>
    /// Modified from codes in Wiki: http://wiki.unity3d.com/index.php/FindMissingScripts 
    /// Modified by Insthync
    /// </summary>
    public abstract class BaseFindObjectsRecursivelyTool : EditorWindow
    {
        protected struct FoundObjectData
        {
            public GameObject Object { get; set; }
            public int ComponentIndex { get; set; }
        }

        protected int _searchedCount = 0;
        protected int _componentsCount = 0;
        protected int _missingCount = 0;
        protected Vector2 _scrollPos = Vector2.zero;
        protected List<FoundObjectData> _missingObjects = new List<FoundObjectData>();

        public abstract string GetObjectName();

        protected virtual void OnGUI()
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button($"Find {GetObjectName()} in selected GameObjects"))
            {
                FindInSelected();
            }
            GUILayout.EndHorizontal();
            _scrollPos = GUILayout.BeginScrollView(_scrollPos);
            for (int i = 0; i < _missingObjects.Count; i++)
            {
                if (_missingObjects[i].Object == null)
                    continue;
                GUILayout.BeginHorizontal();
                EditorGUILayout.ObjectField(_missingObjects[i].Object.name, _missingObjects[i].Object, typeof(GameObject), true);
                GUILayout.Space(20);
                GUILayout.Label("Index: " + _missingObjects[i].ComponentIndex, GUILayout.Width(80));
                GUILayout.EndHorizontal();
            }
            GUILayout.BeginHorizontal();
            GUILayout.Label(string.Format("Searched {0} GameObjects, {1} components, found {2} objects", _searchedCount, _componentsCount, _missingCount));
            GUILayout.EndHorizontal();
            GUILayout.EndScrollView();
        }

        private void FindInSelected()
        {
            GameObject[] go = Selection.gameObjects;
            _searchedCount = 0;
            _componentsCount = 0;
            _missingCount = 0;
            _missingObjects.Clear();
            foreach (GameObject g in go)
            {
                FindInGameObject(g);
            }
            Debug.Log(string.Format("Searched {0} GameObjects, {1} components, found {2} objects", _searchedCount, _componentsCount, _missingCount));
        }

        protected abstract bool IsTargetObject(Component comp);

        private void FindInGameObject(GameObject obj)
        {
            _searchedCount++;
            Component[] components = obj.GetComponents<Component>();
            for (int idx = 0; idx < components.Length; idx++)
            {
                _componentsCount++;
                if (IsTargetObject(components[idx]))
                {
                    _missingCount++;
                    string path = obj.name;
                    Transform t = obj.transform;
                    while (t.parent != null)
                    {
                        path = t.parent.name + "/" + path;
                        t = t.parent;
                    }
                    Debug.Log($"{path} has an {GetObjectName()} attached in position: {idx}", obj);
                    _missingObjects.Add(new FoundObjectData()
                    {
                        Object = obj,
                        ComponentIndex = idx,
                    });
                }
            }

            // Now recurse through each child GO (if there are any):
            foreach (Transform childTransform in obj.transform)
            {
                FindInGameObject(childTransform.gameObject);
            }
        }
    }
}
