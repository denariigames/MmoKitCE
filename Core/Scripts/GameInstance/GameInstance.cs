using Cysharp.Threading.Tasks;
using Insthync.AddressableAssetTools;
using Insthync.CameraAndInput;
using Insthync.DevExtension;
using Insthync.UnityEditorUtils;
using LiteNetLibManager;
using System.Collections.Generic;
using UnityEngine;
#if !DISABLE_ADDRESSABLES
using UnityEngine.AddressableAssets;
#endif
using UnityEngine.Rendering;
using UnityEngine.Serialization;
#if ENABLE_PURCHASING && (UNITY_IOS || UNITY_ANDROID)
using UnityEngine.Purchasing;
#endif

namespace MultiplayerARPG
{
    public enum InventorySystem
    {
        Simple,
        LimitSlots,
    }

    public enum CurrentPositionSaveMode
    {
        UseCurrentPosition,
        UseRespawnPosition
    }

    public enum PlayerDropItemMode
    {
        DropOnGround,
        DestroyItem,
    }

    public enum DeadDropItemMode
    {
        DropOnGround,
        CorpseLooting,
    }

    public enum RewardingItemMode
    {
        DropOnGround,
        CorpseLooting,
        Immediately,
    }

    public enum RewardingMode
    {
        Immediately,
        DropOnGround,
    }

    public enum GoldStoreMode
    {
        Default,
        UserGoldOnly,
    }

    public enum TestInEditorMode
    {
        Standalone,
        Mobile,
        MobileWithKeyInputs,
        Console,
    }

    [DefaultExecutionOrder(DefaultExecutionOrders.GAME_INSTANCE)]
    [RequireComponent(typeof(EventSystemManager))]
    public partial class GameInstance : MonoBehaviour
    {
        public const string KEY_REFRESH_TOKEN = "__REFRESH_TOKEN";

        public static readonly string LogTag = nameof(GameInstance);
        public static GameInstance Singleton { get; protected set; }
        public static IClientCashShopHandlers ClientCashShopHandlers { get; set; }
        public static IClientMailHandlers ClientMailHandlers { get; set; }
        public static IClientCharacterHandlers ClientCharacterHandlers { get; set; }
        public static IClientInventoryHandlers ClientInventoryHandlers { get; set; }
        public static IClientStorageHandlers ClientStorageHandlers { get; set; }
        public static IClientPartyHandlers ClientPartyHandlers { get; set; }
        public static IClientGuildHandlers ClientGuildHandlers { get; set; }
        public static IClientGachaHandlers ClientGachaHandlers { get; set; }
        public static IClientFriendHandlers ClientFriendHandlers { get; set; }
        public static IClientBankHandlers ClientBankHandlers { get; set; }
        public static IClientUserContentHandlers ClientUserContentHandlers { get; set; }
        public static IClientOnlineCharacterHandlers ClientOnlineCharacterHandlers { get; set; }
        public static IClientChatHandlers ClientChatHandlers { get; set; }
        public static IServerMailHandlers ServerMailHandlers { get; set; }
        public static IServerUserHandlers ServerUserHandlers { get; set; }
        public static IServerBuildingHandlers ServerBuildingHandlers { get; set; }
        public static IServerGameMessageHandlers ServerGameMessageHandlers { get; set; }
        public static IServerCharacterHandlers ServerCharacterHandlers { get; set; }
        public static IServerStorageHandlers ServerStorageHandlers { get; set; }
        public static IServerPartyHandlers ServerPartyHandlers { get; set; }
        public static IServerGuildHandlers ServerGuildHandlers { get; set; }
        public static IServerChatHandlers ServerChatHandlers { get; set; }
        public static IServerUserContentHandlers ServerUserContentHandlers { get; set; }
        public static IServerLogHandlers ServerLogHandlers { get; set; }
        public static IItemUIVisibilityManager ItemUIVisibilityManager { get; set; }
        public static IItemsContainerUIVisibilityManager ItemsContainerUIVisibilityManager { get; set; }
        public static ICustomSummonManager CustomSummonManager { get; set; }
        public static string UserId { get; set; }
        public static string AccessToken { get; set; }
        public static string RefreshToken
        {
            get => PlayerPrefs.GetString(KEY_REFRESH_TOKEN, string.Empty);
            set => PlayerPrefs.SetString(KEY_REFRESH_TOKEN, value);
        }
        public static string SelectedCharacterId { get; set; }
        private static IPlayerCharacterData s_playingCharacter;
        public static IPlayerCharacterData PlayingCharacter
        {
            get { return s_playingCharacter; }
            set
            {
                s_playingCharacter = value;
                if (OnSetPlayingCharacterEvent != null)
                    OnSetPlayingCharacterEvent.Invoke(value);
            }
        }
        public static BasePlayerCharacterEntity PlayingCharacterEntity { get { return PlayingCharacter as BasePlayerCharacterEntity; } }
        private static PartyData s_joinedParty;
        public static PartyData JoinedParty
        {
            get { return s_joinedParty; }
            set
            {
                s_joinedParty = value;
                if (OnSetPartyDataEvent != null)
                    OnSetPartyDataEvent.Invoke(value);
            }
        }
        private static GuildData s_joinedGuild;
        public static GuildData JoinedGuild
        {
            get { return s_joinedGuild; }
            set
            {
                s_joinedGuild = value;
                if (OnSetGuildDataEvent != null)
                    OnSetGuildDataEvent.Invoke(value);
            }
        }
        public static Dictionary<StorageId, List<CharacterItem>> OpenedStorages { get; set; } = new Dictionary<StorageId, List<CharacterItem>>();

        [Header("Gameplay Systems")]
        [SerializeField]
        private DimensionType dimensionType = DimensionType.Dimension3D;
        [SerializeField]
        private BaseMessageManager messageManager = null;
        [SerializeField]
        private BaseGameSaveSystem saveSystem = null;
        [SerializeField]
        private BaseGameplayRule gameplayRule = null;
        [SerializeField]
        private BaseInventoryManager inventoryManager = null;
        [SerializeField]
        private BaseDayNightTimeUpdater dayNightTimeUpdater = null;
        [SerializeField]
        private BaseGMCommands gmCommands = null;
        [SerializeField]
        private BaseEquipmentModelBonesSetupManager equipmentModelBonesSetupManager = null;
        [SerializeField]
        private NetworkSetting networkSetting = null;

        [Header("Gameplay Objects")]
        [SerializeField]
        private BaseItem expDropRepresentItem = null;
        [SerializeField]
        private BaseItem goldDropRepresentItem = null;
        [SerializeField]
        private CurrencyItemPair[] currencyDropRepresentItems = new CurrencyItemPair[0];
#if UNITY_EDITOR && EXCLUDE_PREFAB_REFS && !DISABLE_ADDRESSABLES
        public UnityHelpBox entityHelpBox = new UnityHelpBox("`EXCLUDE_PREFAB_REFS` is set, you have to use only addressable assets!", UnityHelpBox.Type.Warning);
#endif
#if UNITY_EDITOR || !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
        [SerializeField]
        private ItemDropEntity itemDropEntityPrefab = null;
        [SerializeField]
        private ExpDropEntity expDropEntityPrefab = null;
        [SerializeField]
        private GoldDropEntity goldDropEntityPrefab = null;
        [SerializeField]
        private CurrencyDropEntity currencyDropEntityPrefab = null;
        [SerializeField]
        private WarpPortalEntity warpPortalEntityPrefab = null;
        [SerializeField]
        private ItemsContainerEntity playerCorpsePrefab = null;
        [SerializeField]
        private ItemsContainerEntity monsterCorpsePrefab = null;
        [SerializeField]
        private BaseUISceneGameplay uiSceneGameplayPrefab = null;
        [Tooltip("If this is empty, it will use `UI Scene Gameplay Prefab` as gameplay UI prefab")]
        [SerializeField]
        private BaseUISceneGameplay uiSceneGameplayMobilePrefab = null;
        [Tooltip("If this is empty, it will use `UI Scene Gameplay Prefab` as gameplay UI prefab")]
        [SerializeField]
        private BaseUISceneGameplay uiSceneGameplayConsolePrefab = null;
        [Tooltip("Default controller prefab will be used when controller prefab at player character entity is null")]
        [SerializeField]
        private BasePlayerCharacterController defaultControllerPrefab = null;
#endif
#if !DISABLE_ADDRESSABLES
        [SerializeField]
        private AssetReferenceItemDropEntity addressableItemDropEntityPrefab = null;
        [SerializeField]
        private AssetReferenceExpDropEntity addressableExpDropEntityPrefab = null;
        [SerializeField]
        private AssetReferenceGoldDropEntity addressableGoldDropEntityPrefab = null;
        [SerializeField]
        private AssetReferenceCurrencyDropEntity addressableCurrencyDropEntityPrefab = null;
        [SerializeField]
        private AssetReferenceWarpPortalEntity addressableWarpPortalEntityPrefab = null;
        [SerializeField]
        private AssetReferenceItemsContainerEntity addressablePlayerCorpsePrefab = null;
        [SerializeField]
        private AssetReferenceItemsContainerEntity addressableMonsterCorpsePrefab = null;
        [SerializeField]
        private AssetReferenceBaseUISceneGameplay addressableUiSceneGameplayPrefab = null;
        [Tooltip("If this is empty, it will use `Addressable UI Scene Gameplay Prefab` as gameplay UI prefab")]
        [SerializeField]
        private AssetReferenceBaseUISceneGameplay addressableUiSceneGameplayMobilePrefab = null;
        [Tooltip("If this is empty, it will use `Addressable UI Scene Gameplay Prefab` as gameplay UI prefab")]
        [SerializeField]
        private AssetReferenceBaseUISceneGameplay addressableUiSceneGameplayConsolePrefab = null;
        [Tooltip("Default controller prefab will be used when controller prefab at player character entity is null")]
        [SerializeField]
        private AssetReferenceBasePlayerCharacterController addressableDefaultControllerPrefab = null;
#endif

