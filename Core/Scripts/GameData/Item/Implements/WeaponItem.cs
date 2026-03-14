using Insthync.UnityEditorUtils;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MultiplayerARPG
{
    [CreateAssetMenu(fileName = GameDataMenuConsts.WEAPON_ITEM_FILE, menuName = GameDataMenuConsts.WEAPON_ITEM_MENU, order = GameDataMenuConsts.WEAPON_ITEM_ORDER)]
    public partial class WeaponItem : BaseEquipmentItem, IWeaponItem
    {
        public override string TypeTitle
        {
            get { return WeaponType.Title; }
        }

        public override ItemType ItemType
        {
            get { return ItemType.Weapon; }
        }

        [Category("In-Scene Objects/Appearance")]
        [SerializeField]
        private EquipmentModel[] offHandEquipmentModels = new EquipmentModel[0];
        public EquipmentModel[] OffHandEquipmentModels
        {
            get { return offHandEquipmentModels; }
            set { offHandEquipmentModels = value; }
        }

        [SerializeField]
        private EquipmentModel[] sheathModels = new EquipmentModel[0];
        public EquipmentModel[] SheathModels
        {
            get { return sheathModels; }
            set { sheathModels = value; }
        }

        [SerializeField]
        private EquipmentModel[] offHandSheathModels = new EquipmentModel[0];
        public EquipmentModel[] OffHandSheathModels
        {
            get { return offHandSheathModels; }
            set { offHandSheathModels = value; }
        }

        [Category("Equipment Settings")]
        [Header("Weapon Settings")]
        [SerializeField]
        [Tooltip("Weapon type data")]
        private WeaponType weaponType = null;
        public WeaponType WeaponType
        {
            get
            {
                if (weaponType == null && GameInstance.Singleton != null)
                    weaponType = GameInstance.Singleton.DefaultWeaponType;
                return weaponType;
            }
            set { weaponType = value; }
        }
#if UNITY_EDITOR
        [InspectorButton(nameof(CreateWeaponType))]
        public bool createWeaponType;
#endif

        [SerializeField]
        [Tooltip("This value will being used by character model's animator to set its `WeaponType` value")]
        private bool doRecoilingAsAttackAnimation = false;
        public bool DoRecoilingAsAttackAnimation
        {
            get { return doRecoilingAsAttackAnimation; }
            set { doRecoilingAsAttackAnimation = value; }
        }

        [SerializeField]
        [Tooltip("Damange amount which will be used when attacking characters, buildings and so on")]
        private DamageIncremental damageAmount = default;
        public DamageIncremental DamageAmount
        {
            get { return damageAmount; }
        }

        [SerializeField]
        [Tooltip("Damage amount which will be used when attacking harvestable entities")]
        private IncrementalMinMaxFloat harvestDamageAmount = default;
        public IncrementalMinMaxFloat HarvestDamageAmount
        {
            get { return harvestDamageAmount; }
        }

        [SerializeField]
        [Tooltip("This will be multiplied with character's movement speed while equipped this weapon")]
        private float moveSpeedRateWhileEquipped = 1f;
        public float MoveSpeedRateWhileEquipped
        {
            get { return moveSpeedRateWhileEquipped; }
        }

        [SerializeField]
        [Tooltip("This will be multiplied with character's movement speed while reloading this weapon")]
        private float moveSpeedRateWhileReloading = 1f;
        public float MoveSpeedRateWhileReloading
        {
            get { return moveSpeedRateWhileReloading; }
        }

        [SerializeField]
        [Tooltip("This will be multiplied with character's movement speed while charging this weapon")]
        private float moveSpeedRateWhileCharging = 1f;
        public float MoveSpeedRateWhileCharging
        {
            get { return moveSpeedRateWhileCharging; }
        }

        [SerializeField]
        [Tooltip("This will be multiplied with character's movement speed while attacking with this weapon")]
        private float moveSpeedRateWhileAttacking = 0f;
        public float MoveSpeedRateWhileAttacking
        {
            get { return moveSpeedRateWhileAttacking; }
        }

        [SerializeField]
        private MovementRestriction movementRestrictionWhileReloading = default;
        public MovementRestriction MovementRestrictionWhileReloading
        {
            get { return movementRestrictionWhileReloading; }
        }

        [SerializeField]
        private MovementRestriction movementRestrictionWhileCharging = default;
        public MovementRestriction MovementRestrictionWhileCharging
        {
            get { return movementRestrictionWhileCharging; }
        }

        [SerializeField]
        private MovementRestriction movementRestrictionWhileAttacking = default;
        public MovementRestriction MovementRestrictionWhileAttacking
        {
            get { return movementRestrictionWhileAttacking; }
        }

        [SerializeField]
        private ActionRestriction attackRestriction = new ActionRestriction()
        {
            restrictedWhileCrawlMoving = true,
        };
        public ActionRestriction AttackRestriction
        {
            get { return attackRestriction; }
        }

        [SerializeField]
        private ActionRestriction reloadRestriction = new ActionRestriction()
        {
            restrictedWhileCrawlMoving = true,
        };
        public ActionRestriction ReloadRestriction
        {
            get { return reloadRestriction; }
        }

        [SerializeField]
        [Tooltip("You can set ammo items into this list to use it as weapon instead of the one which setup on weapon type's require ammo type\r\nThis setting is useful for shooter games which can have the same type of weapon (eg. machine-gun for 20 guns) but can be reloaded by differences ammo items")]
        private BaseItem[] ammoItems = new BaseItem[0];
        [System.NonSerialized]
        private HashSet<int> _cacheAmmoItemIds = null;
        public HashSet<int> AmmoItemIds
        {
            get
            {
                if (_cacheAmmoItemIds == null)
                {
                    _cacheAmmoItemIds = new HashSet<int>();
                    for (int i = 0; i < ammoItems.Length; ++i)
                    {
                        if (ammoItems[i] == null)
                            continue;
                        _cacheAmmoItemIds.Add(ammoItems[i].DataId);
                    }
                }
                return _cacheAmmoItemIds;
            }
        }

        [SerializeField]
        [Tooltip("How many ammo can store in the gun's magazine")]
        private int ammoCapacity = 0;
        public int AmmoCapacity
        {
            get { return ammoCapacity; }
        }

        [SerializeField]
        [Tooltip("If this is `TRUE`, ammo capacity will not overrided by ammo's setting")]
        private bool noAmmoCapacityOverriding = false;
        public bool NoAmmoCapacityOverriding
        {
            get { return noAmmoCapacityOverriding; }
        }

        [SerializeField]
        [Tooltip("If this is `TRUE`, ammo data ID will not being changed by ammo item's data ID")]
        private bool noAmmoDataIdChange = false;
        public bool NoAmmoDataIdChange
        {
            get { return noAmmoDataIdChange; }
        }

        [HideInInspector]
        [SerializeField]
        private BaseWeaponAbility weaponAbility = null;

        [SerializeField]
        private BaseWeaponAbility[] weaponAbilities = new BaseWeaponAbility[0];

        public BaseWeaponAbility[] WeaponAbilities
        {
            get { return weaponAbilities; }
        }

        [SerializeField]
        private CrosshairSetting crosshairSetting = new CrosshairSetting()
        {
            expandPerFrame = 3f,
            shrinkPerFrame = 8f,
            minSpread = 10f,
            maxSpread = 50f,
            addSpreadWhileAttackAndMoving = 20f,
        };
        public CrosshairSetting CrosshairSetting
        {
            get { return crosshairSetting; }
        }

        [HideInInspector]
        [SerializeField]
        private AudioClip launchClip = null;

        [HideInInspector]
        [SerializeField]
        private AudioClip[] launchClips = new AudioClip[0];

        [SerializeField]
        private AudioClipWithVolumeSettings[] launchClipSettings = new AudioClipWithVolumeSettings[0];

        public AudioClipWithVolumeSettings LaunchClip
        {
            get
            {
                if (launchClipSettings != null && launchClipSettings.Length > 0)
                    return launchClipSettings[Random.Range(0, launchClipSettings.Length - 1)];
                return null;
            }
        }

#if UNITY_EDITOR || !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
        [HideInInspector]
        [SerializeField]
        private AudioClip reloadClip = null;

        [HideInInspector]
        [SerializeField]
        private AudioClip[] reloadClips = new AudioClip[0];
#endif

        [SerializeField]
        private AudioClipWithVolumeSettings[] reloadClipSettings = new AudioClipWithVolumeSettings[0];

        public AudioClipWithVolumeSettings ReloadClip
        {
            get
            {
                if (reloadClipSettings != null && reloadClipSettings.Length > 0)
                    return reloadClipSettings[Random.Range(0, reloadClipSettings.Length - 1)];
                return null;
            }
        }

        [HideInInspector]
        [SerializeField]
        private AudioClip reloadedClip = null;

        [HideInInspector]
        [SerializeField]
        private AudioClip[] reloadedClips = new AudioClip[0];

        [SerializeField]
        private AudioClipWithVolumeSettings[] reloadedClipSettings = new AudioClipWithVolumeSettings[0];

        public AudioClipWithVolumeSettings ReloadedClip
        {
            get
            {
                if (reloadedClipSettings != null && reloadedClipSettings.Length > 0)
                    return reloadedClipSettings[Random.Range(0, reloadedClipSettings.Length - 1)];
                return null;
            }
        }

        [HideInInspector]
        [SerializeField]
        private AudioClip emptyClip = null;

        [HideInInspector]
        [SerializeField]
        private AudioClip[] emptyClips = new AudioClip[0];

        [SerializeField]
        private AudioClipWithVolumeSettings[] emptyClipSettings = new AudioClipWithVolumeSettings[0];

        public AudioClipWithVolumeSettings EmptyClip
        {
            get
            {
                if (emptyClipSettings != null && emptyClipSettings.Length > 0)
                    return emptyClipSettings[Random.Range(0, emptyClipSettings.Length - 1)];
                return null;
            }
        }

        [SerializeField]
        [Tooltip("How is its firing")]
        private FireType fireType = FireType.SingleFire;
        public FireType FireType
        {
            get { return fireType; }
        }

        [SerializeField]
        [Tooltip("If this value > 0, it will fire by duration which being calculated by this value, default duration calculation formula is `60f / rate of fire`")]
        private float rateOfFire;
        public float RateOfFire
        {
            get { return rateOfFire; }
        }

        [SerializeField]
        [Tooltip("If this value > 0, it will reload by using this duration, NOT by animation length")]
        private float reloadDuration;
        public float ReloadDuration
        {
            get { return reloadDuration; }
        }

        [SerializeField]
        [Tooltip("If this is <= 0, it will fill full to amount of capacity, if > 0 it will fill by this value, made this for shotgun which will fill only 1 each reloading")]
        private int maxAmmoEachReload;
        public int MaxAmmoEachReload
        {
            get { return maxAmmoEachReload; }
        }

        [SerializeField]
        [FormerlySerializedAs("fireStagger")]
        [Tooltip("Random spread from aiming position, then when shoot actual shot position will be {aim position} + {randomed spread}")]
        private Vector2 fireSpreadRange = Vector2.zero;
        public Vector2 FireSpreadRange
        {
            get { return fireSpreadRange; }
        }

        [SerializeField]
        [Tooltip("Random spread from aiming position, then when shoot actual shot position will be {aim position} + {randomed spread}, while in FPS view mode")]
        private Vector2 fireSpreadRangeWhileFpsViewMode = Vector2.one * float.MinValue;
        public Vector2 FireSpreadRangeWhileFpsViewMode
        {
            get { return fireSpreadRangeWhileFpsViewMode; }
        }

        [SerializeField]
        [Tooltip("Random spread from aiming position, then when shoot actual shot position will be {aim position} + {randomed spread}, while in shoulder view mode")]
        private Vector2 fireSpreadRangeWhileShoulderViewMode = Vector2.one * float.MinValue;
        public Vector2 FireSpreadRangeWhileShoulderViewMode
        {
            get { return fireSpreadRangeWhileShoulderViewMode; }
        }

        [SerializeField]
        [Tooltip("Random spread from aiming position, then when shoot actual shot position will be {aim position} + {randomed spread}, while aiming")]
        private Vector2 fireSpreadRangeWhileAiming = Vector2.zero;
        public Vector2 FireSpreadRangeWhileAiming
        {
            get { return fireSpreadRangeWhileAiming; }
        }

        [SerializeField]
        [FormerlySerializedAs("fireSpread")]
        [Tooltip("Amount of bullets that will be launched when fire once, will be used for shotgun items")]
        private byte fireSpreadAmount = 0;
        public byte FireSpreadAmount
        {
            get { return fireSpreadAmount; }
        }

        [SerializeField]
        private float recoil = 1f;
        public float Recoil
        {
            get { return recoil; }
        }

        [SerializeField]
        private float recoilWhileFpsViewMode = float.MinValue;
        public float RecoilWhileFpsViewMode
        {
            get { return recoilWhileFpsViewMode; }
        }

        [SerializeField]
        private float recoilWhileShoulderViewMode = float.MinValue;
        public float RecoilWhileShoulderViewMode
        {
            get { return recoilWhileShoulderViewMode; }
        }

        [SerializeField]
        private float recoilWhileAiming = 1;
        public float RecoilWhileAiming
        {
            get { return recoilWhileAiming; }
        }

        [SerializeField]
        [Tooltip("Minimum charge duration to attack")]
        private float chargeDuration = 0;
        public float ChargeDuration
        {
            get { return chargeDuration; }
        }

        [SerializeField]
        [Tooltip("If this is `TRUE`, character's item will be destroyed after fired, will be used for grenade items")]
        private bool destroyImmediatelyAfterFired = false;
        public bool DestroyImmediatelyAfterFired
        {
            get { return destroyImmediatelyAfterFired; }
        }

        public override bool Validate()
        {
            bool hasChanges = false;
            if (weaponAbility != null && (weaponAbilities == null || weaponAbilities.Length == 0))
            {
                weaponAbilities = new List<BaseWeaponAbility>()
                {
                    weaponAbility,
                }.ToArray();
                hasChanges = true;
            }
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
            if (MigrateAudioClips(ref launchClip, ref launchClips, ref launchClipSettings))
                hasChanges = true;
            if (MigrateAudioClips(ref reloadClip, ref reloadClips, ref reloadClipSettings))
                hasChanges = true;
            if (MigrateAudioClips(ref reloadedClip, ref reloadedClips, ref reloadedClipSettings))
                hasChanges = true;
            if (MigrateAudioClips(ref emptyClip, ref emptyClips, ref emptyClipSettings))
                hasChanges = true;
#endif
            return hasChanges || base.Validate();
        }

#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
        private bool MigrateAudioClips(ref AudioClip singleClip, ref AudioClip[] multipleClips, ref AudioClipWithVolumeSettings[] destinationSettings)
        {
            if (singleClip == null && (multipleClips == null || multipleClips.Length == 0))
                return false;

            bool hasChanges = false;

            List<AudioClip> clips = new List<AudioClip>();
            if (multipleClips != null && multipleClips.Length > 0)
            {
                clips.AddRange(multipleClips);
                multipleClips = null;
                hasChanges = true;
            }
            if (singleClip != null && !clips.Contains(singleClip))
            {
                clips.Add(singleClip);
                singleClip = null;
                hasChanges = true;
            }
            if (!hasChanges)
                return false;

            List<AudioClipWithVolumeSettings> clipSettings = new List<AudioClipWithVolumeSettings>();
            if (destinationSettings != null && destinationSettings.Length > 0)
                clipSettings.AddRange(destinationSettings);
            for (int i = 0; i < clips.Count; ++i)
            {
                clipSettings.Add(new AudioClipWithVolumeSettings()
                {
                    audioClip = clips[i],
                    minRandomVolume = 1f,
                    maxRandomVolume = 1f,
                });
            }
            destinationSettings = clipSettings.ToArray();
            return true;
        }
#endif

        public override void PrepareRelatesData()
        {
            base.PrepareRelatesData();
            GameInstance.AddDamageElements(DamageAmount);
            GameInstance.AddWeaponTypes(WeaponType);
            // Data migration
            GameInstance.MigrateEquipmentEntities(OffHandEquipmentModels);
        }

#if UNITY_EDITOR
        public void CreateWeaponType()
        {
            string path = AssetDatabase.GetAssetPath(this);
            List<string> splitedPaths = new List<string>(path.Split('/'));
            splitedPaths.RemoveAt(splitedPaths.Count - 1);
            path = string.Join('/', splitedPaths);
            string fileName = $"{name}_TYPE";
            path = EditorUtility.SaveFilePanel("Save data to ...", path, fileName, "asset");

            WeaponType weaponType = CreateInstance<WeaponType>();
            weaponType.DefaultTitle = DefaultTitle;
            weaponType.LanguageSpecificTitles = LanguageSpecificTitles;
            weaponType.DefaultDescription = DefaultDescription;
            weaponType.LanguageSpecificDescriptions = LanguageSpecificDescriptions;
            weaponType.Category = Category;
            weaponType.Icon = Icon;
#if !DISABLE_ADDRESSABLES
            weaponType.AddressableIcon = AddressableIcon;
#endif

            path = path.Substring(path.IndexOf("Assets"));
            AssetDatabase.DeleteAsset(path);
            AssetDatabase.CreateAsset(weaponType, path);

            this.weaponType = weaponType;
            EditorUtility.SetDirty(this);
        }
#endif
        }
    }
