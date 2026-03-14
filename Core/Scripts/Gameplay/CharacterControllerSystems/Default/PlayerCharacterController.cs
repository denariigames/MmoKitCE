using Insthync.CameraAndInput;
using Insthync.ManagedUpdating;
using LiteNetLibManager;
using UnityEngine;
using UnityEngine.Serialization;

namespace MultiplayerARPG
{
    public partial class PlayerCharacterController : BasePlayerCharacterController, IManagedLateUpdate
    {
        public enum PlayerCharacterControllerMode
        {
            PointClick,
            WASD,
            Both,
        }

        public enum TargetActionType
        {
            None,
            ClickActivate,
            Attack,
            UseSkill,
            HoldClickActivate,
            ActionRequested,
        }

        public const float DETECT_MOUSE_DRAG_DISTANCE_SQUARED = 100f;
        public const float DETECT_MOUSE_HOLD_DURATION = 1f;

        [Header("Camera Controls Prefabs")]
        [SerializeField]
        protected FollowCameraControls gameplayCameraPrefab;
        [SerializeField]
        protected FollowCameraControls minimapCameraPrefab;

        [Header("Controller Settings")]
        [SerializeField]
        protected PlayerCharacterControllerMode controllerMode;
        [Tooltip("Set this to `TRUE` to find nearby enemy and follow it to attack while `Controller Mode` is `WASD`")]
        [SerializeField]
        protected bool wasdLockAttackTarget;
        [Tooltip("This will be used to find nearby enemy while `Controller Mode` is `Point Click` or when `Wasd Lock Attack Target` is `TRUE`")]
        [SerializeField]
        [FormerlySerializedAs("lockAttackTargetDistance")]
        protected float distanceToLockActionTarget = 10f;
        [Tooltip("This will be used to clear selected target when character move with WASD keys and far from target")]
        [SerializeField]
        protected float wasdClearTargetDistance = 15f;
        [Tooltip("Set this to TRUE to move to target immediately when clicked on target, if this is FALSE it will not move to target immediately")]
        [SerializeField]
        protected bool pointClickSetTargetImmediately;
        [Tooltip("Set this to TRUE to interrupt casting skill when click on ground to move")]
        [SerializeField]
        protected bool pointClickInterruptCastingSkill;
        [SerializeField]
        protected float turnSmoothSpeed = 10f;
        [Tooltip("The object which will represent where character is moving to")]
        [SerializeField]
        protected GameObject targetObjectPrefab;

        [Header("Building Settings")]
        [SerializeField]
        protected bool buildGridSnap;
        [SerializeField]
        protected Vector3 buildGridOffsets = Vector3.zero;
        [SerializeField]
        protected float buildGridSize = 4f;
        [SerializeField]
        protected bool buildRotationSnap;
        [SerializeField]
        protected float buildRotateAngle = 45f;
        [SerializeField]
        protected float buildRotateSpeed = 200f;

        [Header("Entity Activating Settings")]
        [SerializeField]
        [Tooltip("If this value is `0`, this value will be set as `GameInstance` -> `conversationDistance`")]
        protected float distanceToActivateByActivateKey = 0f;
        [SerializeField]
        [Tooltip("If this value is `0`, this value will be set as `GameInstance` -> `pickUpItemDistance`")]
        protected float distanceToActivateByPickupKey = 0f;
        [SerializeField]
        [Tooltip("This value will being used for distance decreasing in overlap entity functions to make character move a bit closer to target")]
        [Min(0.01f)]
        protected float activateDistanceBuffer = 0.1f;

        [Header("UI Blocking Settings")]
        [SerializeField]
        protected bool uiNotBlockForStandalone = true;
        [SerializeField]
        protected bool uiNotBlockForMobile = true;
        [SerializeField]
        protected bool uiNotBlockForConsole = false;

        #region Events
        /// <summary>
        /// RelateId (string), AimPosition (AimPosition)
        /// </summary>
        public event System.Action<string, AimPosition> onBeforeUseSkillHotkey;
        /// <summary>
        /// RelateId (string), AimPosition (AimPosition)
        /// </summary>
        public event System.Action<string, AimPosition> onAfterUseSkillHotkey;
        /// <summary>
        /// RelateId (string), AimPosition (AimPosition)
        /// </summary>
        public event System.Action<string, AimPosition> onBeforeUseItemHotkey;
        /// <summary>
        /// RelateId (string), AimPosition (AimPosition)
        /// </summary>
        public event System.Action<string, AimPosition> onAfterUseItemHotkey;
        #endregion