        [Tooltip("This is camera controller when start game as server (not start with client as host)")]
        public ServerCharacter serverCharacterPrefab = null;

        [Header("Character Objects")]
#if UNITY_EDITOR && EXCLUDE_PREFAB_REFS && !DISABLE_ADDRESSABLES
        public UnityHelpBox effectHelpBox = new UnityHelpBox("`EXCLUDE_PREFAB_REFS` is set, you have to use only addressable assets!", UnityHelpBox.Type.Warning);
#endif
#if UNITY_EDITOR || !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
        [Tooltip("These objects will be instantiate as owning character's children")]
        [SerializeField]
        private GameObject[] owningCharacterObjects = new GameObject[0];
        [Tooltip("These objects will be instantiate as owning character's children to show in minimap")]        
        [SerializeField]
        private GameObject[] owningCharacterMiniMapObjects = new GameObject[0];
        [Tooltip("These objects will be instantiate as non-owning character's children")]        
        [SerializeField]
        private GameObject[] nonOwningCharacterObjects = new GameObject[0];
        [Tooltip("These objects will be instantiate as non-owning character's children to show in minimap")]        
        [SerializeField]
        private GameObject[] nonOwningCharacterMiniMapObjects = new GameObject[0];
        [Tooltip("These objects will be instantiate as monster character's children")]        
        [SerializeField]
        private GameObject[] monsterCharacterObjects = new GameObject[0];
        [Tooltip("These objects will be instantiate as monster character's children to show in minimap")]        
        [SerializeField]
        private GameObject[] monsterCharacterMiniMapObjects = new GameObject[0];
        [Tooltip("These objects will be instantiate as npc's children")]        
        [SerializeField]
        private GameObject[] npcObjects = new GameObject[0];
        [Tooltip("These objects will be instantiate as npc's children to show in minimap")]        
        [SerializeField]
        private GameObject[] npcMiniMapObjects = new GameObject[0];
#endif
#if !DISABLE_ADDRESSABLES
        [Tooltip("These objects will be instantiate as owning character's children")]
        [SerializeField]
        private AssetReference[] addressableOwningCharacterObjects = new AssetReference[0];
        [Tooltip("These objects will be instantiate as owning character's children to show in minimap")]
        [SerializeField]
        private AssetReference[] addressableOwningCharacterMiniMapObjects = new AssetReference[0];
        [Tooltip("These objects will be instantiate as non-owning character's children")]
        [SerializeField]
        private AssetReference[] addressableNonOwningCharacterObjects = new AssetReference[0];
        [Tooltip("These objects will be instantiate as non-owning character's children to show in minimap")]
        [SerializeField]
        private AssetReference[] addressableNonOwningCharacterMiniMapObjects = new AssetReference[0];
        [Tooltip("These objects will be instantiate as monster character's children")]
        [SerializeField]
        private AssetReference[] addressableMonsterCharacterObjects = new AssetReference[0];
        [Tooltip("These objects will be instantiate as monster character's children to show in minimap")]
        [SerializeField]
        private AssetReference[] addressableMonsterCharacterMiniMapObjects = new AssetReference[0];
        [Tooltip("These objects will be instantiate as npc's children")]
        [SerializeField]
        private AssetReference[] addressableNpcObjects = new AssetReference[0];
        [Tooltip("These objects will be instantiate as npc's children to show in minimap")]
        [SerializeField]
        private AssetReference[] addressableNpcMiniMapObjects = new AssetReference[0];
#endif

        [Header("Character UIs")]
#if UNITY_EDITOR && EXCLUDE_PREFAB_REFS && !DISABLE_ADDRESSABLES
        public UnityHelpBox effectHelpBox = new UnityHelpBox("`EXCLUDE_PREFAB_REFS` is set, you have to use only addressable assets!", UnityHelpBox.Type.Warning);
#endif
#if UNITY_EDITOR || !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
        [Tooltip("This UI will be instaniate as owning character's child to show character name / HP / MP / Food / Water")]
        [SerializeField]
        private UICharacterEntity owningCharacterUI = null;
        [Tooltip("This UI will be instaniate as non owning character's child to show character name / HP / MP / Food / Water")]
        [SerializeField]
        private UICharacterEntity nonOwningCharacterUI = null;
        [Tooltip("This UI will be instaniate as monster character's child to show character name / HP / MP / Food / Water")]
        [SerializeField]
        private UICharacterEntity monsterCharacterUI = null;
        [Tooltip("This UI will be instaniate as NPC's child to show character name")]
        [SerializeField]
        private UINpcEntity npcUI = null;
        [Tooltip("This UI will be instaniate as NPC's child to show quest indecator")]
        [SerializeField]
        private NpcQuestIndicator npcQuestIndicator = null;
#endif
#if !DISABLE_ADDRESSABLES
        [Tooltip("This UI will be instaniate as owning character's child to show character name / HP / MP / Food / Water")]
        [SerializeField]
        private AssetReferenceUICharacterEntity addressableOwningCharacterUI = null;
        [Tooltip("This UI will be instaniate as non owning character's child to show character name / HP / MP / Food / Water")]
        [SerializeField]
        private AssetReferenceUICharacterEntity addressableNonOwningCharacterUI = null;
        [Tooltip("This UI will be instaniate as monster character's child to show character name / HP / MP / Food / Water")]
        [SerializeField]
        private AssetReferenceUICharacterEntity addressableMonsterCharacterUI = null;
        [Tooltip("This UI will be instaniate as NPC's child to show character name")]
        [SerializeField]
        private AssetReferenceUINpcEntity addressableNpcUI = null;
        [Tooltip("This UI will be instaniate as NPC's child to show quest indecator")]
        [SerializeField]
        private AssetReferenceNpcQuestIndicator addressableNpcQuestIndicator = null;
#endif

        [Header("Gameplay Effects")]
#if UNITY_EDITOR && EXCLUDE_PREFAB_REFS && !DISABLE_ADDRESSABLES
        public UnityHelpBox effectHelpBox = new UnityHelpBox("`EXCLUDE_PREFAB_REFS` is set, you have to use only addressable assets!", UnityHelpBox.Type.Warning);
#endif
#if UNITY_EDITOR || !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
        [SerializeField]
        [HideInInspector]
        // TODO: Deprecated, use `levelUpEffects` instead.
        private GameEffect levelUpEffect = null;
        [SerializeField]
        private GameEffect[] levelUpEffects = new GameEffect[0];
        [SerializeField]
        private GameEffect[] stunEffects = new GameEffect[0];
        [SerializeField]
        private GameEffect[] muteEffects = new GameEffect[0];
        [SerializeField]
        private GameEffect[] freezeEffects = new GameEffect[0];
#endif
#if !DISABLE_ADDRESSABLES
        [SerializeField]
        private AssetReferenceGameEffect[] addressableLevelUpEffects = new AssetReferenceGameEffect[0];
        [SerializeField]
        private AssetReferenceGameEffect[] addressableStunEffects = new AssetReferenceGameEffect[0];
        [SerializeField]
        private AssetReferenceGameEffect[] addressableMuteEffects = new AssetReferenceGameEffect[0];
        [SerializeField]
        private AssetReferenceGameEffect[] addressableFreezeEffects = new AssetReferenceGameEffect[0];
#endif

