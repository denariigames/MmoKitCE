using Insthync.ManagedUpdating;
using LiteNetLibManager;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

namespace MultiplayerARPG
{
    [RequireComponent(typeof(LiteNetLibIdentity))]
    public partial class AreaDamageEntity : BaseDamageEntity, IManagedUpdate
    {
        public bool canApplyDamageToUser;
        public bool canApplyDamageToAllies;
        public UnityEvent onDestroy = new UnityEvent();

        private LiteNetLibIdentity _identity;
        public LiteNetLibIdentity Identity
        {
            get
            {
                if (_identity == null)
                    _identity = GetComponent<LiteNetLibIdentity>();
                return _identity;
            }
        }

        protected float _applyDuration;
        protected float _lastAppliedTime;
        protected readonly Dictionary<uint, DamageableHitBox> _receivingDamageHitBoxes = new Dictionary<uint, DamageableHitBox>();

        protected override void Awake()
        {
            base.Awake();
            Identity.onGetInstance.AddListener(OnGetInstance);
            Identity.onPushBack.AddListener(OnPushBack);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (Identity != null && Identity.onGetInstance != null)
                Identity.onGetInstance.RemoveListener(OnGetInstance);
            if (Identity != null && Identity.onPushBack != null)
                Identity.onPushBack.RemoveListener(OnPushBack);
            _identity = null;
            _receivingDamageHitBoxes?.Clear();
            onDestroy?.RemoveAllListeners();
            onDestroy = null;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            UpdateManager.Register(this);
        }

        protected virtual void OnDisable()
        {
            UpdateManager.Unregister(this);
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
        /// <param name="hitRegisterData"></param>
        /// <param name="areaDuration"></param>
        /// <param name="applyDuration"></param>
        public virtual void Setup(
            EntityInfo instigator,
            CharacterItem weapon,
            int simulateSeed,
            byte triggerIndex,
            byte spreadIndex,
            Dictionary<DamageElement, MinMaxFloat> damageAmounts,
            BaseSkill skill,
            int skillLevel,
            HitRegisterData hitRegisterData,
            float areaDuration,
            float applyDuration)
        {
            Setup(instigator, weapon, simulateSeed, triggerIndex, spreadIndex, damageAmounts, skill, skillLevel, hitRegisterData);
            PushBack(areaDuration);
            _applyDuration = applyDuration;
            _lastAppliedTime = Time.unscaledTime;
        }

        public virtual void ManagedUpdate()
        {
            if (!IsServer)
                return;

            if (Time.unscaledTime - _lastAppliedTime >= _applyDuration)
            {
                _lastAppliedTime = Time.unscaledTime;
                foreach (DamageableHitBox hitBox in _receivingDamageHitBoxes.Values)
                {
                    if (hitBox == null)
                        continue;

                    ApplyDamageTo(hitBox);
                }
            }
        }

        public override void ApplyDamageTo(DamageableHitBox target)
        {
            if (target == null || target.IsDead() || target.IsInvincible || target.IsInSafeArea)
                return;

            if (target.GetObjectId() == _instigator.ObjectId)
            {
                if (canApplyDamageToUser)
                    target.ReceiveDamageWithoutConditionCheck(CacheTransform.position, _instigator, _damageAmounts, _weapon, _skill, _skillLevel, Random.Range(0, 255));
                return;
            }

            if (target.DamageableEntity is BaseCharacterEntity characterEntity && characterEntity.IsAlly(_instigator))
            {
                if (canApplyDamageToAllies)
                    target.ReceiveDamageWithoutConditionCheck(CacheTransform.position, _instigator, _damageAmounts, _weapon, _skill, _skillLevel, Random.Range(0, 255));
                return;
            }

            target.ReceiveDamageWithoutConditionCheck(CacheTransform.position, _instigator, _damageAmounts, _weapon, _skill, _skillLevel, Random.Range(0, 255));
        }

        public override void OnPushBack()
        {
            _receivingDamageHitBoxes.Clear();
            if (onDestroy != null)
                onDestroy.Invoke();
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            TriggerEnter(other.gameObject);
        }

        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            TriggerEnter(other.gameObject);
        }

        protected virtual void TriggerEnter(GameObject other)
        {
            DamageableHitBox target = other.GetComponent<DamageableHitBox>();
            if (target == null)
                return;

            if (_receivingDamageHitBoxes.ContainsKey(target.GetObjectId()))
                return;

            _receivingDamageHitBoxes.Add(target.GetObjectId(), target);
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            TriggerExit(other.gameObject);
        }

        protected virtual void OnTriggerExit2D(Collider2D other)
        {
            TriggerExit(other.gameObject);
        }

        protected virtual void TriggerExit(GameObject other)
        {
            IDamageableEntity target = other.GetComponent<IDamageableEntity>();
            if (target.IsNull())
                return;

            if (!_receivingDamageHitBoxes.ContainsKey(target.GetObjectId()))
                return;

            _receivingDamageHitBoxes.Remove(target.GetObjectId());
        }

        public override void InitPrefab()
        {
            if (this == null)
            {
                Debug.LogWarning("The Base Damage Entity is null, this should not happens");
                return;
            }
            FxCollection.InitPrefab();
            if (Identity == null)
            {
                Debug.LogWarning($"No `LiteNetLibIdentity` attached with the same game object with `AreaDamageEntity` (prefab name: {name}), it will add new identity component with asset ID which geneared by prefab name.");
                LiteNetLibIdentity identity = gameObject.AddComponent<LiteNetLibIdentity>();
                FieldInfo prop = typeof(LiteNetLibIdentity).GetField("assetId", BindingFlags.NonPublic | BindingFlags.Instance);
                prop.SetValue(identity, $"AreaDamageEntity_{name}");
            }
            Identity.PoolingSize = PoolSize;
        }

        public override void PushBack()
        {
            Identity.NetworkDestroy();
        }
    }
}
