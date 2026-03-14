using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using LiteNetLibManager;
using System.Reflection;

namespace MultiplayerARPG
{
    public class NpcEntityCreatorEditor : EditorWindow
    {
        private string fileName;
        private NpcDatabase npcDatabase;
        private BaseMapInfo mapInfo;
        private Vector3 entityPosition;
        private Vector3 entityRotation;
        private GameObject fbx;

        private static readonly List<ICharacterModelFactory> modelFactories = new List<ICharacterModelFactory>();
        private static readonly List<string> modelNames = new List<string>();
        private static int selectedModelIndex = 0;

        [MenuItem(EditorMenuConsts.NPC_ENTITY_CREATOR_MENU, false, EditorMenuConsts.NPC_ENTITY_CREATOR_ORDER)]
        public static void CreateNewNpcEntity()
        {
            // Init classes
            selectedModelIndex = 0;
            modelFactories.Clear();
            modelNames.Clear();
            System.Type modelFactoryType = typeof(ICharacterModelFactory);
            Assembly[] assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
            {
                foreach (System.Type type in assembly.GetTypes())
                {
                    if (type != modelFactoryType && modelFactoryType.IsAssignableFrom(type))
                    {
                        ICharacterModelFactory modelFactory = System.Activator.CreateInstance(type) as ICharacterModelFactory;
                        if (modelFactory.DimensionType == DimensionType.Dimension3D)
                        {
                            modelFactories.Add(modelFactory);
                            modelNames.Add(modelFactory.Name);
                        }
                    }
                }
            }

            bool gettingWindow;
            if (EditorGlobalData.EditorScene.HasValue)
            {
                gettingWindow = true;
                EditorSceneManager.CloseScene(EditorGlobalData.EditorScene.Value, true);
                EditorGlobalData.EditorScene = null;
            }
            else
            {
                gettingWindow = EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            }
            if (gettingWindow)
            {
                EditorGlobalData.EditorScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                GetWindow<NpcEntityCreatorEditor>();
            }
        }