        [Header("Gameplay Database and Default Data")]
        [Tooltip("Exp tree for both player character, monster character and item, this may be deprecated in the future, you should setup `Exp Table` instead.")]
        [SerializeField]
        private int[] expTree = new int[0];
        [SerializeField]
        private ExpTable expTable = null;
        [Tooltip("You should add game data to game database and set the game database to this. If you leave this empty, it will load game data from `Resources` folders")]
        [SerializeField]
        private BaseGameDatabase gameDatabase = null;
        [SerializeField]
        private BaseEntitySetting entitySetting = null;
        [Tooltip("You can add NPCs to NPC database or may add NPCs into the scene directly, so you can leave this empty if you are going to add NPCs into the scene directly only")]
        [SerializeField]
        private NpcDatabase npcDatabase = null;
        [Tooltip("You can add warp portals to warp portal database or may add warp portals into the scene directly, So you can leave this empty if you are going to add warp portals into the scene directly only")]
        [SerializeField]
        private WarpPortalDatabase warpPortalDatabase = null;
        [Tooltip("You can add social system settings or leave this empty to use default settings")]
        [SerializeField]
        private SocialSystemSetting socialSystemSetting = null;
        [Tooltip("Default weapon item, will be used when character not equip any weapon")]
        [SerializeField]
        private BaseItem defaultWeaponItem = null;
        [Tooltip("Default damage element, will be used when attacks to enemies or receives damages from enemies")]
        [SerializeField]
        private DamageElement defaultDamageElement = null;
        [Tooltip("Default hit effects, will be used when attack to enemies or receive damages from enemies")]
#if UNITY_EDITOR && EXCLUDE_PREFAB_REFS && !DISABLE_ADDRESSABLES
        public UnityHelpBox damageHitEffectHelpBox = new UnityHelpBox("`EXCLUDE_PREFAB_REFS` is set, you have to use only addressable assets!", UnityHelpBox.Type.Warning);
#endif
#if UNITY_EDITOR || !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
        [SerializeField]
        private GameEffect[] defaultDamageHitEffects = new GameEffect[0];
#endif
#if !DISABLE_ADDRESSABLES
        [SerializeField]
        private AssetReferenceGameEffect[] addressableDefaultDamageHitEffects = new AssetReferenceGameEffect[0];
#endif

        [Header("Object Tags and Layers")]
        [Tooltip("Tag for player character entities, this tag will set to player character entities game object when instantiated")]
        public UnityTag playerTag = new UnityTag("PlayerTag");
        [Tooltip("Tag for monster character entities, this tag will set to monster character entities game object when instantiated")]
        public UnityTag monsterTag = new UnityTag("MonsterTag");
        [Tooltip("Tag for NPC entities, this tag will set to NPC entities game object when instantiated")]
        public UnityTag npcTag = new UnityTag("NpcTag");
        [Tooltip("Tag for vehicle entities, this tag will set to vehicle entities game object when instantiated")]
        public UnityTag vehicleTag = new UnityTag("VehicleTag");
        [Tooltip("Tag for item drop entities, this tag will set to item drop entities game object when instantiated")]
        public UnityTag itemDropTag = new UnityTag("ItemDropTag");
        [Tooltip("Tag for building entities, this tag will set to building entities game object when instantiated")]
        public UnityTag buildingTag = new UnityTag("BuildingTag");
        [Tooltip("Tag for harvestable entities, this tag will set to harvestable entities game object when instantiated")]
        public UnityTag harvestableTag = new UnityTag("HarvestableTag");
        [Tooltip("Layer for player character entities, this layer will be set to player character entities game object when instantiated")]
        public UnityLayer playerLayer = new UnityLayer(17);
        [Tooltip("Layer for playing character entities, this layer will be set to playing character entities game object when instantiated")]
        public UnityLayer playingLayer = new UnityLayer(17);
        [Tooltip("Layer for monster character entities, this layer will be set to monster character entities game object when instantiated")]
        public UnityLayer monsterLayer = new UnityLayer(18);
        [Tooltip("Layer for NPC entities, this layer will be set to NPC entities game object when instantiated")]
        public UnityLayer npcLayer = new UnityLayer(19);
        [Tooltip("Layer for vehicle entities, this layer will be set to vehicle entities game object when instantiated")]
        public UnityLayer vehicleLayer = new UnityLayer(20);
        [Tooltip("Layer for item drop entities, this layer will set to item drop entities game object when instantiated")]
        public UnityLayer itemDropLayer = new UnityLayer(9);
        [Tooltip("Layer for building entities, this layer will set to building entities game object when instantiated")]
        public UnityLayer buildingLayer = new UnityLayer(13);
        [Tooltip("Layer for harvestable entities, this layer will set to harvestable entities game object when instantiated")]
        public UnityLayer harvestableLayer = new UnityLayer(14);
        [Tooltip("Layers which will be used when raycasting to find hitting obstacle/wall/floor/ceil when attacking damageable objects")]
        public UnityLayer[] attackObstacleLayers = new UnityLayer[]
        {
            new UnityLayer(0),
        };
        [Tooltip("Layers which will be ignored when raycasting")]
        [FormerlySerializedAs("nonTargetingLayers")]
        public UnityLayer[] ignoreRaycastLayers = new UnityLayer[]
        {
            new UnityLayer(11)
        };

        [Header("Gameplay Configs - Generic")]
        [Tooltip("If dropped items does not picked up within this duration, it will be destroyed from the server")]
        public float itemAppearDuration = 60f;
        [Tooltip("If dropped items does not picked up by killer within this duration, anyone can pick up the items")]
        public float itemLootLockDuration = 5f;
        [Tooltip("Dropped item picked up by Looters, will go to 1 random party member, Instead of only to whoever picked first if item share is on")]
        public bool itemLootRandomPartyMember;
        [Tooltip("If this is `TRUE` anyone can pick up items which drops by players immediately")]
        public bool canPickupItemsWhichDropsByPlayersImmediately = false;
        [Tooltip("If dealing request does not accepted within this duration, the request will be cancelled")]
        public float dealingRequestDuration = 5f;
        [Tooltip("If this is > 0, it will limit amount of dealing items")]
        public int dealingItemsLimit = 16;
        [Tooltip("If this is `TRUE`, dealing feature will be disabled, all players won't be able to deal items to each other")]
        public bool disableDealing = false;
        [Tooltip("If this is > 0, it will limit amount of vending items")]
        public int vendingItemsLimit = 16;
        [Tooltip("If this is `TRUE`, vending feature will be disabled, all players won't be able to deal items to each other")]
        public bool disableVending = false;
        [Tooltip("If dueling request does not accepted within this duration, the request will be cancelled")]
        public float duelingRequestDuration = 5f;
        [Tooltip("Count down duration before start a dueling")]
        public float duelingCountDownDuration = 3f;
        [Tooltip("Dueling duration (in seconds)")]
        public float duelingDuration = 60f * 3f;
        [Tooltip("If this is `TRUE`, dueling feature will be disabled, all players won't be able to deal items to each other")]
        public bool disableDueling = false;
        [Tooltip("This is a distance that allows a player to pick up an item")]
        public float pickUpItemDistance = 1f;
        [Tooltip("This is a distance that random drop item around a player")]
        public float dropDistance = 1f;
        [Tooltip("This is a distance that allows players to start converstion with NPC, send requests to other player entities and activate an building entities")]
        public float conversationDistance = 1f;
        [Tooltip("This is a distance that other players will receives local chat")]
        public float localChatDistance = 10f;
        [Tooltip("This is a distance from controlling character that combat texts will instantiates")]
        public float combatTextDistance = 20f;
        [Tooltip("This is a distance from monster killer to other characters in party to share EXP, if this value is <= 0, it will share EXP to all other characters in the same map")]
        public float partyShareExpDistance = 0f;
        [Tooltip("This is a distance from monster killer to other characters in party to share item (allow other characters to pickup item immediately), if this value is <= 0, it will share item to all other characters in the same map")]
        public float partyShareItemDistance = 0f;
        [Tooltip("Maximum number of equip weapon set")]
        [Range(1, 16)]
        public byte maxEquipWeaponSet = 2;
        [Tooltip("How character position load when start game")]
        public CurrentPositionSaveMode currentPositionSaveMode = CurrentPositionSaveMode.UseCurrentPosition;
        [Tooltip("How player drop item")]
        public PlayerDropItemMode playerDropItemMode = PlayerDropItemMode.DropOnGround;
        [Tooltip("How player character drop item when dying (it will drop items if map info was set to drop items)")]
        public DeadDropItemMode playerDeadDropItemMode = DeadDropItemMode.DropOnGround;
        [Tooltip("If all items does not picked up from corpse within this duration, it will be destroyed from the server")]
        public float playerCorpseAppearDuration = 60f;
        [Tooltip("How monster character drop item when dying")]
        public RewardingItemMode monsterDeadDropItemMode = RewardingItemMode.DropOnGround;
        [Tooltip("How monster character drop exp when dying")]
        public RewardingMode monsterExpRewardingMode = RewardingMode.Immediately;
        [Tooltip("How monster character drop gold when dying")]
        public RewardingMode monsterGoldRewardingMode = RewardingMode.Immediately;
        [Tooltip("How monster character drop currency when dying")]
        public RewardingMode monsterCurrencyRewardingMode = RewardingMode.Immediately;
        [Tooltip("If all items does not picked up from corpse within this duration, it will be destroyed from the server")]
        public float monsterCorpseAppearDuration = 60f;
        [Tooltip("Delay before return move speed while attack or use skill to generic move speed")]
        public float returnMoveSpeedDelayAfterAction = 0.1f;
        [Tooltip("Delay before mount again")]
        public float mountDelay = 1f;
        [Tooltip("Delay before use item again")]
        public float useItemDelay = 0.25f;
        [Tooltip("If this is `TRUE`, it will clear skills cooldown when character dead")]
        public bool clearSkillCooldownOnDead = true;
        [Tooltip("How the gold stored and being used, If this is `UserGoldOnly`, it won't have character's gold, all gold will being used from user's gold")]
        public GoldStoreMode goldStoreMode = GoldStoreMode.Default;

