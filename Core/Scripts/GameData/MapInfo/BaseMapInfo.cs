using Cysharp.Threading.Tasks;
using Insthync.AddressableAssetTools;
using Insthync.UnityEditorUtils;
using LiteNetLib.Utils;
using LiteNetLibManager;
using System.Collections.Generic;
using UnityEngine;
#if !DISABLE_ADDRESSABLES
using UnityEngine.AddressableAssets;
#endif
using UnityEngine.Serialization;

namespace MultiplayerARPG
{
    public abstract partial class BaseMapInfo : BaseGameData, INetSerializable
    {
        #region Map Info Settings
        [Category("Map Info Settings")]
        [SerializeField]
        private SceneField scene = default;
        public virtual SceneField Scene { get { return scene; } }

#if !DISABLE_ADDRESSABLES
        [SerializeField]
        private AssetReferenceScene addressableScene;
        public virtual AssetReferenceScene AddressableScene { get { return addressableScene; } }
#endif

        [Tooltip("This will be used when new character has been created to set its position, and this map data is the start map")]
        [SerializeField]
        private Vector3 startPosition = Vector3.zero;
        public virtual Vector3 StartPosition { get { return startPosition; } }

        [Tooltip("This will be used when new character has been created to set its rotation, and this map data is the start map")]
        [SerializeField]
        private Vector3 startRotation = Vector3.zero;
        public virtual Vector3 StartRotation { get { return startRotation; } }

        [SerializeField]
        private bool enablePkRules = false;
        public virtual bool EnablePkRules { get { return enablePkRules; } }

        [Tooltip("If this is `TRUE`, dealing feature will be disabled, all players in this map won't be able to deal items to each other")]
        [SerializeField]
        private bool disableDealing = false;
        public virtual bool DisableDealing { get { return disableDealing; } }

        [Tooltip("If this is `TRUE`, vending feature will be disabled, all players won't be able to deal items to each other")]
        [SerializeField]
        private bool disableVending = false;
        public virtual bool DisableVending { get { return disableVending; } }

        [Tooltip("If this is `TRUE`, dueling feature will be disabled, all players won't be able to deal items to each other")]
        [SerializeField]
        private bool disableDueling = false;
        public virtual bool DisableDueling { get { return disableDueling; } }
#endregion

        #region Character Death Rules
        [Category("Character Death Rules")]
        [Tooltip("When character fall to this position, character will dead")]
        [SerializeField]
        private float deadY = -100f;
        public virtual float DeadY { get { return deadY; } }

        [Tooltip("When character dead, it will drop equipping weapons or not?")]
        [SerializeField]
        private PlayerItemDropMode playerDeadDropsEquipWeapons = PlayerItemDropMode.None;
        public virtual PlayerItemDropMode PlayerDeadDropsEquipWeapons { get { return playerDeadDropsEquipWeapons; } }

        [Tooltip("When character dead, it will drop equipping items or not?")]
        [SerializeField]
        private PlayerItemDropMode playerDeadDropsEquipItems = PlayerItemDropMode.None;
        public virtual PlayerItemDropMode PlayerDeadDropsEquipItems { get { return playerDeadDropsEquipItems; } }

        [Tooltip("When character dead, it will drop non equipping items or not?")]
        [SerializeField]
        private PlayerItemDropMode playerDeadDropsNonEquipItems = PlayerItemDropMode.None;
        public virtual PlayerItemDropMode PlayerDeadDropsNonEquipItems { get { return playerDeadDropsNonEquipItems; } }

        [Tooltip("When character dead, it will drop gold or not?")]
        [SerializeField]
        private PlayerItemDropMode playerDeadDropsGold = PlayerItemDropMode.None;
        public virtual PlayerItemDropMode PlayerDeadDropsGold { get { return playerDeadDropsGold; } }
        #endregion

        #region Item Drop Rules
        [Category("Item Drop Rules")]
        [Tooltip("These items will be excluded when monster dropping items")]
        [SerializeField]
        private List<BaseItem> excludeItems = new List<BaseItem>();
        public List<BaseItem> ExcludeItems { get { return excludeItems; } }

        [Tooltip("Items with these ammo types will be excluded when monster dropping items")]
        [SerializeField]
        private List<AmmoType> excludeAmmoTypes = new List<AmmoType>();
        public List<AmmoType> ExcludeAmmoTypes { get { return excludeAmmoTypes; } }

        [Tooltip("Items with these armor types will be excluded when monster dropping items")]
        [SerializeField]
        private List<ArmorType> excludeArmorTypes = new List<ArmorType>();
        public List<ArmorType> ExcludeArmorTypes { get { return excludeArmorTypes; } }

        [Tooltip("Items with these weapon types will be excluded when monster dropping items")]
        [SerializeField]
        private List<WeaponType> excludeWeaponTypes = new List<WeaponType>();
        public List<WeaponType> ExcludeWeaponTypes { get { return excludeWeaponTypes; } }

        [Tooltip("Junk items will be excluded when monster dropping items or not?")]
        [SerializeField]
        private bool excludeJunk = false;
        public bool ExcludeJunk { get { return excludeJunk; } }

