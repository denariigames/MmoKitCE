using Cysharp.Threading.Tasks;
using Insthync.AddressableAssetTools;
using LiteNetLibManager;
using System.Collections.Generic;
using UnityEngine;
#if !DISABLE_ADDRESSABLES
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
#endif

namespace MultiplayerARPG
{
    public partial class GameInstance
    {
        public static readonly Dictionary<int, Attribute> Attributes = new Dictionary<int, Attribute>();
        public static readonly Dictionary<int, Currency> Currencies = new Dictionary<int, Currency>();
        public static readonly Dictionary<int, BaseItem> CurrencyDropRepresentItems = new Dictionary<int, BaseItem>();
        public static readonly Dictionary<int, BaseItem> Items = new Dictionary<int, BaseItem>();
        public static readonly Dictionary<int, Dictionary<int, BaseItem>> ItemsByAmmoType = new Dictionary<int, Dictionary<int, BaseItem>>();
        public static readonly Dictionary<int, ItemCraftFormula> ItemCraftFormulas = new Dictionary<int, ItemCraftFormula>();
        public static readonly Dictionary<int, Harvestable> Harvestables = new Dictionary<int, Harvestable>();
        public static readonly Dictionary<int, BaseCharacter> Characters = new Dictionary<int, BaseCharacter>();
        public static readonly Dictionary<int, PlayerCharacter> PlayerCharacters = new Dictionary<int, PlayerCharacter>();
        public static readonly Dictionary<int, PlayerCharacterEntityMetaData> PlayerCharacterEntityMetaDataList = new Dictionary<int, PlayerCharacterEntityMetaData>();
        public static readonly Dictionary<int, MonsterCharacter> MonsterCharacters = new Dictionary<int, MonsterCharacter>();
        public static readonly Dictionary<int, ArmorType> ArmorTypes = new Dictionary<int, ArmorType>();
        public static readonly Dictionary<int, WeaponType> WeaponTypes = new Dictionary<int, WeaponType>();
        public static readonly Dictionary<int, AmmoType> AmmoTypes = new Dictionary<int, AmmoType>();
        public static readonly Dictionary<int, BaseSkill> Skills = new Dictionary<int, BaseSkill>();
        public static readonly Dictionary<int, BaseNpcDialog> NpcDialogs = new Dictionary<int, BaseNpcDialog>();
        public static readonly Dictionary<int, Quest> Quests = new Dictionary<int, Quest>();
        public static readonly Dictionary<int, PlayerIcon> PlayerIcons = new Dictionary<int, PlayerIcon>();
        public static readonly Dictionary<int, PlayerFrame> PlayerFrames = new Dictionary<int, PlayerFrame>();
        public static readonly Dictionary<int, PlayerBackground> PlayerBackgrounds = new Dictionary<int, PlayerBackground>();
        public static readonly Dictionary<int, PlayerTitle> PlayerTitles = new Dictionary<int, PlayerTitle>();
        public static readonly Dictionary<int, GuildSkill> GuildSkills = new Dictionary<int, GuildSkill>();
        public static readonly Dictionary<int, GuildIcon> GuildIcons = new Dictionary<int, GuildIcon>();
        public static readonly Dictionary<int, Gacha> Gachas = new Dictionary<int, Gacha>();
        public static readonly Dictionary<int, StatusEffect> StatusEffects = new Dictionary<int, StatusEffect>();
        public static readonly Dictionary<int, DamageElement> DamageElements = new Dictionary<int, DamageElement>();
        public static readonly Dictionary<int, EquipmentSet> EquipmentSets = new Dictionary<int, EquipmentSet>();
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
        public static readonly Dictionary<int, BuildingEntity> BuildingEntities = new Dictionary<int, BuildingEntity>();
        public static readonly Dictionary<int, BasePlayerCharacterEntity> PlayerCharacterEntities = new Dictionary<int, BasePlayerCharacterEntity>();
        public static readonly Dictionary<int, BaseMonsterCharacterEntity> MonsterCharacterEntities = new Dictionary<int, BaseMonsterCharacterEntity>();
        public static readonly Dictionary<int, ItemDropEntity> ItemDropEntities = new Dictionary<int, ItemDropEntity>();
        public static readonly Dictionary<int, HarvestableEntity> HarvestableEntities = new Dictionary<int, HarvestableEntity>();
        public static readonly Dictionary<int, VehicleEntity> VehicleEntities = new Dictionary<int, VehicleEntity>();
        public static readonly Dictionary<int, WarpPortalEntity> WarpPortalEntities = new Dictionary<int, WarpPortalEntity>();
        public static readonly Dictionary<int, NpcEntity> NpcEntities = new Dictionary<int, NpcEntity>();
#endif
#if !DISABLE_ADDRESSABLES
        public static readonly Dictionary<int, AssetReferenceLiteNetLibBehaviour<BuildingEntity>> AddressableBuildingEntities = new Dictionary<int, AssetReferenceLiteNetLibBehaviour<BuildingEntity>>();
        public static readonly Dictionary<int, AssetReferenceLiteNetLibBehaviour<BasePlayerCharacterEntity>> AddressablePlayerCharacterEntities = new Dictionary<int, AssetReferenceLiteNetLibBehaviour<BasePlayerCharacterEntity>>();
        public static readonly Dictionary<int, AssetReferenceLiteNetLibBehaviour<BaseMonsterCharacterEntity>> AddressableMonsterCharacterEntities = new Dictionary<int, AssetReferenceLiteNetLibBehaviour<BaseMonsterCharacterEntity>>();
        public static readonly Dictionary<int, AssetReferenceLiteNetLibBehaviour<ItemDropEntity>> AddressableItemDropEntities = new Dictionary<int, AssetReferenceLiteNetLibBehaviour<ItemDropEntity>>();
        public static readonly Dictionary<int, AssetReferenceLiteNetLibBehaviour<HarvestableEntity>> AddressableHarvestableEntities = new Dictionary<int, AssetReferenceLiteNetLibBehaviour<HarvestableEntity>>();
        public static readonly Dictionary<int, AssetReferenceLiteNetLibBehaviour<VehicleEntity>> AddressableVehicleEntities = new Dictionary<int, AssetReferenceLiteNetLibBehaviour<VehicleEntity>>();
        public static readonly Dictionary<int, AssetReferenceLiteNetLibBehaviour<WarpPortalEntity>> AddressableWarpPortalEntities = new Dictionary<int, AssetReferenceLiteNetLibBehaviour<WarpPortalEntity>>();
        public static readonly Dictionary<int, AssetReferenceLiteNetLibBehaviour<NpcEntity>> AddressableNpcEntities = new Dictionary<int, AssetReferenceLiteNetLibBehaviour<NpcEntity>>();
#endif
        public static readonly Dictionary<string, List<WarpPortal>> MapWarpPortals = new Dictionary<string, List<WarpPortal>>();
        public static readonly Dictionary<string, List<Npc>> MapNpcs = new Dictionary<string, List<Npc>>();
        public static readonly Dictionary<string, BaseMapInfo> MapInfos = new Dictionary<string, BaseMapInfo>();
        public static readonly Dictionary<int, Faction> Factions = new Dictionary<int, Faction>();
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
        public static readonly Dictionary<int, LiteNetLibIdentity> OtherNetworkObjectPrefabs = new Dictionary<int, LiteNetLibIdentity>();
#endif
#if !DISABLE_ADDRESSABLES
        public static readonly Dictionary<int, AssetReferenceLiteNetLibIdentity> AddressableOtherNetworkObjectPrefabs = new Dictionary<int, AssetReferenceLiteNetLibIdentity>();
#endif
        public static readonly Dictionary<int, MonsterCharacter> MonsterEntitiesData = new Dictionary<int, MonsterCharacter>();

