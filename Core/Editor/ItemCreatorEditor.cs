using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UtilsComponents;

namespace MultiplayerARPG
{
    public class ItemCreatorEditor : EditorWindow
    {
        private string fileName;
        private string id;
        private ItemType itemType;
        private GameDatabase gameDatabase;
        private GameObject dropModel;
        private Vector3 dropModelOffsets;
        private Vector3 dropModelRotateOffsets;
        private GameObject equipModelR;
        private string equipSocketR;
        private bool useDifferenceModelWithR;
        private GameObject equipModelL;
        private string equipSocketL;
        private BaseItem copySource;

        private static Vector3 savedDropModelOffsets;
        private static Vector3 savedDropModelRotateOffsets;

        [MenuItem(EditorMenuConsts.ITEM_CREATOR_MENU, false, EditorMenuConsts.ITEM_CREATOR_ORDER)]
        public static void CreateNewEditor()
        {
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
                GetWindow<ItemCreatorEditor>();
            }
        }

        private void OnGUI()
        {
            Vector2 wndRect = new Vector2(500, 500);
            maxSize = wndRect;
            minSize = wndRect;
            titleContent = new GUIContent("Item", null, "Item Creator");
            GUILayout.BeginVertical("Item Creator", "window");
            {
                GUILayout.BeginVertical("box");
                {
                    fileName = EditorGUILayout.TextField("Filename", fileName);
                    id = EditorGUILayout.TextField("ID", id);
                    itemType = (ItemType)EditorGUILayout.EnumPopup("Item to create", itemType);
                    if (gameDatabase == null)
                        EditorGUILayout.HelpBox("Select your game database which you want to add new item data, leave it `None` if you don't want to add item data to game database", MessageType.Info);
                    gameDatabase = EditorGUILayout.ObjectField("Game database", gameDatabase, typeof(GameDatabase), false, GUILayout.ExpandWidth(true)) as GameDatabase;
                    if (dropModel == null)
                        EditorGUILayout.HelpBox("Select your FBX model which you want to use as drop model", MessageType.Info);
                    dropModel = EditorGUILayout.ObjectField("Drop Model", dropModel, typeof(GameObject), false, GUILayout.ExpandWidth(true)) as GameObject;
                    dropModelOffsets = EditorGUILayout.Vector3Field("Offsets", dropModelOffsets);
                    dropModelRotateOffsets = EditorGUILayout.Vector3Field("Rotate Offsets", dropModelRotateOffsets);
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("Load Offsets From Latest Save", GUILayout.ExpandWidth(true), GUILayout.Height(30)))
                    {
                        dropModelOffsets = savedDropModelOffsets;
                        dropModelRotateOffsets = savedDropModelRotateOffsets;
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();

                if (itemType == ItemType.Armor || itemType == ItemType.Weapon || itemType == ItemType.Shield)
                {
                    GUILayout.BeginVertical("box");
                    equipSocketR = EditorGUILayout.TextField("Equip Socket: ", equipSocketR);
                    if (equipModelR == null)
                        EditorGUILayout.HelpBox("Select your FBX model which you want to use as equip model", MessageType.Info);
                    equipModelR = EditorGUILayout.ObjectField("Equip Model", equipModelR, typeof(GameObject), false, GUILayout.ExpandWidth(true)) as GameObject;
                    GUILayout.EndVertical();
                }
                else
                {
                    equipModelR = null;
                }
                if (itemType == ItemType.Weapon)
                {
                    GUILayout.BeginVertical("box");
                    equipSocketL = EditorGUILayout.TextField("Offhand Equip Socket: ", equipSocketL);
                    useDifferenceModelWithR = EditorGUILayout.Toggle("Use Different Equip Model", useDifferenceModelWithR);
                    if (useDifferenceModelWithR)
                    {
                        if (equipModelL == null)
                            EditorGUILayout.HelpBox("Select your FBX model which you want to use as offhand equip model", MessageType.Info);
                        equipModelL = EditorGUILayout.ObjectField("Offhand Equip Model", equipModelL, typeof(GameObject), false, GUILayout.ExpandWidth(true)) as GameObject;
                    }
                    else
                    {
                        equipModelL = null;
                    }
                    GUILayout.EndVertical();
                }
                else
                {
                    equipModelL = null;
                }

                GUILayout.BeginVertical("box");
                copySource = EditorGUILayout.ObjectField("Copy Source", copySource, typeof(BaseItem), false, GUILayout.ExpandWidth(true)) as BaseItem;
                if (copySource != null && copySource.ItemType != itemType)
                {
                    Debug.LogError("Cannot set different kind of copy source data");
                    copySource = null;
                }
                GUILayout.EndVertical();

                if (!string.IsNullOrEmpty(fileName) && !string.IsNullOrEmpty(id))
                {
                    savedDropModelOffsets = dropModelOffsets;
                    savedDropModelRotateOffsets = dropModelRotateOffsets;
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

            // Create drop item
            GameObject savedDropModel = null;
            if (dropModel != null)
            {
                var newDropModelPlaceHolder = new GameObject(fileName + "_DropModel");
                newDropModelPlaceHolder.AddComponent<PivotHighlighter>();
                var newDropModel = PrefabUtility.InstantiatePrefab(dropModel) as GameObject;
                newDropModel.transform.parent = newDropModelPlaceHolder.transform;
                newDropModel.transform.localPosition = dropModelOffsets;
                newDropModel.transform.localEulerAngles = dropModelRotateOffsets;
                newDropModel.transform.localScale = Vector3.one;

                var dropModelSavePath = path + "\\" + fileName + "_DropModel.prefab";
                Debug.Log("Saving drop model to " + dropModelSavePath);
                AssetDatabase.DeleteAsset(dropModelSavePath);
                savedDropModel = PrefabUtility.SaveAsPrefabAssetAndConnect(newDropModelPlaceHolder, dropModelSavePath, InteractionMode.AutomatedAction);
            }

            // Create equip model
            GameObject savedEquipModelR = null;
            if (equipModelR != null && (itemType == ItemType.Armor || itemType == ItemType.Weapon || itemType == ItemType.Shield))
            {
                var newEquipModelPlaceHolder = new GameObject(fileName + "_EquipModel");
                var newMissileDamageTransform = new GameObject("_MissileDamageTransform");
                newMissileDamageTransform.transform.parent = newEquipModelPlaceHolder.transform;
                newMissileDamageTransform.transform.localPosition = Vector3.zero;
                newMissileDamageTransform.transform.localEulerAngles = Vector3.zero;
                newMissileDamageTransform.transform.localScale = Vector3.one;
                var newEntity = newEquipModelPlaceHolder.AddComponent<EquipmentEntity>();
                newEntity.missileDamageTransform = newMissileDamageTransform.transform;
                var newEquipModel = PrefabUtility.InstantiatePrefab(equipModelR) as GameObject;
                newEquipModel.transform.parent = newEquipModelPlaceHolder.transform;
                newEquipModel.transform.localPosition = Vector3.zero;
                newEquipModel.transform.localEulerAngles = Vector3.zero;
                newEquipModel.transform.localScale = Vector3.one;

                var savePath = path + "\\" + fileName + "_EquipModel.prefab";
                Debug.Log("Saving drop model to " + savePath);
                AssetDatabase.DeleteAsset(savePath);
                savedEquipModelR = PrefabUtility.SaveAsPrefabAssetAndConnect(newEquipModelPlaceHolder, savePath, InteractionMode.AutomatedAction);
            }

            // Create offhand equip model
            GameObject savedEquipModelL = null;
            if (equipModelL != null && itemType == ItemType.Weapon)
            {
                var newEquipModelPlaceHolder = new GameObject(fileName + "_EquipModel_Offhand");
                var newMissileDamageTransform = new GameObject("_MissileDamageTransform");
                newMissileDamageTransform.transform.parent = newEquipModelPlaceHolder.transform;
                newMissileDamageTransform.transform.localPosition = Vector3.zero;
                newMissileDamageTransform.transform.localEulerAngles = Vector3.zero;
                newMissileDamageTransform.transform.localScale = Vector3.one;
                var newEntity = newEquipModelPlaceHolder.AddComponent<EquipmentEntity>();
                newEntity.missileDamageTransform = newMissileDamageTransform.transform;
                var newEquipModel = PrefabUtility.InstantiatePrefab(equipModelL) as GameObject;
                newEquipModel.transform.parent = newEquipModelPlaceHolder.transform;
                newEquipModel.transform.localPosition = Vector3.zero;
                newEquipModel.transform.localEulerAngles = Vector3.zero;
                newEquipModel.transform.localScale = Vector3.one;

                var savePath = path + "\\" + fileName + "_EquipModel_Offhand.prefab";
                Debug.Log("Saving drop model to " + savePath);
                AssetDatabase.DeleteAsset(savePath);
                savedEquipModelL = PrefabUtility.SaveAsPrefabAssetAndConnect(newEquipModelPlaceHolder, savePath, InteractionMode.AutomatedAction);
            }

            // Create or Copy item
            BaseItem newItemData = null;
            if (copySource == null)
            {
                switch (itemType)
                {
                    case ItemType.Junk:
                        newItemData = CreateInstance<JunkItem>();
                        break;
                    case ItemType.Armor:
                        newItemData = CreateInstance<ArmorItem>();
                        break;
                    case ItemType.Weapon:
                        newItemData = CreateInstance<WeaponItem>();
                        break;
                    case ItemType.Shield:
                        newItemData = CreateInstance<ShieldItem>();
                        break;
                    case ItemType.Potion:
                        newItemData = CreateInstance<PotionItem>();
                        break;
                    case ItemType.Ammo:
                        newItemData = CreateInstance<AmmoItem>();
                        break;
                    case ItemType.Building:
                        newItemData = CreateInstance<BuildingItem>();
                        break;
                    case ItemType.Pet:
                        newItemData = CreateInstance<PetItem>();
                        break;
                    case ItemType.SocketEnhancer:
                        newItemData = CreateInstance<SocketEnhancerItem>();
                        break;
                    case ItemType.Mount:
                        newItemData = CreateInstance<MountItem>();
                        break;
                    case ItemType.Skill:
                        newItemData = CreateInstance<SkillItem>();
                        break;
                }
            }
            else
            {
                newItemData = Instantiate(copySource);
            }
            newItemData.Id = id;
            if (savedDropModel != null)
                newItemData.SetDropModel(savedDropModel);

            if (savedEquipModelR != null && (itemType == ItemType.Armor || itemType == ItemType.Weapon || itemType == ItemType.Shield))
            {
                var equipmentModels = new EquipmentModel[]
                {
                    new EquipmentModel()
                    {
                        equipSocket = equipSocketR,
                        useInstantiatedObject = false,
                    }.SetMeshPrefab(savedEquipModelR),
                };
                if (newItemData is Item)
                {
                    (newItemData as Item).equipmentModels = equipmentModels;
                }
                else
                {
                    (newItemData as BaseEquipmentItem).EquipmentModels = equipmentModels;
                }
            }

            if (!useDifferenceModelWithR)
                savedEquipModelL = savedEquipModelR;

            if (savedEquipModelL != null && itemType == ItemType.Weapon)
            {
                var equipmentModels = new EquipmentModel[]
                {
                    new EquipmentModel()
                    {
                        equipSocket = equipSocketL,
                        useInstantiatedObject = false,
                    }.SetMeshPrefab(savedEquipModelL),
                };
                if (newItemData is Item)
                {
                    (newItemData as Item).offHandEquipmentModels = equipmentModels;
                }
                else
                {
                    (newItemData as WeaponItem).OffHandEquipmentModels = equipmentModels;
                }
            }


            var itemDataSavePath = path + "\\" + fileName + ".asset";
            Debug.Log("Saving item data to " + itemDataSavePath);
            AssetDatabase.DeleteAsset(itemDataSavePath);
            AssetDatabase.CreateAsset(newItemData, itemDataSavePath);

            if (gameDatabase != null)
            {
                BaseItem savedItem = AssetDatabase.LoadAssetAtPath<BaseItem>(itemDataSavePath);
                List<BaseItem> items = new List<BaseItem>(gameDatabase.items);
                for (int i = items.Count - 1; i >= 0; --i)
                {
                    if (items[i].Id.Equals(savedItem.Id))
                    {
                        Debug.Log("Found old item data with the same ID " + i + ", old item data will be removed from the list in game database");
                        items.RemoveAt(i);
                    }
                }
                items.Add(savedItem);
                gameDatabase.items = items.ToArray();
                EditorUtility.SetDirty(gameDatabase);
            }
        }
    }
}
