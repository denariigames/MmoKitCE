using Insthync.ManagedUpdating;
using LiteNetLibManager;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

namespace MultiplayerARPG
{
    [RequireComponent(typeof(LiteNetLibIdentity))]
    public partial class AreaBuffEntity : BaseBuffEntity, IManagedUpdate
    {
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
        protected readonly Dictionary<uint, BaseCharacterEntity> _receivingBuffCharacters = new Dictionary<uint, BaseCharacterEntity>();

        protected override void Awake()
        {
            base.Awake();
            gameObject.layer = PhysicLayers.IgnoreRaycast;
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
            _receivingBuffCharacters?.Clear();
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
        /// <param name="buffApplier"></param>
        /// <param name="skill">Skill which was used to attack enemy</param>
        /// <param name="skillLevel">Level of the skill</param>
        /// <param name="areaDuration"></param>
        /// <param name="applyDuration"></param>
        public virtual void Setup(
            EntityInfo buffApplier,
            BaseSkill skill,
            int skillLevel,
            bool applyBuffToEveryone,
            float areaDuration,
            float applyDuration)
        {
            base.Setup(buffApplier, skill, skillLevel, applyBuffToEveryone);
            PushBack(areaDuration);
            _applyDuration = applyDuration;
            _lastAppliedTime = Time.unscaledTime;
        }

        public virtual void ManagedUpdate()
        {
            if (Time.unscaledTime - _lastAppliedTime >= _applyDuration)
            {
                _lastAppliedTime = Time.unscaledTime;
                foreach (BaseCharacterEntity entity in _receivingBuffCharacters.Values)
                {
                    if (entity == null)
                        continue;

                    ApplyBuffTo(entity);
                }
            }
        }

        public override void OnPushBack()
        {
            _receivingBuffCharacters.Clear();
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
            BaseCharacterEntity target = other.GetComponent<BaseCharacterEntity>();
            if (target == null)
                return;

            if (_receivingBuffCharacters.ContainsKey(target.ObjectId))
                return;

            _receivingBuffCharacters.Add(target.ObjectId, target);
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
            BaseCharacterEntity target = other.GetComponent<BaseCharacterEntity>();
            if (target == null)
                return;

            if (!_receivingBuffCharacters.ContainsKey(target.ObjectId))
                return;

            _receivingBuffCharacters.Remove(target.ObjectId);
        }

        public override void InitPrefab()
        {
            if (this == null)
            {
                Debug.LogWarning("The Base Bufff Entity is null, this should not happens");
                return;
            }
            FxCollection.InitPrefab();
            if (Identity == null)
            {
                Debug.LogWarning($"No `LiteNetLibIdentity` attached with the same game object with `AreaBuffEntity` (prefab name: {name}), it will add new identity component with asset ID which geneared by prefab name.");
                LiteNetLibIdentity identity = gameObject.AddComponent<LiteNetLibIdentity>();
                FieldInfo prop = typeof(LiteNetLibIdentity).GetField("assetId", BindingFlags.NonPublic | BindingFlags.Instance);
                prop.SetValue(identity, $"AreaBuffEntity_{name}");
            }
            Identity.PoolingSize = PoolSize;
        }

        public override void PushBack()
        {
            Identity.NetworkDestroy();
        }
    }
}
