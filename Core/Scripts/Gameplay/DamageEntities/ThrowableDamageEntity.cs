using Insthync.ManagedUpdating;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace MultiplayerARPG
{
    public class ThrowableDamageEntity : BaseDamageEntity, IManagedUpdate
    {
        public bool canApplyDamageToUser;
        public bool canApplyDamageToAllies;
        public float destroyDelay;
        public UnityEvent onExploded = new UnityEvent();
        public UnityEvent onDestroy = new UnityEvent();
        public float explodeDistance;

        public Rigidbody CacheRigidbody { get; private set; }
        public Rigidbody2D CacheRigidbody2D { get; private set; }

        protected float _throwForce;
        protected float _lifetime;
        protected bool _isExploded;
        protected float _throwedTime;
        protected bool _destroying;
        protected readonly HashSet<uint> _alreadyHitObjects = new HashSet<uint>();
        protected Collider[] _colliders;
        protected Collider2D[] _colliders2D;
        protected bool _exittedThrower;
        protected int _awakenFrame;
        protected bool _readyToHitWalls;

        protected override void Awake()
        {
            base.Awake();

            CacheRigidbody = GetComponent<Rigidbody>();
            CacheRigidbody2D = GetComponent<Rigidbody2D>();
            // Set colliders to be trigger mode, before exiting thrower
            _colliders = GetComponents<Collider>();
            _colliders2D = GetComponents<Collider2D>();
            _readyToHitWalls = true;
            SetReadyToHitWalls(false);
            _exittedThrower = false;
            _awakenFrame = Time.frameCount;
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

        public void SetReadyToHitWalls(bool isReady)
        {
            if (_readyToHitWalls == isReady)
                return;
            for (int i = 0; i < _colliders.Length; ++i)
            {
                _colliders[i].isTrigger = !isReady;
            }
            for (int i = 0; i < _colliders2D.Length; ++i)
            {
                _colliders2D[i].isTrigger = !isReady;
            }
            _readyToHitWalls = isReady;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            onExploded?.RemoveAllListeners();
            onExploded = null;
            onDestroy?.RemoveAllListeners();
            onDestroy = null;
            CacheRigidbody = null;
            CacheRigidbody2D = null;
            _alreadyHitObjects?.Clear();
            _colliders.Nullify();
            _colliders2D.Nullify();
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
        /// <param name="throwForce">Calculated throw force</param>
        /// <param name="lifetime">Calculated life time</param>
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
            float throwForce,
            float lifetime)
        {
            Setup(instigator, weapon, simulateSeed, triggerIndex, spreadIndex, damageAmounts, skill, skillLevel, hitRegisterData);
            _throwForce = throwForce;
            _lifetime = lifetime;

            if (lifetime <= 0)
            {
                // Explode immediately when lifetime is 0
                Explode();
                PushBack(destroyDelay);
                _destroying = true;
                return;
            }
            _isExploded = false;
            _destroying = false;
            _throwedTime = Time.unscaledTime;
            if (CurrentGameInstance.DimensionType == DimensionType.Dimension2D)
            {
#if UNITY_6000_0_OR_NEWER
                CacheRigidbody2D.linearVelocity = Vector2.zero;
#else
                CacheRigidbody2D.velocity = Vector2.zero;
#endif
                CacheRigidbody2D.angularVelocity = 0f;
                CacheRigidbody2D.AddForce(CacheTransform.forward * _throwForce, ForceMode2D.Impulse);
            }
            else
            {
#if UNITY_6000_0_OR_NEWER
                CacheRigidbody.linearVelocity = Vector3.zero;
#else
                CacheRigidbody.velocity = Vector3.zero;
#endif
                CacheRigidbody.angularVelocity = Vector3.zero;
                CacheRigidbody.AddForce(CacheTransform.forward * _throwForce, ForceMode.Impulse);
            }
        }

        public virtual void ManagedUpdate()
        {
            if (_destroying)
                return;

            if (Time.unscaledTime - _throwedTime >= _lifetime)
            {
                Explode();
                PushBack(destroyDelay);
                _destroying = true;
            }
        }

        public override void OnGetInstance()
        {
            base.OnGetInstance();
            _awakenFrame = Time.frameCount;
        }

        public override void OnPushBack()
        {
            _readyToHitWalls = true;
            SetReadyToHitWalls(false);
            _exittedThrower = false;
            if (onDestroy != null)
                onDestroy.Invoke();
            base.OnPushBack();
        }

        protected virtual bool FindTargetHitBox(GameObject other, out DamageableHitBox target)
        {
            target = null;

            if (!other.GetComponent<IUnHittable>().IsNull())
                return false;

            target = other.GetComponent<DamageableHitBox>();

            if (target == null || target.IsDead() || target.IsInvincible || target.IsInSafeArea)
            {
                target = null;
                return false;
            }

            if (target.GetObjectId() == _instigator.ObjectId)
                return canApplyDamageToUser;

            if (target.DamageableEntity is BaseCharacterEntity characterEntity && characterEntity.IsAlly(_instigator))
                return canApplyDamageToAllies;

            return true;
        }

        protected virtual bool FindAndApplyDamage(GameObject other, HashSet<uint> alreadyHitObjects)
        {
            if (FindTargetHitBox(other, out DamageableHitBox target) && !alreadyHitObjects.Contains(target.GetObjectId()))
            {
                target.ReceiveDamageWithoutConditionCheck(CacheTransform.position, _instigator, _damageAmounts, _weapon, _skill, _skillLevel, Random.Range(0, 255));
                alreadyHitObjects.Add(target.GetObjectId());
                return true;
            }
            return false;
        }

        protected virtual void Explode()
        {
            if (_isExploded)
                return;

            _isExploded = true;

            if (onExploded != null)
                onExploded.Invoke();

            if (!IsServer)
                return;

            ExplodeApplyDamage();
        }

        protected virtual void ExplodeApplyDamage()
        {
            if (CurrentGameInstance.DimensionType == DimensionType.Dimension2D)
            {
                _alreadyHitObjects.Clear();
                Collider2D[] colliders2D = Physics2D.OverlapCircleAll(CacheTransform.position, explodeDistance);
                foreach (Collider2D collider in colliders2D)
                {
                    FindAndApplyDamage(collider.gameObject, _alreadyHitObjects);
                }
            }
            else
            {
                _alreadyHitObjects.Clear();
                Collider[] colliders = Physics.OverlapSphere(CacheTransform.position, explodeDistance);
                foreach (Collider collider in colliders)
                {
                    FindAndApplyDamage(collider.gameObject, _alreadyHitObjects);
                }
            }
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            ProceedEnteringThrower(other.transform, other.isTrigger);
        }

        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            ProceedEnteringThrower(other.transform, other.isTrigger);
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            ProceedExitingThrower(other.transform);
        }

        protected virtual void OnTriggerExit2D(Collider2D other)
        {
            ProceedExitingThrower(other.transform);
        }

        protected virtual void ProceedEnteringThrower(Transform other, bool otherIsTrigger)
        {
            if (otherIsTrigger)
                return;
            if (_exittedThrower)
                return;
            if (!_instigator.TryGetEntity(out BaseGameEntity entity))
            {
                // No instigator (may logoff?)
                _exittedThrower = true;
                SetReadyToHitWalls(true);
                return;
            }
            if (other.transform.root != entity.EntityTransform.root)
            {
                // Hit something which is not a thrower, so we can determine that this one can hit it
                _exittedThrower = true;
                SetReadyToHitWalls(true);
                return;
            }
        }

        protected virtual void ProceedExitingThrower(Transform other)
        {
            if (_exittedThrower)
                return;
            if (!_instigator.TryGetEntity(out BaseGameEntity entity))
            {
                // No instigator (may logoff?)
                _exittedThrower = true;
                SetReadyToHitWalls(true);
                return;
            }
            if (other.root == entity.EntityTransform.root)
            {
                // Exited from thrower, ready to hit the wall
                _exittedThrower = true;
                SetReadyToHitWalls(true);
                return;
            }
        }
    }
}