        #region Add game data functions
        public static void AddAttributes(params Attribute[] attributes)
        {
            AddAttributes((IEnumerable<Attribute>)attributes);
        }

        public static void AddAttributes(IEnumerable<Attribute> attributes)
        {
            AddManyGameData(Attributes, attributes);
        }

        public static void AddAttributes(params AttributeAmount[] attributeAmounts)
        {
            AddAttributes((IEnumerable<AttributeAmount>)attributeAmounts);
        }

        public static void AddAttributes(IEnumerable<AttributeAmount> attributeAmounts)
        {
            if (attributeAmounts == null)
                return;
            foreach (AttributeAmount attributeAmount in attributeAmounts)
            {
                AddGameData(Attributes, attributeAmount.attribute);
            }
        }

        public static void AddAttributes(params AttributeRandomAmount[] attributeAmounts)
        {
            AddAttributes((IEnumerable<AttributeRandomAmount>)attributeAmounts);
        }

        public static void AddAttributes(IEnumerable<AttributeRandomAmount> attributeAmounts)
        {
            if (attributeAmounts == null)
                return;
            foreach (AttributeRandomAmount attributeAmount in attributeAmounts)
            {
                AddGameData(Attributes, attributeAmount.attribute);
            }
        }

        public static void AddAttributes(params AttributeIncremental[] attributeIncrementals)
        {
            AddAttributes((IEnumerable<AttributeIncremental>)attributeIncrementals);
        }

        public static void AddAttributes(IEnumerable<AttributeIncremental> attributeIncrementals)
        {
            if (attributeIncrementals == null)
                return;
            foreach (AttributeIncremental attributeIncremental in attributeIncrementals)
            {
                AddGameData(Attributes, attributeIncremental.attribute);
            }
        }

        public static void AddCurrencies(params Currency[] currencies)
        {
            AddCurrencies((IEnumerable<Currency>)currencies);
        }

        public static void AddCurrencies(IEnumerable<Currency> currencies)
        {
            AddManyGameData(Currencies, currencies);
        }

        public static void AddCurrencies(params CurrencyAmount[] currencyAmounts)
        {
            AddCurrencies((IEnumerable<CurrencyAmount>)currencyAmounts);
        }

        public static void AddCurrencies(IEnumerable<CurrencyAmount> currencyAmounts)
        {
            if (currencyAmounts == null)
                return;
            foreach (CurrencyAmount currencyAmount in currencyAmounts)
            {
                AddGameData(Currencies, currencyAmount.currency);
            }
        }

        public static void AddItems(params ItemAmount[] itemAmounts)
        {
            AddItems((IEnumerable<ItemAmount>)itemAmounts);
        }

        public static void AddItems(IEnumerable<ItemAmount> itemAmounts)
        {
            if (itemAmounts == null)
                return;
            foreach (ItemAmount itemAmount in itemAmounts)
            {
                AddItems(itemAmount.item);
            }
        }

        public static void AddItems(params ItemDrop[] itemDrops)
        {
            AddItems((IEnumerable<ItemDrop>)itemDrops);
        }

        public static void AddItems(IEnumerable<ItemDrop> itemDrops)
        {
            if (itemDrops == null)
                return;
            foreach (ItemDrop itemDrop in itemDrops)
            {
                AddItems(itemDrop.item);
            }
        }

        public static void AddItems(params ItemDropForHarvestable[] itemDrops)
        {
            AddItems((IEnumerable<ItemDropForHarvestable>)itemDrops);
        }

        public static void AddItems(IEnumerable<ItemDropForHarvestable> itemDrops)
        {
            if (itemDrops == null)
                return;
            foreach (ItemDropForHarvestable itemDrop in itemDrops)
            {
                AddItems(itemDrop.item);
            }
        }

        public static void AddItems(params ItemRandomByWeight[] itemDrops)
        {
            AddItems((IEnumerable<ItemRandomByWeight>)itemDrops);
        }

        public static void AddItems(IEnumerable<ItemRandomByWeight> itemDrops)
        {
            if (itemDrops == null)
                return;
            foreach (ItemRandomByWeight itemDrop in itemDrops)
            {
                AddItems(itemDrop.item);
            }
        }

        public static void AddItems(params BaseItem[] items)
        {
            AddItems((IEnumerable<BaseItem>)items);
        }

        public static void AddItems(IEnumerable<BaseItem> items)
        {
            AddManyGameData(Items, items);
        }

        public static void AddItemCraftFormulas(int sourceId, params ItemCraftFormula[] itemCraftFormulas)
        {
            AddItemCraftFormulas(sourceId, (IEnumerable<ItemCraftFormula>)itemCraftFormulas);
        }