        [Header("Gameplay Configs - Items, Inventory and Storage")]
        public ItemTypeFilter dismantleFilter = new ItemTypeFilter()
        {
            includeArmor = true,
            includeShield = true,
            includeWeapon = true
        };
        [Tooltip("If this is `TRUE`, player will be able to refine an items by themself, doesn't have to talk to NPCs")]
        public bool canRefineItemByPlayer = false;
        [Tooltip("If this is > 0, it will limit amount of refine enhancer items")]
        public int refineEnhancerItemsLimit = 16;
        [Tooltip("If this is `TRUE`, player will be able to dismantle an items by themself, doesn't have to talk to NPCs")]
        public bool canDismantleItemByPlayer = false;
        [Tooltip("If this is `TRUE`, player will be able to repair an items by themself, doesn't have to talk to NPCs")]
        public bool canRepairItemByPlayer = false;
        [Tooltip("How player's inventory works")]
        public InventorySystem inventorySystem = InventorySystem.Simple;
        [Tooltip("If this is `TRUE`, weight limit won't be applied")]
        public bool noInventoryWeightLimit;
        [Tooltip("If this is `TRUE` it won't fill empty slots")]
        public bool doNotFillEmptySlots = false;
        [Tooltip("Base slot limit for all characters, it will be used when `InventorySystem` is `LimitSlots`")]
        public int baseSlotLimit = 0;
        public Storage playerStorage = default;
        public Storage guildStorage = default;
        public EnhancerRemoval enhancerRemoval = new EnhancerRemoval();

        [Header("Gameplay Configs - Summon Monster")]
        [Tooltip("This is a distance that random summon around a character")]
        public float minSummonDistance = 2f;
        [Tooltip("This is a distance that random summon around a character")]
        public float maxSummonDistance = 3f;
        [Tooltip("Min distance to follow summoner")]
        public float minFollowSummonerDistance = 5f;
        [Tooltip("Max distance to follow summoner, if distance between characters more than this it will teleport to summoner")]
        public float maxFollowSummonerDistance = 10f;

        [Header("Gameplay Configs - Summon Pet Item")]
        [Tooltip("This is duration to lock item before it is able to summon later after character dead")]
        public float petDeadLockDuration = 60f;
        [Tooltip("This is duration to lock item before it is able to summon later after unsummon")]
        public float petUnSummonLockDuration = 30f;

        [Header("Gameplay Configs - Instance Dungeon")]
        [Tooltip("Distance from party leader character to join instance map")]
        public float joinInstanceMapDistance = 20f;

        [Header("New Character")]
        [Tooltip("If this is NULL, it will use `startGold` and `startItems`")]
        public NewCharacterSetting newCharacterSetting;
        [Tooltip("Amount of gold that will be added to character when create new character")]
        public int startGold = 0;
        [Tooltip("Items that will be added to character when create new character")]
        [ArrayElementTitle("item")]
        public ItemAmount[] startItems = new ItemAmount[0];
        [Tooltip("If it is running in editor, and if this is not NULL, it will use data from this setting for testing purpose")]
        public NewCharacterSetting testingNewCharacterSetting;

        [Header("Server Settings")]
        public bool updateAnimationAtServer = true;

        [Header("Player Configs")]
        public int minCharacterNameLength = 2;
        public int maxCharacterNameLength = 16;
        [Tooltip("Max characters that player can create, set it to 0 to unlimit")]
        public byte maxCharacterSaves = 5;

        [Header("Platforms Configs")]
        public int serverTargetFrameRate = 30;

#if UNITY_EDITOR
        [Header("Playing In Editor")]
        public TestInEditorMode testInEditorMode = TestInEditorMode.Standalone;
#if !DISABLE_ADDRESSABLES
        public AssetReferenceLanRpgNetworkManager networkManagerForOfflineTesting;
#endif
#endif

        // Static events
        public static event System.Action<IPlayerCharacterData> OnSetPlayingCharacterEvent;
        public static event System.Action<GuildData> OnSetGuildDataEvent;
        public static event System.Action<PartyData> OnSetPartyDataEvent;
        public static event System.Action OnGameDataLoadedEvent;

        #region Cache Data
        public EventSystemManager EventSystemManager { get; private set; }

        public DimensionType DimensionType
        {
            get { return dimensionType; }
        }

        public bool IsLimitInventorySlot
        {
            get { return inventorySystem == InventorySystem.LimitSlots; }
        }

        public bool IsLimitInventoryWeight
        {
            get { return !noInventoryWeightLimit; }
        }

        public BaseMessageManager MessageManager
        {
            get { return messageManager; }
        }

        public BaseGameSaveSystem SaveSystem
        {
            get { return saveSystem; }
        }

        public BaseGameplayRule GameplayRule
        {
            get { return gameplayRule; }
        }

        public BaseInventoryManager InventoryManager
        {
            get { return inventoryManager; }
        }

        public BaseDayNightTimeUpdater DayNightTimeUpdater
        {
            get { return dayNightTimeUpdater; }
        }

        public BaseGMCommands GMCommands
        {
            get { return gmCommands; }
        }

        public BaseEquipmentModelBonesSetupManager EquipmentModelBonesSetupManager
        {
            get { return equipmentModelBonesSetupManager; }
        }

        public NetworkSetting NetworkSetting
        {
            get { return networkSetting; }
        }

        public BaseGameDatabase GameDatabase
        {
            get { return gameDatabase; }
        }

        public BaseEntitySetting EntitySetting
        {
            get { return entitySetting; }
        }

        public SocialSystemSetting SocialSystemSetting
        {
            get { return socialSystemSetting; }
        }

        public BaseUISceneGameplay UISceneGameplayPrefab
        {
            get
            {
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
                if ((Application.isMobilePlatform || IsMobileTestInEditor()) && uiSceneGameplayMobilePrefab != null)
                    return uiSceneGameplayMobilePrefab;
                if ((Application.isConsolePlatform || IsConsoleTestInEditor()) && uiSceneGameplayConsolePrefab != null)
                    return uiSceneGameplayConsolePrefab;
                return uiSceneGameplayPrefab;
#else
                return null;
#endif
            }
        }

#if !DISABLE_ADDRESSABLES
        public AssetReferenceBaseUISceneGameplay AddressableUISceneGameplayPrefab
        {
            get
            {
                if ((Application.isMobilePlatform || IsMobileTestInEditor()) && addressableUiSceneGameplayMobilePrefab.IsDataValid())
                    return addressableUiSceneGameplayMobilePrefab;
                if ((Application.isConsolePlatform || IsConsoleTestInEditor()) && addressableUiSceneGameplayConsolePrefab.IsDataValid())
                    return addressableUiSceneGameplayConsolePrefab;
                return addressableUiSceneGameplayPrefab;
            }
        }
#endif

        public ExpTable ExpTable
        {
            get { return expTable; }
        }

        public BaseItem ExpDropRepresentItem
        {
            get { return expDropRepresentItem; }
        }

        public BaseItem GoldDropRepresentItem
        {
            get { return goldDropRepresentItem; }
        }

        public ItemDropEntity ItemDropEntityPrefab
        {
            get
            {
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
                return itemDropEntityPrefab;
#else
                return null;
#endif
            }
        }

        public ExpDropEntity ExpDropEntityPrefab
        {
            get
            {
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
                return expDropEntityPrefab;
#else
                return null;
#endif
            }
        }

        public GoldDropEntity GoldDropEntityPrefab
        {
            get
            {
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
                return goldDropEntityPrefab;
#else
                return null;
#endif
            }
        }

        public CurrencyDropEntity CurrencyDropEntityPrefab
        {
            get
            {
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
                return currencyDropEntityPrefab;
#else
                return null;
#endif
            }
        }

        public WarpPortalEntity WarpPortalEntityPrefab
        {
            get
            {
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
                return warpPortalEntityPrefab;
#else
                return null;
#endif
            }
        }

        public ItemsContainerEntity PlayerCorpsePrefab
        {
            get
            {
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
                return playerCorpsePrefab;
#else
                return null;
#endif
            }
        }

        public ItemsContainerEntity MonsterCorpsePrefab
        {
            get
            {
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
                return monsterCorpsePrefab;
#else
                return null;
#endif
            }
        }

        public BaseUISceneGameplay UiSceneGameplayPrefab
        {
            get
            {
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
                return uiSceneGameplayPrefab;
#else
                return null;
#endif
            }
        }

        public BaseUISceneGameplay UiSceneGameplayMobilePrefab
        {
            get
            {
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
                return uiSceneGameplayMobilePrefab;
#else
                return null;
#endif
            }
        }

        public BaseUISceneGameplay UiSceneGameplayConsolePrefab
        {
            get
            {
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
                return uiSceneGameplayConsolePrefab;
#else
                return null;
#endif
            }
        }

        public BasePlayerCharacterController DefaultControllerPrefab
        {
            get
            {
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
                return defaultControllerPrefab;
#else
                return null;
#endif
            }
        }

#if !DISABLE_ADDRESSABLES
        public AssetReferenceItemDropEntity AddressableItemDropEntityPrefab
        {
            get { return addressableItemDropEntityPrefab; }
        }

        public AssetReferenceExpDropEntity AddressableExpDropEntityPrefab
        {
            get { return addressableExpDropEntityPrefab; }
        }

