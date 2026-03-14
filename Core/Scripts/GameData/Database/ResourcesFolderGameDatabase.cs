using Cysharp.Threading.Tasks;
using Insthync.DevExtension;
using UnityEngine;

namespace MultiplayerARPG
{
    /// <summary>
    /// This game database will load and setup game data from Resources folder
    /// </summary>
    [CreateAssetMenu(fileName = GameDataMenuConsts.RES_GAME_DATABASE_FILE, menuName = GameDataMenuConsts.RES_GAME_DATABASE_MENU, order = GameDataMenuConsts.RES_GAME_DATABASE_ORDER)]
    public partial class ResourcesFolderGameDatabase : BaseGameDatabase
    {
        protected override UniTask LoadDataImplement(GameInstance gameInstance)
        {
            Attribute[] attributes = Resources.LoadAll<Attribute>("");
            BaseItem[] items = Resources.LoadAll<BaseItem>("");
            ItemCraftFormula[] itemCraftFormulas = Resources.LoadAll<ItemCraftFormula>("");
            BaseSkill[] skills = Resources.LoadAll<BaseSkill>("");
            BaseNpcDialog[] npcDialogs = Resources.LoadAll<BaseNpcDialog>("");
            Quest[] quests = Resources.LoadAll<Quest>("");
            PlayerIcon[] playerIcons = Resources.LoadAll<PlayerIcon>("");
            PlayerFrame[] playerFrames = Resources.LoadAll<PlayerFrame>("");
            PlayerBackground[] playerBackgrounds = Resources.LoadAll<PlayerBackground>("");
            PlayerTitle[] playerTitles = Resources.LoadAll<PlayerTitle>("");
            GuildSkill[] guildSkills = Resources.LoadAll<GuildSkill>("");
            GuildIcon[] guildIcons = Resources.LoadAll<GuildIcon>("");
            StatusEffect[] statusEffects = Resources.LoadAll<StatusEffect>("");
            PlayerCharacter[] playerCharacters = Resources.LoadAll<PlayerCharacter>("");
            MonsterCharacter[] monsterCharacters = Resources.LoadAll<MonsterCharacter>("");
            BaseMapInfo[] mapInfos = Resources.LoadAll<BaseMapInfo>("");
            Faction[] factions = Resources.LoadAll<Faction>("");
            Gacha[] gachas = Resources.LoadAll<Gacha>("");
            BasePlayerCharacterEntity[] playerCharacterEntities = Resources.LoadAll<BasePlayerCharacterEntity>("");
            BaseMonsterCharacterEntity[] monsterCharacterEntities = Resources.LoadAll<BaseMonsterCharacterEntity>("");
            VehicleEntity[] vehicleEntities = Resources.LoadAll<VehicleEntity>("");
            GameInstance.AddAttributes(attributes);
            GameInstance.AddItems(items);
            GameInstance.AddItemCraftFormulas(0, itemCraftFormulas);
            GameInstance.AddSkills(skills);
            GameInstance.AddNpcDialogs(npcDialogs);
            GameInstance.AddQuests(quests);
            GameInstance.AddPlayerIcons(playerIcons);
            GameInstance.AddPlayerFrames(playerFrames);
            GameInstance.AddPlayerBackgrounds(playerBackgrounds);
            GameInstance.AddPlayerTitles(playerTitles);
            GameInstance.AddGuildSkills(guildSkills);
            GameInstance.AddGuildIcons(guildIcons);
            GameInstance.AddStatusEffects(statusEffects);
            GameInstance.AddCharacters(playerCharacters);
            GameInstance.AddCharacters(monsterCharacters);
            GameInstance.AddMapInfos(mapInfos);
            GameInstance.AddFactions(factions);
            GameInstance.AddGachas(gachas);
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
            GameInstance.AddPlayerCharacterEntities(playerCharacterEntities);
            GameInstance.AddMonsterCharacterEntities(monsterCharacterEntities);
            GameInstance.AddVehicleEntities(vehicleEntities);
#endif
            this.InvokeInstanceDevExtMethods("LoadDataImplement", gameInstance);
            return default;
        }
    }
}