        public static void AddItemCraftFormulas(int sourceId, IEnumerable<ItemCraftFormula> itemCraftFormulas)
        {
            foreach (ItemCraftFormula formula in itemCraftFormulas)
            {
                if (sourceId != 0)
                    formula.SourceIds.Add(sourceId);
                if (formula.CanBeCraftedWithoutSource)
                    formula.SourceIds.Add(0);
            }
            AddManyGameData(ItemCraftFormulas, itemCraftFormulas);
        }

        public static void AddHarvestables(params Harvestable[] harvestables)
        {
            AddHarvestables((IEnumerable<Harvestable>)harvestables);
        }

        public static void AddHarvestables(IEnumerable<Harvestable> harvestables)
        {
            AddManyGameData(Harvestables, harvestables);
        }

        public static void AddArmorTypes(params ArmorType[] armorTypes)
        {
            AddArmorTypes((IEnumerable<ArmorType>)armorTypes);
        }

        public static void AddArmorTypes(IEnumerable<ArmorType> armorTypes)
        {
            AddManyGameData(ArmorTypes, armorTypes);
        }

        public static void AddWeaponTypes(params WeaponType[] weaponTypes)
        {
            AddWeaponTypes((IEnumerable<WeaponType>)weaponTypes);
        }

        public static void AddWeaponTypes(IEnumerable<WeaponType> weaponTypes)
        {
            AddManyGameData(WeaponTypes, weaponTypes);
        }

        public static void AddAmmoTypes(params AmmoType[] ammoTypes)
        {
            AddAmmoTypes((IEnumerable<AmmoType>)ammoTypes);
        }

        public static void AddAmmoTypes(IEnumerable<AmmoType> ammoTypes)
        {
            AddManyGameData(AmmoTypes, ammoTypes);
        }

        public static void AddSkills(params SkillLevel[] skillLevels)
        {
            AddSkills((IEnumerable<SkillLevel>)skillLevels);
        }

        public static void AddSkills(IEnumerable<SkillLevel> skillLevels)
        {
            if (skillLevels == null)
                return;
            foreach (SkillLevel skillLevel in skillLevels)
            {
                AddGameData(Skills, skillLevel.skill);
            }
        }

        public static void AddSkills(params SkillIncremental[] skillLevels)
        {
            AddSkills((IEnumerable<SkillIncremental>)skillLevels);
        }

        public static void AddSkills(IEnumerable<SkillIncremental> skillLevels)
        {
            if (skillLevels == null)
                return;
            foreach (SkillIncremental skillLevel in skillLevels)
            {
                AddGameData(Skills, skillLevel.skill);
            }
        }

        public static void AddSkills(params SkillRandomLevel[] skillLevels)
        {
            AddSkills((IEnumerable<SkillRandomLevel>)skillLevels);
        }

        public static void AddSkills(IEnumerable<SkillRandomLevel> skillLevels)
        {
            if (skillLevels == null)
                return;
            foreach (SkillRandomLevel skillLevel in skillLevels)
            {
                AddGameData(Skills, skillLevel.skill);
            }
        }

        public static void AddSkills(params PlayerSkill[] skillLevels)
        {
            AddSkills((IEnumerable<PlayerSkill>)skillLevels);
        }

        public static void AddSkills(IEnumerable<PlayerSkill> skillLevels)
        {
            if (skillLevels == null)
                return;
            foreach (PlayerSkill skillLevel in skillLevels)
            {
                AddGameData(Skills, skillLevel.skill);
            }
        }

        public static void AddSkills(params MonsterSkill[] skillLevels)
        {
            AddSkills((IEnumerable<MonsterSkill>)skillLevels);
        }

        public static void AddSkills(IEnumerable<MonsterSkill> skillLevels)
        {
            if (skillLevels == null)
                return;
            foreach (MonsterSkill skillLevel in skillLevels)
            {
                AddGameData(Skills, skillLevel.skill);
            }
        }

        public static void AddSkills(params BaseSkill[] skills)
        {
            AddSkills((IEnumerable<BaseSkill>)skills);
        }

        public static void AddSkills(IEnumerable<BaseSkill> skills)
        {
            AddManyGameData(Skills, skills);
        }

        public static void AddNpcDialogs(params BaseNpcDialog[] npcDialogs)
        {
            AddNpcDialogs((IEnumerable<BaseNpcDialog>)npcDialogs);
        }

        public static void AddNpcDialogs(IEnumerable<BaseNpcDialog> npcDialogs)
        {
            AddManyGameData(NpcDialogs, npcDialogs);
        }

        public static void AddQuests(params Quest[] quests)
        {
            AddQuests((IEnumerable<Quest>)quests);
        }

        public static void AddQuests(IEnumerable<Quest> quests)
        {
            AddManyGameData(Quests, quests);
        }

        public static void AddPlayerIcons(params PlayerIcon[] playerIcons)
        {
            AddPlayerIcons((IEnumerable<PlayerIcon>)playerIcons);
        }

        public static void AddPlayerIcons(IEnumerable<PlayerIcon> playerIcons)
        {
            AddManyGameData(PlayerIcons, playerIcons);
        }

        public static void AddPlayerFrames(params PlayerFrame[] playerFrames)
        {
            AddPlayerFrames((IEnumerable<PlayerFrame>)playerFrames);
        }

        public static void AddPlayerFrames(IEnumerable<PlayerFrame> playerFrames)
        {
            AddManyGameData(PlayerFrames, playerFrames);
        }

        public static void AddPlayerBackgrounds(params PlayerBackground[] playerBackgrounds)
        {
            AddPlayerBackgrounds((IEnumerable<PlayerBackground>)playerBackgrounds);
        }

        public static void AddPlayerBackgrounds(IEnumerable<PlayerBackground> playerBackgrounds)
        {
            AddManyGameData(PlayerBackgrounds, playerBackgrounds);
        }

        public static void AddPlayerTitles(params PlayerTitle[] playerTitles)
        {
            AddPlayerTitles((IEnumerable<PlayerTitle>)playerTitles);
        }