        public byte HotkeyEquipWeaponSet { get; set; }
        public NearbyEntityDetector ActivatableEntityDetector { get; protected set; }
        public NearbyEntityDetector ItemDropEntityDetector { get; protected set; }
        public NearbyEntityDetector EnemyEntityDetector { get; protected set; }
        public IGameplayCameraController CacheGameplayCameraController { get; protected set; }
        public IMinimapCameraController CacheMinimapCameraController { get; protected set; }
        public GameObject CacheTargetObject { get; protected set; }

        // Input & control states variables
        protected Vector3? _destination;
        protected TargetActionType _previousTargetActionType;
        protected TargetActionType _targetActionType;
        protected TargetActionType _turnToTargetActionType;
        protected Vector3? _turnToTargetPosition = null;
        protected IPhysicFunctions _physicFunctions;
        protected bool _isLeftHandAttacking;
        protected bool _isFollowingTarget;
        protected InputStateManager _activateInput;
        protected InputStateManager _pickupItemInput;
        protected InputStateManager _reloadInput;
        protected InputStateManager _findEnemyInput;
        protected InputStateManager _exitVehicleInput;
        protected InputStateManager _switchEquipWeaponSetInput;

        protected override void Awake()
        {
            base.Awake();
            // Initial physic functions
            if (CurrentGameInstance.DimensionType == DimensionType.Dimension3D)
                _physicFunctions = new PhysicFunctions(512);
            else
                _physicFunctions = new PhysicFunctions2D(512);
            // Initial gameplay camera controller
            CacheGameplayCameraController = gameObject.GetOrAddComponent<IGameplayCameraController, DefaultGameplayCameraController>((obj) =>
            {
                DefaultGameplayCameraController castedObj = obj as DefaultGameplayCameraController;
                castedObj.SetData(gameplayCameraPrefab);
            });
            CacheGameplayCameraController.Init();
            // Initial minimap camera controller
            CacheMinimapCameraController = gameObject.GetOrAddComponent<IMinimapCameraController, DefaultMinimapCameraController>((obj) =>
            {
                DefaultMinimapCameraController castedObj = obj as DefaultMinimapCameraController;
                castedObj.SetData(minimapCameraPrefab);
            });
            CacheMinimapCameraController.Init();
            // Initial build aim controller
            BuildAimController = gameObject.GetOrAddComponent<IBuildAimController, DefaultBuildAimController>((obj) =>
            {
                DefaultBuildAimController castedObj = obj as DefaultBuildAimController;
                castedObj.SetData(buildGridSnap, buildGridOffsets, buildGridSize, buildRotationSnap, buildRotateAngle, buildRotateSpeed);
            });
            BuildAimController.Init();
            // Initial area skill aim controller
            AreaSkillAimController = gameObject.GetOrAddComponent<IAreaSkillAimController, DefaultAreaSkillAimController>();

            _isLeftHandAttacking = false;
            ConstructingBuildingEntity = null;
            _activateInput = new InputStateManager("Activate");
            _pickupItemInput = new InputStateManager("PickUpItem");
            _reloadInput = new InputStateManager("Reload");
            _findEnemyInput = new InputStateManager("FindEnemy");
            _exitVehicleInput = new InputStateManager("ExitVehicle");
            _switchEquipWeaponSetInput = new InputStateManager("SwitchEquipWeaponSet");

            if (targetObjectPrefab != null)
            {
                // Set parent transform to root for the best performance
                CacheTargetObject = Instantiate(targetObjectPrefab);
                CacheTargetObject.SetActive(false);
            }
            // Setup activate distance
            if (distanceToActivateByActivateKey <= 0f)
                distanceToActivateByActivateKey = GameInstance.Singleton.conversationDistance;
            if (distanceToActivateByPickupKey <= 0f)
                distanceToActivateByPickupKey = GameInstance.Singleton.pickUpItemDistance;
            GameObject tempGameObject;
            // This entity detector will find for an entities to activate when pressed activate key
            tempGameObject = new GameObject("_ActivatingEntityDetector");
            ActivatableEntityDetector = tempGameObject.AddComponent<NearbyEntityDetector>();
            ActivatableEntityDetector.detectingRadius = distanceToActivateByActivateKey;
            ActivatableEntityDetector.findActivatableEntity = true;
            ActivatableEntityDetector.findHoldActivatableEntity = true;
            // This entity detector will find for an item drop entities to activate when pressed pickup key
            tempGameObject = new GameObject("_ItemDropEntityDetector");
            ItemDropEntityDetector = tempGameObject.AddComponent<NearbyEntityDetector>();
            ItemDropEntityDetector.detectingRadius = distanceToActivateByPickupKey;
            ItemDropEntityDetector.findPickupActivatableEntity = true;
            // This entity detector will 
            tempGameObject = new GameObject("_EnemyEntityDetector");
            EnemyEntityDetector = tempGameObject.AddComponent<NearbyEntityDetector>();
            EnemyEntityDetector.findPlayer = true;
            EnemyEntityDetector.findOnlyAlivePlayers = true;
            EnemyEntityDetector.findPlayerToAttack = true;
            EnemyEntityDetector.findMonster = true;
            EnemyEntityDetector.findOnlyAliveMonsters = true;
            EnemyEntityDetector.findMonsterToAttack = true;
        }

