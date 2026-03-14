using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    public abstract partial class BaseDamageEntity : PoolDescriptor
    {
        protected EntityInfo _instigator;
        protected CharacterItem _weapon;
        protected int _simulateSeed;
        protected byte _triggerIndex;
        protected byte _spreadIndex;
        protected Dictionary<DamageElement, MinMaxFloat> _damageAmounts;
        protected BaseSkill _skill;
        protected int _skillLevel;
        protected HitRegisterData _hitRegisterData;

        public GameInstance CurrentGameInstance
        {
            get { return GameInstance.Singleton; }
        }

        public BaseGameplayRule CurrentGameplayRule
        {
            get { return CurrentGameInstance.GameplayRule; }
        }

        public BaseGameNetworkManager CurrentGameManager
        {
            get { return BaseGameNetworkManager.Singleton; }
        }

        public bool IsServer
        {
            get { return CurrentGameManager.IsServer; }
        }

        public bool IsClient
        {
            get { return CurrentGameManager.IsClient; }
        }

        public Transform CacheTransform { get; private set; }
        private FxCollection _fxCollection = null;
        public FxCollection FxCollection
        {
            get
            {
                if (_fxCollection == null && gameObject != null)
                    _fxCollection = new FxCollection(gameObject);
                return _fxCollection;
            }
        }
        private bool _playFxOnEnable;

        protected virtual void Awake()
        {
            CacheTransform = transform;
        }

        protected virtual void OnDestroy()
        {
            CacheTransform = null;
            _damageAmounts?.Clear();
            _skill = null;
            _fxCollection = null;
        }

        protected virtual void OnEnable()
        {
            if (_playFxOnEnable)
                PlayFx();
        }

        /// <summary>
        /// Setup this component data
        /// </summary>
        /// <param name="instigator">Weapon's or skill's instigator who to spawn this to attack enemy</param>
        /// <param name="weapon">Weapon which was used to attack enemy</param>
        /// <param name="simulateSeed">Launch random seed</param>
        /// <param name="triggerIndex"></param>
        /// <param name="spreadIndex"></param>
        /// <param name="damageAmounts">Calculated damage amounts</param>
        /// <param name="skill">Skill which was used to attack enemy</param>
        /// <param name="skillLevel">Level of the skill</param>
        /// <param name="hitRegisterData">Action when hit</param>
        public virtual void Setup(
            EntityInfo instigator,
            CharacterItem weapon,
            int simulateSeed,
            byte triggerIndex,
            byte spreadIndex,
            Dictionary<DamageElement, MinMaxFloat> damageAmounts,
            BaseSkill skill,
            int skillLevel,
            HitRegisterData hitRegisterData)
        {
            _instigator = instigator;
            _weapon = weapon;
            _simulateSeed = simulateSeed;
            _triggerIndex = triggerIndex;
            _spreadIndex = spreadIndex;
            _damageAmounts = damageAmounts;
            _skill = skill;
            _skillLevel = skillLevel;
            _hitRegisterData = hitRegisterData;
        }

        public virtual void ApplyDamageTo(DamageableHitBox target)
        {
            if (target == null || target.IsDead() || !target.CanReceiveDamageFrom(_instigator))
                return;

            bool willProceedHitRegByClient = false;
            bool isOwnerClient = false;
            if (_instigator.TryGetEntity(out BaseGameEntity entity))
            {
                isOwnerClient = entity.IsOwnerClient;
                willProceedHitRegByClient = !entity.IsOwnedByServer && !entity.IsOwnerHost;
            }

            if (IsServer && !willProceedHitRegByClient)
            {
                target.ReceiveDamage(CacheTransform.position, _instigator, _damageAmounts, _weapon, _skill, _skillLevel, _simulateSeed);
            }

            if (isOwnerClient && willProceedHitRegByClient)
            {
                _hitRegisterData.HitTimestamp = CurrentGameManager.ServerTimestamp;
                _hitRegisterData.HitObjectId = target.GetObjectId();
                _hitRegisterData.HitBoxIndex = target.Index;
                _hitRegisterData.HitOrigin = CacheTransform.position;
                _hitRegisterData.HitDestination = target.CacheTransform.position;
                entity.CallCmdPerformHitRegValidation(_hitRegisterData);
            }
        }

        public override void InitPrefab()
        {
            if (this == null)
            {
                Debug.LogWarning("The Base Damage Entity is null, this should not happens");
                return;
            }
            FxCollection?.InitPrefab();
            base.InitPrefab();
        }

        public override void OnGetInstance()
        {
            PlayFx();
            base.OnGetInstance();
        }

        public override void OnPushBack()
        {
            StopFx();
            base.OnPushBack();
        }

        public virtual void PlayFx()
        {
            if (!gameObject.activeInHierarchy)
            {
                _playFxOnEnable = true;
                return;
            }
            FxCollection?.Play();
            _playFxOnEnable = false;
        }

        public virtual void StopFx()
        {
            FxCollection?.Stop();
        }
    }
}
