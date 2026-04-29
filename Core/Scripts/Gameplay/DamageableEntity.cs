using Cysharp.Threading.Tasks;
using Insthync.UnityEditorUtils;
using LiteNetLib;
using LiteNetLibManager;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Events;

namespace MultiplayerARPG
{
    public abstract partial class DamageableEntity : BaseGameEntity, IDamageableEntity
    {
        protected static readonly ProfilerMarker s_UpdateHitboxesTransformHistoryProfilerMarker = new ProfilerMarker("DamageableEntity - UpdateHitboxesTransformHistory");

        [Category("Relative GameObjects/Transforms")]
        [Tooltip("This is transform where combat texts will be instantiates from")]
        [SerializeField]
        private Transform combatTextTransform;
        public Transform CombatTextTransform
        {
            get { return combatTextTransform; }
            set { combatTextTransform = value; }
        }

        [Tooltip("This is transform for other entities to aim to this entity")]
        [SerializeField]
        private Transform opponentAimTransform;
        public Transform OpponentAimTransform
        {
            get { return opponentAimTransform; }
            set { opponentAimTransform = value; }
        }

        [Category(4, "Hit Boxes")]
        [SerializeField]
        protected bool isStaticHitBoxes;

        [Category(99, "Events", false)]
        public UnityEvent onNormalDamageHit = new UnityEvent();
        public UnityEvent onCriticalDamageHit = new UnityEvent();
        public UnityEvent onBlockedDamageHit = new UnityEvent();
        public UnityEvent onDamageMissed = new UnityEvent();
        public event DamageableEntityInt32ChangeDelegate onCurrentHpChange;
        public event ReceiveDamageDelegate onReceiveDamage;
        public event ReceivedDamageDelegate onReceivedDamage;

        [Category("Sync Fields")]
        [SerializeField]
        protected SyncFieldBool isInvincible = new SyncFieldBool();
        [SerializeField]
        protected SyncFieldInt currentHp = new SyncFieldInt();

        public virtual bool IsInvincible { get { return isInvincible.Value || IsInSafeArea; } set { isInvincible.Value = value; } }
        [System.Obsolete("Use `IsInvincible` instead.")]
        public bool IsImmune { get { return IsInvincible; } set { IsInvincible = value; } }
        public virtual int CurrentHp { get { return currentHp.Value; } set { currentHp.Value = value; } }
        public SafeArea SafeArea { get; set; }
        public bool IsInSafeArea { get { return SafeArea != null; } }
        public abstract int MaxHp { get; }
        public float HpRate { get { return (float)CurrentHp / (float)MaxHp; } }
        public DamageableHitBox[] HitBoxes { get; protected set; }

        public override void InitialRequiredComponents()
        {
            base.InitialRequiredComponents();
            // Cache components
            if (combatTextTransform == null)
                combatTextTransform = EntityTransform;
            if (opponentAimTransform == null)
                opponentAimTransform = CombatTextTransform;
        }

        protected override void EntityStart()
        {
            base.EntityStart();
            // Prepare hitboxes
            HitBoxes = GetComponentsInChildren<DamageableHitBox>(true);
            if (HitBoxes == null || HitBoxes.Length == 0)
                HitBoxes = CreateHitBoxes();
            // Assign index to hitboxes
            for (byte i = 0; i < HitBoxes.Length; ++i)
            {
                HitBoxes[i].Setup(i);
            }
            // Add to lag compensation manager
            if (!isStaticHitBoxes)
                CurrentGameManager.LagCompensationManager.AddDamageableEntity(this);
        }

        protected override void SetupNetElements()
        {
            base.SetupNetElements();
            currentHp.syncMode = LiteNetLibSyncFieldMode.ServerToClients;
            currentHp.onChange += OnCurrentHpChange;
        }

        protected override void EntityOnDestroy()
        {
            base.EntityOnDestroy();
            currentHp.onChange -= OnCurrentHpChange;
            CurrentGameManager.LagCompensationManager.RemoveDamageableEntity(this);
        }

        /// <summary>
        /// This will be called when current hp changed
        /// </summary>
        /// <param name="isInitial"></param>
        /// <param name="oldCurrentHp"></param>
        /// <param name="currentHp"></param>
        private void OnCurrentHpChange(bool isInitial, int oldCurrentHp, int currentHp)
        {
            if (onCurrentHpChange != null)
                onCurrentHpChange.Invoke(this, oldCurrentHp, currentHp);
        }