        private void OnGUI()
        {
            Vector2 wndRect = new Vector2(500, 500);
            maxSize = wndRect;
            minSize = wndRect;
            titleContent = new GUIContent("Npc Entity", null, "Npc Entity Creator (3D)");
            GUILayout.BeginVertical("Npc Entity Creator", "window");
            {
                GUILayout.BeginVertical("box");
                {
                    fileName = EditorGUILayout.TextField("Filename", fileName);
                    selectedModelIndex = EditorGUILayout.Popup("Character model type", selectedModelIndex, modelNames.ToArray());
                    if (npcDatabase == null)
                        EditorGUILayout.HelpBox("Select your NPC database which you want to add new NPC entity, leave it `None` if you don't want to add NPC entity to NPC database", MessageType.Info);
                    npcDatabase = EditorGUILayout.ObjectField("NPC database", npcDatabase, typeof(NpcDatabase), false, GUILayout.ExpandWidth(true)) as NpcDatabase;
                    if (npcDatabase != null)
                    {
                        if (mapInfo == null)
                            EditorGUILayout.HelpBox("Select the map which you want to spawn the NPC entity", MessageType.Info);
                        mapInfo = EditorGUILayout.ObjectField("Map Info", mapInfo, typeof(BaseMapInfo), false, GUILayout.ExpandWidth(true)) as BaseMapInfo;
                        entityPosition = EditorGUILayout.Vector3Field("NPC Position", entityPosition);
                        entityRotation = EditorGUILayout.Vector3Field("NPC Rotation", entityRotation);
                    }
                    if (fbx == null)
                        EditorGUILayout.HelpBox("Select your FBX model which you want to create Npc entity", MessageType.Info);
                    fbx = EditorGUILayout.ObjectField("FBX", fbx, typeof(GameObject), false, GUILayout.ExpandWidth(true)) as GameObject;
                }
                GUILayout.EndVertical();

                if (fbx != null && !string.IsNullOrEmpty(fileName))
                {
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("Create", GUILayout.ExpandWidth(true), GUILayout.Height(40)))
                        Create();
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.EndVertical();
        }

        private void Create()
        {
            var path = EditorUtility.SaveFolderPanel("Save data to folder", "Assets", "");
            path = path.Substring(path.IndexOf("Assets"));

            var newObject = Instantiate(fbx, Vector3.zero, Quaternion.identity);
            newObject.AddComponent<LiteNetLibIdentity>();

            ICharacterModelFactory modelFactory = modelFactories[selectedModelIndex];
            if (!modelFactory.ValidateSourceObject(newObject))
            {
                DestroyImmediate(newObject);
                return;
            }
            //Add Character Model Type
            modelFactory.Setup(newObject);

            NpcEntity npcEntity = newObject.AddComponent<NpcEntity>();
            if (npcEntity != null)
            {
                var tpsCamTarget = new GameObject("_TpsCamTarget");
                tpsCamTarget.transform.parent = npcEntity.transform;
                tpsCamTarget.transform.localPosition = Vector3.zero;
                tpsCamTarget.transform.localRotation = Quaternion.identity;
                tpsCamTarget.transform.localScale = Vector3.one;
                npcEntity.CameraTargetTransform = tpsCamTarget.transform;

                var fpsCamTarget = new GameObject("_FpsCamTarget");
                fpsCamTarget.transform.parent = npcEntity.transform;
                fpsCamTarget.transform.localPosition = Vector3.zero;
                fpsCamTarget.transform.localRotation = Quaternion.identity;
                fpsCamTarget.transform.localScale = Vector3.one;
                npcEntity.FpsCameraTargetTransform = fpsCamTarget.transform;

                //Add Collider
                Bounds bounds = default;
                MeshRenderer[] meshes = newObject.GetComponentsInChildren<MeshRenderer>();
                for (int i = 0; i < meshes.Length; ++i)
                {
                    if (i > 0)
                        bounds.Encapsulate(meshes[i].bounds);
                    else
                        bounds = meshes[i].bounds;
                }

                SkinnedMeshRenderer[] skinnedMeshes = newObject.GetComponentsInChildren<SkinnedMeshRenderer>();
                for (int i = 0; i < skinnedMeshes.Length; ++i)
                {
                    if (i > 0)
                        bounds.Encapsulate(skinnedMeshes[i].bounds);
                    else
                        bounds = skinnedMeshes[i].bounds;
                }

                float scale = Mathf.Max(newObject.transform.localScale.x, newObject.transform.localScale.y, newObject.transform.localScale.z);
                bounds.size = bounds.size / scale;
                bounds.center = bounds.center / scale;

                CapsuleCollider capsuleCollider = newObject.AddComponent<CapsuleCollider>();
                capsuleCollider.height = bounds.size.y;
                capsuleCollider.radius = Mathf.Min(bounds.extents.x, bounds.extents.z);
                capsuleCollider.center = Vector3.zero + (Vector3.up * capsuleCollider.height * 0.5f);

                var savePath = path + "\\" + fileName + ".prefab";
                Debug.Log("Saving Npc entity to " + savePath);
                AssetDatabase.DeleteAsset(savePath);
                PrefabUtility.SaveAsPrefabAssetAndConnect(npcEntity.gameObject, savePath, InteractionMode.AutomatedAction);

                NpcDialogGraph graph = CreateInstance<NpcDialogGraph>();
                var graphSavePath = path + "\\" + fileName + "_NpcDialogGraph.asset";
                Debug.Log("Saving NPC entity to " + graphSavePath);
                AssetDatabase.DeleteAsset(graphSavePath);
                AssetDatabase.CreateAsset(graph, graphSavePath);

                if (npcDatabase != null && mapInfo != null)
                {
                    GameObject savedObject = AssetDatabase.LoadAssetAtPath<GameObject>(savePath);
                    NpcEntity savedEntity = savedObject.GetComponent<NpcEntity>();
                    NpcDialogGraph savedGraph = AssetDatabase.LoadAssetAtPath<NpcDialogGraph>(graphSavePath);
                    bool foundMapInfo = false;
                    List<Npcs> maps = new List<Npcs>(npcDatabase.maps);
                    for (int i = 0; i < maps.Count; ++i)
                    {
                        Npcs map = npcDatabase.maps[i];
                        if (map.mapInfo == mapInfo)
                        {
                            foundMapInfo = true;
                            List<Npc> npcs = new List<Npc>(map.npcs);
                            npcs.Add(new Npc()
                            {
                                entityPrefab = savedEntity,
                                graph = savedGraph,
                                position = entityPosition,
                                rotation = entityRotation,
                            });
                            map.npcs = npcs.ToArray();
                            npcDatabase.maps[i] = map;
                            break;
                        }
                    }
                    if (!foundMapInfo)
                    {
                        maps.Add(new Npcs()
                        {
                            mapInfo = mapInfo,
                            npcs = new Npc[1]
                            {
                                new Npc()
                                {
                                    entityPrefab = savedEntity,
                                    graph = savedGraph,
                                    position = entityPosition,
                                    rotation = entityRotation,
                                }
                            }
                        });
                    }
                    npcDatabase.maps = maps.ToArray();
                    EditorUtility.SetDirty(npcDatabase);
                }
            }
        }
    }
}
