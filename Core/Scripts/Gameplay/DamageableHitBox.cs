using UnityEngine;
using System.Collections.Generic;
using LiteNetLibManager;
using UnityEngine.Pool;
#if ENABLE_PROFILER
using Unity.Profiling;
#endif

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MultiplayerARPG
{
    public class DamageableHitBox : MonoBehaviour, IDamageableEntity, IBaseActivatableEntity, IActivatableEntity, IHoldActivatableEntity
    {
#if !ENABLE_PROFILER
        // No-op profiler shim. Keeps profiling scopes compile-safe without pulling Unity.Profiling into
        // non-profiler/headless builds.
        private struct ProfilerMarker
        {
            public ProfilerMarker(string name) { }
            public AutoScope Auto() { return default; }
            public struct AutoScope : System.IDisposable
            {
                public void Dispose() { }
            }
        }
#endif

        // Defensive upper bound. Lag compensation should normally be far below this.
        // This prevents bad config/runtime state from allowing per-hitbox history lists to grow without limit.
        private const int MaxAllowedTransformHistories = 512;
        private const int MaxRetainedHistoryCapacityWhenCleared = 64;
        private static readonly ProfilerMarker AwakeProfilerMarker = new ProfilerMarker("DamageableHitBox.Awake");
        private static readonly ProfilerMarker OnDisableProfilerMarker = new ProfilerMarker("DamageableHitBox.OnDisable");
        private static readonly ProfilerMarker OnDestroyProfilerMarker = new ProfilerMarker("DamageableHitBox.OnDestroy");
        private static readonly ProfilerMarker SetupProfilerMarker = new ProfilerMarker("DamageableHitBox.Setup");
        private static readonly ProfilerMarker ReceiveDamageProfilerMarker = new ProfilerMarker("DamageableHitBox.ReceiveDamage");
        private static readonly ProfilerMarker ReceiveDamageWithoutConditionCheckProfilerMarker = new ProfilerMarker("DamageableHitBox.ReceiveDamageWithoutConditionCheck");
        private static readonly ProfilerMarker VehiclePassengerDamageProfilerMarker = new ProfilerMarker("DamageableHitBox.VehiclePassengerDamage");
        private static readonly ProfilerMarker ApplyOwnerDamageProfilerMarker = new ProfilerMarker("DamageableHitBox.ApplyOwnerDamage");
        private static readonly ProfilerMarker GetTransformHistoryProfilerMarker = new ProfilerMarker("DamageableHitBox.GetTransformHistory");
        private static readonly ProfilerMarker RewindProfilerMarker = new ProfilerMarker("DamageableHitBox.Rewind");
        private static readonly ProfilerMarker RestoreProfilerMarker = new ProfilerMarker("DamageableHitBox.Restore");
        private static readonly ProfilerMarker AddTransformHistoryProfilerMarker = new ProfilerMarker("DamageableHitBox.AddTransformHistory");
        private static readonly ProfilerMarker ClearTransformHistoryProfilerMarker = new ProfilerMarker("DamageableHitBox.ClearTransformHistory");

        [System.Serializable]
        public struct TransformHistory
        {
            public long Time { get; set; }
            public Vector3 Position { get; set; }
            public Quaternion Rotation { get; set; }
            public Bounds Bounds { get; set; }
        }

        [SerializeField]
        protected HitBoxPosition position;
        public HitBoxPosition Position
        {
            get { return position; }
            set { position = value; }
        }

        [SerializeField]
        protected float damageRate = 1f;
        public float DamageRate
        {
            get { return damageRate; }
            set { damageRate = value; }
        }

        private DamageableEntity _damageableEntity;
        public DamageableEntity DamageableEntity { get { return _damageableEntity; } }
        public BaseGameEntity Entity { get { return DamageableEntity == null ? null : DamageableEntity.Entity; } }
        public Transform EntityTransform { get { return DamageableEntity == null ? null : DamageableEntity.EntityTransform; } }
        public GameObject EntityGameObject { get { return DamageableEntity == null ? null : DamageableEntity.EntityGameObject; } }
        public IBaseActivatableEntity BaseActivatableEntity { get { return DamageableEntity == null ? null : DamageableEntity as IBaseActivatableEntity; } }
        public IActivatableEntity ActivatableEntity { get { return DamageableEntity == null ? null : DamageableEntity as IActivatableEntity; } }
        public IHoldActivatableEntity HoldActivatableEntity { get { return DamageableEntity == null ? null : DamageableEntity as IHoldActivatableEntity; } }
        public bool IsInvincible { get { return DamageableEntity == null ? false : DamageableEntity.IsInvincible; } }
        public int CurrentHp
        {
            get { return DamageableEntity == null ? 0 : DamageableEntity.CurrentHp; }
            set
            {
                if (DamageableEntity != null)
                    DamageableEntity.CurrentHp = value;
            }
        }
        public SafeArea SafeArea
        {
            get { return DamageableEntity == null ? null : DamageableEntity.SafeArea; }
            set
            {
                if (DamageableEntity != null)
                    DamageableEntity.SafeArea = value;
            }
        }
        public bool IsInSafeArea { get { return DamageableEntity == null ? false : DamageableEntity.IsInSafeArea; } }
        public Transform OpponentAimTransform { get { return DamageableEntity == null ? null : DamageableEntity.OpponentAimTransform; } }
        public LiteNetLibIdentity Identity { get { return DamageableEntity == null ? null : DamageableEntity.Identity; } }
        public Transform CacheTransform { get; private set; }
        public Collider CacheCollider { get; private set; }
        public Rigidbody CacheRigidbody { get; private set; }
        public Collider2D CacheCollider2D { get; private set; }
        public Rigidbody2D CacheRigidbody2D { get; private set; }
        public byte Index { get; private set; }
        public int DebugHistoryCount { get { return _histories != null ? _histories.Count : 0; } }
        public int DebugHistoryCapacity { get { return _histories != null ? _histories.Capacity : 0; } }

        public Bounds Bounds
        {
            get
            {
                if (CacheTransform == null)
                    return default;

                Quaternion rotation = CacheTransform.rotation;
                Vector3 scaledSize = new Vector3(
                    Mathf.Abs(CacheTransform.lossyScale.x) * _boundsSize.x,
                    Mathf.Abs(CacheTransform.lossyScale.y) * _boundsSize.y,
                    Mathf.Abs(CacheTransform.lossyScale.z) * _boundsSize.z);

                return new Bounds()
                {
                    center = CacheTransform.position + (rotation * _boundsOffset),
                    size = scaledSize,
                };
            }
        }

        protected bool _isSetup;
        protected Vector3 _defaultLocalPosition;
        protected Quaternion _defaultLocalRotation = Quaternion.identity;
        protected List<TransformHistory> _histories = new List<TransformHistory>();
        protected Vector3 _boundsOffset;
        protected Vector3 _boundsSize;

#if UNITY_EDITOR
        [Header("Rewind Debugging")]
        public Color debugHistoryColor = new Color(0, 1, 0, 0.25f);
        public Color debugRewindColor = new Color(0, 0, 1, 0.5f);
#endif

        private void Awake()
        {
            using (AwakeProfilerMarker.Auto())
            {
                CacheComponents();
            }
        }

        private void OnDisable()
        {
            using (OnDisableProfilerMarker.Auto())
            {
                // Pooled/despawned entities can remain managed. Do not clear owner/component refs here.
                // Only clear runtime lag-compensation state so disabled hitboxes cannot keep growing memory.
                ClearTransformHistory();
            }
        }

        private void OnDestroy()
        {
            using (OnDestroyProfilerMarker.Auto())
            {
                ClearTransformHistory();
                _damageableEntity = null;
                CacheTransform = null;
                CacheCollider = null;
                CacheRigidbody = null;
                CacheCollider2D = null;
                CacheRigidbody2D = null;
                _isSetup = false;
                Index = 0;
            }
        }

        protected virtual void CacheComponents()
        {
            _damageableEntity = GetComponentInParent<DamageableEntity>();
            CacheTransform = transform;
            CacheCollider = GetComponent<Collider>();
            CacheCollider2D = null;
            CacheRigidbody = null;
            CacheRigidbody2D = null;
            _boundsOffset = Vector3.zero;
            _boundsSize = Vector3.zero;

            if (CacheCollider != null)
            {
                if (CacheCollider is BoxCollider boxCollider)
                {
                    _boundsOffset = boxCollider.center;
                    _boundsSize = boxCollider.size;
                }
                else if (CacheCollider is SphereCollider sphereCollider)
                {
                    _boundsOffset = sphereCollider.center;
                    _boundsSize = sphereCollider.radius * Vector3.one * 2f;
                }
                else if (CacheCollider is CapsuleCollider capsuleCollider)
                {
                    _boundsOffset = capsuleCollider.center;
                    _boundsSize = capsuleCollider.radius * Vector3.one * 2f;
                    switch (capsuleCollider.direction)
                    {
                        case 1:
                            // Y
                            _boundsSize = new Vector3(_boundsSize.x, capsuleCollider.height, _boundsSize.z);
                            break;
                        case 2:
                            // Z
                            _boundsSize = new Vector3(_boundsSize.x, _boundsSize.y, capsuleCollider.height);
                            break;
                        default:
                            // X
                            _boundsSize = new Vector3(capsuleCollider.height, _boundsSize.y, _boundsSize.z);
                            break;
                    }
                }
                else
                {
                    Logging.LogError(ToString(), "Only `BoxCollider`, `SphereCollider` and `CapsuleCollider` can be used for damageable hit box (3D games)");
                    enabled = false;
                    return;
                }

                CacheRigidbody = gameObject.GetOrAddComponent<Rigidbody>();
                CacheRigidbody.useGravity = false;
                CacheRigidbody.isKinematic = true;
                return;
            }

            CacheCollider2D = GetComponent<Collider2D>();
            if (CacheCollider2D != null)
            {
                if (CacheCollider2D is BoxCollider2D boxCollider2D)
                {
                    _boundsOffset = boxCollider2D.offset;
                    _boundsSize = new Vector3(boxCollider2D.size.x, boxCollider2D.size.y, 0f);
                }
                else if (CacheCollider2D is CircleCollider2D circleCollider2D)
                {
                    _boundsOffset = circleCollider2D.offset;
                    _boundsSize = circleCollider2D.radius * Vector3.one * 2f;
                }
                else if (CacheCollider2D is CapsuleCollider2D capsuleCollider2D)
                {
                    _boundsOffset = capsuleCollider2D.offset;
                    // Keep the collider's actual local size. Do not swap X/Y for horizontal capsules.
                    _boundsSize = new Vector3(capsuleCollider2D.size.x, capsuleCollider2D.size.y, 0f);
                }
                else
                {
                    Logging.LogError(ToString(), "Only `BoxCollider2D`, `CircleCollider2D` and `CapsuleCollider2D` can be used for damageable hit box (2D games)");
                    enabled = false;
                    return;
                }

                CacheRigidbody2D = gameObject.GetOrAddComponent<Rigidbody2D>();
                CacheRigidbody2D.gravityScale = 0;
                CacheRigidbody2D.isKinematic = true;
                return;
            }

            Logging.LogWarning(ToString(), "Damageable hit box has no supported Collider or Collider2D. Disabling hit box.");
            enabled = false;
        }

        public virtual void Setup(byte index)
        {
            using (SetupProfilerMarker.Auto())
            {
                if (_damageableEntity == null)
                    _damageableEntity = GetComponentInParent<DamageableEntity>();

                if (_damageableEntity == null)
                {
                    Logging.LogError(ToString(), "DamageableHitBox requires a parent DamageableEntity.");
                    enabled = false;
                    return;
                }

                if (CacheTransform == null)
                    CacheComponents();

                if (CacheTransform == null || (CacheCollider == null && CacheCollider2D == null))
                {
                    Logging.LogWarning(ToString(), "DamageableHitBox has no cached transform/collider during setup. Disabling hit box.");
                    enabled = false;
                    return;
                }

                _isSetup = true;
                gameObject.tag = _damageableEntity.gameObject.tag;
                gameObject.layer = _damageableEntity.gameObject.layer;
                _defaultLocalPosition = CacheTransform.localPosition;
                _defaultLocalRotation = CacheTransform.localRotation;
                Index = index;
            }
        }

#if UNITY_EDITOR
        protected virtual void OnDrawGizmos()
        {
            if (_histories != null)
            {
                Matrix4x4 oldGizmosMatrix = Gizmos.matrix;
                foreach (TransformHistory history in _histories)
                {
                    Gizmos.color = debugHistoryColor;
                    Gizmos.matrix = Matrix4x4.TRS(history.Bounds.center, history.Rotation, Vector3.one);
                    Gizmos.DrawWireCube(Vector3.zero, history.Bounds.size);
                }
                Gizmos.matrix = oldGizmosMatrix;
            }

            if (transform != null)
                Handles.Label(transform.position, name + "(HitBox)");
        }
#endif

        public virtual bool CanReceiveDamageFrom(EntityInfo instigator)
        {
            return DamageableEntity == null ? false : DamageableEntity.CanReceiveDamageFrom(instigator);
        }

        public virtual void ReceiveDamage(Vector3 fromPosition, EntityInfo instigator, Dictionary<DamageElement, MinMaxFloat> damageAmounts, CharacterItem weapon, BaseSkill skill, int skillLevel, int randomSeed)
        {
            using (ReceiveDamageProfilerMarker.Auto())
            {
                if (!isActiveAndEnabled)
                    return;

                DamageableEntity owner = DamageableEntity;
                if (owner == null || !owner.IsServer || this.IsDead() || !owner.CanReceiveDamageFrom(instigator))
                    return;
                ReceiveDamageWithoutConditionCheck(fromPosition, instigator, damageAmounts, weapon, skill, skillLevel, randomSeed);
            }
        }

        public virtual void ReceiveDamageWithoutConditionCheck(Vector3 fromPosition, EntityInfo instigator, Dictionary<DamageElement, MinMaxFloat> damageAmounts, CharacterItem weapon, BaseSkill skill, int skillLevel, int randomSeed)
        {
            using (ReceiveDamageWithoutConditionCheckProfilerMarker.Auto())
            {
                if (!isActiveAndEnabled)
                    return;

                DamageableEntity owner = DamageableEntity;
                if (owner == null)
                    return;

                if (owner.IsHitBoxesOverridedByVehicle())
                    return;

                float safeDamageRate = damageRate;
                if (float.IsNaN(safeDamageRate) || float.IsInfinity(safeDamageRate))
                    safeDamageRate = 1f;

                using (CollectionPool<Dictionary<DamageElement, MinMaxFloat>, KeyValuePair<DamageElement, MinMaxFloat>>.Get(out Dictionary<DamageElement, MinMaxFloat> modifiedDamageAmounts))
                {
                    modifiedDamageAmounts.Clear();

                    if (damageAmounts != null)
                    {
                        foreach (KeyValuePair<DamageElement, MinMaxFloat> entry in damageAmounts)
                        {
                            modifiedDamageAmounts[entry.Key] = entry.Value * safeDamageRate;
                        }
                    }

                    if (owner is IVehicleEntity vehicleEntity && vehicleEntity.Seats != null)
                    {
                        using (VehiclePassengerDamageProfilerMarker.Auto())
                        {
                            int seatCount = vehicleEntity.Seats.Count;
                            for (int i = 0; i < seatCount && i <= byte.MaxValue; ++i)
                            {
                                if (!vehicleEntity.Seats[i].overridePassengerHitBoxes)
                                    continue;

                                if (vehicleEntity.GetPassenger((byte)i) is DamageableEntity damageablePassenger)
                                    damageablePassenger.ApplyDamage(position, fromPosition, instigator, modifiedDamageAmounts, weapon, skill, skillLevel, randomSeed);
                            }
                        }
                    }

                    using (ApplyOwnerDamageProfilerMarker.Auto())
                    {
                        owner.ApplyDamage(position, fromPosition, instigator, modifiedDamageAmounts, weapon, skill, skillLevel, randomSeed);
                    }
                }
            }
        }

        public virtual void PrepareRelatesData()
        {
            // Do nothing
        }

        public EntityInfo GetInfo()
        {
            return DamageableEntity == null ? EntityInfo.Empty : DamageableEntity.GetInfo();
        }

        public bool IsHide()
        {
            return DamageableEntity == null ? false : DamageableEntity.IsHide();
        }

        public bool IsRevealsHide()
        {
            return DamageableEntity == null ? false : DamageableEntity.IsRevealsHide();
        }

        public bool IsBlind()
        {
            return DamageableEntity == null ? false : DamageableEntity.IsBlind();
        }

        public TransformHistory GetTransformHistory(long currentTime, long rewindTime)
        {
            using (GetTransformHistoryProfilerMarker.Auto())
            {
                TransformHistory current = GetCurrentTransformHistory(currentTime);

                if (_histories == null || _histories.Count == 0)
                    return current;

                if (rewindTime <= _histories[0].Time)
                    return _histories[0];

                for (int i = 1; i < _histories.Count; ++i)
                {
                    TransformHistory before = _histories[i - 1];
                    TransformHistory after = _histories[i];
                    if (after.Time < rewindTime)
                        continue;

                    long duration = after.Time - before.Time;
                    if (duration <= 0)
                        return before;

                    float t = Mathf.Clamp01((float)(rewindTime - before.Time) / duration);
                    return LerpTransformHistory(before, after, rewindTime, t);
                }

                TransformHistory latest = _histories[_histories.Count - 1];
                long currentDuration = current.Time - latest.Time;
                if (currentDuration <= 0)
                    return latest;

                float currentT = Mathf.Clamp01((float)(rewindTime - latest.Time) / currentDuration);
                return LerpTransformHistory(latest, current, rewindTime, currentT);
            }
        }

        protected virtual TransformHistory GetCurrentTransformHistory(long time)
        {
            if (CacheTransform == null)
            {
                return new TransformHistory()
                {
                    Time = time,
                    Position = Vector3.zero,
                    Rotation = Quaternion.identity,
                    Bounds = default,
                };
            }

            return new TransformHistory()
            {
                Time = time,
                Position = CacheTransform.position,
                Rotation = CacheTransform.rotation,
                Bounds = Bounds,
            };
        }

        protected virtual TransformHistory LerpTransformHistory(TransformHistory before, TransformHistory after, long time, float t)
        {
            return new TransformHistory()
            {
                Time = time,
                Position = Vector3.Lerp(before.Position, after.Position, t),
                Rotation = Quaternion.Slerp(before.Rotation, after.Rotation, t),
                Bounds = new Bounds(
                    Vector3.Lerp(before.Bounds.center, after.Bounds.center, t),
                    Vector3.Lerp(before.Bounds.size, after.Bounds.size, t)),
            };
        }

        public void Rewind(long currentTime, long rewindTime)
        {
            using (RewindProfilerMarker.Auto())
            {
                if (!isActiveAndEnabled || CacheTransform == null)
                    return;

                TransformHistory transformHistory = GetTransformHistory(currentTime, rewindTime);
                CacheTransform.position = transformHistory.Position;
                CacheTransform.rotation = transformHistory.Rotation;
            }
        }

        public void Restore()
        {
            using (RestoreProfilerMarker.Auto())
            {
                if (!isActiveAndEnabled || CacheTransform == null)
                    return;

                CacheTransform.localPosition = _defaultLocalPosition;
                CacheTransform.localRotation = _defaultLocalRotation;
            }
        }

        public void AddTransformHistory(long time)
        {
            using (AddTransformHistoryProfilerMarker.Auto())
            {
                if (!isActiveAndEnabled || CacheTransform == null)
                    return;

                if (_histories == null)
                    _histories = new List<TransformHistory>();

                if (BaseGameNetworkManager.Singleton == null || BaseGameNetworkManager.Singleton.LagCompensationManager == null)
                    return;

                int maxHistorySize = BaseGameNetworkManager.Singleton.LagCompensationManager.MaxHistorySize;
                if (maxHistorySize <= 0)
                {
                    ClearTransformHistory();
                    return;
                }

                maxHistorySize = Mathf.Min(maxHistorySize, MaxAllowedTransformHistories);

                while (_histories.Count >= maxHistorySize)
                    _histories.RemoveAt(0);

                _histories.Add(GetCurrentTransformHistory(time));
            }
        }

        public void ClearTransformHistory()
        {
            using (ClearTransformHistoryProfilerMarker.Auto())
            {
                if (_histories == null)
                    return;

                if (_histories.Count > 0)
                    _histories.Clear();

                // List<T>.Clear() retains capacity. In pooled/despawned mobs, a temporarily large
                // history list can otherwise keep memory reserved forever.
                if (_histories.Capacity > MaxRetainedHistoryCapacityWhenCleared)
                    _histories.TrimExcess();
            }
        }

        public bool SetAsTargetInOneClick()
        {
            if (BaseActivatableEntity != null)
                return BaseActivatableEntity.SetAsTargetInOneClick();
            return false;
        }

        public bool NotBeingSelectedOnClick()
        {
            if (BaseActivatableEntity != null)
                return BaseActivatableEntity.NotBeingSelectedOnClick();
            return false;
        }

        public float GetActivatableDistance()
        {
            if (BaseActivatableEntity != null)
                return BaseActivatableEntity.GetActivatableDistance();
            return 0f;
        }

        public virtual bool ShouldClearTargetAfterActivated()
        {
            if (BaseActivatableEntity != null)
                return BaseActivatableEntity.ShouldClearTargetAfterActivated();
            return false;
        }

        public bool ShouldBeAttackTarget()
        {
            if (ActivatableEntity != null)
                return ActivatableEntity.ShouldBeAttackTarget();
            return true;
        }

        public virtual bool ShouldNotActivateAfterFollowed()
        {
            if (ActivatableEntity != null)
                return ActivatableEntity.ShouldNotActivateAfterFollowed();
            return false;
        }

        public bool CanActivate()
        {
            if (Identity != null && Identity.IsServer && GameInstance.PlayingCharacterEntity != null && Identity.IsHideFrom(GameInstance.PlayingCharacterEntity.Identity))
                return false;
            if (ActivatableEntity != null)
                return ActivatableEntity.CanActivate();
            return false;
        }

        public void OnActivate()
        {
            if (ActivatableEntity != null)
                ActivatableEntity.OnActivate();
        }

        public bool CanHoldActivate()
        {
            if (Identity != null && Identity.IsServer && GameInstance.PlayingCharacterEntity != null && Identity.IsHideFrom(GameInstance.PlayingCharacterEntity.Identity))
                return false;
            if (HoldActivatableEntity != null)
                return HoldActivatableEntity.CanHoldActivate();
            return false;
        }

        public void OnHoldActivate()
        {
            if (HoldActivatableEntity != null)
                HoldActivatableEntity.OnHoldActivate();
        }
    }
}