        public AssetReferenceGoldDropEntity AddressableGoldDropEntityPrefab
        {
            get { return addressableGoldDropEntityPrefab; }
        }

        public AssetReferenceCurrencyDropEntity AddressableCurrencyDropEntityPrefab
        {
            get { return addressableCurrencyDropEntityPrefab; }
        }

        public AssetReferenceWarpPortalEntity AddressableWarpPortalEntityPrefab
        {
            get { return addressableWarpPortalEntityPrefab; }
        }

        public AssetReferenceItemsContainerEntity AddressablePlayerCorpsePrefab
        {
            get { return addressablePlayerCorpsePrefab; }
        }

        public AssetReferenceItemsContainerEntity AddressableMonsterCorpsePrefab
        {
            get { return addressableMonsterCorpsePrefab; }
        }

        public AssetReferenceBaseUISceneGameplay AddressableUiSceneGameplayPrefab
        {
            get { return addressableUiSceneGameplayPrefab; }
        }

        public AssetReferenceBaseUISceneGameplay AddressableUiSceneGameplayMobilePrefab
        {
            get { return addressableUiSceneGameplayMobilePrefab; }
        }

        public AssetReferenceBaseUISceneGameplay AddressableUiSceneGameplayConsolePrefab
        {
            get { return addressableUiSceneGameplayConsolePrefab; }
        }

        public AssetReferenceBasePlayerCharacterController AddressableDefaultControllerPrefab
        {
            get { return addressableDefaultControllerPrefab; }
        }
#endif

        public async UniTask<ItemDropEntity> GetLoadedItemDropEntityPrefab()
        {
#if !DISABLE_ADDRESSABLES
            return await AddressableItemDropEntityPrefab.GetOrLoadAssetAsyncOrUsePrefab(ItemDropEntityPrefab);
#else
            await UniTask.Yield();
            return ItemDropEntityPrefab;
#endif
        }

        public async UniTask<ExpDropEntity> GetLoadedExpDropEntityPrefab()
        {
#if !DISABLE_ADDRESSABLES
            return await AddressableExpDropEntityPrefab.GetOrLoadAssetAsyncOrUsePrefab(ExpDropEntityPrefab);
#else
            await UniTask.Yield();
            return ExpDropEntityPrefab;
#endif
        }

        public async UniTask<GoldDropEntity> GetLoadedGoldDropEntityPrefab()
        {
#if !DISABLE_ADDRESSABLES
            return await AddressableGoldDropEntityPrefab.GetOrLoadAssetAsyncOrUsePrefab(GoldDropEntityPrefab);
#else
            await UniTask.Yield();
            return GoldDropEntityPrefab;
#endif
        }

        public async UniTask<CurrencyDropEntity> GetLoadedCurrencyDropEntityPrefab()
        {
#if !DISABLE_ADDRESSABLES
            return await AddressableCurrencyDropEntityPrefab.GetOrLoadAssetAsyncOrUsePrefab(CurrencyDropEntityPrefab);
#else
            await UniTask.Yield();
            return CurrencyDropEntityPrefab;
#endif
        }

        public async UniTask<WarpPortalEntity> GetLoadedWarpPortalEntityPrefab()
        {
#if !DISABLE_ADDRESSABLES
            return await AddressableWarpPortalEntityPrefab.GetOrLoadAssetAsyncOrUsePrefab(WarpPortalEntityPrefab);
#else
            await UniTask.Yield();
            return WarpPortalEntityPrefab;
#endif
        }

        public async UniTask<ItemsContainerEntity> GetLoadedPlayerCorpsePrefab()
        {
#if !DISABLE_ADDRESSABLES
            return await AddressablePlayerCorpsePrefab.GetOrLoadAssetAsyncOrUsePrefab(PlayerCorpsePrefab);
#else
            await UniTask.Yield();
            return PlayerCorpsePrefab;
#endif
        }

        public async UniTask<ItemsContainerEntity> GetLoadedMonsterCorpsePrefab()
        {
#if !DISABLE_ADDRESSABLES
            return await AddressableMonsterCorpsePrefab.GetOrLoadAssetAsyncOrUsePrefab(MonsterCorpsePrefab);
#else
            await UniTask.Yield();
            return MonsterCorpsePrefab;
#endif
        }

        public async UniTask<BaseUISceneGameplay> GetLoadedUiSceneGameplayPrefab()
        {
#if !DISABLE_ADDRESSABLES
            return await AddressableUiSceneGameplayPrefab.GetOrLoadAssetAsyncOrUsePrefab(UiSceneGameplayPrefab);
#else
            await UniTask.Yield();
            return UiSceneGameplayPrefab;
#endif
        }

        public async UniTask<BaseUISceneGameplay> GetLoadedUiSceneGameplayMobilePrefab()
        {
#if !DISABLE_ADDRESSABLES
            return await AddressableUiSceneGameplayMobilePrefab.GetOrLoadAssetAsyncOrUsePrefab(UiSceneGameplayMobilePrefab);
#else
            await UniTask.Yield();
            return UiSceneGameplayMobilePrefab;
#endif
        }

        public async UniTask<BaseUISceneGameplay> GetLoadedUiSceneGameplayConsolePrefab()
        {
#if !DISABLE_ADDRESSABLES
            return await AddressableUiSceneGameplayConsolePrefab.GetOrLoadAssetAsyncOrUsePrefab(UiSceneGameplayConsolePrefab);
#else
            await UniTask.Yield();
            return UiSceneGameplayConsolePrefab;
#endif
        }

        public async UniTask<BasePlayerCharacterController> GetLoadedDefaultControllerPrefab()
        {
#if !DISABLE_ADDRESSABLES
            return await AddressableDefaultControllerPrefab.GetOrLoadAssetAsyncOrUsePrefab(DefaultControllerPrefab);
#else
            await UniTask.Yield();
            return DefaultControllerPrefab;
#endif
        }

        public GameObject[] OwningCharacterObjects
        {
            get
            {
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
                return owningCharacterObjects;
#else
                return System.Array.Empty<GameObject>();
#endif
            }
        }

#if !DISABLE_ADDRESSABLES
        public AssetReference[] AddressableOwningCharacterObjects
        {
            get { return addressableOwningCharacterObjects; }
        }
#endif

        public GameObject[] OwningCharacterMiniMapObjects
        {
            get
            {
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
                return owningCharacterMiniMapObjects;
#else
                return System.Array.Empty<GameObject>();
#endif
            }
        }

#if !DISABLE_ADDRESSABLES
        public AssetReference[] AddressableOwningCharacterMiniMapObjects
        {
            get { return addressableOwningCharacterMiniMapObjects; }
        }
#endif

        public GameObject[] NonOwningCharacterObjects
        {
            get
            {
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
                return nonOwningCharacterObjects;
#else
                return System.Array.Empty<GameObject>();
#endif
            }
        }

#if !DISABLE_ADDRESSABLES
        public AssetReference[] AddressableNonOwningCharacterObjects
        {
            get { return addressableNonOwningCharacterObjects; }
        }
#endif

        public GameObject[] NonOwningCharacterMiniMapObjects
        {
            get
            {
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
                return nonOwningCharacterMiniMapObjects;
#else
                return System.Array.Empty<GameObject>();
#endif
            }
        }

#if !DISABLE_ADDRESSABLES
        public AssetReference[] AddressableNonOwningCharacterMiniMapObjects
        {
            get { return addressableNonOwningCharacterMiniMapObjects; }
        }
#endif

        public GameObject[] MonsterCharacterObjects
        {
            get
            {
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
                return monsterCharacterObjects;
#else
                return System.Array.Empty<GameObject>();
#endif
            }
        }

#if !DISABLE_ADDRESSABLES
        public AssetReference[] AddressableMonsterCharacterObjects
        {
            get { return addressableMonsterCharacterObjects; }
        }
#endif

        public GameObject[] MonsterCharacterMiniMapObjects
        {
            get
            {
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
                return monsterCharacterMiniMapObjects;
#else
                return System.Array.Empty<GameObject>();
#endif
            }
        }

#if !DISABLE_ADDRESSABLES
        public AssetReference[] AddressableMonsterCharacterMiniMapObjects
        {
            get { return addressableMonsterCharacterMiniMapObjects; }
        }
#endif

        public GameObject[] NpcObjects
        {
            get
            {
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
                return npcObjects;
#else
                return System.Array.Empty<GameObject>();
#endif
            }
        }

#if !DISABLE_ADDRESSABLES
        public AssetReference[] AddressableNpcObjects
        {
            get { return addressableNpcObjects; }
        }
#endif

        public GameObject[] NpcMiniMapObjects
        {
            get
            {
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
                return npcMiniMapObjects;
#else
                return System.Array.Empty<GameObject>();
#endif
            }
        }

#if !DISABLE_ADDRESSABLES
        public AssetReference[] AddressableNpcMiniMapObjects
        {
            get { return addressableNpcMiniMapObjects; }
        }
#endif

        public UICharacterEntity OwningCharacterUI
        {
            get
            {
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
                return owningCharacterUI;
#else
                return null;
#endif
            }
        }