        public static void AddPlayerTitles(IEnumerable<PlayerTitle> playerTitles)
        {
            AddManyGameData(PlayerTitles, playerTitles);
        }

        public static void AddGuildSkills(params GuildSkill[] guildSkills)
        {
            AddGuildSkills((IEnumerable<GuildSkill>)guildSkills);
        }

        public static void AddGuildSkills(IEnumerable<GuildSkill> guildSkills)
        {
            AddManyGameData(GuildSkills, guildSkills);
        }

        public static void AddGuildIcons(params GuildIcon[] guildIcons)
        {
            AddGuildIcons((IEnumerable<GuildIcon>)guildIcons);
        }

        public static void AddGuildIcons(IEnumerable<GuildIcon> guildIcons)
        {
            AddManyGameData(GuildIcons, guildIcons);
        }

        public static void AddGachas(params Gacha[] gachas)
        {
            AddGachas((IEnumerable<Gacha>)gachas);
        }

        public static void AddGachas(IEnumerable<Gacha> gachas)
        {
            AddManyGameData(Gachas, gachas);
        }

        public static void AddStatusEffects(params StatusEffectApplying[] statusEffects)
        {
            AddStatusEffects((IEnumerable<StatusEffectApplying>)statusEffects);
        }

        public static void AddStatusEffects(IEnumerable<StatusEffectApplying> statusEffects)
        {
            if (statusEffects == null)
                return;
            foreach (StatusEffectApplying statusEffect in statusEffects)
            {
                AddStatusEffects(statusEffect.statusEffect);
            }
        }

        public static void AddStatusEffects(params StatusEffectResistanceAmount[] statusEffects)
        {
            AddStatusEffects((IEnumerable<StatusEffectResistanceAmount>)statusEffects);
        }

        public static void AddStatusEffects(IEnumerable<StatusEffectResistanceAmount> statusEffects)
        {
            if (statusEffects == null)
                return;
            foreach (StatusEffectResistanceAmount statusEffect in statusEffects)
            {
                AddStatusEffects(statusEffect.statusEffect);
            }
        }

        public static void AddStatusEffects(params StatusEffectResistanceIncremental[] statusEffects)
        {
            AddStatusEffects((IEnumerable<StatusEffectResistanceIncremental>)statusEffects);
        }

        public static void AddStatusEffects(IEnumerable<StatusEffectResistanceIncremental> statusEffects)
        {
            if (statusEffects == null)
                return;
            foreach (StatusEffectResistanceIncremental statusEffect in statusEffects)
            {
                AddStatusEffects(statusEffect.statusEffect);
            }
        }

        public static void AddStatusEffects(params StatusEffect[] statusEffects)
        {
            AddStatusEffects((IEnumerable<StatusEffect>)statusEffects);
        }

        public static void AddStatusEffects(IEnumerable<StatusEffect> statusEffects)
        {
            AddManyGameData(StatusEffects, statusEffects);
        }

        public static void AddCharacters(params BaseCharacter[] characters)
        {
            AddCharacters((IEnumerable<BaseCharacter>)characters);
        }

        public static void AddCharacters(IEnumerable<BaseCharacter> characters)
        {
            if (characters == null)
                return;
            foreach (BaseCharacter character in characters)
            {
                if (AddGameData(Characters, character))
                {
                    if (character is PlayerCharacter playerCharacter)
                        AddGameData(PlayerCharacters, playerCharacter);
                    else if (character is MonsterCharacter monsterCharacter)
                        AddGameData(MonsterCharacters, monsterCharacter);
                }
            }
        }

        public static void AddPlayerCharacterEntityMetaDataList(params PlayerCharacterEntityMetaData[] metaDataList)
        {
            AddManyGameData(PlayerCharacterEntityMetaDataList, metaDataList);
        }

        public static void AddPlayerCharacterEntityMetaDataList(IEnumerable<PlayerCharacterEntityMetaData> metaDataList)
        {
            AddManyGameData(PlayerCharacterEntityMetaDataList, metaDataList);
        }

        public static void AddMapWarpPortals(params WarpPortals[] mapWarpPortals)
        {
            AddMapWarpPortals((IEnumerable<WarpPortals>)mapWarpPortals);
        }

        public static async void AddMapWarpPortals(IEnumerable<WarpPortals> mapWarpPortals)
        {
            if (mapWarpPortals == null)
                return;
            foreach (WarpPortals mapWarpPortal in mapWarpPortals)
            {
                if (mapWarpPortal.mapInfo == null)
                    continue;
                if (MapWarpPortals.ContainsKey(mapWarpPortal.mapInfo.Id))
                    MapWarpPortals[mapWarpPortal.mapInfo.Id].AddRange(mapWarpPortal.warpPortals);
                else
                    MapWarpPortals[mapWarpPortal.mapInfo.Id] = new List<WarpPortal>(mapWarpPortal.warpPortals);
                foreach (WarpPortal warpPortal in mapWarpPortal.warpPortals)
                {
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
                    AddGameEntity(WarpPortalEntities, warpPortal.entityPrefab);
#endif
#if !DISABLE_ADDRESSABLES
                    await AddAssetReference<AssetReferenceLiteNetLibBehaviour<WarpPortalEntity>, WarpPortalEntity>(AddressableWarpPortalEntities, warpPortal.addressableEntityPrefab);
#endif
                }
            }
            await UniTask.Yield();
        }

        public static void AddMapNpcs(params Npcs[] mapNpcs)
        {
            AddMapNpcs((IEnumerable<Npcs>)mapNpcs);
        }

        public static async void AddMapNpcs(IEnumerable<Npcs> mapNpcs)
        {
            if (mapNpcs == null)
                return;
            foreach (Npcs mapNpc in mapNpcs)
            {
                if (mapNpc.mapInfo == null)
                    continue;
                if (MapNpcs.ContainsKey(mapNpc.mapInfo.Id))
                    MapNpcs[mapNpc.mapInfo.Id].AddRange(mapNpc.npcs);
                else
                    MapNpcs[mapNpc.mapInfo.Id] = new List<Npc>(mapNpc.npcs);
                foreach (Npc npc in mapNpc.npcs)
                {
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
                    AddGameEntity(NpcEntities, npc.entityPrefab);
#endif
#if !DISABLE_ADDRESSABLES
                    await AddAssetReference<AssetReferenceLiteNetLibBehaviour<NpcEntity>, NpcEntity>(AddressableNpcEntities, npc.addressableEntityPrefab);
#endif
                    if (npc.startDialog != null)
                        AddGameData(NpcDialogs, npc.startDialog);
                    if (npc.graph != null)
                        AddNpcDialogs(npc.graph.GetDialogs());
                }
            }
            await UniTask.Yield();
        }