        private DamageableHitBox[] CreateHitBoxes()
        {
            // Get colliders to calculate bounds
            if (CurrentGameInstance.DimensionType == DimensionType.Dimension3D)
            {
                GameObject obj = new GameObject("_HitBoxes");
                obj.transform.parent = EntityTransform;
                Collider[] colliders = GetComponents<Collider>();
                Bounds bounds = default;
                for (int i = 0; i < colliders.Length; ++i)
                {
                    if (i > 0)
                    {
                        bounds.Encapsulate(colliders[i].bounds);
                    }
                    else
                    {
                        bounds = colliders[i].bounds;
                    }
                }
                BoxCollider newCollider = obj.AddComponent<BoxCollider>();
                newCollider.center = bounds.center - EntityTransform.position;
                newCollider.size = bounds.size;
                newCollider.isTrigger = true;
                obj.transform.localPosition = Vector3.zero;
                obj.layer = gameObject.layer;
                return new DamageableHitBox[] { obj.AddComponent<DamageableHitBox>() };
            }
            else
            {
                GameObject obj = new GameObject("_HitBoxes");
                obj.transform.parent = EntityTransform;
                Collider2D[] colliders = GetComponents<Collider2D>();
                Bounds bounds = default;
                for (int i = 0; i < colliders.Length; ++i)
                {
                    if (i > 0)
                    {
                        bounds.Encapsulate(colliders[i].bounds);
                    }
                    else
                    {
                        bounds = colliders[i].bounds;
                    }
                }
                BoxCollider2D newCollider = obj.AddComponent<BoxCollider2D>();
                newCollider.offset = bounds.center - EntityTransform.position;
                newCollider.size = bounds.size;
                newCollider.isTrigger = true;
                obj.transform.localPosition = Vector3.zero;
                obj.layer = gameObject.layer;
                return new DamageableHitBox[] { obj.AddComponent<DamageableHitBox>() };
            }
        }

        protected override void EntityUpdate()
        {
            SetModelIsDead(this.IsDead());
            UpdateHitboxesTransformHistory();
            base.EntityUpdate();
        }

        public void UpdateHitboxesTransformHistory()
        {
            if (!IsServer)
                return;
            using (s_UpdateHitboxesTransformHistoryProfilerMarker.Auto())
            {
                if (CurrentGameManager.LagCompensationManager.ShouldStoreHitBoxesTransformHistory)
                    AddHitBoxesTransformHistory(CurrentGameManager.ServerTimestamp);
                else if (CurrentGameManager.LagCompensationManager.IsDisabled)
                    ClearHitBoxesTransformHistory();
            }
        }

        public void AddHitBoxesTransformHistory(long time)
        {
            if (isStaticHitBoxes)
                return;
            for (int i = 0; i < HitBoxes.Length; ++i)
            {
                HitBoxes[i].AddTransformHistory(time);
            }
        }

        public void ClearHitBoxesTransformHistory()
        {
            if (isStaticHitBoxes)
                return;
            for (int i = 0; i < HitBoxes.Length; ++i)
            {
                HitBoxes[i].ClearTransformHistory();
            }
        }

        public void RewindHitBoxes(long targetTime)
        {
            long serverTimestamp = Manager.ServerTimestamp;
            for (int i = 0; i < HitBoxes.Length; ++i)
            {
                HitBoxes[i].Rewind(serverTimestamp, targetTime);
            }
        }

        public void RestoreHitBoxes()
        {
            for (int i = 0; i < HitBoxes.Length; ++i)
            {
                HitBoxes[i].Restore();
            }
        }

        /// <summary>
        /// This will be called on clients to display combat texts, play hit effects, play hit animation
        /// </summary>
        /// <param name="combatAmountType"></param>
        /// <param name="hitEffectsSourceType"></param>
        /// <param name="hitEffectsSourceDataId"></param>
        /// <param name="amount"></param>
        [AllRpc]
        protected void RpcAppendCombatText(CombatAmountType combatAmountType, HitEffectsSourceType hitEffectsSourceType, int hitEffectsSourceDataId, int amount)
        {
            switch (combatAmountType)
            {
                case CombatAmountType.NormalDamage:
                    onNormalDamageHit.Invoke();
                    break;
                case CombatAmountType.CriticalDamage:
                    onCriticalDamageHit.Invoke();
                    break;
                case CombatAmountType.BlockedDamage:
                    onBlockedDamageHit.Invoke();
                    break;
                case CombatAmountType.Miss:
                    onDamageMissed.Invoke();
                    break;
            }

            if (!IsClient)
                return;

            BaseUISceneGameplay.Singleton.PrepareCombatText(this, combatAmountType, amount);
            PlayHitEffects(combatAmountType, hitEffectsSourceType, hitEffectsSourceDataId);
        }