        protected override void Setup(BasePlayerCharacterEntity characterEntity)
        {
            base.Setup(characterEntity);
            CacheGameplayCameraController.Setup(characterEntity);
            CacheMinimapCameraController.Setup(characterEntity);
        }

        protected override void Desetup(BasePlayerCharacterEntity characterEntity)
        {
            base.Desetup(characterEntity);
            CacheGameplayCameraController.Desetup(characterEntity);
            CacheMinimapCameraController.Desetup(characterEntity);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (CacheTargetObject != null)
                Destroy(CacheTargetObject.gameObject);
            if (ActivatableEntityDetector != null)
                Destroy(ActivatableEntityDetector.gameObject);
            if (ItemDropEntityDetector != null)
                Destroy(ItemDropEntityDetector.gameObject);
            if (EnemyEntityDetector != null)
                Destroy(EnemyEntityDetector.gameObject);
        }

        public override void ManagedUpdate()
        {
            if (PlayingCharacterEntity == null || !PlayingCharacterEntity.IsOwnerClient)
                return;

            CacheGameplayCameraController.FollowingEntityTransform = CameraTargetTransform;
            CacheMinimapCameraController.FollowingEntityTransform = CameraTargetTransform;
            CacheMinimapCameraController.FollowingGameplayCameraTransform = CacheGameplayCameraController.CameraTransform;

            if (CacheTargetObject != null)
                CacheTargetObject.gameObject.SetActive(_destination.HasValue);

            if (PlayingCharacterEntity.IsDead() ||
                PlayingCharacterEntity.IsDealing ||
                PlayingCharacterEntity.IsVendingStarted)
            {
                ClearQueueUsingSkill();
                _destination = null;
                _isFollowingTarget = false;
                _previousTargetActionType = TargetActionType.None;
                _targetActionType = TargetActionType.None;
                _turnToTargetActionType = TargetActionType.None;
                CancelBuild();
                UISceneGameplay.SetTargetEntity(null);
            }
            else
            {
                UISceneGameplay.SetTargetEntity(SelectedGameEntity);
            }

            if (_destination.HasValue)
            {
                if (CacheTargetObject != null)
                    CacheTargetObject.transform.position = _destination.Value;
                if (Vector3.Distance(_destination.Value, MovementTransform.position) < StoppingDistance + 0.5f)
                    _destination = null;
            }

            float deltaTime = Time.deltaTime;
            _activateInput.OnUpdate(deltaTime);
            _pickupItemInput.OnUpdate(deltaTime);
            _reloadInput.OnUpdate(deltaTime);
            _findEnemyInput.OnUpdate(deltaTime);
            _exitVehicleInput.OnUpdate(deltaTime);
            _switchEquipWeaponSetInput.OnUpdate(deltaTime);

            UpdateInput();
            UpdateFollowTarget();
            PlayingCharacterEntity.AimPosition = PlayingCharacterEntity.GetAttackAimPosition(ref _isLeftHandAttacking);
            PlayingCharacterEntity.SetSmoothTurnSpeed(turnSmoothSpeed);
        }

