using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MultiplayerARPG
{
    public static class AddressableEditorMenu
    {
        [MenuItem(EditorMenuConsts.BAKE_SERVER_SCENE_MENU, false, EditorMenuConsts.BAKE_SERVER_SCENE_ORDER)]
        public static void BakeServerScene()
        {
            Scene scene = SceneManager.GetActiveScene();
            GameObject[] rootObjects = scene.GetRootGameObjects();
            for (int i = 0; i < rootObjects.Length; ++i)
            {
                UnpackPrefabInstances(rootObjects[i]);
                Terrain[] terrains = rootObjects[i].GetComponentsInChildren<Terrain>();
                for (int j = 0; j < terrains.Length; ++j)
                {
                    Object.DestroyImmediate(terrains[j]);
                }
                TMPro.TextMeshPro[] tmps = rootObjects[i].GetComponentsInChildren<TMPro.TextMeshPro>();
                for (int j = 0; j < tmps.Length; ++j)
                {
                    Object.DestroyImmediate(tmps[j]);
                }
                Animator[] animators = rootObjects[i].GetComponentsInChildren<Animator>();
                for (int j = 0; j < animators.Length; ++j)
                {
                    Object.DestroyImmediate(animators[j]);
                }
                SkinnedMeshRenderer[] skinnedMeshRenderers = rootObjects[i].GetComponentsInChildren<SkinnedMeshRenderer>();
                for (int j = 0; j < skinnedMeshRenderers.Length; ++j)
                {
                    Object.DestroyImmediate(skinnedMeshRenderers[j]);
                }
                MeshRenderer[] meshRenderers = rootObjects[i].GetComponentsInChildren<MeshRenderer>();
                for (int j = 0; j < meshRenderers.Length; ++j)
                {
                    Object.DestroyImmediate(meshRenderers[j]);
                }
                MeshFilter[] meshFilters = rootObjects[i].GetComponentsInChildren<MeshFilter>();
                for (int j = 0; j < meshFilters.Length; ++j)
                {
                    Object.DestroyImmediate(meshFilters[j]);
                }
            }

            // Check if the scene is valid
            if (scene.IsValid())
            {
                // Show a save file dialog to choose the path
                string path = EditorUtility.SaveFilePanel("Save Scene As", "Assets", $"{scene.name}_SERVER", "unity");

                // Check if the path is valid
                if (!string.IsNullOrEmpty(path))
                {
                    // Convert the path to a relative path
                    string relativePath = FileUtil.GetProjectRelativePath(path);

                    // Save the scene to the new path
                    bool success = EditorSceneManager.SaveScene(scene, relativePath);

                    // Log the result
                    if (success)
                    {
                        Debug.Log("Scene saved successfully as " + relativePath);
                    }
                    else
                    {
                        Debug.LogError("Failed to save the scene.");
                    }
                }
                else
                {
                    Debug.LogError("Invalid path specified.");
                }
            }
            else
            {
                Debug.LogError("No valid active scene to save.");
            }
        }

        static void UnpackPrefabInstances(GameObject gameObject)
        {
            // Check if the GameObject is part of a prefab instance
            if (PrefabUtility.IsAnyPrefabInstanceRoot(gameObject))
            {
                // Unpack the prefab instance completely
                PrefabUtility.UnpackPrefabInstance(gameObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
            }

            // Recursively unpack child objects
            foreach (Transform child in gameObject.transform)
            {
                UnpackPrefabInstances(child.gameObject);
            }
        }
    }
}