        [Tooltip("Armor items will be excluded when monster dropping items or not?")]
        [SerializeField]
        private bool excludeArmor = false;
        public bool ExcludeArmor { get { return excludeArmor; } }

        [Tooltip("Shield items will be excluded when monster dropping items or not?")]
        [SerializeField]
        private bool excludeShield = false;
        public bool ExcludeShield { get { return excludeShield; } }

        [Tooltip("Weapon items will be excluded when monster dropping items or not?")]
        [SerializeField]
        private bool excludeWeapon = false;
        public bool ExcludeWeapon { get { return excludeWeapon; } }

        [Tooltip("Potion items will be excluded when monster dropping items or not?")]
        [SerializeField]
        private bool excludePotion = false;
        public bool ExcludePotion { get { return excludePotion; } }

        [Tooltip("Ammo items will be excluded when monster dropping items or not?")]
        [SerializeField]
        private bool excludeAmmo = false;
        public bool ExcludeAmmo { get { return excludeAmmo; } }

        [Tooltip("Building items will be excluded when monster dropping items or not?")]
        [SerializeField]
        private bool excludeBuilding = false;
        public bool ExcludeBuilding { get { return excludeBuilding; } }

        [Tooltip("Pet items will be excluded when monster dropping items or not?")]
        [SerializeField]
        private bool excludePet = false;
        public bool ExcludePet { get { return excludePet; } }

        [Tooltip("Socket enhancer items will be excluded when monster dropping items or not?")]
        [SerializeField]
        private bool excludeSocketEnhancer = false;
        public bool ExcludeSocketEnhancer { get { return excludeSocketEnhancer; } }

        [Tooltip("Mount items will be excluded when monster dropping items or not?")]
        [SerializeField]
        private bool excludeMount = false;
        public bool ExcludeMount { get { return excludeMount; } }

        [Tooltip("Skill items will be excluded when monster dropping items or not?")]
        [SerializeField]
        private bool excludeSkill = false;
        public bool ExcludeSkill { get { return excludeSkill; } }
        #endregion

        #region Minimap Settings
#if UNITY_EDITOR || !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
        [Category("Minimap Settings")]
        [PreviewSprite(50)]
        [SerializeField]
#if !DISABLE_ADDRESSABLES
        [AddressableAssetConversion(nameof(addressableMinimapSprite))]
#endif
        private Sprite minimapSprite;
#endif
        public Sprite MinimapSprite
        {
            private get
            {
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
                return minimapSprite;
#else
                return null;
#endif
            }
            set
            {
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
                minimapSprite = value;
#endif
            }
        }

#if !DISABLE_ADDRESSABLES
        [SerializeField]
        protected AssetReferenceSprite addressableMinimapSprite;
        public AssetReferenceSprite AddressableMinimapSprite
        {
            get
            {
                return addressableMinimapSprite;
            }
            set
            {
                addressableMinimapSprite = value;
            }
        }
#endif

        public UniTask<Sprite> GetMinimapSprite()
        {
#if !DISABLE_ADDRESSABLES
            return AddressableMinimapSprite.GetOrLoadObjectAsyncOrUseAsset(MinimapSprite);
#else
            return UniTask.FromResult(MinimapSprite);
#endif
        }

#if UNITY_EDITOR || !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
        [PreviewSprite(50)]
        [SerializeField]
#if !DISABLE_ADDRESSABLES
        [AddressableAssetConversion(nameof(addressableMinimapSprite2))]
#endif
        private Sprite minimapSprite2;
#endif
        public Sprite MinimapSprite2
        {
            private get
            {
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
                return minimapSprite2;
#else
                return null;
#endif
            }
            set
            {
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
                minimapSprite2 = value;
#endif
            }
        }

#if !DISABLE_ADDRESSABLES
        [SerializeField]
        protected AssetReferenceSprite addressableMinimapSprite2;
        public AssetReferenceSprite AddressableMinimapSprite2
        {
            get
            {
                return addressableMinimapSprite2;
            }
            set
            {
                addressableMinimapSprite2 = value;
            }
        }
#endif

        public UniTask<Sprite> GetMinimapSprite2()
        {
#if !DISABLE_ADDRESSABLES
            return AddressableMinimapSprite2.GetOrLoadObjectAsyncOrUseAsset(MinimapSprite2);
#else
            return UniTask.FromResult(MinimapSprite2);
#endif
        }

        [SerializeField]
        private Vector3 minimapPosition;
        public Vector3 MinimapPosition { get { return minimapPosition; } set { minimapPosition = value; } }

        [SerializeField]
        [FormerlySerializedAs("minimapBoundsSizeX")]
        private float minimapBoundsWidth;
        public float MinimapBoundsWidth { get { return minimapBoundsWidth; } set { minimapBoundsWidth = value; } }

        [SerializeField]
        [FormerlySerializedAs("minimapBoundsSizeZ")]
        private float minimapBoundsLength;
        public float MinimapBoundsLength { get { return minimapBoundsLength; } set { minimapBoundsLength = value; } }

