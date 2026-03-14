using Insthync.UnityEditorUtils;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace MultiplayerARPG
{
    public enum WeaponItemEquipType : byte
    {
        MainHandOnly,
        DualWieldable,
        TwoHand,
        OffHandOnly,
    }

    public enum DualWieldRestriction : byte
    {
        None,
        MainHandRestricted,
        OffHandRestricted,
    }

    [CreateAssetMenu(fileName = GameDataMenuConsts.WEAPON_TYPE_FILE, menuName = GameDataMenuConsts.WEAPON_TYPE_MENU, order = GameDataMenuConsts.WEAPON_TYPE_ORDER)]
    public partial class WeaponType : BaseGameData
    {
        [Category("Weapon Type Settings")]
        [SerializeField]
        private WeaponItemEquipType equipType = WeaponItemEquipType.MainHandOnly;
        public WeaponItemEquipType EquipType { get { return equipType; } }

        [SerializeField]
        private DualWieldRestriction dualWieldRestriction = DualWieldRestriction.None;
        public DualWieldRestriction DualWieldRestriction { get { return dualWieldRestriction; } }

        [SerializeField]
        [Tooltip("Example: If you want to make ASR to be equippable on 1st and 2nd weapon sets, set this to [0, 1].\r\nIf you want to make Pistol to be equippable on 3rd weapon set, set this to [2]. \r\nEach weapon set contains slots for right-hand and left-hand. Useful for shooter game, for an RPG games, set it to be empty.")]
        [Range(0, 15)]
        private List<byte> equippableSetIndexes = new List<byte>();
        public List<byte> EquippableSetIndexes { get { return equippableSetIndexes; } }

        [SerializeField]
        private DamageInfo damageInfo = new DamageInfo();
        public DamageInfo DamageInfo { get { return damageInfo; } }

        [SerializeField]
        private DamageEffectivenessAttribute[] effectivenessAttributes = new DamageEffectivenessAttribute[0];

        [Category("Ammo Settings")]
        [Tooltip("Require Ammo, Leave it to null when it is not required")]
        [FormerlySerializedAs("requireAmmoType")]
        [SerializeField]
        private AmmoType ammoType = null;
        public AmmoType AmmoType { get { return ammoType; } }

        [System.NonSerialized]
        private Dictionary<Attribute, float> _cacheEffectivenessAttributes = null;
        public Dictionary<Attribute, float> CacheEffectivenessAttributes
        {
            get
            {
                if (_cacheEffectivenessAttributes == null)
                {
                    _cacheEffectivenessAttributes = new Dictionary<Attribute, float>();
                    GameDataHelpers.CombineDamageEffectivenessAttributes(effectivenessAttributes, _cacheEffectivenessAttributes);
                }
                return _cacheEffectivenessAttributes;
            }
        }

        public WeaponType GenerateDefaultWeaponType()
        {
            name = GameDataConst.UNKNOW_WEAPON_TYPE_ID;
            defaultTitle = GameDataConst.UNKNOW_WEAPON_TYPE_TITLE;
            return this;
        }

        public override void PrepareRelatesData()
        {
            base.PrepareRelatesData();
            DamageInfo.PrepareRelatesData();
            GameInstance.AddAmmoTypes(AmmoType);
        }
    }
}