        public UICharacterEntity NonOwningCharacterUI
        {
            get
            {
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
                return nonOwningCharacterUI;
#else
                return null;
#endif
            }
        }

        public UICharacterEntity MonsterCharacterUI
        {
            get
            {
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
                return monsterCharacterUI;
#else
                return null;
#endif
            }
        }

        public UINpcEntity NpcUI
        {
            get
            {
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
                return npcUI;
#else
                return null;
#endif
            }
        }

        public NpcQuestIndicator NpcQuestIndicator
        {
            get
            {
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
                return npcQuestIndicator;
#else
                return null;
#endif
            }
        }

#if !DISABLE_ADDRESSABLES
        public AssetReferenceUICharacterEntity AddressableOwningCharacterUI
        {
            get { return addressableOwningCharacterUI; }
        }

        public AssetReferenceUICharacterEntity AddressableNonOwningCharacterUI
        {
            get { return addressableNonOwningCharacterUI; }
        }

        public AssetReferenceUICharacterEntity AddressableMonsterCharacterUI
        {
            get { return addressableMonsterCharacterUI; }
        }

        public AssetReferenceUINpcEntity AddressableNpcUI
        {
            get { return addressableNpcUI; }
        }

        public AssetReferenceNpcQuestIndicator AddressableNpcQuestIndicator
        {
            get { return addressableNpcQuestIndicator; }
        }
#endif

        public async UniTask<UICharacterEntity> GetLoadedOwningCharacterUIPrefab()
        {
#if !DISABLE_ADDRESSABLES
            return await AddressableOwningCharacterUI.GetOrLoadAssetAsyncOrUsePrefab(OwningCharacterUI);
#else
            await UniTask.Yield();
            return OwningCharacterUI;
#endif
        }

        public async UniTask<UICharacterEntity> GetLoadedNonOwningCharacterUIPrefab()
        {
#if !DISABLE_ADDRESSABLES
            return await AddressableNonOwningCharacterUI.GetOrLoadAssetAsyncOrUsePrefab(NonOwningCharacterUI);
#else
            await UniTask.Yield();
            return NonOwningCharacterUI;
#endif
        }

        public async UniTask<UICharacterEntity> GetLoadedMonsterCharacterUIPrefab()
        {
#if !DISABLE_ADDRESSABLES
            return await AddressableMonsterCharacterUI.GetOrLoadAssetAsyncOrUsePrefab(MonsterCharacterUI);
#else
            await UniTask.Yield();
            return MonsterCharacterUI;
#endif
        }

        public async UniTask<UINpcEntity> GetLoadedNpcUIPrefab()
        {
#if !DISABLE_ADDRESSABLES
            return await AddressableNpcUI.GetOrLoadAssetAsyncOrUsePrefab(NpcUI);
#else
            await UniTask.Yield();
            return NpcUI;
#endif
        }

        public async UniTask<NpcQuestIndicator> GetLoadedNpcQuestIndicatorPrefab()
        {
#if !DISABLE_ADDRESSABLES
            return await AddressableNpcQuestIndicator.GetOrLoadAssetAsyncOrUsePrefab(NpcQuestIndicator);
#else
            await UniTask.Yield();
            return NpcQuestIndicator;
#endif
        }

        public GameEffect[] LevelUpEffects
        {
            get
            {
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
                return levelUpEffects;
#else
                return System.Array.Empty<GameEffect>();
#endif
            }
        }

        public GameEffect[] StunEffects
        {
            get
            {
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
                return stunEffects;
#else
                return System.Array.Empty<GameEffect>();
#endif
            }
        }

        public GameEffect[] MuteEffects
        {
            get
            {
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
                return muteEffects;
#else
                return System.Array.Empty<GameEffect>();
#endif
            }
        }

        public GameEffect[] FreezeEffects
        {
            get
            {
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
                return freezeEffects;
#else
                return System.Array.Empty<GameEffect>();
#endif
            }
        }

#if !DISABLE_ADDRESSABLES
        public AssetReferenceGameEffect[] AddressableLevelUpEffects
        {
            get { return addressableLevelUpEffects; }
        }

        public AssetReferenceGameEffect[] AddressableStunEffects
        {
            get { return addressableStunEffects; }
        }

        public AssetReferenceGameEffect[] AddressableMuteEffects
        {
            get { return addressableMuteEffects; }
        }

        public AssetReferenceGameEffect[] AddressableFreezeEffects
        {
            get { return addressableFreezeEffects; }
        }
#endif

        public ArmorType DefaultArmorType
        {
            get; private set;
        }

        public WeaponType DefaultWeaponType
        {
            get; private set;
        }

        public IWeaponItem DefaultWeaponItem
        {
            get { return defaultWeaponItem as IWeaponItem; }
        }

        public IWeaponItem MonsterWeaponItem
        {
            get; private set;
        }

        public DamageElement DefaultDamageElement
        {
            get { return defaultDamageElement; }
        }

        public GameEffect[] DefaultDamageHitEffects
        {
            get
            {
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
                return defaultDamageHitEffects;
#else
                return System.Array.Empty<GameEffect>();
#endif
            }
        }

#if !DISABLE_ADDRESSABLES
        public AssetReferenceGameEffect[] AddressableDefaultDamageHitEffects
        {
            get { return addressableDefaultDamageHitEffects; }
        }
#endif

        public NewCharacterSetting NewCharacterSetting
        {
            get
            {
#if UNITY_EDITOR
                if (testingNewCharacterSetting != null)
                    return testingNewCharacterSetting;
#endif
                return newCharacterSetting;
            }
        }

        public bool HasNewCharacterSetting
        {
            get
            {
                return NewCharacterSetting != null;
            }
        }
        
        public HashSet<int> IgnoreRaycastLayersValues { get; private set; }

        public static readonly Dictionary<string, bool> LoadHomeScenePreventions = new Dictionary<string, bool>();
        public static bool DoNotLoadHomeScene
        {
            get
            {
                foreach (bool doNotLoad in LoadHomeScenePreventions.Values)
                {
                    if (doNotLoad)
                        return true;
                }
                return false;
            }
        }
#endregion

        protected virtual void Awake()
        {
            if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null)
            {
                // Set target framerate when running headless to reduce CPU usage
                QualitySettings.vSyncCount = 0;
                Application.targetFrameRate = serverTargetFrameRate;
            }
            Application.runInBackground = true;
            if (Singleton != null)
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
            Singleton = this;
            s_playingCharacter = default;
            JoinedParty = null;
            JoinedGuild = null;
            OpenedStorages.Clear();
            LoadHomeScenePreventions.Clear();
            EventSystemManager = gameObject.GetOrAddComponent<EventSystemManager>();
#if UNITY_EDITOR
            InputManager.UseMobileInputOnNonMobile = IsMobileTestInEditor();
            InputManager.UseNonMobileInput = testInEditorMode == TestInEditorMode.MobileWithKeyInputs && Application.isEditor;
#endif

            DefaultArmorType = ScriptableObject.CreateInstance<ArmorType>()
                .GenerateDefaultArmorType();

            DefaultWeaponType = ScriptableObject.CreateInstance<WeaponType>()
                .GenerateDefaultWeaponType();

            // Setup default weapon item if not existed
            if (defaultWeaponItem == null || !defaultWeaponItem.IsWeapon())
            {
                defaultWeaponItem = ScriptableObject.CreateInstance<Item>()
                    .GenerateDefaultItem(DefaultWeaponType);
                // Use the same item with default weapon item (if default weapon not set by user)
                MonsterWeaponItem = defaultWeaponItem as IWeaponItem;
            }

            // Setup monster weapon item if not existed
            if (MonsterWeaponItem == null)
            {
                MonsterWeaponItem = ScriptableObject.CreateInstance<Item>()
                    .GenerateDefaultItem(DefaultWeaponType);
            }

            // Setup default damage element if not existed
            if (defaultDamageElement == null)
            {
                defaultDamageElement = ScriptableObject.CreateInstance<DamageElement>()
                    .GenerateDefaultDamageElement(DefaultDamageHitEffects
#if !DISABLE_ADDRESSABLES
                    , AddressableDefaultDamageHitEffects
#endif
                    );
            }

            // Setup string formatter if not existed
            if (messageManager == null)
                messageManager = ScriptableObject.CreateInstance<DefaultMessageManager>();

            // Setup save system if not existed
            if (saveSystem == null)
                saveSystem = ScriptableObject.CreateInstance<DefaultGameSaveSystem>();

            // Setup gameplay rule if not existed
            if (gameplayRule == null)
                gameplayRule = ScriptableObject.CreateInstance<DefaultGameplayRule>();

            // Reset gold and exp rate
            gameplayRule.GoldRate = 1f;
            gameplayRule.ExpRate = 1f;

            // Setup inventory manager
            if (inventoryManager == null)
                inventoryManager = ScriptableObject.CreateInstance<DefaultInventoryManager>();

            // Setup day night time updater if not existed
            if (dayNightTimeUpdater == null)
                dayNightTimeUpdater = ScriptableObject.CreateInstance<DefaultDayNightTimeUpdater>();

