using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace MultiplayerARPG
{
    public abstract class GameDataListSection
    {
        public abstract string MenuTitle { get; }
        public abstract void OnGUI(float width, float height);
    }

    public class GameDataListSection<T> : GameDataListSection
        where T : BaseGameData
    {
        protected Vector2 menuScrollPosition;
        protected Vector2 contentScrollPosition;
        protected int selectedMenuIndex;
        protected T selectedUnlistedObject;
        protected string fieldName;
        public string FieldName { get { return fieldName; } }
        protected string menuTitle;
        protected T editorData;
        protected Editor editor;
        public override string MenuTitle { get { return menuTitle; } }

        public GameDataListSection(string fieldName, string menuTitle)
        {
            this.fieldName = fieldName;
            this.menuTitle = menuTitle;
        }

        public override void OnGUI(float width, float height)
        {
            if (EditorGlobalData.WorkingDatabase == null)
                return;

            T[] arr = (T[])EditorGlobalData.WorkingDatabase.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).GetValue(EditorGlobalData.WorkingDatabase);
            // Prepare GUI styles
            GUIStyle leftMenuButton = new GUIStyle(EditorStyles.label);
            leftMenuButton.fontSize = 12;
            leftMenuButton.alignment = TextAnchor.MiddleLeft;

            GUIStyle selectedLeftMenuButton = new GUIStyle(EditorStyles.label);
            selectedLeftMenuButton.fontSize = 12;
            selectedLeftMenuButton.alignment = TextAnchor.MiddleLeft;
            var background = new Texture2D(1, 1, TextureFormat.RGBA32, false);
            background.SetPixel(0, 0, EditorGUIUtility.isProSkin ? new Color(0.243f, 0.373f, 0.588f) : new Color(0.247f, 0.494f, 0.871f));
            background.Apply();
            selectedLeftMenuButton.active.background = selectedLeftMenuButton.normal.background = background;

            if (arr != null && selectedMenuIndex >= arr.Length)
                selectedMenuIndex = arr.Length - 1;

            // List
            GUILayout.BeginArea(new Rect(0, 0, 200, height), string.Empty);
            {
                menuScrollPosition = GUILayout.BeginScrollView(menuScrollPosition);
                GUILayout.BeginVertical();
                {
                    if (arr != null && arr.Length > 0)
                    {
                        for (int i = 0; i < arr.Length; ++i)
                        {
                            if (GUILayout.Button("ID: " + arr[i].Id + "\nFile: " + arr[i].name, (i == selectedMenuIndex ? selectedLeftMenuButton : leftMenuButton), GUILayout.Height(32)))
                            {
                                selectedMenuIndex = i;
                            }
                        }
                    }
                    GUILayout.EndVertical();
                }
                GUILayout.EndScrollView();
            }
            GUILayout.EndArea();

            Color defaultBackgroundColor = GUI.backgroundColor;

            // Buttons
            GUILayout.BeginArea(new Rect(200, 0, width - 200, 30), string.Empty);
            {
                GUILayout.Space(5);
                GUILayout.BeginHorizontal();
                {
                    selectedUnlistedObject = EditorGUILayout.ObjectField("Adding Unlisted Data", selectedUnlistedObject, typeof(T), true) as T;
                    if (selectedUnlistedObject != null)
                    {
                        // Add button
                        GUI.backgroundColor = Color.green;
                        if (GUILayout.Button("Add", GUILayout.Width(100)))
                        {
                            Add(arr);
                            return;
                        }
                        GUI.backgroundColor = defaultBackgroundColor;
                    }

                    // Create button
                    GUI.backgroundColor = Color.green;
                    if (GUILayout.Button("Create", GUILayout.Width(100)))
                    {
                        Create(arr);
                        return;
                    }
                    GUI.backgroundColor = defaultBackgroundColor;

                    if (arr.Length > 0)
                    {
                        // Duplicate button
                        GUI.backgroundColor = Color.yellow;
                        if (GUILayout.Button("Duplicate", GUILayout.Width(100)))
                        {
                            Duplicate(arr);
                            return;
                        }
                        GUI.backgroundColor = defaultBackgroundColor;
                        // Delete button
                        GUI.backgroundColor = Color.red;
                        if (GUILayout.Button("Delete", GUILayout.Width(100)))
                        {
                            Delete(arr);
                            return;
                        }
                        GUI.backgroundColor = defaultBackgroundColor;
                    }
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndArea();

            // Selected Content
            GUI.backgroundColor = Color.grey;//
            GUILayout.BeginArea(new Rect(200, 30, width - 200, height - 30), string.Empty, "window");
            {
                contentScrollPosition = GUILayout.BeginScrollView(contentScrollPosition);
                if (arr.Length > 0)
                {
                    if (selectedMenuIndex < 0)
                        selectedMenuIndex = 0;
                    if (editorData != arr[selectedMenuIndex])
                    {
                        editor = Editor.CreateEditor(arr[selectedMenuIndex]);
                        editorData = arr[selectedMenuIndex];
                    }
                    editor.OnInspectorGUI();
                }
                GUILayout.EndScrollView();
            }
            GUILayout.EndArea();
            GUI.backgroundColor = defaultBackgroundColor;
        }

        protected virtual void Add(T[] arr)
        {
            if (selectedUnlistedObject == null)
                return;
            List<T> appending = new List<T>(arr);
            if (appending.Contains(selectedUnlistedObject))
                return;
            appending.Add(selectedUnlistedObject);
            T[] newArr = appending.ToArray();
            EditorGlobalData.WorkingDatabase.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).SetValue(EditorGlobalData.WorkingDatabase, newArr);
            EditorUtility.SetDirty(EditorGlobalData.WorkingDatabase);
            selectedUnlistedObject = null;
        }

        protected virtual void Create(T[] arr)
        {
            GameDataCreatorEditor.CreateNewEditor(EditorGlobalData.WorkingDatabase, typeof(T), fieldName, arr);
        }

        protected virtual void Duplicate(T[] arr)
        {
            string path = AssetDatabase.GetAssetPath(arr[selectedMenuIndex]);
            List<string> splitedPath = new List<string>(path.Split('.'));
            string extension = splitedPath[splitedPath.Count - 1];
            splitedPath.RemoveAt(splitedPath.Count - 1);
            string savePath = string.Join(".", splitedPath);
            int retries = 20;
            string savedPath = string.Empty;
            for (var i = 0; i < retries; i++)
            {
                savedPath = $"{savePath}{i}.{extension}";
                if (AssetDatabase.CopyAsset(path, savedPath))
                    break;
                if (i == retries - 1)
                {
                    Debug.LogError($"Cannot duplication {path} ({retries} retries)");
                    return;
                }
            }
            List<T> appending = new List<T>(arr);
            appending.Add(AssetDatabase.LoadAssetAtPath<T>(savedPath));
            T[] newArr = appending.ToArray();
            EditorGlobalData.WorkingDatabase.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).SetValue(EditorGlobalData.WorkingDatabase, newArr);
            EditorUtility.SetDirty(EditorGlobalData.WorkingDatabase);
        }

        protected virtual void Delete(T[] arr)
        {
            if (EditorUtility.DisplayDialog("Warning", "The selected game data file will be deleted, do you want to do that?", "Yes", "No"))
            {
                if (AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(arr[selectedMenuIndex])))
                {
                    List<T> list = new List<T>(arr);
                    list.RemoveAt(selectedMenuIndex);
                    T[] newArr = list.ToArray();
                    EditorGlobalData.WorkingDatabase.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).SetValue(EditorGlobalData.WorkingDatabase, newArr);
                    EditorUtility.SetDirty(EditorGlobalData.WorkingDatabase);
                }
            }
        }
    }
}
