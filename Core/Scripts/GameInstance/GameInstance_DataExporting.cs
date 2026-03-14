#if UNITY_EDITOR
using Insthync.AddressableAssetTools;
using Insthync.UnityEditorUtils;
using LiteNetLibManager;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
#if !DISABLE_ADDRESSABLES
using UnityEngine.AddressableAssets;
#endif

namespace MultiplayerARPG
{
    public partial class GameInstance
    {
        [Header("Data Exporting")]
        [InspectorButton(nameof(ExportSocialSystemSettingAsJson))]
        public bool exportSocialSystemSettingAsJson;
        public void ExportSocialSystemSettingAsJson()
        {
            SocialSystemSetting socialSystemSetting = this.socialSystemSetting;
            if (socialSystemSetting == null)
                socialSystemSetting = ScriptableObject.CreateInstance<SocialSystemSetting>();
            string path = EditorUtility.SaveFilePanel("Export Social System Setting", Application.dataPath, "socialSystemSetting", "json");
            if (path.Length > 0)
            {
                File.WriteAllText(path, JsonConvert.SerializeObject(socialSystemSetting, Formatting.Indented, new JsonSerializerSettings()
                {
                    ContractResolver = new SocialSystemSettingContractResolver(),
                    Converters = new List<JsonConverter>
                    {
                        new GameDataIntDictJsonConverter<BaseItem>(Items),
                        new GameDataIntDictJsonConverter<Currency>(Currencies),
                    },
                }));
            }
        }

        [InspectorButton(nameof(ExportMinimalItemsAsJson))]
        public bool exportMinimalItemsAsJson;
        public void ExportMinimalItemsAsJson()
        {
            ClearData();
            OnGameDataLoadedEvent += OnGameDataLoadedToExportMinimalItemsAsJson;
            gameDatabase.LoadData(this).Forget();
        }

        private void OnGameDataLoadedToExportMinimalItemsAsJson()
        {
            OnGameDataLoadedEvent -= OnGameDataLoadedToExportMinimalItemsAsJson;
            Dictionary<int, MinimalItem> exportingItems = new Dictionary<int, MinimalItem>();
            foreach (var kv in Items)
            {
                MinimalItem item = new MinimalItem()
                {
                    Id = kv.Value.Id,
                    DataId = kv.Value.DataId,
                    ItemType = (int)kv.Value.ItemType,
                    SellPrice = kv.Value.SellPrice,
                    Weight = kv.Value.Weight,
                    MaxStack = kv.Value.MaxStack,
                    MaxLevel = kv.Value.MaxLevel,
                    LockDuration = kv.Value.LockDuration,
                    ExpireDuration = kv.Value.ExpireDuration,
                };
                if (kv.Value.IsEquipment())
                {
                    item.MaxDurability = (kv.Value as IEquipmentItem).MaxDurability;
                    item.DestroyIfBroken = (kv.Value as IEquipmentItem).DestroyIfBroken;
                    item.MaxSocket = (kv.Value as IEquipmentItem).AvailableSocketEnhancerTypes?.Length ?? 0;
                }
                if (kv.Value.IsWeapon())
                {
                    item.AmmoCapacity = (kv.Value as IWeaponItem).AmmoCapacity;
                }
                exportingItems[kv.Key] = item;
            }
            string path = EditorUtility.SaveFilePanel("Export Minimal Items", Application.dataPath, "items", "json");
            if (path.Length > 0)
                File.WriteAllText(path, JsonConvert.SerializeObject(exportingItems, Formatting.Indented));
        }

        [InspectorButton(nameof(ExportCharacterCreationDataAsJson))]
        public bool exportCharacterCreationDataAsJson;
        public void ExportCharacterCreationDataAsJson()
        {
            ClearData();
            OnGameDataLoadedEvent += OnGameDataLoadedToExportCharacterCreationDataAsJson;
            gameDatabase.LoadData(this).Forget();
        }