            // Setup GM commands if not existed
            if (gmCommands == null)
                gmCommands = ScriptableObject.CreateInstance<DefaultGMCommands>();

            // Setup equipment model bones setup manager if not existed
            if (equipmentModelBonesSetupManager == null)
                equipmentModelBonesSetupManager = ScriptableObject.CreateInstance<EquipmentModelBonesSetupByHumanBodyBonesManager>();

            // Setup network setting if not existed
            if (networkSetting == null)
                networkSetting = ScriptableObject.CreateInstance<NetworkSetting>();

            // Setup exp table if not existed, and use exp tree
            if (expTable == null)
            {
                expTable = ScriptableObject.CreateInstance<ExpTable>();
                expTable.expTree = expTree;
            }

            // Setup game database if not existed
            if (gameDatabase == null)
                gameDatabase = ScriptableObject.CreateInstance<ResourcesFolderGameDatabase>();

            // Setup entity setting if not existed
            if (entitySetting == null)
                entitySetting = ScriptableObject.CreateInstance<DefaultEntitySetting>();

            // Setup social system setting if not existed
            if (socialSystemSetting == null)
                socialSystemSetting = ScriptableObject.CreateInstance<SocialSystemSetting>();
            socialSystemSetting.Migrate();

            // Setup non target layers
            IgnoreRaycastLayersValues = new HashSet<int>();
            foreach (UnityLayer layer in ignoreRaycastLayers)
            {
                IgnoreRaycastLayersValues.Add(layer.LayerIndex);
            }

            // Setup default home scenes
#if !DISABLE_ADDRESSABLES
            if (!addressableHomeMobileScene.IsDataValid())
                addressableHomeMobileScene = addressableHomeScene;
            if (!addressableHomeConsoleScene.IsDataValid())
                addressableHomeConsoleScene = addressableHomeScene;
#endif
            if (!homeMobileScene.IsDataValid())
                homeMobileScene = homeScene;
            if (!homeConsoleScene.IsDataValid())
                homeConsoleScene = homeScene;

            ClearData();
            this.InvokeInstanceDevExtMethods("Awake");
        }

        protected virtual void Start()
        {
            GameDatabase.LoadData(this).Forget();
        }

        protected virtual void OnDestroy()
        {
            this.InvokeInstanceDevExtMethods("OnDestroy");
        }

        public static void ClearData()
        {
            Attributes.Clear();
            Currencies.Clear();
            CurrencyDropRepresentItems.Clear();
            Items.Clear();
            ItemsByAmmoType.Clear();
            ItemCraftFormulas.Clear();
            Harvestables.Clear();
            Characters.Clear();
            PlayerCharacters.Clear();
            PlayerCharacterEntityMetaDataList.Clear();
            MonsterCharacters.Clear();
            ArmorTypes.Clear();
            WeaponTypes.Clear();
            AmmoTypes.Clear();
            Skills.Clear();
            NpcDialogs.Clear();
            Quests.Clear();
            PlayerIcons.Clear();
            PlayerFrames.Clear();
            PlayerBackgrounds.Clear();
            PlayerTitles.Clear();
            GuildSkills.Clear();
            GuildIcons.Clear();
            Gachas.Clear();
            StatusEffects.Clear();
            DamageElements.Clear();
            EquipmentSets.Clear();
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
            BuildingEntities.Clear();
            PlayerCharacterEntities.Clear();
            MonsterCharacterEntities.Clear();
            ItemDropEntities.Clear();
            HarvestableEntities.Clear();
            VehicleEntities.Clear();
            WarpPortalEntities.Clear();
            NpcEntities.Clear();
#endif
#if !DISABLE_ADDRESSABLES
            AddressableBuildingEntities.Clear();
            AddressablePlayerCharacterEntities.Clear();
            AddressableMonsterCharacterEntities.Clear();
            AddressableItemDropEntities.Clear();
            AddressableHarvestableEntities.Clear();
            AddressableVehicleEntities.Clear();
            AddressableWarpPortalEntities.Clear();
            AddressableNpcEntities.Clear();
#endif
            MapWarpPortals.Clear();
            MapNpcs.Clear();
            MapInfos.Clear();
            Factions.Clear();
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
            OtherNetworkObjectPrefabs.Clear();
#endif
#if !DISABLE_ADDRESSABLES
            AddressableOtherNetworkObjectPrefabs.Clear();
#endif
        }

        public static bool UseMobileInput()
        {
            return Application.isMobilePlatform || IsMobileTestInEditor();
        }

        public static bool UseConsoleInput()
        {
            return Application.isConsolePlatform || IsConsoleTestInEditor();
        }

        public static bool IsMobileTestInEditor()
        {
#if UNITY_EDITOR
            return (Singleton.testInEditorMode == TestInEditorMode.Mobile || Singleton.testInEditorMode == TestInEditorMode.MobileWithKeyInputs) && Application.isEditor;
#else
            return false;
#endif
        }

        public static bool IsConsoleTestInEditor()
        {
#if UNITY_EDITOR
            return Singleton.testInEditorMode == TestInEditorMode.Console && Application.isEditor;
#else
            return false;
#endif
        }

        public void LoadedGameData()
        {
            this.InvokeInstanceDevExtMethods("LoadedGameData");
            // Add ammo items to dictionary
            foreach (BaseItem item in Items.Values)
            {
                if (item.IsAmmo())
                {
                    IAmmoItem ammoItem = item as IAmmoItem;
                    if (ammoItem.AmmoType == null)
                        continue;
                    if (!ItemsByAmmoType.ContainsKey(ammoItem.AmmoType.DataId))
                        ItemsByAmmoType.Add(ammoItem.AmmoType.DataId, new Dictionary<int, BaseItem>());
                    ItemsByAmmoType[ammoItem.AmmoType.DataId][item.DataId] = item;
                }
            }

            List<Currency> representCurrencies = new List<Currency>();
            List<BaseItem> representItems = new List<BaseItem>()
            {
                DefaultWeaponItem as BaseItem,
                MonsterWeaponItem as BaseItem,
            };

            if (ExpDropRepresentItem != null)
            {
                ExpDropRepresentItem.MaxStack = int.MaxValue;
                representItems.Add(ExpDropRepresentItem);
            }

            if (GoldDropRepresentItem != null)
            {
                GoldDropRepresentItem.MaxStack = int.MaxValue;
                representItems.Add(GoldDropRepresentItem);
            }

            foreach (CurrencyItemPair currencyItemPair in currencyDropRepresentItems)
            {
                if (currencyItemPair.currency == null || currencyItemPair.item == null)
                    continue;
                representCurrencies.Add(currencyItemPair.currency);
                representItems.Add(currencyItemPair.item);
                currencyItemPair.item.MaxStack = int.MaxValue;
                CurrencyDropRepresentItems[currencyItemPair.currency.DataId] = currencyItemPair.item;
            }

            // Add required represent game data
            AddCurrencies(representCurrencies);
            AddItems(representItems);
            MigrateLevelUpEffect();

            if (newCharacterSetting != null && newCharacterSetting.startItems != null)
                AddItems(newCharacterSetting.startItems);

#if UNITY_EDITOR
            if (testingNewCharacterSetting != null && testingNewCharacterSetting.startItems != null)
                AddItems(testingNewCharacterSetting.startItems);
#endif

            if (startItems != null)
                AddItems(startItems);

            if (warpPortalDatabase != null && warpPortalDatabase.maps != null)
                AddMapWarpPortals(warpPortalDatabase.maps);

            if (npcDatabase != null && npcDatabase.maps != null)
                AddMapNpcs(npcDatabase.maps);

            if (Application.isPlaying)
                InitializePurchasing();

            System.GC.Collect();
            OnGameDataLoaded();
        }

        public void OnGameDataLoaded()
        {
            if (OnGameDataLoadedEvent != null)
                OnGameDataLoadedEvent.Invoke();
            if (Application.isPlaying && !DoNotLoadHomeScene)
                LoadHomeScene();
        }

        public bool IsRepresentItem(BaseItem item)
        {
            return
                IsExpDropRepresentItem(item) ||
                IsGoldDropRepresentItem(item) ||
                IsCurrencyDropRepresentItem(item, out _);
        }

        public bool IsExpDropRepresentItem(BaseItem item)
        {
            if (expDropRepresentItem != null && expDropRepresentItem == item)
                return true;
            return false;
        }

        public bool IsGoldDropRepresentItem(BaseItem item)
        {
            if (goldDropRepresentItem != null && goldDropRepresentItem == item)
                return true;
            return false;
        }

        public bool IsCurrencyDropRepresentItem(BaseItem item, out Currency currency)
        {
#if !DISABLE_CUSTOM_CHARACTER_CURRENCIES
            if (currencyDropRepresentItems != null && currencyDropRepresentItems.Length > 0)
            {
                for (int i = 0; i < currencyDropRepresentItems.Length; ++i)
                {
                    if (currencyDropRepresentItems[i].item != null && currencyDropRepresentItems[i].item == item)
                    {
                        currency = currencyDropRepresentItems[i].currency;
                        return true;
                    }
                }
            }
#endif
            currency = null;
            return false;
        }