        public static void AddMapInfos(params BaseMapInfo[] mapInfos)
        {
            AddMapInfos((IEnumerable<BaseMapInfo>)mapInfos);
        }

        public static void AddMapInfos(IEnumerable<BaseMapInfo> mapInfos)
        {
            if (mapInfos == null)
                return;
            foreach (BaseMapInfo mapInfo in mapInfos)
            {
                if (mapInfo == null || (!mapInfo.IsAddressableSceneValid() && !mapInfo.IsSceneValid()) ||
                    (MapInfos.TryGetValue(mapInfo.Id, out BaseMapInfo tempData) && tempData != null))
                {
                    continue;
                }
                mapInfo.Validate();
                MapInfos[mapInfo.Id] = mapInfo;
                mapInfo.PrepareRelatesData();
            }
        }

        public static void AddFactions(params Faction[] factions)
        {
            AddFactions((IEnumerable<Faction>)factions);
        }

        public static void AddFactions(IEnumerable<Faction> factions)
        {
            AddManyGameData(Factions, factions);
        }

        public static void AddDamageElements(params ArmorAmount[] armorAmounts)
        {
            AddDamageElements((IEnumerable<ArmorAmount>)armorAmounts);
        }

        public static void AddDamageElements(IEnumerable<ArmorAmount> armorAmounts)
        {
            if (armorAmounts == null)
                return;
            foreach (ArmorAmount armorAmount in armorAmounts)
            {
                AddGameData(DamageElements, armorAmount.damageElement);
            }
        }

        public static void AddDamageElements(params ArmorRandomAmount[] armorAmounts)
        {
            AddDamageElements((IEnumerable<ArmorRandomAmount>)armorAmounts);
        }

        public static void AddDamageElements(IEnumerable<ArmorRandomAmount> armorAmounts)
        {
            if (armorAmounts == null)
                return;
            foreach (ArmorRandomAmount armorAmount in armorAmounts)
            {
                AddGameData(DamageElements, armorAmount.damageElement);
            }
        }

        public static void AddDamageElements(params ArmorIncremental[] armorIncrementals)
        {
            AddDamageElements((IEnumerable<ArmorIncremental>)armorIncrementals);
        }

        public static void AddDamageElements(IEnumerable<ArmorIncremental> armorIncrementals)
        {
            if (armorIncrementals == null)
                return;
            foreach (ArmorIncremental armorIncremental in armorIncrementals)
            {
                AddGameData(DamageElements, armorIncremental.damageElement);
            }
        }

        public static void AddDamageElements(params DamageAmount[] damageAmounts)
        {
            AddDamageElements((IEnumerable<DamageAmount>)damageAmounts);
        }

        public static void AddDamageElements(IEnumerable<DamageAmount> damageAmounts)
        {
            if (damageAmounts == null)
                return;
            foreach (DamageAmount damageAmount in damageAmounts)
            {
                AddGameData(DamageElements, damageAmount.damageElement);
            }
        }

        public static void AddDamageElements(params DamageRandomAmount[] damageAmounts)
        {
            AddDamageElements((IEnumerable<DamageRandomAmount>)damageAmounts);
        }

        public static void AddDamageElements(IEnumerable<DamageRandomAmount> damageAmounts)
        {
            if (damageAmounts == null)
                return;
            foreach (DamageRandomAmount damageAmount in damageAmounts)
            {
                AddGameData(DamageElements, damageAmount.damageElement);
            }
        }

        public static void AddDamageElements(params DamageIncremental[] damageIncrementals)
        {
            AddDamageElements((IEnumerable<DamageIncremental>)damageIncrementals);
        }

        public static void AddDamageElements(IEnumerable<DamageIncremental> damageIncrementals)
        {
            if (damageIncrementals == null)
                return;
            foreach (DamageIncremental damageIncremental in damageIncrementals)
            {
                AddGameData(DamageElements, damageIncremental.damageElement);
            }
        }

        public static void AddDamageElements(params DamageElement[] damageElements)
        {
            AddDamageElements((IEnumerable<DamageElement>)damageElements);
        }

        public static void AddDamageElements(IEnumerable<DamageElement> damageElements)
        {
            AddManyGameData(DamageElements, damageElements);
        }

        public static void AddDamageElements(params ResistanceAmount[] resistanceAmounts)
        {
            AddDamageElements((IEnumerable<ResistanceAmount>)resistanceAmounts);
        }

        public static void AddDamageElements(IEnumerable<ResistanceAmount> resistanceAmounts)
        {
            if (resistanceAmounts == null)
                return;
            foreach (ResistanceAmount resistanceAmount in resistanceAmounts)
            {
                AddGameData(DamageElements, resistanceAmount.damageElement);
            }
        }

        public static void AddDamageElements(params ResistanceRandomAmount[] resistanceAmounts)
        {
            AddDamageElements((IEnumerable<ResistanceRandomAmount>)resistanceAmounts);
        }

        public static void AddDamageElements(IEnumerable<ResistanceRandomAmount> resistanceAmounts)
        {
            if (resistanceAmounts == null)
                return;
            foreach (ResistanceRandomAmount resistanceAmount in resistanceAmounts)
            {
                AddGameData(DamageElements, resistanceAmount.damageElement);
            }
        }

        public static void AddDamageElements(params ResistanceIncremental[] resistanceIncrementals)
        {
            AddDamageElements((IEnumerable<ResistanceIncremental>)resistanceIncrementals);
        }

