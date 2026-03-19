using Insthync.UnityEditorUtils;
using LiteNetLibManager;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    public partial class BaseCharacterEntity
    {
        #region Sync data
        [Category("Sync Fields")]
        [SerializeField]
        protected SyncFieldString id = new SyncFieldString();
        [SerializeField]
        protected SyncFieldInt level = new SyncFieldInt();
        [SerializeField]
        protected SyncFieldInt exp = new SyncFieldInt();
        [SerializeField]
        protected SyncFieldInt currentMp = new SyncFieldInt();
        [SerializeField]
        protected SyncFieldInt currentStamina = new SyncFieldInt();
        [SerializeField]
        protected SyncFieldInt currentFood = new SyncFieldInt();
        [SerializeField]
        protected SyncFieldInt currentWater = new SyncFieldInt();
        [SerializeField]
        protected SyncFieldByte equipWeaponSet = new SyncFieldByte();
        [SerializeField]
        protected SyncFieldBool isWeaponsSheathed = new SyncFieldBool();
        [SerializeField]
        protected SyncFieldUShort pitch = new SyncFieldUShort();
        [SerializeField]
        protected SyncFieldVector3 lookPosition = new SyncFieldVector3();
        [SerializeField]
        protected SyncFieldAimPosition aimPosition = new SyncFieldAimPosition();
        [SerializeField]
        protected SyncFieldUInt targetEntityId = new SyncFieldUInt();
        [SerializeField]
        protected SyncFieldCharacterMount mount = new SyncFieldCharacterMount();
        [SerializeField]
        protected SyncFieldCharacterSummoner summoner = new SyncFieldCharacterSummoner();

        [Category(101, "Sync Lists", false)]
        [SerializeField]
        protected SyncListEquipWeapons selectableWeaponSets = new SyncListEquipWeapons();
        [SerializeField]
        protected SyncListCharacterAttribute attributes = new SyncListCharacterAttribute();
        [SerializeField]
        protected SyncListCharacterSkill skills = new SyncListCharacterSkill();
        [SerializeField]
        protected SyncListCharacterSkillUsage skillUsages = new SyncListCharacterSkillUsage();
        [SerializeField]
        protected SyncListCharacterBuff buffs = new SyncListCharacterBuff();
        [SerializeField]
        protected SyncListCharacterItem equipItems = new SyncListCharacterItem();
        [SerializeField]
        protected SyncListCharacterItem nonEquipItems = new SyncListCharacterItem();
        [SerializeField]
        protected SyncListCharacterSummon summons = new SyncListCharacterSummon();
        #endregion

        #region Fields/Interface implementation
        public string Id { get { return id.Value; } set { id.Value = value; } }
        public string CharacterName { get { return syncTitle.Value; } set { syncTitle.Value = value; } }
        public int Level { get { return level.Value; } set { level.Value = value; } }
        public int Exp { get { return exp.Value; } set { exp.Value = value; } }
        public int CurrentMp { get { return currentMp.Value; } set { currentMp.Value = value; } }
        public int CurrentStamina { get { return currentStamina.Value; } set { currentStamina.Value = value; } }
        public int CurrentFood { get { return currentFood.Value; } set { currentFood.Value = value; } }
        public int CurrentWater { get { return currentWater.Value; } set { currentWater.Value = value; } }
        public virtual int IconDataId { get; set; }
        public virtual int FrameDataId { get; set; }
        public virtual int BackgroundDataId { get; set; }
        public virtual int TitleDataId { get; set; }
        public virtual int FactionId { get; set; }
        public virtual int Reputation { get; set; }
        public byte EquipWeaponSet
        {
            get { return equipWeaponSet.Value; }
            set { equipWeaponSet.Value = value; }
        }
        public bool IsWeaponsSheathed
        {
            get { return isWeaponsSheathed.Value; }
            set { isWeaponsSheathed.Value = value; }
        }
        public EquipWeapons EquipWeapons
        {
            get
            {
                if (EquipWeaponSet < SelectableWeaponSets.Count)
                    return SelectableWeaponSets[EquipWeaponSet];
                return new EquipWeapons();
            }
            set
            {
                this.FillWeaponSetsIfNeeded(EquipWeaponSet);
                SelectableWeaponSets[EquipWeaponSet] = value;
            }
        }
        public float Pitch
        {
            get
            {
                return (float)pitch.Value * 0.0001f * 360f;
            }
            set
            {
                pitch.Value = (ushort)(value / 360f * 10000);
            }
        }
        public Vector3 LookPosition
        {
            get
            {
                return lookPosition.Value;
            }
            set
            {
                lookPosition.Value = value;
            }
        }
        public AimPosition AimPosition
        {
            get
            {
                return aimPosition.Value;
            }
            set
            {
                aimPosition.Value = value;
            }
        }

        public CharacterMount Mount
        {
            get
            {
                return mount.Value;
            }
            set
            {
                mount.Value = value;
            }
        }

        public CharacterSummoner Summoner
        {
            get { return summoner.Value; }
            set { summoner.Value = value; }
        }

        private BaseCharacterEntity _summonerEntity;
        public BaseCharacterEntity SummonerEntity
        {
            get
            {
                if (IsSummoned && _summonerEntity == null)
                {
                    LiteNetLibIdentity identity;
                    if (Manager.Assets.TryGetSpawnedObject(Summoner.objectId, out identity))
                        _summonerEntity = identity.GetComponent<BaseCharacterEntity>();
                }
                return _summonerEntity;
            }
            protected set
            {
                _summonerEntity = value;
                if (IsServer)
                {
                    Summoner = new CharacterSummoner()
                    {
                        type = Summoner.type,
                        objectId = _summonerEntity != null ? _summonerEntity.ObjectId : 0,
                    };
                }
            }
        }

        public SummonType SummonType
        {
            get { return Summoner.type; }
            protected set
            {
                if (IsServer)
                {
                    Summoner = new CharacterSummoner()
                    {
                        type = value,
                        objectId = Summoner.objectId,
                    };
                }
            }
        }

        public bool IsSummoned
        {
            get { return SummonType != SummonType.None; }
        }

        public bool IsSummonedAndSummonerExisted
        {
            get { return IsSummoned && SummonerEntity != null; }
        }

        public IList<EquipWeapons> SelectableWeaponSets
        {
            get { return selectableWeaponSets; }
            set
            {
                selectableWeaponSets.Clear();
                selectableWeaponSets.AddRange(value);
            }
        }

        public IList<CharacterAttribute> Attributes
        {
            get { return attributes; }
            set
            {
                attributes.Clear();
                attributes.AddRange(value);
            }
        }

        public IList<CharacterSkill> Skills
        {
            get { return skills; }
            set
            {
                skills.Clear();
                skills.AddRange(value);
            }
        }

        public IList<CharacterSkillUsage> SkillUsages
        {
            get { return skillUsages; }
            set
            {
                skillUsages.Clear();
                skillUsages.AddRange(value);
            }
        }

        public IList<CharacterBuff> Buffs
        {
            get { return buffs; }
            set
            {
                buffs.Clear();
                buffs.AddRange(value);
            }
        }

        public IList<CharacterItem> EquipItems
        {
            get { return equipItems; }
            set
            {
                // Validate items
                HashSet<string> equipPositions = new HashSet<string>();
                IArmorItem tempArmor;
                string tempEquipPosition;
                for (int i = value.Count - 1; i >= 0; --i)
                {
                    // Remove empty slot
                    if (value[i].IsEmptySlot())
                    {
                        value.RemoveAt(i);
                        continue;
                    }
                    // Remove non-armor item
                    tempArmor = value[i].GetArmorItem();
                    if (tempArmor == null)
                    {
                        value.RemoveAt(i);
                        continue;
                    }
                    tempEquipPosition = GetEquipPosition(tempArmor.GetEquipPosition(), value[i].equipSlotIndex);
                    if (equipPositions.Contains(tempEquipPosition))
                    {
                        value.RemoveAt(i);
                        continue;
                    }
                    equipPositions.Add(tempEquipPosition);
                }
                equipItems.Clear();
                equipItems.AddRange(value);
            }
        }

        public IList<CharacterItem> NonEquipItems
        {
            get { return nonEquipItems; }
            set
            {
                nonEquipItems.Clear();
                nonEquipItems.AddRange(value);
            }
        }

        public IList<CharacterSummon> Summons
        {
            get { return summons; }
            set
            {
                summons.Clear();
                summons.AddRange(value);
            }
        }
        #endregion

        #region Network setup functions
        protected override void SetupNetElements()
        {
            base.SetupNetElements();
            // Sync fields
            id.syncMode = LiteNetLibSyncFieldMode.ServerToClients;
            id.redundancyCount = 0;
            level.syncMode = LiteNetLibSyncFieldMode.ServerToClients;
            exp.syncMode = LiteNetLibSyncFieldMode.ServerToClients;
            isInvincible.syncMode = LiteNetLibSyncFieldMode.ServerToClients;
            currentHp.syncMode = LiteNetLibSyncFieldMode.ServerToClients;
            currentMp.syncMode = LiteNetLibSyncFieldMode.ServerToClients;
            currentStamina.syncMode = LiteNetLibSyncFieldMode.ServerToClients;
            currentFood.syncMode = LiteNetLibSyncFieldMode.ServerToClients;
            currentWater.syncMode = LiteNetLibSyncFieldMode.ServerToClients;
            equipWeaponSet.syncMode = LiteNetLibSyncFieldMode.ServerToClients;
            isWeaponsSheathed.syncMode = LiteNetLibSyncFieldMode.ClientMulticast;
            pitch.syncMode = LiteNetLibSyncFieldMode.ServerToClients;
            aimPosition.syncMode = LiteNetLibSyncFieldMode.ClientMulticast;
            targetEntityId.syncMode = LiteNetLibSyncFieldMode.ServerToClients;
            mount.syncMode = LiteNetLibSyncFieldMode.ServerToOwnerClient;
            summoner.syncMode = LiteNetLibSyncFieldMode.ServerToClients;
            // Sync lists
            selectableWeaponSets.forOwnerOnly = false;
            attributes.forOwnerOnly = false;
            skills.forOwnerOnly = false;
            skillUsages.forOwnerOnly = true;
            buffs.forOwnerOnly = false;
            equipItems.forOwnerOnly = false;
            nonEquipItems.forOwnerOnly = true;
            summons.forOwnerOnly = true;

            // On data changed events
            syncTitle.onChange += OnCharacterNameChange;
            id.onChange += OnIdChange;
            level.onChange += OnLevelChange;
            exp.onChange += OnExpChange;
            isInvincible.onChange += OnIsInvincibleChange;
            currentMp.onChange += OnCurrentMpChange;
            currentStamina.onChange += OnCurrentStaminaChange;
            currentFood.onChange += OnCurrentFoodChange;
            currentWater.onChange += OnCurrentWaterChange;
            equipWeaponSet.onChange += OnEquipWeaponSetChange;
            isWeaponsSheathed.onChange += OnIsWeaponsSheathedChange;
            pitch.onChange += OnPitchChange;
            lookPosition.onChange += OnLookPositionChange;
            aimPosition.onChange += OnAimPositionChange;
            targetEntityId.onChange += OnTargetEntityIdChange;
            mount.onChange += OnMountChange;
            summoner.onChange += OnSummonerChange;
            // On list changed events
            selectableWeaponSets.onOperation += OnSelectableWeaponSetsOperation;
            attributes.onOperation += OnAttributesOperation;
            skills.onOperation += OnSkillsOperation;
            skillUsages.onOperation += OnSkillUsagesOperation;
            buffs.onOperation += OnBuffsOperation;
            equipItems.onOperation += OnEquipItemsOperation;
            nonEquipItems.onOperation += OnNonEquipItemsOperation;
            summons.onOperation += OnSummonsOperation;
        }

        protected override void EntityOnDestroy()
        {
            base.EntityOnDestroy();
            // On data changed events
            id.onChange -= OnIdChange;
            syncTitle.onChange -= OnCharacterNameChange;
            level.onChange -= OnLevelChange;
            exp.onChange -= OnExpChange;
            isInvincible.onChange -= OnIsInvincibleChange;
            currentMp.onChange -= OnCurrentMpChange;
            currentStamina.onChange -= OnCurrentStaminaChange;
            currentFood.onChange -= OnCurrentFoodChange;
            currentWater.onChange -= OnCurrentWaterChange;
            equipWeaponSet.onChange -= OnEquipWeaponSetChange;
            isWeaponsSheathed.onChange -= OnIsWeaponsSheathedChange;
            pitch.onChange -= OnPitchChange;
            lookPosition.onChange -= OnLookPositionChange;
            aimPosition.onChange -= OnAimPositionChange;
            targetEntityId.onChange -= OnTargetEntityIdChange;
            mount.onChange -= OnMountChange;
            summoner.onChange -= OnSummonerChange;
            // On list changed events
            selectableWeaponSets.onOperation -= OnSelectableWeaponSetsOperation;
            attributes.onOperation -= OnAttributesOperation;
            skills.onOperation -= OnSkillsOperation;
            skillUsages.onOperation -= OnSkillUsagesOperation;
            buffs.onOperation -= OnBuffsOperation;
            equipItems.onOperation -= OnEquipItemsOperation;
            nonEquipItems.onOperation -= OnNonEquipItemsOperation;
            summons.onOperation -= OnSummonsOperation;

            if (UICharacterEntity != null)
                Destroy(UICharacterEntity.gameObject);
        }
        #endregion

        #region Sync data changed callback
        private void OnIdChange(bool isInitial, string oldId, string id)
        {
            if (onIdChange != null)
                onIdChange.Invoke(id);
        }

        private void OnCharacterNameChange(bool isInitial, string oldCharacterName, string characterName)
        {
            if (onCharacterNameChange != null)
                onCharacterNameChange.Invoke(characterName);
        }

        private void OnLevelChange(bool isInitial, int oldLevel, int level)
        {
            IsRecaching = true;
            if (onLevelChange != null)
                onLevelChange.Invoke(level);
        }

        private void OnExpChange(bool isInitial, int oldExp, int exp)
        {
            if (onExpChange != null)
                onExpChange.Invoke(exp);
        }

        private void OnIsInvincibleChange(bool isInitial, bool oldIsInvincible, bool isInvincible)
        {
            if (onIsInvincibleChange != null)
                onIsInvincibleChange.Invoke(isInvincible);
        }

        private void OnCurrentMpChange(bool isInitial, int oldCurrentMp, int currentMp)
        {
            if (onCurrentMpChange != null)
                onCurrentMpChange.Invoke(currentMp);
        }

        private void OnCurrentStaminaChange(bool isInitial, int oldCurrentStamina, int currentStamina)
        {
            if (onCurrentStaminaChange != null)
                onCurrentStaminaChange.Invoke(currentStamina);
        }

        private void OnCurrentFoodChange(bool isInitial, int oldCurrentFood, int currentFood)
        {
            if (onCurrentFoodChange != null)
                onCurrentFoodChange.Invoke(currentFood);
        }

        private void OnCurrentWaterChange(bool isInitial, int oldCurrentWater, int currentWater)
        {
            if (onCurrentWaterChange != null)
                onCurrentWaterChange.Invoke(currentWater);
        }

        protected virtual void OnEquipWeaponSetChange(bool isInitial, byte oldEquipWeaponSet, byte equipWeaponSet)
        {
            MarkToUpdateAppearances();
            MarkToUpdateAmmoSim();
            IsRecaching = true;
            if (onEquipWeaponSetChange != null)
                onEquipWeaponSetChange.Invoke(equipWeaponSet);
        }

        protected virtual void OnIsWeaponsSheathedChange(bool isInitial, bool oldIsWeaponsSheathed, bool isWeaponsSheathed)
        {
            MarkToUpdateAppearances();
            IsRecaching = true;
            if (onIsWeaponsSheathedChange != null)
                onIsWeaponsSheathedChange.Invoke(isWeaponsSheathed);
        }

        private void OnPitchChange(bool isInitial, ushort oldPitch, ushort pitch)
        {
            if (onPitchChange != null)
                onPitchChange.Invoke(pitch);
        }

        private void OnLookPositionChange(bool isInitial, Vector3 oldLookPosition, Vector3 lookPosition)
        {
            if (onLookPositionChange != null)
                onLookPositionChange.Invoke(lookPosition);
        }

        private void OnAimPositionChange(bool isInitial, AimPosition oldAimPosition, AimPosition aimPosition)
        {
            if (onAimPositionChange != null)
                onAimPositionChange.Invoke(aimPosition);
        }

        private void OnTargetEntityIdChange(bool isInitial, uint oldTargetEntityId, uint targetEntityId)
        {
            if (onTargetEntityIdChange != null)
                onTargetEntityIdChange.Invoke(targetEntityId);
        }

        private void OnMountChange(bool isInitial, CharacterMount oldMount, CharacterMount mount)
        {
            // Only recache when mount type/source/level changes (not timer/HP updates)
            if (oldMount.type != mount.type || oldMount.sourceId != mount.sourceId || oldMount.level != mount.level)
                IsRecaching = true;
            if (onMountChange != null)
                onMountChange.Invoke(mount);
        }

        private void OnSummonerChange(bool isInitial, CharacterSummoner oldSummoner, CharacterSummoner mount)
        {
            // Only recache when mount type/source/level changes (not timer/HP updates)
            if (oldSummoner.type != mount.type || oldSummoner.objectId != mount.objectId)
                IsRecaching = true;
            if (onSummonerChange != null)
                onSummonerChange.Invoke(mount);
        }
        #endregion

        #region Net functions operation callback
        private void OnSelectableWeaponSetsOperation(LiteNetLibSyncListOp operation, int index, EquipWeapons oldItem, EquipWeapons newItem)
        {
            switch (operation)
            {
                case LiteNetLibSyncListOp.Set:
                case LiteNetLibSyncListOp.Dirty:
                    if (oldItem.IsDiffer(newItem, out _, out _, true, true, true, true))
                    {
                        MarkToUpdateAppearances();
                        MarkToUpdateAmmoSim();
                        IsRecaching = true;
                    }
                    else if (oldItem.rightHand.ammo != newItem.rightHand.ammo ||
                        oldItem.leftHand.ammo != newItem.leftHand.ammo)
                    {
                        MarkToUpdateAmmoSim();
                        IsRecaching = true;
                    }
                    else if (!Mathf.Approximately(oldItem.rightHand.durability, newItem.rightHand.durability) ||
                        !Mathf.Approximately(oldItem.leftHand.durability, newItem.leftHand.durability))
                    {
                        IsRecaching = true;
                    }
                    break;
                default:
                    MarkToUpdateAppearances();
                    MarkToUpdateAmmoSim();
                    IsRecaching = true;
                    break;
            }
            if (onSelectableWeaponSetsOperation != null)
                onSelectableWeaponSetsOperation.Invoke(operation, index, oldItem, newItem);
        }

        private void OnAttributesOperation(LiteNetLibSyncListOp operation, int index, CharacterAttribute oldItem, CharacterAttribute newItem)
        {
            switch (operation)
            {
                case LiteNetLibSyncListOp.Set:
                case LiteNetLibSyncListOp.Dirty:
                    if (oldItem.dataId != newItem.dataId ||
                        oldItem.amount != newItem.amount)
                        IsRecaching = true;
                    break;
                default:
                    IsRecaching = true;
                    break;
            }
            if (onAttributesOperation != null)
                onAttributesOperation.Invoke(operation, index, oldItem, newItem);
        }

        private void OnSkillsOperation(LiteNetLibSyncListOp operation, int index, CharacterSkill oldItem, CharacterSkill newItem)
        {
            switch (operation)
            {
                case LiteNetLibSyncListOp.Set:
                case LiteNetLibSyncListOp.Dirty:
                    if (oldItem.dataId != newItem.dataId ||
                        oldItem.level != newItem.level)
                        IsRecaching = true;
                    break;
                default:
                    IsRecaching = true;
                    break;
            }
            if (onSkillsOperation != null)
                onSkillsOperation.Invoke(operation, index, oldItem, newItem);
        }

        private void OnSkillUsagesOperation(LiteNetLibSyncListOp operation, int index, CharacterSkillUsage oldItem, CharacterSkillUsage newItem)
        {
            if (onSkillUsagesOperation != null)
                onSkillUsagesOperation.Invoke(operation, index, oldItem, newItem);
        }

        private void OnBuffsOperation(LiteNetLibSyncListOp operation, int index, CharacterBuff oldItem, CharacterBuff newItem)
        {
            // Update model's buffs effects
            CharacterModel.SetBuffs(buffs);
            if (FpsModel)
                FpsModel.SetBuffs(buffs);

            switch (operation)
            {
                case LiteNetLibSyncListOp.Add:
                case LiteNetLibSyncListOp.AddInitial:
                case LiteNetLibSyncListOp.Insert:
                    // Check last buff to update disallow status
                    if (buffs.Count > 0 && buffs[buffs.Count - 1].GetBuff().GetBuff().disallowMove)
                        StopMove();
                    break;
            }

            switch (operation)
            {
                case LiteNetLibSyncListOp.Set:
                case LiteNetLibSyncListOp.Dirty:
                    if (!string.Equals(oldItem.id, newItem.id) ||
                        oldItem.type != newItem.type ||
                        oldItem.dataId != newItem.dataId ||
                        oldItem.level != newItem.level)
                        IsRecaching = true;
                    break;
                default:
                    IsRecaching = true;
                    break;
            }

            if (onBuffsOperation != null)
                onBuffsOperation.Invoke(operation, index, oldItem, newItem);
        }

        private void OnEquipItemsOperation(LiteNetLibSyncListOp operation, int index, CharacterItem oldItem, CharacterItem newItem)
        {
            switch (operation)
            {
                case LiteNetLibSyncListOp.Set:
                case LiteNetLibSyncListOp.Dirty:
                    if (oldItem.IsDiffer(newItem, true, true, true, true))
                    {
                        MarkToUpdateAppearances();
                        IsRecaching = true;
                    }
                    else if (!Mathf.Approximately(oldItem.durability, newItem.durability))
                    {
                        IsRecaching = true;
                    }
                    else if (oldItem.amount != newItem.amount)
                    {
                        IsRecaching = true;
                    }
                    break;
                default:
                    MarkToUpdateAppearances();
                    IsRecaching = true;
                    break;
            }
            if (onEquipItemsOperation != null)
                onEquipItemsOperation.Invoke(operation, index, oldItem, newItem);
        }

        private void OnNonEquipItemsOperation(LiteNetLibSyncListOp operation, int index, CharacterItem oldItem, CharacterItem newItem)
        {
            switch (operation)
            {
                case LiteNetLibSyncListOp.Set:
                case LiteNetLibSyncListOp.Dirty:
                    if (oldItem.IsDiffer(newItem, true, true, true, true, checkAmount: true))
                        IsRecaching = true;
                    break;
                default:
                    IsRecaching = true;
                    break;
            }
            if (onNonEquipItemsOperation != null)
                onNonEquipItemsOperation.Invoke(operation, index, oldItem, newItem);
        }

        private void OnSummonsOperation(LiteNetLibSyncListOp operation, int index, CharacterSummon oldItem, CharacterSummon newItem)
        {
            IsRecaching = true;
            if (onSummonsOperation != null)
                onSummonsOperation.Invoke(operation, index, oldItem, newItem);
        }
        #endregion
    }
}
