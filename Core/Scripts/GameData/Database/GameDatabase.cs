using Cysharp.Threading.Tasks;
using Insthync.DevExtension;
using Insthync.UnityEditorUtils;
using LiteNetLibManager;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MultiplayerARPG
{
    /// <summary>
    /// This game database will load and setup game data from data that set in lists
    /// </summary>
    [CreateAssetMenu(fileName = GameDataMenuConsts.GAME_DATABASE_FILE, menuName = GameDataMenuConsts.GAME_DATABASE_MENU, order = GameDataMenuConsts.GAME_DATABASE_ORDER)]
    public partial class GameDatabase : BaseGameDatabase
    {
#if UNITY_EDITOR
        public UnityHelpBox entityHelpBox = new UnityHelpBox("Game database will load referring game data from an entities when game instance initializing");
#endif
        [Header("Entity")]
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
        public BasePlayerCharacterEntity[] playerCharacterEntities;
        public BaseMonsterCharacterEntity[] monsterCharacterEntities;
        [FormerlySerializedAs("mountEntities")]
        public VehicleEntity[] vehicleEntities;
        public LiteNetLibIdentity[] otherNetworkObjects;
#endif
#if !DISABLE_ADDRESSABLES
        [Header("Addressable Entity")]
        public AssetReferenceBasePlayerCharacterEntity[] addressablePlayerCharacterEntities;
        public AssetReferenceBaseMonsterCharacterEntity[] addressableMonsterCharacterEntities;
        public AssetReferenceVehicleEntity[] addressableVehicleEntities;
        public AssetReferenceLiteNetLibIdentity[] addressableOtherNetworkObjects;
#endif

        [Header("Game Data")]
        public Attribute[] attributes;
        public Currency[] currencies;
        public DamageElement[] damageElements;
        public BaseItem[] items;
        public ItemCraftFormula[] itemCraftFormulas;
        public ArmorType[] armorTypes;
        public WeaponType[] weaponTypes;
        public AmmoType[] ammoTypes;
        public BaseSkill[] skills;
        public PlayerIcon[] playerIcons;
        public PlayerFrame[] playerFrames;
        public PlayerBackground[] playerBackgrounds;
        public PlayerTitle[] playerTitles;
        public GuildSkill[] guildSkills;
        public GuildIcon[] guildIcons;
        public StatusEffect[] statusEffects;
        public PlayerCharacter[] playerCharacters;
        public PlayerCharacterEntityMetaData[] playerCharacterEntityMetaDataList;
        public MonsterCharacter[] monsterCharacters;
        public Harvestable[] harvestables;
        public BaseMapInfo[] mapInfos;
        public Quest[] quests;
        public Faction[] factions;
        public Gacha[] gachas;

        protected override UniTask LoadDataImplement(GameInstance gameInstance)
        {
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
            GameInstance.AddPlayerCharacterEntities(playerCharacterEntities);
#endif
#if !DISABLE_ADDRESSABLES
            GameInstance.AddAssetReferencePlayerCharacterEntities(addressablePlayerCharacterEntities);
#endif
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
            GameInstance.AddMonsterCharacterEntities(monsterCharacterEntities);
#endif
#if !DISABLE_ADDRESSABLES
            GameInstance.AddAssetReferenceMonsterCharacterEntities(addressableMonsterCharacterEntities);
#endif
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
            GameInstance.AddVehicleEntities(vehicleEntities);
#endif
#if !DISABLE_ADDRESSABLES
            GameInstance.AddAssetReferenceVehicleEntities(addressableVehicleEntities);
#endif
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
            GameInstance.AddOtherNetworkObjects(otherNetworkObjects);
#endif
#if !DISABLE_ADDRESSABLES
            GameInstance.AddAssetReferenceOtherNetworkObjects(addressableOtherNetworkObjects);
#endif
            GameInstance.AddAttributes(attributes);
            GameInstance.AddCurrencies(currencies);
            GameInstance.AddDamageElements(damageElements);
            GameInstance.AddItems(items);
            GameInstance.AddItemCraftFormulas(0, itemCraftFormulas);
            GameInstance.AddArmorTypes(armorTypes);
            GameInstance.AddWeaponTypes(weaponTypes);
            GameInstance.AddAmmoTypes(ammoTypes);
            GameInstance.AddSkills(skills);
            GameInstance.AddPlayerIcons(playerIcons);
            GameInstance.AddPlayerFrames(playerFrames);
            GameInstance.AddPlayerBackgrounds(playerBackgrounds);
            GameInstance.AddPlayerTitles(playerTitles);
            GameInstance.AddGuildSkills(guildSkills);
            GameInstance.AddGuildIcons(guildIcons);
            GameInstance.AddStatusEffects(statusEffects);
            GameInstance.AddCharacters(playerCharacters);
            GameInstance.AddPlayerCharacterEntityMetaDataList(playerCharacterEntityMetaDataList);
            GameInstance.AddCharacters(monsterCharacters);
            GameInstance.AddHarvestables(harvestables);
            GameInstance.AddMapInfos(mapInfos);
            GameInstance.AddQuests(quests);
            GameInstance.AddFactions(factions);
            GameInstance.AddGachas(gachas);
            this.InvokeInstanceDevExtMethods("LoadDataImplement", gameInstance);
            return default;
        }

        public void LoadReferredData()
        {
            GameInstance.ClearData();
            GameInstance.AddAttributes(attributes);
            GameInstance.AddCurrencies(currencies);
            GameInstance.AddDamageElements(damageElements);
            GameInstance.AddItems(items);
            GameInstance.AddItemCraftFormulas(0, itemCraftFormulas);
            GameInstance.AddArmorTypes(armorTypes);
            GameInstance.AddWeaponTypes(weaponTypes);
            GameInstance.AddAmmoTypes(ammoTypes);
            GameInstance.AddSkills(skills);
            GameInstance.AddPlayerIcons(playerIcons);
            GameInstance.AddPlayerFrames(playerFrames);
            GameInstance.AddPlayerBackgrounds(playerBackgrounds);
            GameInstance.AddPlayerTitles(playerTitles);
            GameInstance.AddGuildSkills(guildSkills);
            GameInstance.AddGuildIcons(guildIcons);
            GameInstance.AddStatusEffects(statusEffects);
            GameInstance.AddCharacters(playerCharacters);
            GameInstance.AddPlayerCharacterEntityMetaDataList(playerCharacterEntityMetaDataList);
            GameInstance.AddCharacters(monsterCharacters);
            GameInstance.AddHarvestables(harvestables);
            GameInstance.AddMapInfos(mapInfos);
            GameInstance.AddQuests(quests);
            GameInstance.AddFactions(factions);
            GameInstance.AddGachas(gachas);
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
            GameInstance.AddPlayerCharacterEntities(playerCharacterEntities);
#endif
#if !DISABLE_ADDRESSABLES
            GameInstance.AddAssetReferencePlayerCharacterEntities(addressablePlayerCharacterEntities);
#endif
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
            GameInstance.AddMonsterCharacterEntities(monsterCharacterEntities);
#endif
#if !DISABLE_ADDRESSABLES
            GameInstance.AddAssetReferenceMonsterCharacterEntities(addressableMonsterCharacterEntities);
#endif
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
            GameInstance.AddVehicleEntities(vehicleEntities);
#endif
#if !DISABLE_ADDRESSABLES
            GameInstance.AddAssetReferenceVehicleEntities(addressableVehicleEntities);
#endif

            List<Attribute> tempAttributes = new List<Attribute>(GameInstance.Attributes.Values);
            tempAttributes.Sort();
            attributes = tempAttributes.ToArray();

            List<Currency> tempCurrencies = new List<Currency>(GameInstance.Currencies.Values);
            tempCurrencies.Sort();
            currencies = tempCurrencies.ToArray();

            List<DamageElement> tempDamageElements = new List<DamageElement>(GameInstance.DamageElements.Values);
            tempDamageElements.Sort();
            damageElements = tempDamageElements.ToArray();

            List<ArmorType> tempArmorTypes = new List<ArmorType>(GameInstance.ArmorTypes.Values);
            tempArmorTypes.Sort();
            armorTypes = tempArmorTypes.ToArray();

            List<WeaponType> tempWeaponTypes = new List<WeaponType>(GameInstance.WeaponTypes.Values);
            tempWeaponTypes.Sort();
            weaponTypes = tempWeaponTypes.ToArray();

            List<AmmoType> tempAmmoTypes = new List<AmmoType>(GameInstance.AmmoTypes.Values);
            tempAmmoTypes.Sort();
            ammoTypes = tempAmmoTypes.ToArray();

            List<BaseItem> tempItems = new List<BaseItem>(GameInstance.Items.Values);
            tempItems.Sort();
            items = tempItems.ToArray();

            List<ItemCraftFormula> tempItemCraftFormulas = new List<ItemCraftFormula>(GameInstance.ItemCraftFormulas.Values);
            tempItemCraftFormulas.Sort();
            itemCraftFormulas = tempItemCraftFormulas.ToArray();

            List<BaseSkill> tempSkills = new List<BaseSkill>(GameInstance.Skills.Values);
            tempSkills.Sort();
            skills = tempSkills.ToArray();

            List<PlayerIcon> tempPlayerIcons = new List<PlayerIcon>(GameInstance.PlayerIcons.Values);
            tempPlayerIcons.Sort();
            playerIcons = tempPlayerIcons.ToArray();

            List<PlayerFrame> tempPlayerFrames = new List<PlayerFrame>(GameInstance.PlayerFrames.Values);
            tempPlayerFrames.Sort();
            playerFrames = tempPlayerFrames.ToArray();

            List<PlayerBackground> tempPlayerBackgrounds = new List<PlayerBackground>(GameInstance.PlayerBackgrounds.Values);
            tempPlayerBackgrounds.Sort();
            playerBackgrounds = tempPlayerBackgrounds.ToArray();

            List<PlayerTitle> tempPlayerTitles = new List<PlayerTitle>(GameInstance.PlayerTitles.Values);
            tempPlayerTitles.Sort();
            playerTitles = tempPlayerTitles.ToArray();

            List<GuildSkill> tempGuildSkills = new List<GuildSkill>(GameInstance.GuildSkills.Values);
            tempGuildSkills.Sort();
            guildSkills = tempGuildSkills.ToArray();

            List<GuildIcon> tempGuildIcons = new List<GuildIcon>(GameInstance.GuildIcons.Values);
            tempGuildIcons.Sort();
            guildIcons = tempGuildIcons.ToArray();

            List<StatusEffect> tempStatusEffects = new List<StatusEffect>(GameInstance.StatusEffects.Values);
            tempStatusEffects.Sort();
            statusEffects = tempStatusEffects.ToArray();

            List<PlayerCharacter> tempPlayerCharacters = new List<PlayerCharacter>(GameInstance.PlayerCharacters.Values);
            tempPlayerCharacters.Sort();
            playerCharacters = tempPlayerCharacters.ToArray();

            List<PlayerCharacterEntityMetaData> tempPlayerCharacterEntityMetadataList = new List<PlayerCharacterEntityMetaData>(GameInstance.PlayerCharacterEntityMetaDataList.Values);
            tempPlayerCharacterEntityMetadataList.Sort();
            playerCharacterEntityMetaDataList = tempPlayerCharacterEntityMetadataList.ToArray();

            List<MonsterCharacter> tempMonsterCharacters = new List<MonsterCharacter>(GameInstance.MonsterCharacters.Values);
            tempMonsterCharacters.Sort();
            monsterCharacters = tempMonsterCharacters.ToArray();

            List<Harvestable> tempHarvestables = new List<Harvestable>(GameInstance.Harvestables.Values);
            tempHarvestables.Sort();
            harvestables = tempHarvestables.ToArray();

            List<BaseMapInfo> tempMapInfos = new List<BaseMapInfo>(GameInstance.MapInfos.Values);
            mapInfos = tempMapInfos.ToArray();

            List<Quest> tempQuests = new List<Quest>(GameInstance.Quests.Values);
            tempQuests.Sort();
            quests = tempQuests.ToArray();

            List<Faction> tempFactions = new List<Faction>(GameInstance.Factions.Values);
            factions = tempFactions.ToArray();

            List<Gacha> tempGachas = new List<Gacha>(GameInstance.Gachas.Values);
            gachas = tempGachas.ToArray();

            this.InvokeInstanceDevExtMethods("LoadReferredData");
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }
    }
}