        public virtual void ManagedLateUpdate()
        {
            _activateInput.OnLateUpdate();
            _pickupItemInput.OnLateUpdate();
            _reloadInput.OnLateUpdate();
            _findEnemyInput.OnLateUpdate();
            _exitVehicleInput.OnLateUpdate();
            _switchEquipWeaponSetInput.OnLateUpdate();
        }

        public bool TryGetSelectedTargetAsAttackingEntity(out BaseCharacterEntity character)
        {
            character = null;
            if (SelectedGameEntity != null)
            {
                character = SelectedGameEntity as BaseCharacterEntity;
                if (character == null ||
                    character == PlayingCharacterEntity ||
                    !character.CanReceiveDamageFrom(PlayingCharacterEntity.GetInfo()))
                {
                    character = null;
                    return false;
                }
                bool isHiddenFromHost = PlayingCharacterEntity.IsServer && SelectedGameEntity.Identity.IsHideFrom(PlayingCharacterEntity.Identity);
                if (isHiddenFromHost)
                    return false;
                return true;
            }
            return false;
        }

        public bool TryGetAttackingEntity<T>(out T entity)
            where T : class, IDamageableEntity
        {
            if (!TryGetDoActionEntity(out entity, TargetActionType.Attack))
                return false;
            if (entity.Entity == PlayingCharacterEntity.Entity || !entity.CanReceiveDamageFrom(PlayingCharacterEntity.GetInfo()))
            {
                entity = null;
                return false;
            }
            return true;
        }

        public bool TryGetUsingSkillEntity<T>(out T entity)
            where T : class, IDamageableEntity
        {
            if (!TryGetDoActionEntity(out entity, TargetActionType.UseSkill))
                return false;
            if (_queueUsingSkill.skill == null)
            {
                entity = null;
                return false;
            }
            return true;
        }

        public bool TryGetDoActionEntity<T>(out T entity, TargetActionType actionType = TargetActionType.ClickActivate)
            where T : class, ITargetableEntity
        {
            entity = default;
            if (_targetActionType != actionType)
                return false;
            if (TargetEntity == null)
                return false;
            entity = TargetEntity as T;
            if (entity.IsNull())
                return false;
            if (PlayingCharacterEntity.IsServer)
            {
                bool isHiddenFromHost;
                isHiddenFromHost = entity.EntityGameObject.TryGetComponent(out LiteNetLibIdentity identity) && identity.IsHideFrom(PlayingCharacterEntity.Identity);
                if (isHiddenFromHost)
                    return false;
            }
            return true;
        }

        public void GetAttackDistanceAndFov(bool isLeftHand, out float attackDistance, out float attackFov)
        {
            attackDistance = PlayingCharacterEntity.GetAttackDistance(isLeftHand);
            attackFov = PlayingCharacterEntity.GetAttackFov(isLeftHand);
            attackDistance -= PlayingCharacterEntity.StoppingDistance;
        }

        public void GetUseSkillDistanceAndFov(bool isLeftHand, out float castDistance, out float castFov)
        {
            castDistance = CurrentGameInstance.conversationDistance;
            castFov = 360f;
            if (_queueUsingSkill.skill != null)
            {
                // If skill is attack skill, set distance and fov by skill
                castDistance = _queueUsingSkill.skill.GetCastDistance(PlayingCharacterEntity, _queueUsingSkill.level, isLeftHand);
                castFov = _queueUsingSkill.skill.GetCastFov(PlayingCharacterEntity, _queueUsingSkill.level, isLeftHand);
            }
            castDistance -= PlayingCharacterEntity.StoppingDistance;
        }