        public static void AddDamageElements(IEnumerable<ResistanceIncremental> resistanceIncrementals)
        {
            if (resistanceIncrementals == null)
                return;
            foreach (ResistanceIncremental resistanceIncremental in resistanceIncrementals)
            {
                AddGameData(DamageElements, resistanceIncremental.damageElement);
            }
        }

        public static void AddEquipmentSets(params EquipmentSet[] equipmentSets)
        {
            AddEquipmentSets((IEnumerable<EquipmentSet>)equipmentSets);
        }

        public static void AddEquipmentSets(IEnumerable<EquipmentSet> equipmentSets)
        {
            AddManyGameData(EquipmentSets, equipmentSets);
        }
        #endregion

        #region Add game entity functions
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
        public static void AddPlayerCharacterEntities(params BasePlayerCharacterEntity[] playerCharacterEntities)
        {
            AddPlayerCharacterEntities((IEnumerable<BasePlayerCharacterEntity>)playerCharacterEntities);
        }

        public static void AddPlayerCharacterEntities(IEnumerable<BasePlayerCharacterEntity> playerCharacterEntities)
        {
            AddManyGameEntity(PlayerCharacterEntities, playerCharacterEntities);
        }
#endif

#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
        public static void AddMonsterCharacterEntities(params BaseMonsterCharacterEntity[] monsterCharacterEntities)
        {
            AddMonsterCharacterEntities((IEnumerable<BaseMonsterCharacterEntity>)monsterCharacterEntities);
        }

        public static void AddMonsterCharacterEntities(IEnumerable<BaseMonsterCharacterEntity> monsterCharacterEntities)
        {
            AddManyGameEntity(MonsterCharacterEntities, monsterCharacterEntities);
        }
#endif

#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
        public static void AddItemDropEntities(params ItemDropEntity[] itemDropEntities)
        {
            AddItemDropEntities((IEnumerable<ItemDropEntity>)itemDropEntities);
        }

        public static void AddItemDropEntities(IEnumerable<ItemDropEntity> itemDropEntities)
        {
            AddManyGameEntity(ItemDropEntities, itemDropEntities);
        }
#endif

#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
        public static void AddHarvestableEntities(params HarvestableEntity[] harvestableEntities)
        {
            AddHarvestableEntities((IEnumerable<HarvestableEntity>)harvestableEntities);
        }

        public static void AddHarvestableEntities(IEnumerable<HarvestableEntity> harvestableEntities)
        {
            AddManyGameEntity(HarvestableEntities, harvestableEntities);
        }
#endif

#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
        public static void AddVehicleEntities(params VehicleEntity[] vehicleEntities)
        {
            AddVehicleEntities((IEnumerable<VehicleEntity>)vehicleEntities);
        }

        public static void AddVehicleEntities(IEnumerable<VehicleEntity> vehicleEntities)
        {
            AddManyGameEntity(VehicleEntities, vehicleEntities);
        }
#endif

#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
        public static void AddBuildingEntities(params BuildingEntity[] buildingEntities)
        {
            AddBuildingEntities((IEnumerable<BuildingEntity>)buildingEntities);
        }

        public static void AddBuildingEntities(IEnumerable<BuildingEntity> buildingEntities)
        {
            AddManyGameEntity(BuildingEntities, buildingEntities);
        }
#endif

#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
        public static void AddWarpPortalEntities(params WarpPortalEntity[] warpPortalEntities)
        {
            AddWarpPortalEntities((IEnumerable<WarpPortalEntity>)warpPortalEntities);
        }

        public static void AddWarpPortalEntities(IEnumerable<WarpPortalEntity> warpPortalEntities)
        {
            AddManyGameEntity(WarpPortalEntities, warpPortalEntities);
        }
#endif

#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
        public static void AddNpcEntities(params NpcEntity[] npcEntities)
        {
            AddNpcEntities((IEnumerable<NpcEntity>)npcEntities);
        }

        public static void AddNpcEntities(IEnumerable<NpcEntity> npcEntities)
        {
            AddManyGameEntity(NpcEntities, npcEntities);
        }
#endif
        #endregion

#if !DISABLE_ADDRESSABLES
        #region Add asset reference functions
        public static void AddAssetReferencePlayerCharacterEntities(params AssetReferenceLiteNetLibBehaviour<BasePlayerCharacterEntity>[] playerCharacterEntities)
        {
            AddAssetReferencePlayerCharacterEntities((IEnumerable<AssetReferenceLiteNetLibBehaviour<BasePlayerCharacterEntity>>)playerCharacterEntities);
        }

        public static void AddAssetReferencePlayerCharacterEntities(IEnumerable<AssetReferenceLiteNetLibBehaviour<BasePlayerCharacterEntity>> playerCharacterEntities)
        {
            AddManyAssetReference<AssetReferenceLiteNetLibBehaviour<BasePlayerCharacterEntity>, BasePlayerCharacterEntity>(AddressablePlayerCharacterEntities, playerCharacterEntities);
        }

        public static void AddAssetReferenceMonsterCharacterEntities(params AssetReferenceLiteNetLibBehaviour<BaseMonsterCharacterEntity>[] monsterCharacterEntities)
        {
            AddAssetReferenceMonsterCharacterEntities((IEnumerable<AssetReferenceLiteNetLibBehaviour<BaseMonsterCharacterEntity>>)monsterCharacterEntities);
        }

        public static void AddAssetReferenceMonsterCharacterEntities(IEnumerable<AssetReferenceLiteNetLibBehaviour<BaseMonsterCharacterEntity>> monsterCharacterEntities)
        {
            AddManyAssetReference<AssetReferenceLiteNetLibBehaviour<BaseMonsterCharacterEntity>, BaseMonsterCharacterEntity>(AddressableMonsterCharacterEntities, monsterCharacterEntities);
        }

        public static void AddAssetReferenceItemDropEntities(params AssetReferenceLiteNetLibBehaviour<ItemDropEntity>[] itemDropEntities)
        {
            AddAssetReferenceItemDropEntities((IEnumerable<AssetReferenceLiteNetLibBehaviour<ItemDropEntity>>)itemDropEntities);
        }