        protected virtual void PlayHitEffects(CombatAmountType combatAmountType, HitEffectsSourceType hitEffectsSourceType, int hitEffectsSourceDataId)
        {
            if (combatAmountType == CombatAmountType.NormalDamage ||
                combatAmountType == CombatAmountType.CriticalDamage ||
                combatAmountType == CombatAmountType.BlockedDamage)
            {
                if (Model != null)
                {
                    // Find effects to instantiate
                    GameEffect[] effects = CurrentGameInstance.DefaultDamageHitEffects;
#if !DISABLE_ADDRESSABLES
                    AssetReferenceGameEffect[] addressableEffects = CurrentGameInstance.AddressableDefaultDamageHitEffects;
#endif
                    switch (hitEffectsSourceType)
                    {
                        case HitEffectsSourceType.DamageElement:
                            if (GameInstance.DamageElements.TryGetValue(hitEffectsSourceDataId, out DamageElement damageElement))
                            {
                                if (damageElement.DamageHitEffects != null && damageElement.DamageHitEffects.Length > 0)
                                    effects = damageElement.DamageHitEffects;
#if !DISABLE_ADDRESSABLES
                                if (damageElement.AddressableDamageHitEffects != null && damageElement.AddressableDamageHitEffects.Length > 0)
                                    addressableEffects = damageElement.AddressableDamageHitEffects;
#endif
                            }
                            break;
                        case HitEffectsSourceType.Skill:
                            if (GameInstance.Skills.TryGetValue(hitEffectsSourceDataId, out BaseSkill skill))
                            {
                                if (skill.DamageHitEffects != null && skill.DamageHitEffects.Length > 0)
                                    effects = skill.DamageHitEffects;
#if !DISABLE_ADDRESSABLES
                                if (skill.AddressableDamageHitEffects != null && skill.AddressableDamageHitEffects.Length > 0)
                                    addressableEffects = skill.AddressableDamageHitEffects;
#endif
                            }
                            break;
                    }

                    if (hitEffectsSourceType != HitEffectsSourceType.None)
                        PlayHitAnimation();
                    Model.InstantiateEffect(effects);
#if !DISABLE_ADDRESSABLES
                    Model.InstantiateEffect(addressableEffects).Forget();
#endif
                }
            }
        }

        public void CallRpcAppendCombatText(CombatAmountType combatAmountType, HitEffectsSourceType hitEffectsSourceType, int hitEffectsSourceDataId, int amount)
        {
            RPC(RpcAppendCombatText, 0, DeliveryMethod.Unreliable, combatAmountType, hitEffectsSourceType, hitEffectsSourceDataId, amount);
        }

        /// <summary>
        /// Applying damage to this entity
        /// </summary>
        /// <param name="position"></param>
        /// <param name="fromPosition"></param>
        /// <param name="instigator"></param>
        /// <param name="damageAmounts"></param>
        /// <param name="weapon"></param>
        /// <param name="skill"></param>
        /// <param name="skillLevel"></param>
        /// <param name="randomSeed"></param>
        internal void ApplyDamage(HitBoxPosition position, Vector3 fromPosition, EntityInfo instigator, Dictionary<DamageElement, MinMaxFloat> damageAmounts, CharacterItem weapon, BaseSkill skill, int skillLevel, int randomSeed)
        {
            ReceivingDamage(position, fromPosition, instigator, damageAmounts, weapon, skill, skillLevel);
            CombatAmountType combatAmountType = CombatAmountType.Immune;
            int totalDamage = 0;
            if (!IsInvincible)
                ApplyReceiveDamage(position, fromPosition, instigator, damageAmounts, weapon, skill, skillLevel, randomSeed, out combatAmountType, out totalDamage);
            ReceivedDamage(position, fromPosition, instigator, damageAmounts, combatAmountType, totalDamage, weapon, skill, skillLevel, CharacterBuff.Empty);
        }

        /// <summary>
        /// This function will be called before apply receive damage
        /// </summary>
        /// <param name="position"></param>
        /// <param name="fromPosition">Where is attacker?</param>
        /// <param name="instigator">Who is attacking this?</param>
        /// <param name="damageAmounts">Damage amounts from attacker</param>
        /// <param name="weapon">Weapon which used to attack</param>
        /// <param name="skill">Skill which used to attack</param>
        /// <param name="skillLevel">Skill level which used to attack</param>
        public virtual void ReceivingDamage(HitBoxPosition position, Vector3 fromPosition, EntityInfo instigator, Dictionary<DamageElement, MinMaxFloat> damageAmounts, CharacterItem weapon, BaseSkill skill, int skillLevel)
        {
            if (onReceiveDamage != null)
                onReceiveDamage.Invoke(this, position, fromPosition, instigator, damageAmounts, weapon, skill, skillLevel);
        }

