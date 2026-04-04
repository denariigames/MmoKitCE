// This class can be used to create any ScriptableObject type via Reflection.
// To use it, simply right click in the Project Area and select Create ->
// ScriptableObject.
using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

namespace Insthync.UnityEditorUtils.Editor
{
    class EndNameEdit : EndNameEditAction
    {
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            AssetDatabase.CreateAsset(EditorUtility.InstanceIDToObject(instanceId), AssetDatabase.GenerateUniqueAssetPath(pathName));
        }
    }

    public class CreateScriptableObject : EditorWindow
    {
        public static readonly HashSet<string> assemblyNames = new HashSet<string>() { "Assembly-CSharp" };

        Vector2 _scrollViewPosition = Vector2.zero;

        static List<Type> FindTypes(string name)
        {
            List<Type> types = new List<Type>();
            try
            {
                // get project assembly
                Assembly asm = Assembly.Load(new AssemblyName(name));

                // filter out all the ScriptableObject types
                foreach (Type type in asm.GetTypes())
                {
                    if (type.IsSubclassOf(typeof(ScriptableObject)) && !type.IsAbstract)
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
            foreach (string assemblyName in assemblyNames)
            {
                foreach (Type type in FindTypes(assemblyName))
                {
                    if (GUILayout.Button(type.FullName))
                    {
                        // create the asset, select it, allow renaming, close
                        ScriptableObject asset = CreateInstance(type);
                        ProjectWindowUtil.StartNameEditingIfProjectWindowExists(asset.GetInstanceID(), CreateInstance<EndNameEdit>(), type.FullName + ".asset", AssetPreview.GetMiniThumbnail(asset), null);
                        Close();
                    }
                }
            }
            EditorGUILayout.EndScrollView();
        }

        [MenuItem("Assets/Create/ScriptableObject")]
        public static void ShowWindow()
        {
            GetWindow<CreateScriptableObject>(true, "Create ScriptableObject").ShowPopup();
        }
    }
}