        public static void AddAssetReferenceItemDropEntities(IEnumerable<AssetReferenceLiteNetLibBehaviour<ItemDropEntity>> itemDropEntities)
        {
            AddManyAssetReference<AssetReferenceLiteNetLibBehaviour<ItemDropEntity>, ItemDropEntity>(AddressableItemDropEntities, itemDropEntities);
        }

        public static void AddAssetReferenceHarvestableEntities(params AssetReferenceLiteNetLibBehaviour<HarvestableEntity>[] harvestableEntities)
        {
            AddAssetReferenceHarvestableEntities((IEnumerable<AssetReferenceLiteNetLibBehaviour<HarvestableEntity>>)harvestableEntities);
        }

        public static void AddAssetReferenceHarvestableEntities(IEnumerable<AssetReferenceLiteNetLibBehaviour<HarvestableEntity>> harvestableEntities)
        {
            AddManyAssetReference<AssetReferenceLiteNetLibBehaviour<HarvestableEntity>, HarvestableEntity>(AddressableHarvestableEntities, harvestableEntities);
        }

        public static void AddAssetReferenceVehicleEntities(params AssetReferenceLiteNetLibBehaviour<VehicleEntity>[] vehicleEntities)
        {
            AddAssetReferenceVehicleEntities((IEnumerable<AssetReferenceLiteNetLibBehaviour<VehicleEntity>>)vehicleEntities);
        }

        public static void AddAssetReferenceVehicleEntities(IEnumerable<AssetReferenceLiteNetLibBehaviour<VehicleEntity>> vehicleEntities)
        {
            AddManyAssetReference<AssetReferenceLiteNetLibBehaviour<VehicleEntity>, VehicleEntity>(AddressableVehicleEntities, vehicleEntities);
        }

        public static void AddAssetReferenceBuildingEntities(params AssetReferenceLiteNetLibBehaviour<BuildingEntity>[] buildingEntities)
        {
            AddAssetReferenceBuildingEntities((IEnumerable<AssetReferenceLiteNetLibBehaviour<BuildingEntity>>)buildingEntities);
        }

        public static void AddAssetReferenceBuildingEntities(IEnumerable<AssetReferenceLiteNetLibBehaviour<BuildingEntity>> buildingEntities)
        {
            AddManyAssetReference<AssetReferenceLiteNetLibBehaviour<BuildingEntity>, BuildingEntity>(AddressableBuildingEntities, buildingEntities);
        }

        public static void AddAssetReferenceWarpPortalEntities(params AssetReferenceLiteNetLibBehaviour<WarpPortalEntity>[] warpPortalEntities)
        {
            AddAssetReferenceWarpPortalEntities((IEnumerable<AssetReferenceLiteNetLibBehaviour<WarpPortalEntity>>)warpPortalEntities);
        }

        public static void AddAssetReferenceWarpPortalEntities(IEnumerable<AssetReferenceLiteNetLibBehaviour<WarpPortalEntity>> warpPortalEntities)
        {
            AddManyAssetReference<AssetReferenceLiteNetLibBehaviour<WarpPortalEntity>, WarpPortalEntity>(AddressableWarpPortalEntities, warpPortalEntities);
        }

        public static void AddAssetReferenceNpcEntities(params AssetReferenceLiteNetLibBehaviour<NpcEntity>[] npcEntities)
        {
            AddAssetReferenceNpcEntities((IEnumerable<AssetReferenceLiteNetLibBehaviour<NpcEntity>>)npcEntities);
        }

        public static void AddAssetReferenceNpcEntities(IEnumerable<AssetReferenceLiteNetLibBehaviour<NpcEntity>> npcEntities)
        {
            AddManyAssetReference<AssetReferenceLiteNetLibBehaviour<NpcEntity>, NpcEntity>(AddressableNpcEntities, npcEntities);
        }
        #endregion
#endif

#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
        public static void AddOtherNetworkObjects(params LiteNetLibIdentity[] networkObjects)
        {
            AddOtherNetworkObjects((IEnumerable<LiteNetLibIdentity>)networkObjects);
        }

        public static void AddOtherNetworkObjects(IEnumerable<LiteNetLibIdentity> networkObjects)
        {
            if (networkObjects == null)
                return;
            foreach (LiteNetLibIdentity networkObject in networkObjects)
            {
                if (networkObject == null || (OtherNetworkObjectPrefabs.TryGetValue(networkObject.HashAssetId, out LiteNetLibIdentity tempData) && tempData != null))
                    continue;
                OtherNetworkObjectPrefabs.Add(networkObject.HashAssetId, networkObject);
            }
        }
#endif

#if !DISABLE_ADDRESSABLES
        public static void AddAssetReferenceOtherNetworkObjects(params AssetReferenceLiteNetLibIdentity[] networkObjects)
        {
            AddAssetReferenceOtherNetworkObjects((IEnumerable<AssetReferenceLiteNetLibIdentity>)networkObjects);
        }

        public static void AddAssetReferenceOtherNetworkObjects(IEnumerable<AssetReferenceLiteNetLibIdentity> networkObjects)
        {
            if (networkObjects == null)
                return;
            foreach (AssetReferenceLiteNetLibIdentity networkObject in networkObjects)
            {
                if (networkObject == null || AddressableOtherNetworkObjectPrefabs.ContainsKey(networkObject.HashAssetId))
                    continue;
                AddressableOtherNetworkObjectPrefabs.Add(networkObject.HashAssetId, networkObject);
            }
        }
#endif

        private static void AddManyGameData<T>(Dictionary<int, T> dict, IEnumerable<T> list)
            where T : IGameData
        {
            if (list == null)
                return;
            foreach (T entry in list)
            {
                AddGameData(dict, entry);
            }
        }

        private static bool AddGameData<T>(Dictionary<int, T> dict, T data)
            where T : IGameData
        {
            if ((data as Object) == null)
                return false;
            if (!dict.TryGetValue(data.DataId, out T tempData) || (tempData as Object) == null)
            {
                data.Validate();
                dict[data.DataId] = data;
                data.PrepareRelatesData();
            }
            return true;
        }

        private static void AddManyGameEntity<T>(Dictionary<int, T> dict, IEnumerable<T> list)
            where T : IGameEntity
        {
            if (list == null)
                return;
            foreach (T entry in list)
            {
                AddGameEntity(dict, entry);
            }
        }