        public List<string> GetGameMapIds()
        {
            List<string> mapIds = new List<string>();
            foreach (BaseMapInfo mapInfo in MapInfos.Values)
            {
                if (mapInfo != null && !string.IsNullOrEmpty(mapInfo.Id) && !mapIds.Contains(mapInfo.Id))
                    mapIds.Add(mapInfo.Id);
            }

            return mapIds;
        }

        public int GetAttackObstacleLayerMask()
        {
            int layerMask = 0;
            if (attackObstacleLayers.Length > 0)
            {
                foreach (UnityLayer attackObstacleLayer in attackObstacleLayers)
                {
                    layerMask = layerMask | attackObstacleLayer.Mask;
                }
            }
            return layerMask;
        }

        public int MixWithAttackObstacleLayers(int layerMask)
        {
            return layerMask | GetAttackObstacleLayerMask();
        }

        public int GetIgnoreRaycastLayerMask()
        {
            int layerMask = 0;
            if (ignoreRaycastLayers.Length > 0)
            {
                foreach (UnityLayer ignoreRaycastLayer in ignoreRaycastLayers)
                {
                    layerMask = layerMask | ignoreRaycastLayer.Mask;
                }
            }
            layerMask = layerMask | 1 << PhysicLayers.IgnoreRaycast;
            return layerMask;
        }

        public int MixWithIgnoreRaycastLayers(int layerMask)
        {
            return layerMask | GetIgnoreRaycastLayerMask();
        }

        /// <summary>
        /// All layers except `nonTargetingLayers`, `TransparentFX`, `IgnoreRaycast`, `Water` will be used for raycasting
        /// </summary>
        /// <returns></returns>
        public int GetTargetLayerMask()
        {
            // 0 = Nothing, -1 = AllLayers
            int layerMask = 0;
            layerMask = layerMask | 1 << PhysicLayers.TransparentFX;
            layerMask = layerMask | 1 << PhysicLayers.Water;
            layerMask = MixWithIgnoreRaycastLayers(layerMask);
            return ~layerMask;
        }

        /// <summary>
        /// Check is layer is layer for any damageable entities or not
        /// </summary>
        /// <param name="layer"></param>
        /// <returns></returns>
        public bool IsDamageableLayer(int layer)
        {
            return layer == playerLayer ||
                layer == playingLayer ||
                layer == monsterLayer ||
                layer == vehicleLayer ||
                layer == buildingLayer ||
                layer == harvestableLayer;
        }

        /// <summary>
        /// Only `playerLayer`, `playingLayer`, `monsterLayer`, `vehicleLayer`, `buildingLayer`, `harvestableLayer` will be used for hit detection casting
        /// </summary>
        /// <returns></returns>
        public int GetDamageableLayerMask()
        {
            int layerMask = 0;
            layerMask = layerMask | playerLayer.Mask;
            layerMask = layerMask | playingLayer.Mask;
            layerMask = layerMask | monsterLayer.Mask;
            layerMask = layerMask | vehicleLayer.Mask;
            layerMask = layerMask | buildingLayer.Mask;
            layerMask = layerMask | harvestableLayer.Mask;
            return layerMask;
        }

        /// <summary>
        /// Only `playerLayer`, `playingLayer`, `monsterLayer`, `vehicleLayer`, `buildingLayer`, `harvestableLayer` and wall layers will be used for hit detection casting
        /// </summary>
        /// <returns></returns>
        public int GetDamageEntityHitLayerMask()
        {
            int layerMask = 0;
            layerMask = layerMask | playerLayer.Mask;
            layerMask = layerMask | playingLayer.Mask;
            layerMask = layerMask | monsterLayer.Mask;
            layerMask = layerMask | vehicleLayer.Mask;
            layerMask = layerMask | buildingLayer.Mask;
            layerMask = layerMask | harvestableLayer.Mask;
            layerMask = MixWithAttackObstacleLayers(layerMask);
            return layerMask;
        }

        /// <summary>
        /// All layers except `playerLayer`, `playingLayer`, `monsterLayer`, `npcLayer`, `vehicleLayer`, `itemDropLayer, `harvestableLayer`, `TransparentFX`, `IgnoreRaycast`, `Water` will be used for raycasting
        /// </summary>
        /// <returns></returns>
        public int GetBuildLayerMask()
        {
            int layerMask = 0;
            layerMask = layerMask | 1 << PhysicLayers.TransparentFX;
            layerMask = layerMask | 1 << PhysicLayers.Water;
            layerMask = layerMask | 1 << PhysicLayers.IgnoreRaycast;
            layerMask = layerMask | playerLayer.Mask;
            layerMask = layerMask | playingLayer.Mask;
            layerMask = layerMask | monsterLayer.Mask;
            layerMask = layerMask | npcLayer.Mask;
            layerMask = layerMask | vehicleLayer.Mask;
            layerMask = layerMask | itemDropLayer.Mask;
            layerMask = layerMask | harvestableLayer.Mask;
            return ~layerMask;
        }

        /// <summary>
        /// All layers except `playerLayer`, `playingLayer`, `monsterLayer`, `npcLayer`, `vehicleLayer`, `itemDropLayer, `TransparentFX`, `IgnoreRaycast`, `Water` and non-target layers will be used for raycasting
        /// </summary>
        /// <returns></returns>
        public int GetItemDropGroundDetectionLayerMask()
        {
            int layerMask = 0;
            layerMask = layerMask | 1 << PhysicLayers.TransparentFX;
            layerMask = layerMask | 1 << PhysicLayers.Water;
            layerMask = layerMask | playerLayer.Mask;
            layerMask = layerMask | playingLayer.Mask;
            layerMask = layerMask | monsterLayer.Mask;
            layerMask = layerMask | npcLayer.Mask;
            layerMask = layerMask | vehicleLayer.Mask;
            layerMask = layerMask | itemDropLayer.Mask;
            layerMask = MixWithIgnoreRaycastLayers(layerMask);
            return ~layerMask;
        }

        /// <summary>
        /// All layers except `playerLayer`, `playingLayer`, `monsterLayer`, `npcLayer`, `vehicleLayer`, `itemDropLayer, `TransparentFX`, `IgnoreRaycast`, `Water` and non-target layers will be used for raycasting
        /// </summary>
        /// <returns></returns>
        public int GetGameEntityGroundDetectionLayerMask(bool excludeWater = true)
        {
            int layerMask = 0;
            layerMask = layerMask | 1 << PhysicLayers.TransparentFX;
            if (excludeWater)
                layerMask = layerMask | 1 << PhysicLayers.Water;
            layerMask = layerMask | playerLayer.Mask;
            layerMask = layerMask | playingLayer.Mask;
            layerMask = layerMask | monsterLayer.Mask;
            layerMask = layerMask | npcLayer.Mask;
            layerMask = layerMask | vehicleLayer.Mask;
            layerMask = layerMask | itemDropLayer.Mask;
            layerMask = MixWithIgnoreRaycastLayers(layerMask);
            return ~layerMask;
        }

        /// <summary>
        /// All layers except `playerLayer`, `playingLayer`, `monsterLayer`, `npcLayer`, `vehicleLayer`, `itemDropLayer`, `buildingLayer`, `harvestableLayer, `TransparentFX`, `IgnoreRaycast`, `Water` and non-target layers will be used for raycasting
        /// </summary>
        /// <returns></returns>
        public int GetHarvestableSpawnGroundDetectionLayerMask()
        {
            int layerMask = 0;
            layerMask = layerMask | 1 << PhysicLayers.TransparentFX;
            layerMask = layerMask | 1 << PhysicLayers.Water;
            layerMask = layerMask | playerLayer.Mask;
            layerMask = layerMask | playingLayer.Mask;
            layerMask = layerMask | monsterLayer.Mask;
            layerMask = layerMask | npcLayer.Mask;
            layerMask = layerMask | vehicleLayer.Mask;
            layerMask = layerMask | itemDropLayer.Mask;
            layerMask = layerMask | buildingLayer.Mask;
            layerMask = layerMask | harvestableLayer.Mask;
            layerMask = MixWithIgnoreRaycastLayers(layerMask);
            return ~layerMask;
        }

        /// <summary>
        /// All layers except `playerLayer`, `playingLayer`, `monsterLayer`, `npcLayer`, `vehicleLayer`, `itemDropLayer`, `harvestableLayer, `TransparentFX`, `IgnoreRaycast`, `Water` and non-target layers will be used for raycasting
        /// </summary>
        /// <returns></returns>
        public int GetAreaSkillGroundDetectionLayerMask()
        {
            int layerMask = 0;
            layerMask = layerMask | 1 << PhysicLayers.TransparentFX;
            layerMask = layerMask | 1 << PhysicLayers.Water;
            layerMask = layerMask | playerLayer.Mask;
            layerMask = layerMask | playingLayer.Mask;
            layerMask = layerMask | monsterLayer.Mask;
            layerMask = layerMask | npcLayer.Mask;
            layerMask = layerMask | vehicleLayer.Mask;
            layerMask = layerMask | itemDropLayer.Mask;
            layerMask = layerMask | harvestableLayer.Mask;
            layerMask = MixWithIgnoreRaycastLayers(layerMask);
            return ~layerMask;
        }
    }
}
