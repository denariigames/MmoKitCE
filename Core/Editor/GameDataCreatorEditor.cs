using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace MultiplayerARPG
{
    public class GameDataCreatorEditor : EditorWindow
    {
        public static readonly HashSet<string> AssemblyNames = new HashSet<string>() { "Assembly-CSharp" };
        public GameDatabase WorkingDatabase { get; set; }
        public Type WorkingFieldType { get; set; }
        public string WorkingFieldName { get; set; }
        public ScriptableObject[] WorkingArray { get; set; }

        public static void CreateNewEditor(GameDatabase workingDatabase, Type workingFieldType, string workingFieldName, ScriptableObject[] workingArray)
        {
            GameDataCreatorEditor window = GetWindow<GameDataCreatorEditor>();
            window.WorkingDatabase = workingDatabase;
            window.WorkingFieldType = workingFieldType;
            window.WorkingFieldName = workingFieldName;
            window.WorkingArray = workingArray;
        }

        private Vector2 _scrollViewPosition = Vector2.zero;

        List<Type> FindTypes(string name)
        {
            List<Type> types = new List<Type>();
            try
            {
                // get project assembly
                Assembly asm = Assembly.Load(new AssemblyName(name));

                // filter out all the ScriptableObject types
                foreach (Type type in asm.GetTypes())
                {
                    if ((type == WorkingFieldType || type.IsSubclassOf(WorkingFieldType)) && !type.IsAbstract)
                        types.Add(type);
                }
            }
            catch { }

            return types;
        }

        void OnGUI()
        {
            GUILayout.Label("Select the type to create:");
            _scrollViewPosition = EditorGUILayout.BeginScrollView(_scrollViewPosition, false, false);
            foreach (string assemblyName in AssemblyNames)
            {
                foreach (Type type in FindTypes(assemblyName))
                {
                    if (GUILayout.Button(type.FullName))
                    {
                        // create the asset, select it, allow renaming, close
                        ScriptableObject newData = CreateInstance(type);
                        string savedPath = EditorUtility.SaveFilePanel("Save asset", "Assets", type.Name + ".asset", "asset");
                        if (!string.IsNullOrEmpty(savedPath))
                        {
                            savedPath = savedPath.Substring(savedPath.IndexOf("Assets"));
                            AssetDatabase.CreateAsset(newData, savedPath);
                            List<ScriptableObject> appending = new List<ScriptableObject>(WorkingArray);
                            appending.Add(AssetDatabase.LoadAssetAtPath<ScriptableObject>(savedPath));
                            Array newArray = Array.CreateInstance(WorkingFieldType, appending.Count);
                            for (int i = 0; i < newArray.Length; ++i)
                            {
                                newArray.SetValue(appending[i], i);
                            }
                            WorkingDatabase.GetType().GetField(WorkingFieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).SetValue(WorkingDatabase, newArray);
                            EditorUtility.SetDirty(WorkingDatabase);
                            Close();
                        }
                    }
                }
            }
            EditorGUILayout.EndScrollView();
        }
    }
}
