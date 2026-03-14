using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using LiteNetLibManager;
using System.Reflection;

namespace MultiplayerARPG
{
    public class CharacterEntityCreatorEditor : EditorWindow
    {
        public enum CharacterEntityType
        {
            PlayerCharacterEntity,
            MonsterCharacterEntity,
        }

        private string fileName;
        private string dataId;
        private CharacterEntityType characterEntityType;
        private GameDatabase gameDatabase;
        private GameObject fbx;

        private static readonly List<ICharacterModelFactory> modelFactories = new List<ICharacterModelFactory>();
        private static readonly List<string> modelNames = new List<string>();
        private static int selectedModelIndex = 0;

        private static readonly List<IEntityMovementFactory> movementFactories = new List<IEntityMovementFactory>();
        private static readonly List<string> movementNames = new List<string>();
        private static int selectedMovementIndex = 0;

        [MenuItem(EditorMenuConsts.CHARACTER_ENTITY_CREATOR_MENU, false, EditorMenuConsts.CHARACTER_ENTITY_CREATOR_ORDER)]
        public static void CreateNewCharacterEntity()
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
                GetWindow<CharacterEntityCreatorEditor>();
            }
        }

        private void OnGUI()
        {
            Vector2 wndRect = new Vector2(500, 500);
            maxSize = wndRect;
            minSize = wndRect;
            titleContent = new GUIContent("Character Entity", null, "Character Entity Creator (3D)");
            GUILayout.BeginVertical("Character Entity Creator", "window");
            {
                GUILayout.BeginVertical("box");
                {
                    fileName = EditorGUILayout.TextField("Filename", fileName);
                    characterEntityType = (CharacterEntityType)EditorGUILayout.EnumPopup("Character entity type", characterEntityType);
                    selectedModelIndex = EditorGUILayout.Popup("Character model type", selectedModelIndex, modelNames.ToArray());
                    selectedMovementIndex = EditorGUILayout.Popup("Entity movement type", selectedMovementIndex, movementNames.ToArray());
                    if (gameDatabase == null)
                        EditorGUILayout.HelpBox("Select your game database which you want to add new character entity, leave it `None` if you don't want to add character entity to game database", MessageType.Info);
                    gameDatabase = EditorGUILayout.ObjectField("Game database", gameDatabase, typeof(GameDatabase), false, GUILayout.ExpandWidth(true)) as GameDatabase;
                    if (fbx == null)
                        EditorGUILayout.HelpBox("Select your FBX model which you want to create character entity", MessageType.Info);
                    fbx = EditorGUILayout.ObjectField("FBX", fbx, typeof(GameObject), false, GUILayout.ExpandWidth(true)) as GameObject;
                }
                GUILayout.EndVertical();

                GUILayout.BeginVertical("box");
                {
                    EditorGUILayout.HelpBox("Leave `Character Data ID` to be empty to NOT create character data for this entity", MessageType.Info);
                    dataId = EditorGUILayout.TextField("Character Data ID", dataId);
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
            newObject.AddComponent<CharacterRecoveryComponent>();
            newObject.AddComponent<CharacterSkillAndBuffComponent>();

            ICharacterModelFactory modelFactory = modelFactories[selectedModelIndex];
            IEntityMovementFactory movementFactory = movementFactories[selectedMovementIndex];
            if (!modelFactory.ValidateSourceObject(newObject) ||
                !movementFactory.ValidateSourceObject(newObject))
            {
                DestroyImmediate(newObject);
                return;
            }

            CharacterModelManager characterModelManager = newObject.AddComponent<CharacterModelManager>();
            characterModelManager.MainTpsModel = modelFactory.Setup(newObject);

            Bounds bounds = default;
            movementFactory.Setup(newObject, ref bounds);

            BaseCharacterEntity baseCharacterEntity = null;
            switch (characterEntityType)
            {
                case CharacterEntityType.PlayerCharacterEntity:
                    newObject.AddComponent<PlayerCharacterCraftingComponent>();
                    newObject.AddComponent<PlayerCharacterItemLockAndExpireComponent>();
                    PlayerCharacterEntity playerCharacterEntity = newObject.AddComponent<PlayerCharacterEntity>();
                    baseCharacterEntity = playerCharacterEntity;
                    if (!string.IsNullOrEmpty(dataId))
                    {
                        PlayerCharacter data = CreateInstance<PlayerCharacter>();
                        data.Id = dataId;
                        data.Stats = new CharacterStatsIncremental()
                        {
                            baseStats = new CharacterStats()
                            {
                                hp = 100,
                                moveSpeed = 5,
                                atkSpeed = 1,
                            }
                        };
                        var dataSavePath = path + "\\" + fileName + "_PlayerCharacter.asset";
                        Debug.Log("Saving character data to " + dataSavePath);
                        AssetDatabase.DeleteAsset(dataSavePath);
                        AssetDatabase.CreateAsset(data, dataSavePath);
                        PlayerCharacter savedData = AssetDatabase.LoadAssetAtPath<PlayerCharacter>(dataSavePath);
                        playerCharacterEntity.CharacterDatabases = new PlayerCharacter[]
                        {
                            savedData
                        };
                        if (gameDatabase != null)
                        {
                            List<PlayerCharacter> list = new List<PlayerCharacter>(gameDatabase.playerCharacters);
                            list.Add(savedData);
                            gameDatabase.playerCharacters = list.ToArray();
                        }
                    }
                    break;
                case CharacterEntityType.MonsterCharacterEntity:
                    newObject.AddComponent<MonsterActivityComponent>();
                    MonsterCharacterEntity monsterCharacterEntity = newObject.AddComponent<MonsterCharacterEntity>();
                    baseCharacterEntity = monsterCharacterEntity;
                    if (!string.IsNullOrEmpty(dataId))
                    {
                        MonsterCharacter data = CreateInstance<MonsterCharacter>();
                        data.Id = dataId;
                        data.Stats = new CharacterStatsIncremental()
                        {
                            baseStats = new CharacterStats()
                            {
                                hp = 100,
                                moveSpeed = 5,
                                atkSpeed = 1,
                            }
                        };
                        var dataSavePath = path + "\\" + fileName + "_MonsterCharacter.asset";
                        Debug.Log("Saving character data to " + dataSavePath);
                        AssetDatabase.DeleteAsset(dataSavePath);
                        AssetDatabase.CreateAsset(data, dataSavePath);
                        MonsterCharacter savedData = AssetDatabase.LoadAssetAtPath<MonsterCharacter>(dataSavePath);
                        monsterCharacterEntity.CharacterDatabase = savedData;
                        if (gameDatabase != null)
                        {
                            List<MonsterCharacter> list = new List<MonsterCharacter>(gameDatabase.monsterCharacters);
                            list.Add(savedData);
                            gameDatabase.monsterCharacters = list.ToArray();
                        }
                    }
                    break;
            }

            if (baseCharacterEntity != null)
            {
                var tpsCamTarget = new GameObject("_TpsCamTarget");
                tpsCamTarget.transform.parent = baseCharacterEntity.transform;
                tpsCamTarget.transform.localPosition = Vector3.zero;
                tpsCamTarget.transform.localRotation = Quaternion.identity;
                tpsCamTarget.transform.localScale = Vector3.one;
                baseCharacterEntity.CameraTargetTransform = tpsCamTarget.transform;

                var fpsCamTarget = new GameObject("_FpsCamTarget");
                fpsCamTarget.transform.parent = baseCharacterEntity.transform;
                fpsCamTarget.transform.localPosition = Vector3.zero;
                fpsCamTarget.transform.localRotation = Quaternion.identity;
                fpsCamTarget.transform.localScale = Vector3.one;
                baseCharacterEntity.FpsCameraTargetTransform = fpsCamTarget.transform;

                var combatTextObj = new GameObject("_CombatText");
                combatTextObj.transform.parent = baseCharacterEntity.transform;
                combatTextObj.transform.localPosition = Vector3.zero + (Vector3.up * bounds.size.y * 0.75f);
                combatTextObj.transform.localRotation = Quaternion.identity;
                combatTextObj.transform.localScale = Vector3.one;
                baseCharacterEntity.CombatTextTransform = combatTextObj.transform;

                var opponentAimObj = new GameObject("_OpponentAim");
                opponentAimObj.transform.parent = baseCharacterEntity.transform;
                opponentAimObj.transform.localPosition = Vector3.zero + (Vector3.up * bounds.size.y * 0.75f);
                opponentAimObj.transform.localRotation = Quaternion.identity;
                opponentAimObj.transform.localScale = Vector3.one;
                baseCharacterEntity.OpponentAimTransform = opponentAimObj.transform;

                var meleeDamageObj = new GameObject("_MeleeDamage");
                meleeDamageObj.transform.parent = baseCharacterEntity.transform;
                meleeDamageObj.transform.localPosition = Vector3.zero + (Vector3.up * bounds.size.y * 0.75f);
                meleeDamageObj.transform.localRotation = Quaternion.identity;
                meleeDamageObj.transform.localScale = Vector3.one;
                baseCharacterEntity.MeleeDamageTransform = meleeDamageObj.transform;

                var missileDamageObj = new GameObject("_MissileDamage");
                missileDamageObj.transform.parent = baseCharacterEntity.transform;
                missileDamageObj.transform.localPosition = Vector3.zero + (Vector3.up * bounds.size.y * 0.75f);
                missileDamageObj.transform.localRotation = Quaternion.identity;
                missileDamageObj.transform.localScale = Vector3.one;
                baseCharacterEntity.MissileDamageTransform = missileDamageObj.transform;

                var characterUiObj = new GameObject("_CharacterUi");
                characterUiObj.transform.parent = baseCharacterEntity.transform;
                characterUiObj.transform.localPosition = Vector3.zero;
                characterUiObj.transform.localRotation = Quaternion.identity;
                characterUiObj.transform.localScale = Vector3.one;
                baseCharacterEntity.CharacterUiTransform = characterUiObj.transform;

                var miniMapUiObj = new GameObject("_MiniMapUi");
                miniMapUiObj.transform.parent = baseCharacterEntity.transform;
                miniMapUiObj.transform.localPosition = Vector3.zero;
                miniMapUiObj.transform.localRotation = Quaternion.identity;
                miniMapUiObj.transform.localScale = Vector3.one;
                baseCharacterEntity.MiniMapUiTransform = miniMapUiObj.transform;

                var bubbleChat = new GameObject("_ChatBubble");
                bubbleChat.transform.parent = baseCharacterEntity.transform;
                bubbleChat.transform.localPosition = Vector3.zero + (Vector3.up * bounds.size.y);
                bubbleChat.transform.localRotation = Quaternion.identity;
                bubbleChat.transform.localScale = Vector3.one;
                baseCharacterEntity.ChatBubbleTransform = bubbleChat.transform;

                var savePath = path + "\\" + fileName + ".prefab";
                Debug.Log("Saving character entity to " + savePath);
                AssetDatabase.DeleteAsset(savePath);
                PrefabUtility.SaveAsPrefabAssetAndConnect(baseCharacterEntity.gameObject, savePath, InteractionMode.AutomatedAction);

                if (gameDatabase != null)
                {
                    GameObject savedObject = AssetDatabase.LoadAssetAtPath<GameObject>(savePath);
                    BaseCharacterEntity savedEntity = savedObject.GetComponent<BaseCharacterEntity>();
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
                    if (savedEntity is BasePlayerCharacterEntity)
                    {
                        List<BasePlayerCharacterEntity> list = new List<BasePlayerCharacterEntity>(gameDatabase.playerCharacterEntities);
                        list.Add(savedEntity as BasePlayerCharacterEntity);
                        gameDatabase.playerCharacterEntities = list.ToArray();
                    }
                    else if (savedEntity is BaseMonsterCharacterEntity)
                    {
                        List<BaseMonsterCharacterEntity> list = new List<BaseMonsterCharacterEntity>(gameDatabase.monsterCharacterEntities);
                        list.Add(savedEntity as BaseMonsterCharacterEntity);
                        gameDatabase.monsterCharacterEntities = list.ToArray();
                    }
#endif
                    EditorUtility.SetDirty(gameDatabase);
                }
            }
        }
    }
}