        public Vector3 GetMoveDirection(float horizontalInput, float verticalInput)
        {
            Vector3 moveDirection = Vector3.zero;
            switch (CurrentGameInstance.DimensionType)
            {
                case DimensionType.Dimension3D:
                    Vector3 forward = CacheGameplayCameraController.CameraTransform.forward;
                    Vector3 right = CacheGameplayCameraController.CameraTransform.right;
                    forward.y = 0f;
                    right.y = 0f;
                    forward.Normalize();
                    right.Normalize();
                    moveDirection += forward * verticalInput;
                    moveDirection += right * horizontalInput;
                    // normalize input if it exceeds 1 in combined length:
                    if (moveDirection.sqrMagnitude > 1)
                        moveDirection.Normalize();
                    break;
                case DimensionType.Dimension2D:
                    moveDirection = new Vector2(horizontalInput, verticalInput);
                    break;
            }
            return moveDirection;
        }

        public bool RequestAttack()
        {
            // Switching right/left/right/left...
            WeaponHandlingState weaponHandlingState = GetWeaponHandlingState(_isLeftHandAttacking);
            if (PlayingCharacterEntity.Attack(ref weaponHandlingState))
            {
                _isLeftHandAttacking = weaponHandlingState.Has(WeaponHandlingState.IsLeftHand);
                _isLeftHandAttacking = !_isLeftHandAttacking;
                return true;
            }
            return false;
        }

        public bool RequestUsePendingSkill()
        {
            if (_queueUsingSkill.skill != null &&
                !PlayingCharacterEntity.IsPlayingActionAnimation() &&
                !PlayingCharacterEntity.IsAttacking &&
                !PlayingCharacterEntity.IsUsingSkill)
            {
                WeaponHandlingState weaponHandlingState = GetWeaponHandlingState(_isLeftHandAttacking);
                bool requested = false;
                if (_queueUsingSkill.itemIndex >= 0)
                {
                    if (PlayingCharacterEntity.UseSkillItem(_queueUsingSkill.itemIndex, weaponHandlingState, SelectedGameEntityObjectId, _queueUsingSkill.aimPosition))
                    {
                        _isLeftHandAttacking = weaponHandlingState.Has(WeaponHandlingState.IsLeftHand);
                        _isLeftHandAttacking = !_isLeftHandAttacking;
                        requested = true;
                    }
                }
                else
                {
                    if (PlayingCharacterEntity.UseSkill(_queueUsingSkill.skill.DataId, weaponHandlingState, SelectedGameEntityObjectId, _queueUsingSkill.aimPosition))
                    {
                        _isLeftHandAttacking = weaponHandlingState.Has(WeaponHandlingState.IsLeftHand);
                        _isLeftHandAttacking = !_isLeftHandAttacking;
                        requested = true;
                    }
                }
                ClearQueueUsingSkill();
                return requested;
            }

            return false;
        }

        public override bool ShouldShowActivateButtons()
        {
            if (ActivatableEntityDetector.activatableEntities.Count > 0)
            {
                IActivatableEntity activatable;
                for (int i = 0; i < ActivatableEntityDetector.activatableEntities.Count; ++i)
                {
                    activatable = ActivatableEntityDetector.activatableEntities[i];
                    if (activatable.CanActivate())
                        return true;
                }
            }
            return false;
        }

        public override bool ShouldShowHoldActivateButtons()
        {
            if (ActivatableEntityDetector.holdActivatableEntities.Count > 0)
            {
                IHoldActivatableEntity activatable;
                for (int i = 0; i < ActivatableEntityDetector.holdActivatableEntities.Count; ++i)
                {
                    activatable = ActivatableEntityDetector.holdActivatableEntities[i];
                    if (activatable.CanHoldActivate())
                        return true;
                }
            }
            return false;
        }

        public override bool ShouldShowPickUpButtons()
        {
            if (ItemDropEntityDetector.pickupActivatableEntities.Count > 0)
            {
                IPickupActivatableEntity activatable;
                for (int i = 0; i < ItemDropEntityDetector.pickupActivatableEntities.Count; ++i)
                {
                    activatable = ItemDropEntityDetector.pickupActivatableEntities[i];
                    if (activatable.CanPickupActivate())
                        return true;
                }
            }
            return false;
        }

        public WeaponHandlingState GetWeaponHandlingState(bool isLeftHand)
        {
            WeaponHandlingState weaponHandlingState = WeaponHandlingState.None;
            if (isLeftHand)
                weaponHandlingState |= WeaponHandlingState.IsLeftHand;
            return weaponHandlingState;
        }
    }
}