        /// <summary>
        /// Apply damage then return damage type and calculated damage amount
        /// </summary>
        /// <param name="position"></param>
        /// <param name="fromPosition">Where is attacker?</param>
        /// <param name="instigator">Who is attacking this?</param>
        /// <param name="damageAmounts">Damage amounts from attacker</param>
        /// <param name="weapon">Weapon which used to attack</param>
        /// <param name="skill">Skill which used to attack</param>
        /// <param name="skillLevel">Skill level which used to attack</param>
        /// <param name="randomSeed">Random seed for damage randoming</param>
        /// <param name="combatAmountType">Result damage type</param>
        /// <param name="totalDamage">Result damage</param>
        protected abstract void ApplyReceiveDamage(HitBoxPosition position, Vector3 fromPosition, EntityInfo instigator, Dictionary<DamageElement, MinMaxFloat> damageAmounts, CharacterItem weapon, BaseSkill skill, int skillLevel, int randomSeed, out CombatAmountType combatAmountType, out int totalDamage);

        /// <summary>
        /// This function will be called after applied receive damage
        /// </summary>
        /// <param name="position">Which part?</param>
        /// <param name="fromPosition">Where is attacker?</param>
        /// <param name="instigator">Who is attacking this?</param>
        /// <param name="damageAmounts">Damage amount before total damage calculated</param>
        /// <param name="combatAmountType">Result damage type which receives from `ApplyReceiveDamage`</param>
        /// <param name="totalDamage">Result damage which receives from `ApplyReceiveDamage`</param>
        /// <param name="weapon">Which weapon is the source of damages</param>
        /// <param name="skill">Which skill is the source of damages</param>
        /// <param name="skillLevel">Level of the skill</param>
        /// <param name="buff">Which buff is the source of damages</param>
        /// <param name="isDamageOverTime">Received from damage over time debuff</param>
        public virtual void ReceivedDamage(HitBoxPosition position, Vector3 fromPosition, EntityInfo instigator, Dictionary<DamageElement, MinMaxFloat> damageAmounts, CombatAmountType combatAmountType, int totalDamage, CharacterItem weapon, BaseSkill skill, int skillLevel, CharacterBuff buff, bool isDamageOverTime = false)
        {
            HitEffectsSourceType hitEffectsSourceType = HitEffectsSourceType.None;
            int hitEffectsSourceDataId = 0;
            if (!isDamageOverTime && combatAmountType != CombatAmountType.Miss)
            {
                hitEffectsSourceType = skill == null ? HitEffectsSourceType.DamageElement : HitEffectsSourceType.Skill;
                switch (hitEffectsSourceType)
                {
                    case HitEffectsSourceType.DamageElement:
                        if (damageAmounts != null)
                        {
                            foreach (DamageElement element in damageAmounts.Keys)
                            {
                                if (element != null && element != CurrentGameInstance.DefaultDamageElement)
                                {
                                    hitEffectsSourceDataId = element.DataId;
                                    break;
                                }
                            }
                        }
                        break;
                    case HitEffectsSourceType.Skill:
                        hitEffectsSourceDataId = skill.DataId;
                        break;
                }
            }
            CallRpcAppendCombatText(combatAmountType, hitEffectsSourceType, hitEffectsSourceDataId, totalDamage);
            if (onReceivedDamage != null)
                onReceivedDamage.Invoke(this, position, fromPosition, instigator, combatAmountType, totalDamage, weapon, skill, skillLevel, buff, isDamageOverTime);
        }

        public virtual bool CanReceiveDamageFrom(EntityInfo instigator)
        {
            if (!string.Equals(instigator.SubChannelId, SubChannelId))
            {
                // Difference channel
                return false;
            }

            if (instigator.IsInSafeArea)
            {
                // If attacker is in safe area, it will not receives damages
                return false;
            }

            if (string.IsNullOrEmpty(instigator.Id))
            {
                // If attacker is unknow entity, can receive damages
                return true;
            }

            // If this character is not ally so it is enemy and also can receive damage
            return !IsAlly(instigator);
        }

        public virtual bool IsHitBoxesOverridedByVehicle()
        {
            return PassengingVehicleSeat != null && PassengingVehicleSeat.overridePassengerHitBoxes;
        }

        public bool IsAlly(EntityInfo entityInfo)
        {
            if (CurrentMapInfo == null)
                return false;
            return CurrentMapInfo.IsAlly(this, entityInfo);
        }

        public bool IsEnemy(EntityInfo entityInfo)
        {
            if (CurrentMapInfo == null)
                return false;
            return CurrentMapInfo.IsEnemy(this, entityInfo);
        }

        public bool IsNeutral(EntityInfo instigator)
        {
            return !IsAlly(instigator) && !IsEnemy(instigator);
        }

        public virtual void PlayHitAnimation()
        {
            if (Model is IHittableModel hittableModel)
                hittableModel.PlayHitAnimation();
        }

        public virtual void SetModelIsDead(bool isDead)
        {
            if (Model is IDeadableModel deadableModel)
                deadableModel.SetIsDead(isDead);
        }
    }
}
