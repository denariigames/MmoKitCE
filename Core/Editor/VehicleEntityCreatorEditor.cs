using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using LiteNetLibManager;
using System.Reflection;

namespace MultiplayerARPG
{
    public class VehicleEntityCreatorEditor : EditorWindow
    {
        private string fileName;
        private GameDatabase gameDatabase;
        private GameObject fbx;

        private static readonly List<ICharacterModelFactory> modelFactories = new List<ICharacterModelFactory>();
        private static readonly List<string> modelNames = new List<string>();
        private static int selectedModelIndex = 0;

        private static readonly List<IEntityMovementFactory> movementFactories = new List<IEntityMovementFactory>();
        private static readonly List<string> movementNames = new List<string>();
        private static int selectedMovementIndex = 0;

        [MenuItem(EditorMenuConsts.VEHICLE_ENTITY_CREATOR_MENU, false, EditorMenuConsts.VEHICLE_ENTITY_CREATOR_ORDER)]
        public static void CreateNewVehicleEntity()
        {
            // Init classes
            selectedModelIndex = 0;
            modelFactories.Clear();
            modelNames.Clear();
            movementFactories.Clear();
            movementNames.Clear();
            System.Type modelFactoryType = typeof(ICharacterModelFactory);
            System.Type movementFactoryType = typeof(IEntityMovementFactory);
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
                    if (type != movementFactoryType && movementFactoryType.IsAssignableFrom(type))
                    {
                        IEntityMovementFactory movementFactory = System.Activator.CreateInstance(type) as IEntityMovementFactory;
                        if (movementFactory.DimensionType == DimensionType.Dimension3D)
                        {
                            movementFactories.Add(movementFactory);
                            movementNames.Add(movementFactory.Name);
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
                GetWindow<VehicleEntityCreatorEditor>();
            }
        }

        private void OnGUI()
        {
            Vector2 wndRect = new Vector2(500, 500);
            maxSize = wndRect;
            minSize = wndRect;
            titleContent = new GUIContent("Vehicle Entity", null, "Vehicle Entity Creator (3D)");
            GUILayout.BeginVertical("Vehicle Entity Creator", "window");
            {
                GUILayout.BeginVertical("box");
                {
                    fileName = EditorGUILayout.TextField("Filename", fileName);
                    selectedModelIndex = EditorGUILayout.Popup("Character model type", selectedModelIndex, modelNames.ToArray());
                    selectedMovementIndex = EditorGUILayout.Popup("Entity movement type", selectedMovementIndex, movementNames.ToArray());
                    if (gameDatabase == null)
                        EditorGUILayout.HelpBox("Select your game database which you want to add new vehicle entity, leave it `None` if you don't want to add vehicle entity to game database", MessageType.Info);
                    gameDatabase = EditorGUILayout.ObjectField("Game database", gameDatabase, typeof(GameDatabase), false, GUILayout.ExpandWidth(true)) as GameDatabase;
                    if (fbx == null)
                        EditorGUILayout.HelpBox("Select your FBX model which you want to create character entity", MessageType.Info);
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
            IEntityMovementFactory movementFactory = movementFactories[selectedMovementIndex];
            if (!modelFactory.ValidateSourceObject(newObject) ||
                !movementFactory.ValidateSourceObject(newObject))
            {
                DestroyImmediate(newObject);
                return;
            }

            Bounds bounds = default;
            movementFactory.Setup(newObject, ref bounds);

            VehicleEntity baseVehicleEntity = newObject.AddComponent<VehicleEntity>();
            if (baseVehicleEntity != null)
            {
                var tpsCamTarget = new GameObject("_TpsCamTarget");
                tpsCamTarget.transform.parent = baseVehicleEntity.transform;
                tpsCamTarget.transform.localPosition = Vector3.zero;
                tpsCamTarget.transform.localRotation = Quaternion.identity;
                tpsCamTarget.transform.localScale = Vector3.one;
                baseVehicleEntity.CameraTargetTransform = tpsCamTarget.transform;

                var fpsCamTarget = new GameObject("_FpsCamTarget");
                fpsCamTarget.transform.parent = baseVehicleEntity.transform;
                fpsCamTarget.transform.localPosition = Vector3.zero;
                fpsCamTarget.transform.localRotation = Quaternion.identity;
                fpsCamTarget.transform.localScale = Vector3.one;
                baseVehicleEntity.FpsCameraTargetTransform = fpsCamTarget.transform;

                var combatTextObj = new GameObject("_CombatText");
                combatTextObj.transform.parent = baseVehicleEntity.transform;
                combatTextObj.transform.localPosition = Vector3.zero + (Vector3.up * bounds.size.y * 0.75f);
                combatTextObj.transform.localRotation = Quaternion.identity;
                combatTextObj.transform.localScale = Vector3.one;
                baseVehicleEntity.CombatTextTransform = combatTextObj.transform;

                var opponentAimObj = new GameObject("_OpponentAim");
                opponentAimObj.transform.parent = baseVehicleEntity.transform;
                opponentAimObj.transform.localPosition = Vector3.zero + (Vector3.up * bounds.size.y * 0.75f);
                opponentAimObj.transform.localRotation = Quaternion.identity;
                opponentAimObj.transform.localScale = Vector3.one;
                baseVehicleEntity.OpponentAimTransform = opponentAimObj.transform;

                var savePath = path + "\\" + fileName + ".prefab";
                Debug.Log("Saving character entity to " + savePath);
                AssetDatabase.DeleteAsset(savePath);
                PrefabUtility.SaveAsPrefabAssetAndConnect(baseVehicleEntity.gameObject, savePath, InteractionMode.AutomatedAction);

                if (gameDatabase != null)
                {
                    GameObject savedObject = AssetDatabase.LoadAssetAtPath<GameObject>(savePath);
                    VehicleEntity savedEntity = savedObject.GetComponent<VehicleEntity>();
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
                    if (savedEntity is VehicleEntity)
                    {
                        List<VehicleEntity> list = new List<VehicleEntity>(gameDatabase.vehicleEntities)
                        {
                            savedEntity
                        };
                        gameDatabase.vehicleEntities = list.ToArray();
                    }
#endif
                    EditorUtility.SetDirty(gameDatabase);
                }
            }
        }
    }
}