        private void OnGameDataLoadedToExportCharacterCreationDataAsJson()
        {
            OnGameDataLoadedEvent -= OnGameDataLoadedToExportCharacterCreationDataAsJson;

            Singleton = this;

            DefaultArmorType = ScriptableObject.CreateInstance<ArmorType>()
                .GenerateDefaultArmorType();

            DefaultWeaponType = ScriptableObject.CreateInstance<WeaponType>()
                .GenerateDefaultWeaponType();

            bool haveToClearDefaultWeaponItem = false;
            // Setup default weapon item if not existed
            if (defaultWeaponItem == null || !defaultWeaponItem.IsWeapon())
            {
                defaultWeaponItem = ScriptableObject.CreateInstance<Item>()
                    .GenerateDefaultItem(DefaultWeaponType);
                haveToClearDefaultWeaponItem = true;
            }

            bool haveToClearDefaultDamageElement = false;
            // Setup default damage element if not existed
            if (defaultDamageElement == null)
            {
                defaultDamageElement = ScriptableObject.CreateInstance<DamageElement>()
                    .GenerateDefaultDamageElement(DefaultDamageHitEffects
#if !DISABLE_ADDRESSABLES
                    , AddressableDefaultDamageHitEffects
#endif
                    );
                haveToClearDefaultDamageElement = true;
            }

            CharacterCreationData data = new CharacterCreationData();
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
            foreach (var kv in PlayerCharacterEntities)
            {
                if (kv.Value.CharacterDatabases == null || kv.Value.CharacterDatabases.Length == 0)
                    continue;
                if (!data.AvailableCharacters.ContainsKey(kv.Key))
                    data.AvailableCharacters[kv.Key] = new Dictionary<int, PlayerCharacterData>();
                foreach (var database in kv.Value.CharacterDatabases)
                {
                    data.AvailableCharacters[kv.Key][database.DataId] = new PlayerCharacterData().SetNewPlayerCharacterData(string.Empty, database.DataId, kv.Key, 0);
                }
            }
#endif
#if !DISABLE_ADDRESSABLES
            foreach (var kv in AddressablePlayerCharacterEntities)
            {
                if (!kv.Value.IsDataValid())
                    continue;
                var op = kv.Value.InstantiateAsync();
                op.WaitForCompletion();
                BasePlayerCharacterEntity comp = op.Result.GetComponent<BasePlayerCharacterEntity>();
                if (comp.CharacterDatabases == null || comp.CharacterDatabases.Length == 0)
                {
                    Addressables.ReleaseInstance(op.Result);
                    Addressables.Release(op);
                    DestroyImmediate(op.Result);
                    continue;
                }
                if (!data.AvailableCharacters.ContainsKey(kv.Key))
                    data.AvailableCharacters[kv.Key] = new Dictionary<int, PlayerCharacterData>();
                foreach (var database in comp.CharacterDatabases)
                {
                    data.AvailableCharacters[kv.Key][database.DataId] = new PlayerCharacterData().SetNewPlayerCharacterData(string.Empty, database.DataId, kv.Key, 0);
                }
                Addressables.ReleaseInstance(op.Result);
                Addressables.Release(op);
                DestroyImmediate(op.Result);
            }
#endif
            foreach (var kv in Factions)
            {
                if (!kv.Value.IsLocked)
                    data.AvailableFactionIds.Add(kv.Key);
            }

            DestroyImmediate(DefaultArmorType);
            DefaultArmorType = null;

            DestroyImmediate(DefaultWeaponType);
            DefaultWeaponType = null;

            if (haveToClearDefaultWeaponItem)
            {
                DestroyImmediate(defaultWeaponItem);
                defaultWeaponItem = null;
            }

            if (haveToClearDefaultDamageElement)
            {
                DestroyImmediate(defaultDamageElement);
                defaultDamageElement = null;
            }

            string path = EditorUtility.SaveFilePanel("Export Character Creation Data", Application.dataPath, "characterCreationData", "json");
            if (path.Length > 0)
                File.WriteAllText(path, JsonConvert.SerializeObject(data, Formatting.Indented));
        }
    }
}
#endif