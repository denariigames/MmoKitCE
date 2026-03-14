using Insthync.UnityEditorUtils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace MultiplayerARPG
{
    [CreateAssetMenu(fileName = GameDataMenuConsts.PLAYER_CHARACTER_FILE, menuName = GameDataMenuConsts.PLAYER_CHARACTER_MENU, order = GameDataMenuConsts.PLAYER_CHARACTER_ORDER)]
    public partial class PlayerCharacter : BaseCharacter
    {
        [Category(3, "Character Stats")]
        [SerializeField]
        [FormerlySerializedAs("skillLevels")]
        [ArrayElementTitle("skill")]
        private PlayerSkill[] skills = new PlayerSkill[0];

        [Category(4, "Start Items")]
        [Header("Equipped Items")]
        [SerializeField]
        private BaseItem rightHandEquipItem = null;
        public BaseItem RightHandEquipItem { get { return rightHandEquipItem; } }
        [SerializeField]
        private BaseItem leftHandEquipItem = null;
        public BaseItem LeftHandEquipItem { get { return leftHandEquipItem; } }
        [SerializeField]
        private BaseItem[] armorItems = new BaseItem[0];
        public BaseItem[] ArmorItems { get { return armorItems; } }
        [Header("Items in Inventory")]
        [SerializeField]
        [Tooltip("Items that will be added to character when create new character")]
        [ArrayElementTitle("item")]
        private ItemAmount[] startItems = new ItemAmount[0];
        public ItemAmount[] StartItems { get { return startItems; } }

        [Category(6, "Start Map")]
        [SerializeField]
        private BaseMapInfo startMap = null;

        [Tooltip("If this is `TRUE` it will uses `overrideStartPosition` as start position, instead of `startMap` -> `startPosition`")]
        [SerializeField]
        private bool useOverrideStartPosition = false;
        [SerializeField]
        private Vector3 overrideStartPosition = Vector3.zero;

        [Tooltip("If this is `TRUE` it will uses `overrideStartRotation` as start position, instead of `startMap` -> `startRotation`")]
        [SerializeField]
        private bool useOverrideStartRotation = false;
        [SerializeField]
        private Vector3 overrideStartRotation = Vector3.zero;

        [Tooltip("If this length is more than 1 it will find start maps which its condition is match with the character")]
        [SerializeField]
        private StartMapByCondition[] startPointsByCondition = new StartMapByCondition[0];
        [System.NonSerialized]
        private Dictionary<int, List<StartMapByCondition>> _cacheStartMapsByCondition;
        public Dictionary<int, List<StartMapByCondition>> CacheStartMapsByCondition
        {
            get
            {
                if (_cacheStartMapsByCondition == null)
                {
                    _cacheStartMapsByCondition = new Dictionary<int, List<StartMapByCondition>>();
                    int factionDataId;
                    foreach (StartMapByCondition startPointByCondition in startPointsByCondition)
                    {
                        factionDataId = 0;
                        if (startPointByCondition.forFaction != null)
                            factionDataId = startPointByCondition.forFaction.DataId;
                        if (!_cacheStartMapsByCondition.ContainsKey(factionDataId))
                            _cacheStartMapsByCondition.Add(factionDataId, new List<StartMapByCondition>());
                        _cacheStartMapsByCondition[factionDataId].Add(startPointByCondition);
                    }
                }
                return _cacheStartMapsByCondition;
            }
        }

        public BaseMapInfo StartMap
        {
            get
            {
                if (startMap == null)
                    return GameInstance.MapInfos.FirstOrDefault().Value;
                return startMap;
            }
        }

        public Vector3 StartPosition
        {
            get
            {
                return useOverrideStartPosition ? overrideStartPosition : StartMap.StartPosition;
            }
        }

        public Vector3 StartRotation
        {
            get
            {
                return useOverrideStartRotation ? overrideStartRotation : StartMap.StartRotation;
            }
        }

        public void GetStartMapAndTransform(IPlayerCharacterData playerCharacterData, out BaseMapInfo startMap, out Vector3 position, out Vector3 rotation)
        {
            startMap = StartMap;
            position = StartPosition;
            rotation = StartRotation;
            if (CacheStartMapsByCondition.Count > 0)
            {
                List<StartMapByCondition> startPoints;
                if (CacheStartMapsByCondition.TryGetValue(playerCharacterData.FactionId, out startPoints) ||
                    CacheStartMapsByCondition.TryGetValue(0, out startPoints))
                {
                    StartMapByCondition startPoint = startPoints[Random.Range(0, startPoints.Count)];
                    startMap = startPoint.StartMap;
                    position = startPoint.StartPosition;
                    rotation = startPoint.StartRotation;
                }
            }
        }

        [System.NonSerialized]
        private HashSet<int> _learnableSkillIds;
        public override HashSet<int> GetLearnableSkillDataIds()
        {
            if (_learnableSkillIds == null)
            {
                _learnableSkillIds = new HashSet<int>();
                foreach (PlayerSkill skill in skills)
                {
                    if (skill.skill == null)
                        continue;
                    _learnableSkillIds.Add(skill.skill.DataId);
                }
            }
            return _learnableSkillIds;
        }

        public override void GetSkillLevels(int level, Dictionary<BaseSkill, int> result)
        {
            result.Clear();
            if (level > 0)
                GameDataHelpers.CombineSkills(skills, result, level);
        }

        public override void PrepareRelatesData()
        {
            base.PrepareRelatesData();
            GameInstance.AddItems(armorItems);
            GameInstance.AddItems(rightHandEquipItem);
            GameInstance.AddItems(leftHandEquipItem);
            GameInstance.AddSkills(skills);
        }

        public override bool Validate()
        {
            bool hasChanges = false;
            IWeaponItem tempRightHandWeapon = null;
            IWeaponItem tempLeftHandWeapon = null;
            IShieldItem tempLeftHandShield = null;
            if (rightHandEquipItem != null)
            {
                if (rightHandEquipItem.IsWeapon())
                    tempRightHandWeapon = rightHandEquipItem as IWeaponItem;

                if (tempRightHandWeapon == null || tempRightHandWeapon.WeaponType == null)
                {
                    Debug.LogWarning("Right hand equipment is not weapon.");
                    rightHandEquipItem = null;
                    hasChanges = true;
                }
            }
            if (leftHandEquipItem != null)
            {
                if (leftHandEquipItem.IsWeapon())
                    tempLeftHandWeapon = leftHandEquipItem as IWeaponItem;
                if (leftHandEquipItem.IsShield())
                    tempLeftHandShield = leftHandEquipItem as IShieldItem;

                if ((tempLeftHandWeapon == null || tempLeftHandWeapon.WeaponType == null) && tempLeftHandShield == null)
                {
                    Debug.LogWarning("Left hand equipment is not weapon or shield.");
                    leftHandEquipItem = null;
                    hasChanges = true;
                }
                else if (tempRightHandWeapon != null)
                {
                    if (tempLeftHandShield != null && tempRightHandWeapon.GetEquipType() == WeaponItemEquipType.TwoHand)
                    {
                        Debug.LogWarning("Cannot set left hand equipment because it's equipping `TwoHand` item.");
                        leftHandEquipItem = null;
                        hasChanges = true;
                    }
                    else if (tempLeftHandWeapon != null && tempRightHandWeapon.GetEquipType() != WeaponItemEquipType.DualWieldable)
                    {
                        Debug.LogWarning("Cannot set left hand equipment because it isn't equipping `DualWieldable` item.");
                        leftHandEquipItem = null;
                        hasChanges = true;
                    }
                    else if (tempLeftHandWeapon != null && tempLeftHandWeapon.GetEquipType() == WeaponItemEquipType.OffHandOnly)
                    {
                        Debug.LogWarning("Cannot set right hand equipment because it's equipping `OffHandOnly` item.");
                        rightHandEquipItem = null;
                        hasChanges = true;
                    }
                }
                if (tempLeftHandWeapon != null &&
                    (tempLeftHandWeapon.GetEquipType() == WeaponItemEquipType.MainHandOnly ||
                    tempLeftHandWeapon.GetEquipType() == WeaponItemEquipType.TwoHand))
                {
                    Debug.LogWarning("Left hand weapon cannot be `MainHandOnly` or `TwoHand` item.");
                    leftHandEquipItem = null;
                    hasChanges = true;
                }
                if (tempRightHandWeapon != null &&
                    (tempRightHandWeapon.GetEquipType() == WeaponItemEquipType.OffHandOnly))
                {
                    Debug.LogWarning("Right hand weapon cannot be `OffHandOnly` item.");
                    rightHandEquipItem = null;
                    hasChanges = true;
                }
            }
            List<string> equipedPositions = new List<string>();
            BaseItem armorItem;
            for (int i = 0; i < armorItems.Length; ++i)
            {
                armorItem = armorItems[i];
                if (armorItem == null)
                    continue;

                if (!armorItem.IsArmor())
                {
                    // Item is not armor, so set it to NULL
                    armorItems[i] = null;
                    hasChanges = true;
                    continue;
                }

                if (equipedPositions.Contains((armorItem as IArmorItem).GetEquipPosition()))
                {
                    // Already equip armor at the position, it cannot equip same position again, So set it to NULL
                    armorItems[i] = null;
                    hasChanges = true;
                }
                else
                {
                    equipedPositions.Add((armorItem as IArmorItem).GetEquipPosition());
                }
            }
            if (skills != null && skills.Length > 0)
            {
                for (int i = 0; i < skills.Length; ++i)
                {
                    PlayerSkill skill = skills[i];
                    if (skill.skillLevel.baseAmount < skill.level)
                    {
                        skill.skillLevel.baseAmount = skill.level;
                        skill.level = 0;
                        skills[i] = skill;
                        hasChanges = true;
                    }
                }
            }
            return hasChanges || base.Validate();
        }
    }
}