        private static bool AddGameEntity<T>(Dictionary<int, T> dict, T entity)
            where T : IGameEntity
        {
            if ((entity as Object) == null)
                return false;
            if (entity.Identity.IsSceneObject)
            {
                entity.PrepareRelatesData();
                return true;
            }
            if (!dict.TryGetValue(entity.Identity.HashAssetId, out T tempData) || (tempData as Object) == null)
            {
                dict[entity.Identity.HashAssetId] = entity;
                entity.PrepareRelatesData();
            }
            return true;
        }

#if !DISABLE_ADDRESSABLES
        private static async void AddManyAssetReference<TBehaviour, TType>(Dictionary<int, TBehaviour> dict, IEnumerable<TBehaviour> list)
            where TBehaviour : AssetReferenceLiteNetLibBehaviour<TType>
            where TType : BaseGameEntity
        {
            if (list == null)
                return;
            foreach (TBehaviour entry in list)
            {
                await AddAssetReference<TBehaviour, TType>(dict, entry);
            }
        }

        private static async UniTask<bool> AddAssetReference<TBehaviour, TType>(Dictionary<int, TBehaviour> dict, TBehaviour data)
            where TBehaviour : AssetReferenceLiteNetLibBehaviour<TType>
            where TType : BaseGameEntity
        {
            if (!data.IsDataValid())
                return false;
            if (!dict.ContainsKey(data.HashAssetId))
            {
                bool isError = true;
                object runtimeKey = data.RuntimeKey;
                AsyncOperationHandle<GameObject> loadOp = Addressables.LoadAssetAsync<GameObject>(runtimeKey);
                try
                {
                    GameObject loadedObject = await loadOp.Task;
                    if (loadedObject.TryGetComponent(out TType loadedData))
                        loadedData.PrepareRelatesData();
                }
                catch (System.Exception ex)
                {
                    isError = true;
                    Debug.LogException(ex);
                }
                finally
                {
                    Addressables.Release(loadOp);
                }
                System.GC.Collect();
                if (isError)
                    return false;
                else
                    dict[data.HashAssetId] = data;
            }
            return true;
        }
#endif

        public static IUnlockableGameData GetUnlockableGameData(UnlockableContentType type, int dataId)
        {
            TryGetUnlockableGameData(type, dataId, out IUnlockableGameData result);
            return result;
        }

        public static bool TryGetUnlockableGameData(UnlockableContentType type, int dataId, out IUnlockableGameData result)
        {
            switch (type)
            {
                case UnlockableContentType.Icon:
                    if (PlayerIcons.TryGetValue(dataId, out PlayerIcon playerIcon))
                    {
                        result = playerIcon;
                        return true;
                    }
                    break;
                case UnlockableContentType.Frame:
                    if (PlayerFrames.TryGetValue(dataId, out PlayerFrame playerFrame))
                    {
                        result = playerFrame;
                        return true;
                    }
                    break;
                case UnlockableContentType.Title:
                    if (PlayerTitles.TryGetValue(dataId, out PlayerTitle playerTitle))
                    {
                        result = playerTitle;
                        return true;
                    }
                    break;
                case UnlockableContentType.Background:
                    if (PlayerBackgrounds.TryGetValue(dataId, out PlayerBackground playerBanner))
                    {
                        result = playerBanner;
                        return true;
                    }
                    break;
            }
            result = null;
            return false;
        }

        public static UnlockRequirement GetUnlockableContentRequirement(UnlockableContentType type, int dataId)
        {
            TryGetUnlockableContentRequirement(type, dataId, out UnlockRequirement result);
            return result;
        }

        public static bool TryGetUnlockableContentRequirement(UnlockableContentType type, int dataId, out UnlockRequirement result)
        {
            IUnlockableGameData unlockableGameData = GetUnlockableGameData(type, dataId);
            if (unlockableGameData != null)
            {
                result = unlockableGameData.UnlockRequirement;
                return true;
            }
            result = default;
            return false;
        }

        public static Dictionary<int, UnlockRequirement> GetUnlockableContentRequirements(UnlockableContentType type)
        {
            Dictionary<int, UnlockRequirement> result = new Dictionary<int, UnlockRequirement>();
            switch (type)
            {
                case UnlockableContentType.Icon:
                    foreach (var kv in PlayerIcons)
                    {
                        result[kv.Key] = kv.Value.UnlockRequirement;
                    }
                    break;
                case UnlockableContentType.Frame:
                    foreach (var kv in PlayerFrames)
                    {
                        result[kv.Key] = kv.Value.UnlockRequirement;
                    }
                    break;
                case UnlockableContentType.Title:
                    foreach (var kv in PlayerTitles)
                    {
                        result[kv.Key] = kv.Value.UnlockRequirement;
                    }
                    break;
                case UnlockableContentType.Background:
                    foreach (var kv in PlayerBackgrounds)
                    {
                        result[kv.Key] = kv.Value.UnlockRequirement;
                    }
                    break;
            }
            return result;
        }

        public static int GetPlayerCharacterEntityHashAssetId(int entityId, out int metaDataId)
        {
            metaDataId = 0;
            if (PlayerCharacterEntityMetaDataList.TryGetValue(entityId, out PlayerCharacterEntityMetaData metaData))
            {
                metaDataId = metaData.DataId;
                return metaData.GetPlayerCharacterEntityHashAssetId();
            }
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
            else if (PlayerCharacterEntities.ContainsKey(entityId))
            {
                return entityId;
            }
#endif
#if !DISABLE_ADDRESSABLES
            else if (AddressablePlayerCharacterEntities.ContainsKey(entityId))
            {
                return entityId;
            }
#endif
            return 0;
        }

        public static void SetupByMetaData(BasePlayerCharacterEntity playerCharacterEntity, int metaDataId)
        {
            if (playerCharacterEntity == null)
                return;
            playerCharacterEntity.MetaDataId = metaDataId;
            if (metaDataId == 0 || !PlayerCharacterEntityMetaDataList.TryGetValue(metaDataId, out PlayerCharacterEntityMetaData metaData))
                return;
            metaData.Setup(playerCharacterEntity);
        }
    }
}