        [SerializeField]
        private float minimapOrthographicSize;
        public float MinimapOrthographicSize { get { return minimapOrthographicSize; } set { minimapOrthographicSize = value; } }
#endregion

        public virtual bool AutoRespawnWhenDead { get { return false; } }
        public virtual bool SaveCurrentMapPosition { get { return true; } }

        public virtual void GetEnterMapPoint(IPlayerCharacterData playerCharacterData, out string mapName, out Vector3 position, out Vector3 rotation)
        {
            mapName = Id;
            position = playerCharacterData.CurrentPosition;
            rotation = playerCharacterData.CurrentRotation;
        }

        public virtual void GetRespawnPoint(IPlayerCharacterData playerCharacterData, out WarpPortalType portalType, out string mapName, out Vector3 position, out bool overrideRotation, out Vector3 rotation)
        {
            portalType = WarpPortalType.Default;
#if !DISABLE_DIFFER_MAP_RESPAWNING
            mapName = playerCharacterData.RespawnMapName;
            position = playerCharacterData.RespawnPosition;
#else
            mapName = Id;
            position = StartPosition;
#endif
            overrideRotation = false;
            rotation = Vector3.zero;
        }

        public bool IsAlly(DamageableEntity entity, EntityInfo targetEntityInfo)
        {
            if (entity is BasePlayerCharacterEntity player)
                return player.Id == targetEntityInfo.Id || IsPlayerAlly(player, targetEntityInfo);
            else if (entity is BaseMonsterCharacterEntity monster)
                return monster.Id == targetEntityInfo.Id || IsMonsterAlly(monster, targetEntityInfo);
            return false;
        }

        public bool IsEnemy(DamageableEntity entity, EntityInfo targetEntityInfo)
        {
            if (entity is BasePlayerCharacterEntity player)
                return player.Id != targetEntityInfo.Id && IsPlayerEnemy(player, targetEntityInfo);
            else if (entity is BaseMonsterCharacterEntity monster)
                return monster.Id != targetEntityInfo.Id && IsMonsterEnemy(monster, targetEntityInfo);
            return false;
        }

        protected abstract bool IsPlayerAlly(BasePlayerCharacterEntity playerCharacter, EntityInfo targetEntityInfo);
        protected abstract bool IsMonsterAlly(BaseMonsterCharacterEntity monsterCharacter, EntityInfo targetEntityInfo);
        protected abstract bool IsPlayerEnemy(BasePlayerCharacterEntity playerCharacter, EntityInfo targetEntityInfo);
        protected abstract bool IsMonsterEnemy(BaseMonsterCharacterEntity monsterCharacter, EntityInfo targetEntityInfo);

        public bool ExcludeItemFromDropping(BaseItem item)
        {
            if (item == null)
                return false;
            if (item.DeadDropProtected)
                return true;
            if (ExcludeItems.Count > 0 && ExcludeItems.Contains(item))
                return true;
            if (ExcludeAmmoTypes.Count > 0 && item.IsAmmo() && ExcludeAmmoTypes.Contains((item as IAmmoItem).AmmoType))
                return true;
            if (ExcludeArmorTypes.Count > 0 && item.IsArmor() && ExcludeArmorTypes.Contains((item as IArmorItem).ArmorType))
                return true;
            if (ExcludeWeaponTypes.Count > 0 && item.IsWeapon() && ExcludeWeaponTypes.Contains((item as IWeaponItem).WeaponType))
                return true;
            if (ExcludeJunk && item.IsJunk())
                return true;
            if (ExcludeArmor && item.IsArmor())
                return true;
            if (ExcludeShield && item.IsShield())
                return true;
            if (ExcludeWeapon && item.IsWeapon())
                return true;
            if (ExcludePotion && item.IsPotion())
                return true;
            if (ExcludeAmmo && item.IsAmmo())
                return true;
            if (ExcludeBuilding && item.IsBuilding())
                return true;
            if (ExcludePet && item.IsPet())
                return true;
            if (ExcludeSocketEnhancer && item.IsSocketEnhancer())
                return true;
            if (ExcludeMount && item.IsMount())
                return true;
            if (ExcludeSkill && item.IsSkill())
                return true;
            return false;
        }

        /// <summary>
        /// It will write map info data by `BaseGameNetworkManager` at server in `SendMapInfo`, send only data which affect client's controlling
        /// </summary>
        /// <param name="writer"></param>
        public virtual void Serialize(NetDataWriter writer)
        {
            writer.Put(enablePkRules);
            writer.Put(disableDealing);
            writer.Put(disableDueling);
            writer.Put(disableVending);
        }

        /// <summary>
        /// It will read map info data by `BaseGameNetworkManager` at client
        /// </summary>
        /// <param name="reader"></param>
        public virtual void Deserialize(NetDataReader reader)
        {
            enablePkRules = reader.GetBool();
            disableDealing = reader.GetBool();
            disableDueling = reader.GetBool();
            disableVending = reader.GetBool();
        }
    }
}
