using Cysharp.Threading.Tasks;
using Insthync.CameraAndInput;
using Insthync.ManagedUpdating;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace MultiplayerARPG
{
    public partial class ShooterPlayerCharacterController : BasePlayerCharacterController, IShooterWeaponController, IWeaponAbilityController, IManagedLateUpdate
    {
        public const byte PAUSE_FIRE_INPUT_FRAMES_AFTER_CONFIRM_BUILD = 3;

        public enum ControllerMode
        {
            Adventure,
            Combat,
        }

        public enum ExtraMoveActiveMode
        {
            None,
            Toggle,
            Hold
        }

        public enum EmptyAmmoAutoReload
        {
            ReloadImmediately,
            ReloadOnKeysReleased,
            DoNotReload,
        }

        [Header("Camera Controls Prefabs")]
        [SerializeField]
        protected FollowCameraControls gameplayCameraPrefab;
        [SerializeField]
        protected FollowCameraControls minimapCameraPrefab;
        [SerializeField]
        protected Transform fpsModelContainer;
        [SerializeField]
        protected bool updateFpsModelContainerActivating = true;

        [Header("Controller Settings")]
        [SerializeField]
        protected ControllerMode mode;
        [SerializeField]
        protected EmptyAmmoAutoReload emptyAmmoAutoReload;
        [SerializeField]
        protected bool canSwitchViewMode;
        [SerializeField]
        protected ShooterControllerViewMode viewMode;
        [SerializeField]
        protected GameObject[] tpsObjects = new GameObject[0];
        [SerializeField]
        protected GameObject[] fpsObjects = new GameObject[0];
        [SerializeField]
        protected ExtraMoveActiveMode sprintActiveMode;
        [SerializeField]
        protected ExtraMoveActiveMode walkActiveMode;
        [SerializeField]
        protected ExtraMoveActiveMode crouchActiveMode;
        [SerializeField]
        protected ExtraMoveActiveMode crawlActiveMode;
        [SerializeField]
        protected bool unToggleCrouchWhenJump;
        [SerializeField]
        protected bool unToggleCrawlWhenJump;
        [SerializeField]
        protected float findTargetRaycastDistance = 16f;
        [SerializeField]
        protected bool showConfirmConstructionUI = false;
        [SerializeField]
        protected bool buildRotationSnap;
        [SerializeField]
        protected float buildRotateAngle = 45f;
        [SerializeField]
        protected float buildRotateSpeed = 200f;
        [SerializeField]
        protected RectTransform crosshairRect;
        [SerializeField]
        protected bool hideCrosshair = false;
        [SerializeField]
        protected bool disableAttackInSafeArea = false;
        [SerializeField]
        protected bool enableWallHitSpring = true;
        [SerializeField]
        protected string thirdPersonCameraRotationSpeedScaleSaveKey = "3RD_PERSON_CAMERA_SCALE";
        [SerializeField]
        protected string firstPersonCameraRotationSpeedScaleSaveKey = "1ST_PERSON_CAMERA_SCALE";
        [SerializeField]
        protected float sprintDelayAfterActions = 1f;
        [SerializeField]
        protected float walkDelayAfterActions = 1f;

        [Header("TPS Settings")]
        [SerializeField]
        protected float tpsZoomDistance = 3f;
        [SerializeField]
        protected Vector3 tpsTargetOffset = new Vector3(0.75f, 1.25f, 0f);
        [SerializeField]
        protected Vector3 tpsTargetOffsetWhileSprinting = new Vector3(0.75f, 0.5f, 0f);
        [SerializeField]
        protected Vector3 tpsTargetOffsetWhileWalking = new Vector3(0.75f, 0.5f, 0f);
        [SerializeField]
        protected Vector3 tpsTargetOffsetWhileCrouching = new Vector3(0.75f, 0.75f, 0f);
        [SerializeField]
        protected Vector3 tpsTargetOffsetWhileCrawling = new Vector3(0.75f, 0.5f, 0f);
        [SerializeField]
        protected float tpsFov = 60f;
        [SerializeField]
        protected float tpsNearClipPlane = 0.3f;
        [SerializeField]
        protected float tpsFarClipPlane = 1000f;
        [SerializeField]
        protected bool turnForwardWhileDoingAction = true;
        [SerializeField]
        [FormerlySerializedAs("stoppedPlayingAttackOrUseSkillAnimationDelay")]
        protected float durationBeforeStopAimming = 0.5f;
        [SerializeField]
        [Tooltip("Use this to turn character smoothly, Set this <= 0 to turn immediately")]
        protected float turnSpeed = 0f;
        [SerializeField]
        [Tooltip("Use this to turn character smoothly, Set this <= 0 to turn immediately")]
        protected float turnSpeedWhileSprinting = 0f;
        [SerializeField]
        [Tooltip("Use this to turn character smoothly, Set this <= 0 to turn immediately")]
        protected float turnSpeedWhileWalking = 0f;
        [SerializeField]
        [Tooltip("Use this to turn character smoothly, Set this <= 0 to turn immediately")]
        protected float turnSpeedWhileCrouching = 0f;
        [SerializeField]
        [Tooltip("Use this to turn character smoothly, Set this <= 0 to turn immediately")]
        [FormerlySerializedAs("turnSpeedWileCrawling")]
        protected float turnSpeedWhileCrawling = 0f;
        [SerializeField]
        [Tooltip("Use this to turn character smoothly, Set this <= 0 to turn immediately")]
        [FormerlySerializedAs("turnSpeedWileSwimming")]
        protected float turnSpeedWhileSwimming = 0f;
        [SerializeField]
        [Tooltip("Use this to turn character smoothly, Set this <= 0 to turn immediately")]
        [FormerlySerializedAs("turnSpeedWileDoingAction")]
        protected float turnSpeedWhileDoingAction = 0f;

        [Header("TPS Shoulder Settings")]
        [SerializeField]
        protected float shoulderZoomDistance = 0f;
        [SerializeField]
        protected Vector3 shoulderTargetOffset = new Vector3(0f, 0f, 0f);
        [SerializeField]
        protected Vector3 shoulderTargetOffsetWhileSprinting = new Vector3(0f, -0.25f, 0f);
        [SerializeField]
        protected Vector3 shoulderTargetOffsetWhileWalking = new Vector3(0f, -0.25f, 0f);
        [SerializeField]
        protected Vector3 shoulderTargetOffsetWhileCrouching = new Vector3(0f, -0.25f, 0f);
        [SerializeField]
        protected Vector3 shoulderTargetOffsetWhileCrawling = new Vector3(0f, -0.5f, 0f);
        [SerializeField]
        protected float shoulderFov = 50f;
        [SerializeField]
        protected float shoulderNearClipPlane = 0.01f;
        [SerializeField]
        protected float shoulderFarClipPlane = 1000f;

        [Header("FPS Settings")]
        [SerializeField]
        protected float fpsZoomDistance = 0f;
        [SerializeField]
        protected Vector3 fpsTargetOffset = new Vector3(0f, 0f, 0f);
        [SerializeField]
        protected Vector3 fpsTargetOffsetWhileCrouching = new Vector3(0f, -0.25f, 0f);
        [SerializeField]
        protected Vector3 fpsTargetOffsetWhileCrawling = new Vector3(0f, -0.5f, 0f);
        [SerializeField]
        protected float fpsFov = 40f;
        [SerializeField]
        protected float fpsNearClipPlane = 0.01f;
        [SerializeField]
        protected float fpsFarClipPlane = 1000f;

        [Header("Aim Assist Settings")]
        [SerializeField]
        protected bool enableAimAssist = false;
        [SerializeField]
        protected bool enableAimAssistX = false;
        [SerializeField]
        protected bool enableAimAssistY = true;
        [SerializeField]
        protected bool aimAssistOnFireOnly = true;
        [SerializeField]
        protected float aimAssistRadius = 0.5f;
        [SerializeField]
        protected float aimAssistXSpeed = 20f;
        [SerializeField]
        protected float aimAssistYSpeed = 20f;
        [FormerlySerializedAs("aimAssistCharacter")]
        [SerializeField]
        protected bool aimAssistPlayer = true;
        [SerializeField]
        protected bool aimAssistMonster = true;
        [SerializeField]
        protected bool aimAssistBuilding = false;
        [SerializeField]
        protected bool aimAssistHarvestable = false;

        [Header("Recoil Settings")]
        [SerializeField]
        protected float recoilRateWhileMoving = 1.5f;
        [SerializeField]
        protected float recoilRateWhileSprinting = 2f;
        [SerializeField]
        protected float recoilRateWhileWalking = 0.5f;
        [SerializeField]
        protected float recoilRateWhileCrouching = 1f;
        [SerializeField]
        protected float recoilRateWhileCrawling = 0.5f;
        [SerializeField]
        protected float recoilRateWhileSwimming = 0.5f;

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
        public IShooterGameplayCameraController CacheGameplayCameraController { get; protected set; }
        public IMinimapCameraController CacheMinimapCameraController { get; protected set; }
        public BaseCharacterModel CacheFpsModel { get; protected set; }
        public RectTransform CrosshairRect { get => crosshairRect; set => crosshairRect = value; }
        public bool HideCrosshair { get => hideCrosshair; set => hideCrosshair = value; }
        public bool DisableAttackInSafeArea { get => disableAttackInSafeArea; set => disableAttackInSafeArea = value; }
        public bool EnableWallHitSpring { get => enableWallHitSpring; set => enableWallHitSpring = value; }
        public bool IsForceTpsViewMode
        {
            get
            {
                if (TpsViewers.Count > 0)
                {
                    return true;
                }
                return false;
            }
        }
        public bool IsForceFpsViewMode
        {
            get
            {
                if (FpsViewers.Count > 0)
                {
                    return true;
                }
                if (WeaponAbility is ZoomWeaponAbility zoomWeaponAbility && zoomWeaponAbility != null &&
                    (WeaponAbilityState == WeaponAbilityState.Activating || WeaponAbilityState == WeaponAbilityState.Activated))
                {
                    return true;
                }
                return false;
            }
        }
        public ShooterCrosshairUpdater CrosshairUpdater { get; protected set; }
        public ShooterRecoilUpdater RecoilUpdater { get; protected set; }
        public ShooterReloadUpdater ReloadUpdater { get; protected set; }
        public int WeaponAbilityIndex { get; protected set; }
        public BaseWeaponAbility WeaponAbility { get; protected set; }
        public WeaponAbilityState WeaponAbilityState { get; set; }

        public ControllerMode? ForceControllerMode { get; set; }
        public ControllerMode Mode
        {
            get
            {
                if (ActiveViewMode == ShooterControllerViewMode.Fps)
                {
                    // If view mode is fps, controls type must be combat
                    return ControllerMode.Combat;
                }
                if (ForceControllerMode.HasValue)
                    return ForceControllerMode.Value;
                return mode;
            }
        }

        public ShooterControllerViewMode ViewMode
        {
            get { return viewMode; }
            set { viewMode = value; }
        }

        public ShooterControllerViewMode ActiveViewMode
        {
            get
            {
                if (IsForceTpsViewMode)
                    return ShooterControllerViewMode.Tps;
                if (IsForceFpsViewMode)
                    return ShooterControllerViewMode.Fps;
                return ViewMode;
            }
        }

        public float CameraZoomDistance
        {
            get
            {
                switch (ActiveViewMode)
                {
                    case ShooterControllerViewMode.Shoulder:
                        return shoulderZoomDistance;
                    case ShooterControllerViewMode.Fps:
                        return fpsZoomDistance;
                    default:
                        return tpsZoomDistance;
                }
            }
        }

        public float CurrentCameraZoomDistance
        {
            get { return CacheGameplayCameraController.CurrentZoomDistance; }
            set { CacheGameplayCameraController.CurrentZoomDistance = value; }
        }

        public float CurrentCameraMinZoomDistance
        {
            get { return CacheGameplayCameraController.MinZoomDistance; }
            set { CacheGameplayCameraController.MinZoomDistance = value; }
        }

        public float CurrentCameraMaxZoomDistance
        {
            get { return CacheGameplayCameraController.MaxZoomDistance; }
            set { CacheGameplayCameraController.MaxZoomDistance = value; }
        }

        public Vector3 CameraTargetOffset
        {
            get
            {
                switch (ActiveViewMode)
                {
                    case ShooterControllerViewMode.Fps:
                        switch (PlayingCharacterEntity.ExtraMovementState)
                        {
                            case ExtraMovementState.IsCrouching:
                                return fpsTargetOffsetWhileCrouching;
                            case ExtraMovementState.IsCrawling:
                                return fpsTargetOffsetWhileCrawling;
                            default:
                                return fpsTargetOffset;
                        }
                    case ShooterControllerViewMode.Shoulder:
                        switch (PlayingCharacterEntity.ExtraMovementState)
                        {
                            case ExtraMovementState.IsCrouching:
                                return shoulderTargetOffsetWhileCrouching;
                            case ExtraMovementState.IsCrawling:
                                return shoulderTargetOffsetWhileCrawling;
                            case ExtraMovementState.IsSprinting:
                                return shoulderTargetOffsetWhileSprinting;
                            case ExtraMovementState.IsWalking:
                                return shoulderTargetOffsetWhileWalking;
                            default:
                                return shoulderTargetOffset;
                        }
                    default:
                        switch (PlayingCharacterEntity.ExtraMovementState)
                        {
                            case ExtraMovementState.IsCrouching:
                                return tpsTargetOffsetWhileCrouching;
                            case ExtraMovementState.IsCrawling:
                                return tpsTargetOffsetWhileCrawling;
                            case ExtraMovementState.IsSprinting:
                                return tpsTargetOffsetWhileSprinting;
                            case ExtraMovementState.IsWalking:
                                return tpsTargetOffsetWhileWalking;
                            default:
                                return tpsTargetOffset;
                        }
                }
            }
        }

        public float CameraFov
        {
            get
            {
                switch (ActiveViewMode)
                {
                    case ShooterControllerViewMode.Shoulder:
                        return shoulderFov;
                    case ShooterControllerViewMode.Fps:
                        return fpsFov;
                    default:
                        return tpsFov;
                }
            }
        }

        public float CameraNearClipPlane
        {
            get
            {
                switch (ActiveViewMode)
                {
                    case ShooterControllerViewMode.Shoulder:
                        return shoulderNearClipPlane;
                    case ShooterControllerViewMode.Fps:
                        return fpsNearClipPlane;
                    default:
                        return tpsNearClipPlane;
                }
            }
        }

        public float CameraFarClipPlane
        {
            get
            {
                switch (ActiveViewMode)
                {
                    case ShooterControllerViewMode.Shoulder:
                        return shoulderFarClipPlane;
                    case ShooterControllerViewMode.Fps:
                        return fpsFarClipPlane;
                    default:
                        return tpsFarClipPlane;
                }
            }
        }

        public float CurrentCameraFov
        {
            get { return CacheGameplayCameraController.CameraFov; }
            set { CacheGameplayCameraController.CameraFov = value; }
        }

        public float CurrentCameraNearClipPlane
        {
            get { return CacheGameplayCameraController.CameraNearClipPlane; }
            set { CacheGameplayCameraController.CameraNearClipPlane = value; }
        }

        public float CurrentCameraFarClipPlane
        {
            get { return CacheGameplayCameraController.CameraFarClipPlane; }
            set { CacheGameplayCameraController.CameraFarClipPlane = value; }
        }

        public float ThirdPersonCameraRotationSpeedScale
        {
            get { return CameraRotationSpeedScaleSetting.GetCameraRotationSpeedScaleByKey(thirdPersonCameraRotationSpeedScaleSaveKey, 1f); }
        }

        public float FirstPersonCameraRotationSpeedScale
        {
            get { return CameraRotationSpeedScaleSetting.GetCameraRotationSpeedScaleByKey(firstPersonCameraRotationSpeedScaleSaveKey, 0.6f); }
        }

        public float CameraRotationSpeedScale
        {
            get { return CacheGameplayCameraController.CameraRotationSpeedScale; }
            set { CacheGameplayCameraController.CameraRotationSpeedScale = value; }
        }

        public bool IsLeftViewSide
        {
            get { return CacheGameplayCameraController.IsLeftViewSide; }
            set { CacheGameplayCameraController.IsLeftViewSide = value; }
        }

        public bool IsZoomAimming
        {
            get { return CacheGameplayCameraController.IsZoomAimming; }
            set { CacheGameplayCameraController.IsZoomAimming = value; }
        }

        public float CurrentTurnSpeed
        {
            get
            {
                if (PlayingCharacterEntity.MovementState.Has(MovementState.IsUnderWater))
                    return turnSpeedWhileSwimming;
                switch (PlayingCharacterEntity.ExtraMovementState)
                {
                    case ExtraMovementState.IsSprinting:
                        return turnSpeedWhileSprinting;
                    case ExtraMovementState.IsWalking:
                        return turnSpeedWhileWalking;
                    case ExtraMovementState.IsCrouching:
                        return turnSpeedWhileCrouching;
                    case ExtraMovementState.IsCrawling:
                        return turnSpeedWhileCrawling;
                }
                return turnSpeed;
            }
        }
        public byte PauseFireInputFrames { get; set; }
        public bool IsAimming
        {
            get
            {
                return Time.unscaledTime - _lastAimmingTime < durationBeforeStopAimming;
            }
        }
        public Vector2 MoveInput
        {
            get
            {
                if (_moveInput.magnitude > 0f && Mode == ControllerMode.Adventure && !IsAimming)
                {
                    return Vector2.up;
                }
                return _moveInput;
            }
        }

        public readonly HashSet<object> ControllerBlockers = new HashSet<object>();
        public readonly HashSet<object> ActionControllerBlockers = new HashSet<object>();
        public readonly HashSet<object> FollowCameraTurners = new HashSet<object>();
        public readonly HashSet<object> FpsViewers = new HashSet<object>();
        public readonly HashSet<object> TpsViewers = new HashSet<object>();

        // Input data
        protected InputStateManager _activateInput;
        protected InputStateManager _pickupItemInput;
        protected InputStateManager _reloadInput;
        protected InputStateManager _exitVehicleInput;
        protected InputStateManager _switchEquipWeaponSetInput;
        protected float _lastAimmingTime;
        protected bool _updatingInputs;
        // Entity detector
        protected NearbyEntityDetector _warpPortalEntityDetector;
        // Temp physic variables
        protected RaycastHit[] _raycasts = new RaycastHit[100];
        protected Collider[] _overlapColliders = new Collider[200];
        // Temp target
        protected IActivatableEntity _activatableEntity;
        protected IHoldActivatableEntity _holdActivatableEntity;
        // Temp data
        protected Ray _centerRay;
        protected float _centerOriginToCharacterDistance;
        protected Vector3 _moveDirection;
        protected Vector3 _cameraForward;
        protected Vector3 _cameraRight;
        protected Vector3 _cameraEulerAngles;
        protected float _inputV;
        protected float _inputH;
        protected Vector2 _moveInput;
        protected Vector3 _moveLookDirection;
        protected Vector3 _targetLookDirection;
        protected bool _tempPressAttackRight;
        protected bool _tempPressAttackLeft;
        protected bool _tempPressWeaponAbility;
        protected bool _isLeftHandAttacking;
        protected Vector3 _aimTargetPosition;
        protected Vector3 _turnDirection;
        // Controlling states
        protected bool _toggleSprintOn;
        protected bool _toggleWalkOn;
        protected bool _toggleCrouchOn;
        protected bool _toggleCrawlOn;
        protected bool _previouslyCrouch;
        protected bool _previouslyCrawl;
        protected ShooterControllerViewMode _dirtyViewMode;
        protected IWeaponItem _rightHandWeapon;
        protected IWeaponItem _leftHandWeapon;
        protected MovementState _movementState;
        protected ExtraMovementState _extraMovementState;
        protected ShooterControllerViewMode? _viewModeBeforeForcedToSwitch;
        protected bool _mustReleaseFireKey;
        protected bool _isCharging;
        protected float _lastActionTime;

        protected override void Awake()
        {
            base.Awake();
            // Initial gameplay camera controller
            CacheGameplayCameraController = gameObject.GetOrAddComponent<IShooterGameplayCameraController, ShooterGameplayCameraController>((obj) =>
            {
                ShooterGameplayCameraController castedObj = obj as ShooterGameplayCameraController;
                castedObj.SetData(gameplayCameraPrefab);
            });
            CacheGameplayCameraController.Init();
            // TODO: Separated settings for shoulder view mode
            switch (ActiveViewMode)
            {
                case ShooterControllerViewMode.Fps:
                    CameraRotationSpeedScale = FirstPersonCameraRotationSpeedScale;
                    break;
                default:
                    CameraRotationSpeedScale = ThirdPersonCameraRotationSpeedScale;
                    break;
            }
            // Initial minimap camera controller
            CacheMinimapCameraController = gameObject.GetOrAddComponent<IMinimapCameraController, DefaultMinimapCameraController>((obj) =>
            {
                DefaultMinimapCameraController castedObj = obj as DefaultMinimapCameraController;
                castedObj.SetData(minimapCameraPrefab);
            });
            CacheMinimapCameraController.Init();
            // Initial build aim controller
            BuildAimController = gameObject.GetOrAddComponent<IShooterBuildAimController, ShooterBuildAimController>((obj) =>
            {
                ShooterBuildAimController castedObj = obj as ShooterBuildAimController;
                castedObj.SetData(buildRotationSnap, buildRotateAngle, buildRotateSpeed);
            });
            BuildAimController.Init();
            // Initial area skill aim controller
            AreaSkillAimController = gameObject.GetOrAddComponent<IAreaSkillAimController, ShooterAreaSkillAimController>();

            _buildingItemIndex = -1;
            _isLeftHandAttacking = false;
            ConstructingBuildingEntity = null;
            _activateInput = new InputStateManager("Activate");
            _pickupItemInput = new InputStateManager("PickUpItem");
            _reloadInput = new InputStateManager("Reload");
            _exitVehicleInput = new InputStateManager("ExitVehicle");
            _switchEquipWeaponSetInput = new InputStateManager("SwitchEquipWeaponSet");

            // Initialize warp portal entity detector
            GameObject tempGameObject = new GameObject("_WarpPortalEntityDetector");
            _warpPortalEntityDetector = tempGameObject.AddComponent<NearbyEntityDetector>();
            _warpPortalEntityDetector.detectingRadius = CurrentGameInstance.conversationDistance;
            _warpPortalEntityDetector.findWarpPortal = true;

            // Initialize updater
            ReloadUpdater = gameObject.GetOrAddComponent<ShooterReloadUpdater>();
            ReloadUpdater.Controller = this;

            CrosshairUpdater = gameObject.GetOrAddComponent<ShooterCrosshairUpdater>();
            CrosshairUpdater.Controller = this;

            RecoilUpdater = gameObject.GetOrAddComponent<ShooterRecoilUpdater>(comp =>
            {
                comp.recoilRateWhileSwimming = recoilRateWhileSwimming;
                comp.recoilRateWhileSprinting = recoilRateWhileSprinting;
                comp.recoilRateWhileWalking = recoilRateWhileWalking;
                comp.recoilRateWhileMoving = recoilRateWhileMoving;
                comp.recoilRateWhileCrouching = recoilRateWhileCrouching;
                comp.recoilRateWhileCrawling = recoilRateWhileCrawling;
            });
            RecoilUpdater.Controller = this;
        }

        protected override async void Setup(BasePlayerCharacterEntity characterEntity)
        {
            base.Setup(characterEntity);
            CacheGameplayCameraController.Setup(characterEntity);
            CacheMinimapCameraController.Setup(characterEntity);

            if (characterEntity == null)
                return;

            _targetLookDirection = MovementTransform.forward;
            characterEntity.ForceMakeCaches();
            SetupEquipWeapons(characterEntity.EquipWeapons);
            characterEntity.onRecached += SetupEquipWeapons;
            characterEntity.onLaunchDamageEntity += OnLaunchDamageEntity;
            if (CacheFpsModel != null)
                Destroy(CacheFpsModel.gameObject);
            if (fpsModelContainer == null)
            {
                fpsModelContainer = CacheGameplayCameraController.CameraTransform;
                updateFpsModelContainerActivating = false;
            }
            CacheFpsModel = await characterEntity.ModelManager.InstantiateFpsModel(fpsModelContainer);
            await UniTask.NextFrame();
            characterEntity.ModelManager.SetIsFps(ActiveViewMode == ShooterControllerViewMode.Fps);
            UpdateViewMode();
        }

        protected override void Desetup(BasePlayerCharacterEntity characterEntity)
        {
            base.Desetup(characterEntity);
            CacheGameplayCameraController.Desetup(characterEntity);
            CacheMinimapCameraController.Desetup(characterEntity);

            // Clear some cached data
            _rightHandWeapon = null;
            _leftHandWeapon = null;
            if (WeaponAbility != null)
                WeaponAbility.ForceDeactivated();
            WeaponAbility = null;
            WeaponAbilityIndex = 0;
            WeaponAbilityState = WeaponAbilityState.Deactivated;

            if (characterEntity == null)
                return;

            characterEntity.onRecached -= SetupEquipWeapons;
            characterEntity.onLaunchDamageEntity -= OnLaunchDamageEntity;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (_warpPortalEntityDetector != null)
                Destroy(_warpPortalEntityDetector.gameObject);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        protected void SetupEquipWeapons()
        {
            SetupEquipWeapons(PlayingCharacterEntity.EquipWeapons);
        }

        protected virtual void SetupEquipWeapons(EquipWeapons equipWeapons)
        {
            IWeaponItem newRightHandWeapon = equipWeapons.GetRightHandWeaponItem();
            IWeaponItem newLeftHandWeapon = equipWeapons.GetLeftHandWeaponItem();
            bool isSameWeapons = newRightHandWeapon == _rightHandWeapon && newLeftHandWeapon == _leftHandWeapon;
            _rightHandWeapon = newRightHandWeapon;
            _leftHandWeapon = newLeftHandWeapon;
            if (!isSameWeapons)
                ChangeWeaponAbility(0);
        }

        public override void ManagedUpdate()
        {
            // Reset input states
            _moveInput = Vector2.zero;

            if (PauseFireInputFrames > 0)
                --PauseFireInputFrames;

            if (PlayingCharacterEntity == null || !PlayingCharacterEntity.IsOwnerClient)
                return;

            CacheMinimapCameraController.FollowingEntityTransform = CameraTargetTransform;
            CacheMinimapCameraController.FollowingGameplayCameraTransform = CacheGameplayCameraController.CameraTransform;

            CacheGameplayCameraController.ActiveViewMode = ActiveViewMode;
            CacheGameplayCameraController.TargetOffset = CameraTargetOffset;
            CacheGameplayCameraController.EnableWallHitSpring = EnableWallHitSpring && (ActiveViewMode == ShooterControllerViewMode.Tps || ActiveViewMode == ShooterControllerViewMode.Shoulder);
            CacheGameplayCameraController.FollowingEntityTransform = ActiveViewMode == ShooterControllerViewMode.Fps ? PlayingCharacterEntity.FpsCameraTargetTransform : PlayingCharacterEntity.CameraTargetTransform;

            // Set temp data
            float tempDeltaTime = Time.deltaTime;

            // Update inputs
            _activateInput.OnUpdate(tempDeltaTime);
            _pickupItemInput.OnUpdate(tempDeltaTime);
            _reloadInput.OnUpdate(tempDeltaTime);
            _exitVehicleInput.OnUpdate(tempDeltaTime);
            _switchEquipWeaponSetInput.OnUpdate(tempDeltaTime);

            // Check is any UIs block controller or not?
            bool isBlockController = UISceneGameplay.IsBlockController();
            bool isBlockActionController = UISceneGameplay.IsBlockActionController();

            // Lock cursor when not show UIs
            if (GameInstance.IsMobileTestInEditor() || Application.isMobilePlatform)
            {
                // Control camera by touch-screen
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                CacheGameplayCameraController.UpdateRotationX = InputManager.GetButton("CameraRotate");
                CacheGameplayCameraController.UpdateRotationY = InputManager.GetButton("CameraRotate") && PlayingCharacterEntity.CanTurn();
                CacheGameplayCameraController.UpdateRotation = false;
                CacheGameplayCameraController.UpdateZoom = !isBlockController;
            }
            else
            {
                // Control camera by mouse-move
                Cursor.lockState = !isBlockController ? CursorLockMode.Locked : CursorLockMode.None;
                Cursor.visible = isBlockController;
                CacheGameplayCameraController.UpdateRotationX = !isBlockController;
                CacheGameplayCameraController.UpdateRotationY = !isBlockController && PlayingCharacterEntity.CanTurn();
                CacheGameplayCameraController.UpdateRotation = false;
                CacheGameplayCameraController.UpdateZoom = !isBlockController;
            }
            isBlockController |= ControllerBlockers.Count > 0;
            isBlockActionController |= ControllerBlockers.Count > 0;
            isBlockActionController |= ActionControllerBlockers.Count > 0;
            if (InputManager.IsUseNonMobileInput())
                isBlockController |= GenericUtils.IsFocusInputField();

            // Clear selected entity
            SelectedEntity = null;

            // Clear controlling states from last update
            _movementState = MovementState.None;
            _extraMovementState = ExtraMovementState.None;
            switch (ActiveViewMode)
            {
                case ShooterControllerViewMode.Fps:
                    CameraRotationSpeedScale = FirstPersonCameraRotationSpeedScale;
                    break;
                default:
                    CameraRotationSpeedScale = ThirdPersonCameraRotationSpeedScale;
                    break;
            }

            // Prepare variables to find nearest raycasted hit point
            _centerRay = CacheGameplayCameraController.Camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
            _centerOriginToCharacterDistance = Vector3.Distance(_centerRay.origin, EntityTransform.position);
            _cameraForward = CacheGameplayCameraController.CameraTransform.forward;
            _cameraForward.y = 0f;
            _cameraForward.Normalize();
            _cameraRight = CacheGameplayCameraController.CameraTransform.right;
            _cameraRight.y = 0f;
            _cameraRight.Normalize();
            _cameraEulerAngles = CacheGameplayCameraController.CameraTransform.eulerAngles;

            // Update look target and aim position
            if (ConstructingBuildingEntity == null)
                UpdateTarget_BattleMode(isBlockController || isBlockActionController);
            else
                UpdateTarget_BuildMode(isBlockController || isBlockActionController);

            // Update movement inputs
            if (isBlockController)
            {
                // Clear movement inputs
                _moveDirection = Vector3.zero;
                DeactivateWeaponAbility();
            }
            else
            {
                // Update movement and camera pitch
                UpdateMovementInputs();
            }

            // Update aim position
            bool isCharacterTurnForwarding = Mathf.Abs(PlayingCharacterEntity.EntityTransform.eulerAngles.y - _cameraEulerAngles.y) < 15f;
            bool isAimingToCenterOfScreen = Mode == ControllerMode.Combat || turnForwardWhileDoingAction || isCharacterTurnForwarding;
            if (isAimingToCenterOfScreen)
            {
                // Aim to center of screen
                PlayingCharacterEntity.AimPosition = PlayingCharacterEntity.GetAttackAimPosition(ref _isLeftHandAttacking, _aimTargetPosition);
            }
            else
            {
                // Aim to character direction
                Vector3 direction = PlayingCharacterEntity.EntityTransform.forward;
                Vector3 angles = Quaternion.LookRotation(direction, Vector3.up).eulerAngles;
                angles = new Vector3(_cameraEulerAngles.x, angles.y, angles.z);
                direction = Quaternion.Euler(angles) * Vector3.forward;
                PlayingCharacterEntity.AimPosition = PlayingCharacterEntity.GetAttackAimPositionByDirection(ref _isLeftHandAttacking, direction, false);
            }

            // Hide Npc UIs when move
            if (_moveDirection.sqrMagnitude > 0f)
                HideNpcDialog();

            // Update movement state inputs
            if (PlayingCharacterEntity.MovementState.Has(MovementState.IsUnderWater))
            {
                if (!isBlockController)
                {
                    if (InputManager.GetButton("SwimUp"))
                    {
                        _movementState |= MovementState.Up;
                    }
                    else if (InputManager.GetButton("SwimDown"))
                    {
                        _movementState |= MovementState.Down;
                    }
                }
                _extraMovementState = ExtraMovementState.None;
                _toggleSprintOn = false;
                _toggleWalkOn = false;
                _toggleCrouchOn = false;
                _toggleCrawlOn = false;
            }
            else if (PlayingCharacterEntity.MovementState.Has(MovementState.IsGrounded))
            {
                // Crouch
                if (DetectExtraActive("Crouch", crouchActiveMode, isBlockController, ref _toggleCrouchOn))
                {
                    if (_toggleCrouchOn && (_previouslyCrouch || PlayingCharacterEntity.AllowToCrouch()))
                    {
                        _extraMovementState = ExtraMovementState.IsCrouching;
                        _toggleSprintOn = false;
                        _toggleWalkOn = false;
                        _toggleCrawlOn = false;
                    }
                    else
                    {
                        _toggleCrouchOn = false;
                        ClientGenericActions.ClientReceiveGameMessage(UITextKeys.UI_ERROR_UNABLE_TO_CROUCH);
                    }
                }
                // Crawl
                if (DetectExtraActive("Crawl", crawlActiveMode, isBlockController, ref _toggleCrawlOn))
                {
                    if (_toggleCrawlOn && (_previouslyCrawl || PlayingCharacterEntity.AllowToCrawl()))
                    {
                        _extraMovementState = ExtraMovementState.IsCrawling;
                        _toggleSprintOn = false;
                        _toggleWalkOn = false;
                        _toggleCrouchOn = false;
                    }
                    else
                    {
                        _toggleCrawlOn = false;
                        ClientGenericActions.ClientReceiveGameMessage(UITextKeys.UI_ERROR_UNABLE_TO_CRAWL);
                    }
                }
                // Stand animations
                // Stand up from crouching/crawling
                if ((_previouslyCrouch || _previouslyCrawl) && !_toggleCrouchOn && !_toggleCrawlOn)
                {
                    if (PlayingCharacterEntity.AllowToStand())
                    {
                        _extraMovementState = ExtraMovementState.None;
                        _toggleSprintOn = false;
                        _toggleWalkOn = false;
                        _toggleCrouchOn = false;
                        _toggleCrawlOn = false;
                    }
                    else if (_previouslyCrouch)
                    {
                        _extraMovementState = ExtraMovementState.IsCrouching;
                        _toggleCrouchOn = true;
                        ClientGenericActions.ClientReceiveGameMessage(UITextKeys.UI_ERROR_UNABLE_TO_STAND);
                    }
                    else if (_previouslyCrawl)
                    {
                        _extraMovementState = ExtraMovementState.IsCrawling;
                        _toggleCrawlOn = true;
                        ClientGenericActions.ClientReceiveGameMessage(UITextKeys.UI_ERROR_UNABLE_TO_STAND);
                    }
                }
                // Sprinting
                if (PlayingCharacterEntity.MovementState.HasDirectionMovement() &&
                    Time.unscaledTime - _lastActionTime > sprintDelayAfterActions)
                {
                    if ((_extraMovementState == ExtraMovementState.None ||
                        _extraMovementState == ExtraMovementState.IsWalking) &&
                        DetectExtraActive("Sprint", sprintActiveMode, isBlockController, ref _toggleSprintOn))
                    {
                        _extraMovementState = ExtraMovementState.IsSprinting;
                        _toggleWalkOn = false;
                        _toggleCrouchOn = false;
                        _toggleCrawlOn = false;
                    }
                }
                else if (_extraMovementState == ExtraMovementState.IsSprinting)
                {
                    _extraMovementState = ExtraMovementState.None;
                    _toggleSprintOn = false;
                    _toggleWalkOn = false;
                    _toggleCrouchOn = false;
                    _toggleCrawlOn = false;
                }
                // Walking
                if (PlayingCharacterEntity.MovementState.HasDirectionMovement() &&
                    Time.unscaledTime - _lastActionTime > walkDelayAfterActions)
                {
                    if ((_extraMovementState == ExtraMovementState.None ||
                        _extraMovementState == ExtraMovementState.IsWalking) &&
                        DetectExtraActive("Walk", walkActiveMode, isBlockController, ref _toggleWalkOn))
                    {
                        _extraMovementState = ExtraMovementState.IsWalking;
                        _toggleSprintOn = false;
                        _toggleCrouchOn = false;
                        _toggleCrawlOn = false;
                    }
                }
                else if (_extraMovementState == ExtraMovementState.IsWalking)
                {
                    _extraMovementState = ExtraMovementState.None;
                    _toggleSprintOn = false;
                    _toggleWalkOn = false;
                    _toggleCrouchOn = false;
                    _toggleCrawlOn = false;
                }
            }
            else
            {
                _toggleCrouchOn = false;
                _toggleCrawlOn = false;
            }
            _previouslyCrouch = _toggleCrouchOn;
            _previouslyCrawl = _toggleCrawlOn;

            if (!isBlockController)
            {
                if (InputManager.GetButtonDown("Jump"))
                {
                    if (unToggleCrouchWhenJump && PlayingCharacterEntity.ExtraMovementState == ExtraMovementState.IsCrouching)
                    {
                        _toggleCrouchOn = false;
                    }
                    else if (unToggleCrawlWhenJump && PlayingCharacterEntity.ExtraMovementState == ExtraMovementState.IsCrawling)
                    {
                        _toggleCrawlOn = false;
                    }
                    else
                    {
                        _movementState |= MovementState.IsJump;
                    }
                }
                if (InputManager.GetButtonDown("Dash"))
                {
                    _movementState |= MovementState.IsDash;
                }
            }

            // Update input, aimming state will be updated in `UpdateInputs` functions
            if (!_updatingInputs)
            {
                if (ConstructingBuildingEntity == null)
                    UpdateInputs_BattleMode(isBlockController || isBlockActionController).Forget();
                else
                    UpdateInputs_BuildMode(isBlockController || isBlockActionController).Forget();
            }

            if (_moveDirection.magnitude > 0f)
            {
                switch (Mode)
                {
                    case ControllerMode.Adventure:
                        if (IsAimming)
                            _movementState |= GameplayUtils.GetMovementStateByDirection(_moveDirection, MovementTransform.forward);
                        else
                            _movementState |= MovementState.Forward;
                        break;
                    case ControllerMode.Combat:
                        _movementState |= GameplayUtils.GetMovementStateByDirection(_moveDirection, MovementTransform.forward);
                        break;
                }
                if (PlayingCharacterEntity.LadderComponent != null && PlayingCharacterEntity.LadderComponent.ClimbingLadder)
                {
                    _movementState = GameplayUtils.GetMovementStateByDirection(_moveDirection, PlayingCharacterEntity.LadderComponent.ClimbingLadder);
                }
            }

            PlayingCharacterEntity.KeyMovement(_moveDirection, _movementState);
            PlayingCharacterEntity.SetExtraMovementState(_extraMovementState);
            PlayingCharacterEntity.SetSmoothTurnSpeed(0f);

            // View mode switching
            if (canSwitchViewMode)
            {
                if (InputManager.GetButtonDown("SwitchViewMode"))
                {
                    switch (ViewMode)
                    {
                        case ShooterControllerViewMode.Tps:
                            ViewMode = ShooterControllerViewMode.Shoulder;
                            break;
                        case ShooterControllerViewMode.Shoulder:
                            ViewMode = ShooterControllerViewMode.Fps;
                            break;
                        case ShooterControllerViewMode.Fps:
                            ViewMode = ShooterControllerViewMode.Tps;
                            break;
                    }
                    DisableZoomAbility();
                }
                if (InputManager.GetButtonDown("SwitchViewModeTpsFps"))
                {
                    switch (ViewMode)
                    {
                        case ShooterControllerViewMode.Tps:
                        case ShooterControllerViewMode.Shoulder:
                            ViewMode = ShooterControllerViewMode.Fps;
                            break;
                        case ShooterControllerViewMode.Fps:
                            ViewMode = ShooterControllerViewMode.Tps;
                            break;
                    }
                    DisableZoomAbility();
                }
                if (InputManager.GetButtonDown("SwitchViewModeTps"))
                {
                    if (ViewMode != ShooterControllerViewMode.Tps)
                    {
                        ViewMode = ShooterControllerViewMode.Tps;
                        DisableZoomAbility();
                    }
                }
                if (InputManager.GetButtonDown("SwitchViewModeShoulder"))
                {
                    if (ViewMode != ShooterControllerViewMode.Shoulder)
                    {
                        ViewMode = ShooterControllerViewMode.Shoulder;
                    }
                    else
                    {
                        ViewMode = ShooterControllerViewMode.Tps;
                    }
                    DisableZoomAbility();
                }
                if (InputManager.GetButtonDown("SwitchViewModeFps"))
                {
                    if (ViewMode != ShooterControllerViewMode.Fps)
                    {
                        ViewMode = ShooterControllerViewMode.Fps;
                        DisableZoomAbility();
                    }
                }
            }

            if (InputManager.GetButtonDown("SwitchViewSide"))
            {
                IsLeftViewSide = !IsLeftViewSide;
                if (ActiveViewMode == ShooterControllerViewMode.Fps)
                    IsLeftViewSide = false;
            }

            if (InputManager.GetButtonDown("SwitchViewSideLeft"))
            {
                IsLeftViewSide = true;
                if (ActiveViewMode == ShooterControllerViewMode.Fps)
                    IsLeftViewSide = false;
            }

            if (InputManager.GetButtonDown("SwitchViewSideRight"))
            {
                IsLeftViewSide = false;
                if (ActiveViewMode == ShooterControllerViewMode.Fps)
                    IsLeftViewSide = false;
            }

            bool isDead = PlayingCharacterEntity.IsDead();
            if (isDead && WeaponAbility != null && WeaponAbility.ShouldDeactivateOnDead && WeaponAbilityState != WeaponAbilityState.Deactivated)
            {
                WeaponAbility.ForceDeactivated();
                WeaponAbilityState = WeaponAbilityState.Deactivated;
            }
            bool forceToSwitchToTps = isDead || PlayingCharacterEntity.MovementState.Has(MovementState.IsUnderWater);
            if (forceToSwitchToTps)
            {
                // Untoggle all extra movement state
                _toggleSprintOn = false;
                _toggleWalkOn = false;
                _toggleCrouchOn = false;
                _toggleCrawlOn = false;
                // Deactivate weapon ability immediately when dead
                // Set view mode to TPS when character dead
                if (!_viewModeBeforeForcedToSwitch.HasValue)
                    _viewModeBeforeForcedToSwitch = ViewMode;
                ViewMode = ShooterControllerViewMode.Tps;
            }
            else
            {
                // Set view mode to view mode before dead when character alive
                if (_viewModeBeforeForcedToSwitch.HasValue)
                {
                    ViewMode = _viewModeBeforeForcedToSwitch.Value;
                    _viewModeBeforeForcedToSwitch = null;
                }
            }

            // Update weapon ability here to make sure it able to make changes to view mode before apply it
            UpdateWeaponAbilityActivation(tempDeltaTime);

            // Apply view mode updating
            if (_dirtyViewMode != ActiveViewMode)
                UpdateViewMode();
        }

        protected virtual void DisableZoomAbility()
        {
            if (WeaponAbility is ZoomWeaponAbility)
            {
                WeaponAbility.ForceDeactivated();
                WeaponAbilityState = WeaponAbilityState.Deactivated;
            }
        }

        public virtual void ManagedLateUpdate()
        {
            if (PlayingCharacterEntity.MovementState.Has(MovementState.IsUnderWater))
            {
                // Clear toggled sprint, crouch and crawl
                _toggleSprintOn = false;
                _toggleWalkOn = false;
                _toggleCrouchOn = false;
                _toggleCrawlOn = false;
            }
            // Update inputs
            _activateInput.OnLateUpdate();
            _pickupItemInput.OnLateUpdate();
            _reloadInput.OnLateUpdate();
            _exitVehicleInput.OnLateUpdate();
            _switchEquipWeaponSetInput.OnLateUpdate();

            UpdateLookRotation();
        }

        public virtual void UpdateLookRotation()
        {
            if (PlayingCharacterEntity.DisableMovement)
                return;

            _cameraForward = CacheGameplayCameraController.LookForwardTransform.forward;
            _cameraForward.y = 0f;
            _cameraForward.Normalize();

            bool isBlockAction = UISceneGameplay.IsBlockActionController();
            isBlockAction |= ActionControllerBlockers.Count > 0;
            bool isFps = ActiveViewMode == ShooterControllerViewMode.Fps;
            bool isShoulder = ActiveViewMode == ShooterControllerViewMode.Shoulder;
            bool isCombat = Mode == ControllerMode.Combat;
            bool isAdventure = Mode == ControllerMode.Adventure;
            bool isLockLookRotation = IsAimming || isFps || isShoulder;

            if (isBlockAction && isAdventure && !isLockLookRotation)
            {
                _targetLookDirection = _moveLookDirection;
            }
            else if (!isBlockAction && (isLockLookRotation || isCombat))
            {
                _targetLookDirection = _moveLookDirection = _cameraForward;
            }
            if (FollowCameraTurners.Count > 0)
                _targetLookDirection = _moveLookDirection = _cameraForward;
            PlayingCharacterEntity.SetLookRotation(Quaternion.LookRotation(_targetLookDirection), true);
        }

        protected bool DetectExtraActive(string key, ExtraMoveActiveMode activeMode, bool isBlockController, ref bool state)
        {
            if (isBlockController)
            {
                return state;
            }
            switch (activeMode)
            {
                case ExtraMoveActiveMode.Hold:
                    state = InputManager.GetButton(key);
                    break;
                case ExtraMoveActiveMode.Toggle:
                    if (InputManager.GetButtonDown(key))
                        state = !state;
                    break;
            }
            return state;
        }

        protected virtual void UpdateTarget_BattleMode(bool isBlockController)
        {
            // Prepare raycast distance / fov
            float attackDistance;
            bool attacking = false;
            if (IsUsingHotkey())
            {
                _mustReleaseFireKey = true;
            }
            else
            {
                // Attack with right hand weapon
                _tempPressAttackRight = !isBlockController && GetPrimaryAttackButton();
                if (_leftHandWeapon != null)
                {
                    // Attack with left hand weapon if left hand weapon not empty
                    _tempPressAttackLeft = !isBlockController && GetSecondaryAttackButton();
                }
                else if (WeaponAbility != null)
                {
                    // Use weapon ability if it can
                    _tempPressWeaponAbility = !isBlockController && GetSecondaryAttackButtonDown();
                }

                if (DisableAttackInSafeArea && PlayingCharacterEntity.IsInSafeArea)
                {
                    _tempPressAttackRight = false;
                    _tempPressAttackLeft = false;
                }

                attacking = _tempPressAttackRight || _tempPressAttackLeft;
                if (attacking && !PlayingCharacterEntity.IsAttacking && !PlayingCharacterEntity.IsUsingSkill)
                {
                    // Priority is right > left
                    _isLeftHandAttacking = !_tempPressAttackRight && _tempPressAttackLeft;
                }
            }
            // Calculate aim distance by skill or weapon
            if (PlayingCharacterEntity.UsingSkill != null && PlayingCharacterEntity.UsingSkill.IsAttack)
            {
                // Increase aim distance by skill attack distance
                attackDistance = PlayingCharacterEntity.UsingSkill.GetCastDistance(PlayingCharacterEntity, PlayingCharacterEntity.UsingSkillLevel, _isLeftHandAttacking);
                attacking = true;
            }
            else if (_queueUsingSkill.skill != null && _queueUsingSkill.skill.IsAttack)
            {
                // Increase aim distance by skill attack distance
                attackDistance = _queueUsingSkill.skill.GetCastDistance(PlayingCharacterEntity, _queueUsingSkill.level, _isLeftHandAttacking);
                attacking = true;
            }
            else
            {
                // Increase aim distance by attack distance
                attackDistance = PlayingCharacterEntity.GetAttackDistance(_isLeftHandAttacking);
            }
            // Temporary variables
            DamageableHitBox tempHitBox;
            RaycastHit tempHitInfo;
            // Default aim position (aim to sky/space)
            _aimTargetPosition = _centerRay.origin + _centerRay.direction * (_centerOriginToCharacterDistance + attackDistance);
            // Aim to damageable hit boxes (higher priority than other entities)
            // Raycast from camera position to center of screen
            int tempCount = PhysicUtils.SortedRaycastNonAlloc3D(_centerRay.origin, _centerRay.direction, _raycasts, _centerOriginToCharacterDistance + attackDistance, GameInstance.Singleton.GetDamageEntityHitLayerMask());
            for (int tempCounter = 0; tempCounter < tempCount; ++tempCounter)
            {
                tempHitInfo = _raycasts[tempCounter];

                if (!tempHitInfo.collider.GetComponent<IUnHittable>().IsNull())
                {
                    // Don't aim to unhittable objects
                    continue;
                }

                // Get damageable hit box component from hit target
                tempHitBox = tempHitInfo.collider.GetComponent<DamageableHitBox>();
                if (tempHitBox == null || !tempHitBox.Entity)
                {
                    if (GameInstance.Singleton.IsDamageableLayer(tempHitInfo.collider.gameObject.layer))
                    {
                        // Hit something which is part of damageable entities, still continue
                        continue;
                    }

                    // Hit the wall, stop finding target
                    _aimTargetPosition = tempHitInfo.point;
                    break;
                }

                bool isHideFromHost = PlayingCharacterEntity.Identity.IsServer && tempHitBox.Identity.IsHideFrom(PlayingCharacterEntity.Identity);
                if (isHideFromHost || tempHitBox.GetObjectId() == PlayingCharacterEntity.ObjectId)
                {
                    // Skip empty game entity / hidding entity / controlling player's entity
                    continue;
                }

                // Entity isn't in front of character, so it's not the target
                if (turnForwardWhileDoingAction && !IsInFront(tempHitInfo.point))
                    continue;

                // Skip dead entity while attacking (to allow to use resurrect skills)
                if (attacking && tempHitBox.IsDead())
                    continue;

                // Entity is in front of character, so this is target
                if (tempHitBox.CanReceiveDamageFrom(PlayingCharacterEntity.GetInfo()))
                    _aimTargetPosition = tempHitInfo.point;
                SelectedEntity = tempHitBox.Entity;
                break;
            }

            // Aim to activateable entities if it can't find attacking target
            if (SelectedGameEntity == null && !attacking)
            {
                SelectedEntity = null;
                IGameEntity tempGameEntity;
                IBaseActivatableEntity tempActivatableEntity;
                // Raycast from camera position to center of screen
                tempCount = PhysicUtils.SortedRaycastNonAlloc3D(_centerRay.origin, _centerRay.direction, _raycasts, _centerOriginToCharacterDistance + findTargetRaycastDistance, CurrentGameInstance.GetTargetLayerMask());
                for (int tempCounter = 0; tempCounter < tempCount; ++tempCounter)
                {
                    tempHitInfo = _raycasts[tempCounter];
                    if (!tempHitInfo.collider.GetComponent<IUnHittable>().IsNull())
                    {
                        // Don't aim to unhittable objects
                        continue;
                    }

                    // Get distance between character and raycast hit point
                    tempGameEntity = tempHitInfo.collider.GetComponent<IGameEntity>();
                    if (!tempGameEntity.IsNull())
                    {
                        bool isHideFromHost = PlayingCharacterEntity.Identity.IsServer && tempGameEntity.Identity.IsHideFrom(PlayingCharacterEntity.Identity);
                        if (isHideFromHost || tempGameEntity.GetObjectId() == PlayingCharacterEntity.ObjectId)
                        {
                            // Skip hiddeing entity / controlling player's entity
                            continue;
                        }

                        tempActivatableEntity = tempGameEntity as IBaseActivatableEntity;
                        if (tempActivatableEntity != null && Vector3.Distance(EntityTransform.position, tempActivatableEntity.EntityTransform.position) <= tempActivatableEntity.GetActivatableDistance())
                        {
                            // Entity is in front of character, so this is target
                            SelectedEntity = tempActivatableEntity;
                            break;
                        }
                        continue;
                    }

                    tempActivatableEntity = tempHitInfo.collider.GetComponent<IBaseActivatableEntity>();
                    if (tempActivatableEntity != null && Vector3.Distance(EntityTransform.position, tempActivatableEntity.EntityTransform.position) <= tempActivatableEntity.GetActivatableDistance())
                    {
                        // Entity is in front of character, so this is target
                        SelectedEntity = tempActivatableEntity;
                        break;
                    }
                }
            }

            // Calculate aim direction
            _turnDirection = _aimTargetPosition - EntityTransform.position;
            _turnDirection.y = 0f;
            _turnDirection.Normalize();
            // Show target hp/mp
            UISceneGameplay.SetTargetEntity(SelectedGameEntity);
            PlayingCharacterEntity.SetTargetEntity(SelectedGameEntity);
            // Update aim assist
            CacheGameplayCameraController.EnableAimAssist = enableAimAssist && (_tempPressAttackRight || _tempPressAttackLeft || !aimAssistOnFireOnly) && !(SelectedGameEntity is IDamageableEntity);
            CacheGameplayCameraController.EnableAimAssistX = enableAimAssistX;
            CacheGameplayCameraController.EnableAimAssistY = enableAimAssistY;
            CacheGameplayCameraController.AimAssistPlayer = aimAssistPlayer;
            CacheGameplayCameraController.AimAssistMonster = aimAssistMonster;
            CacheGameplayCameraController.AimAssistBuilding = aimAssistBuilding;
            CacheGameplayCameraController.AimAssistHarvestable = aimAssistHarvestable;
            CacheGameplayCameraController.AimAssistRadius = aimAssistRadius;
            CacheGameplayCameraController.AimAssistXSpeed = aimAssistXSpeed;
            CacheGameplayCameraController.AimAssistYSpeed = aimAssistYSpeed;
            CacheGameplayCameraController.AimAssistMaxAngleFromFollowingTarget = 115f;
        }

        protected virtual void UpdateTarget_BuildMode(bool isBlockController)
        {
            // Disable aim assist while constucting the building
            CacheGameplayCameraController.EnableAimAssist = false;
            // Update build aim controller
            (BuildAimController as IShooterBuildAimController)?.UpdateCameraLookData(_centerRay, _centerOriginToCharacterDistance, _cameraForward, _cameraRight);
        }

        protected virtual void UpdateMovementInputs()
        {
            float pitch = _cameraEulerAngles.x;

            // Update charcter look position and pitch
            PlayingCharacterEntity.LookPosition = CacheGameplayCameraController.CameraTransform.position + CacheGameplayCameraController.CameraTransform.forward * findTargetRaycastDistance;
            PlayingCharacterEntity.Pitch = pitch;

            // If mobile platforms, don't receive input raw to make it smooth
            bool raw = !GameInstance.IsMobileTestInEditor() && !Application.isMobilePlatform && !GameInstance.IsConsoleTestInEditor() && !Application.isConsolePlatform;
            _moveDirection = Vector3.zero;
            _inputV = InputManager.GetAxis("Vertical", raw);
            _inputH = InputManager.GetAxis("Horizontal", raw);
            _moveInput = new Vector2(_inputH, _inputV).normalized;
            _moveDirection += _cameraForward * _inputV;
            _moveDirection += _cameraRight * _inputH;

            // Set look direction
            switch (Mode)
            {
                case ControllerMode.Adventure:
                    if (_moveDirection.sqrMagnitude > 0)
                    {
                        _moveLookDirection = _moveDirection;
                        _moveLookDirection.y = 0f;
                    }
                    else
                    {
                        _moveLookDirection = _cameraForward;
                    }
                    break;
                case ControllerMode.Combat:
                    _moveLookDirection = _cameraForward;
                    break;
            }

            _moveDirection.Normalize();
        }

        protected virtual async UniTaskVoid UpdateInputs_BattleMode(bool isBlockController)
        {
            _updatingInputs = true;
            // Prepare fire type data
            FireType rightHandFireType = GameInstance.Singleton.DefaultWeaponItem.FireType;
            if (_rightHandWeapon != null)
            {
                rightHandFireType = _rightHandWeapon.FireType;
            }
            // Prepare fire type data
            FireType leftHandFireType = GameInstance.Singleton.DefaultWeaponItem.FireType;
            if (_leftHandWeapon != null)
            {
                leftHandFireType = _leftHandWeapon.FireType;
            }
            // Have to release fire key, then check press fire key later on next frame
            if (_mustReleaseFireKey)
            {
                _tempPressAttackRight = false;
                _tempPressAttackLeft = false;
                bool isButtonReleased;
                // If release fire key while charging, attack
                isButtonReleased = isBlockController || GetPrimaryAttackButtonUp() || !GetPrimaryAttackButton();
                if (!_isLeftHandAttacking && isButtonReleased)
                {
                    _mustReleaseFireKey = false;
                    await Aimming();
                    // Button released, start attacking while fire type is fire on release
                    if (rightHandFireType == FireType.FireOnRelease)
                        Attack(ref _isLeftHandAttacking);
                    _isCharging = false;
                }
                // If release fire key while charging, attack
                isButtonReleased = isBlockController || GetSecondaryAttackButtonUp() || !GetSecondaryAttackButton();
                if (_isLeftHandAttacking && isButtonReleased)
                {
                    _mustReleaseFireKey = false;
                    await Aimming();
                    // Button released, start attacking while fire type is fire on release
                    if (leftHandFireType == FireType.FireOnRelease)
                        Attack(ref _isLeftHandAttacking);
                    _isCharging = false;
                }
            }
            // Controller blocked, so don't do anything
            if (isBlockController)
            {
                _updatingInputs = false;
                return;
            }

            bool anyKeyPressed = false;
            if (_isCharging ||
                _queueUsingSkill.skill != null ||
                _tempPressAttackRight ||
                _tempPressAttackLeft ||
                _activateInput.IsPress ||
                _activateInput.IsRelease ||
                _activateInput.IsHold ||
                PlayingCharacterEntity.IsPlayingAttackOrUseSkillAnimation())
            {
                // Diable sprinting
                _lastActionTime = Time.unscaledTime;
                if (_extraMovementState == ExtraMovementState.IsSprinting)
                    _extraMovementState = ExtraMovementState.None;
                _toggleSprintOn = false;
                anyKeyPressed = true;
                // Find activatable entities in front of playing character from camera center
                // Check the playing character is playing action animation to turn character forwarding to aim position
                if (!_tempPressAttackRight && !_tempPressAttackLeft)
                {
                    if (_activateInput.IsHold)
                    {
                        _holdActivatableEntity = null;
                        if (SelectedEntity is IHoldActivatableEntity)
                        {
                            _holdActivatableEntity = SelectedEntity as IHoldActivatableEntity;
                        }
                    }
                    else if (_activateInput.IsRelease)
                    {
                        _activatableEntity = null;
                        if (SelectedEntity == null)
                        {
                            if (_warpPortalEntityDetector?.warpPortals.Count > 0)
                            {
                                // It may not able to raycast from inside warp portal, so try to get it from the detector
                                _activatableEntity = _warpPortalEntityDetector.warpPortals[0];
                            }
                        }
                        else
                        {
                            if (SelectedEntity is IActivatableEntity)
                            {
                                _activatableEntity = SelectedEntity as IActivatableEntity;
                            }
                        }
                    }
                }

                // Update look direction
                if (PlayingCharacterEntity.IsPlayingAttackOrUseSkillAnimation() || _isCharging)
                {
                    await Aimming();
                }
                else if (_queueUsingSkill.skill != null)
                {
                    await Aimming();
                    UseSkill(_isLeftHandAttacking);
                }
                else if (_tempPressAttackRight || _tempPressAttackLeft)
                {
                    await Aimming();
                    if (!_isLeftHandAttacking)
                    {
                        // Fire on release weapons have to release to fire, so when start holding, play weapon charge animation
                        if (rightHandFireType == FireType.FireOnRelease)
                        {
                            _isCharging = true;
                            WeaponCharge(ref _isLeftHandAttacking);
                        }
                        else
                        {
                            _isCharging = false;
                            Attack(ref _isLeftHandAttacking);
                        }
                    }
                    else
                    {
                        // Fire on release weapons have to release to fire, so when start holding, play weapon charge animation
                        if (leftHandFireType == FireType.FireOnRelease)
                        {
                            _isCharging = true;
                            WeaponCharge(ref _isLeftHandAttacking);
                        }
                        else
                        {
                            _isCharging = false;
                            Attack(ref _isLeftHandAttacking);
                        }
                    }
                }
                else if (_activateInput.IsHold)
                {
                    if (CanHoldActivate(_holdActivatableEntity))
                    {
                        await Aimming();
                        HoldActivate();
                    }
                }
                else if (_activateInput.IsRelease)
                {
                    if (CanActivate(_activatableEntity))
                    {
                        await Aimming();
                        Activate();
                    }
                }
            }

            if (_tempPressWeaponAbility)
            {
                anyKeyPressed = true;
                // Toggle weapon ability
                switch (WeaponAbilityState)
                {
                    case WeaponAbilityState.Activated:
                    case WeaponAbilityState.Activating:
                        DeactivateWeaponAbility();
                        break;
                    case WeaponAbilityState.Deactivated:
                    case WeaponAbilityState.Deactivating:
                        ActivateWeaponAbility();
                        break;
                }
            }

            if (_pickupItemInput.IsPress)
            {
                anyKeyPressed = true;
                // Find for item to pick up
                if (SelectedEntity is IPickupActivatableEntity pickupActivatableEntity && CanPickupActivate(pickupActivatableEntity))
                {
                    pickupActivatableEntity.OnPickupActivate();
                }
            }

            if (_reloadInput.IsPress)
            {
                anyKeyPressed = true;
                // Reload ammo when press the button
                Reload();
            }

            if (_exitVehicleInput.IsPress)
            {
                anyKeyPressed = true;
                // Exit vehicle
                PlayingCharacterEntity.CallCmdExitVehicle();
            }

            if (_switchEquipWeaponSetInput.IsPress)
            {
                anyKeyPressed = true;
                // Switch equip weapon set
                GameInstance.ClientInventoryHandlers.RequestSwitchEquipWeaponSet(new RequestSwitchEquipWeaponSetMessage()
                {
                    equipWeaponSet = (byte)(PlayingCharacterEntity.EquipWeaponSet + 1),
                }, ClientInventoryActions.ResponseSwitchEquipWeaponSet);
            }

            // Setup releasing state
            if (_tempPressAttackRight && rightHandFireType != FireType.Automatic)
            {
                // The weapon's fire mode is single fire or fire on release, so player have to release fire key for next fire
                _mustReleaseFireKey = true;
            }
            if (_tempPressAttackLeft && leftHandFireType != FireType.Automatic)
            {
                // The weapon's fire mode is single fire or fire on release, so player have to release fire key for next fire
                _mustReleaseFireKey = true;
            }

            // Reloading
            if (PlayingCharacterEntity.EquipWeapons.rightHand.IsAmmoEmpty() ||
                PlayingCharacterEntity.EquipWeapons.leftHand.IsAmmoEmpty())
            {
                switch (emptyAmmoAutoReload)
                {
                    case EmptyAmmoAutoReload.ReloadImmediately:
                        Reload();
                        break;
                    case EmptyAmmoAutoReload.ReloadOnKeysReleased:
                        // Auto reload when ammo empty
                        if (!_tempPressAttackRight && !_tempPressAttackLeft && !_reloadInput.IsPress)
                        {
                            // Reload ammo when empty and not press any keys
                            Reload();
                        }
                        break;
                }
            }

            // Update look direction
            if (!anyKeyPressed)
            {
                // Update look direction while moving without doing any action
                if (IsAimming)
                {
                    await Aimming(false);
                }
                else
                {
                    SetTargetLookDirectionWhileMoving();
                }
            }
            _updatingInputs = false;
        }

        protected virtual UniTaskVoid UpdateInputs_BuildMode(bool isBlockController)
        {
            SetTargetLookDirectionWhileMoving();
            _updatingInputs = false;
            return default;
        }

        public void OnLaunchDamageEntity(
            bool isLeftHand,
            CharacterItem weapon,
            int simulateSeed,
            byte triggerIndex,
            byte spreadIndex,
            List<Dictionary<DamageElement, MinMaxFloat>> damageAmounts,
            BaseSkill skill,
            int skillLevel,
            AimPosition aimPosition)
        {
            CrosshairUpdater.Trigger(isLeftHand, weapon, simulateSeed, triggerIndex, spreadIndex, skill, skillLevel);
            RecoilUpdater.Trigger(isLeftHand, weapon, simulateSeed, triggerIndex, spreadIndex, skill, skillLevel);
        }

        /// <summary>
        /// Turning forwarding and wait character ready to do actions before doing an actions
        /// </summary>
        /// <param name="isAction"></param>
        /// <returns></returns>
        protected virtual async UniTask Aimming(bool isAction = true)
        {
            if (SetTargetLookDirectionWhileDoingAction())
            {
                if (isAction)
                {
                    // This is aimming due to action, not just updating look direction
                    _lastAimmingTime = Time.unscaledTime;
                }
                return;
            }
            do
            {
                _lastAimmingTime = Time.unscaledTime;
                await UniTask.Yield();
            } while (!SetTargetLookDirectionWhileDoingAction());
            _lastAimmingTime = Time.unscaledTime;
        }

        /// <summary>
        /// Return true if it's turned forwarding
        /// </summary>
        /// <returns></returns>
        protected virtual bool SetTargetLookDirectionWhileDoingAction()
        {
            switch (ActiveViewMode)
            {
                case ShooterControllerViewMode.Fps:
                    // It is look forwarding already, so it can do next action
                    return true;
                default:
                    // Just look at camera forward while character playing action animation while `turnForwardWhileDoingAction` is `true`
                    Vector3 doActionLookDirection = turnForwardWhileDoingAction ? _cameraForward : (_moveLookDirection.sqrMagnitude > 0f ? _moveLookDirection : _targetLookDirection);
                    if (turnSpeedWhileDoingAction > 0f)
                    {
                        Quaternion currentRot = Quaternion.LookRotation(_targetLookDirection);
                        Quaternion targetRot = Quaternion.LookRotation(doActionLookDirection);
                        currentRot = Quaternion.Slerp(currentRot, targetRot, turnSpeedWhileDoingAction * Time.deltaTime);
                        _targetLookDirection = currentRot * Vector3.forward;
                        return Quaternion.Angle(currentRot, targetRot) <= 15f;
                    }
                    else
                    {
                        // Turn immediately because turn speed <= 0
                        _targetLookDirection = doActionLookDirection;
                        return true;
                    }
            }
        }

        protected virtual void SetTargetLookDirectionWhileMoving()
        {
            switch (ActiveViewMode)
            {
                case ShooterControllerViewMode.Tps:
                    // Turn character look direction to move direction while moving without doing any action
                    if (_moveDirection.sqrMagnitude > 0f)
                    {
                        float currentTurnSpeed = CurrentTurnSpeed;
                        if (currentTurnSpeed > 0f)
                        {
                            Quaternion currentRot = Quaternion.LookRotation(_targetLookDirection);
                            Quaternion targetRot = Quaternion.LookRotation(_moveLookDirection);
                            currentRot = Quaternion.Slerp(currentRot, targetRot, currentTurnSpeed * Time.deltaTime);
                            _targetLookDirection = currentRot * Vector3.forward;
                        }
                        else
                        {
                            // Turn immediately because turn speed <= 0
                            _targetLookDirection = _moveLookDirection;
                        }
                    }
                    break;
            }
        }

        public override bool UseHotkey(HotkeyType type, string relateId, AimPosition aimPosition)
        {
            ClearQueueUsingSkill();
            bool beingUsed = false;
            switch (type)
            {
                case HotkeyType.Skill:
                    if (onBeforeUseSkillHotkey != null)
                        onBeforeUseSkillHotkey.Invoke(relateId, aimPosition);
                    beingUsed = UseSkill(relateId, aimPosition);
                    if (onAfterUseSkillHotkey != null)
                        onAfterUseSkillHotkey.Invoke(relateId, aimPosition);
                    break;
                case HotkeyType.Item:
                    HotkeyEquipWeaponSet = PlayingCharacterEntity.EquipWeaponSet;
                    if (onBeforeUseItemHotkey != null)
                        onBeforeUseItemHotkey.Invoke(relateId, aimPosition);
                    beingUsed = UseItem(relateId, aimPosition);
                    if (onAfterUseItemHotkey != null)
                        onAfterUseItemHotkey.Invoke(relateId, aimPosition);
                    break;
                case HotkeyType.GuildSkill:
                    beingUsed = UseGuildSkill(relateId);
                    break;
            }
            return beingUsed;
        }

        protected virtual bool UseSkill(string id, AimPosition aimPosition)
        {
            int dataId = BaseGameData.MakeDataId(id);
            if (!GameInstance.Skills.TryGetValue(dataId, out BaseSkill skill) || skill == null ||
                !PlayingCharacterEntity.CachedData.Skills.TryGetValue(skill, out int skillLevel))
                return false;
            SetQueueUsingSkill(aimPosition, skill, skillLevel);
            return true;
        }

        protected virtual bool UseItem(string id, AimPosition aimPosition)
        {
            int itemIndex;
            int dataId = BaseGameData.MakeDataId(id);
            if (GameInstance.Items.TryGetValue(dataId, out BaseItem item))
            {
                itemIndex = GameInstance.PlayingCharacterEntity.IndexOfNonEquipItem(dataId);
            }
            else
            {
                if (PlayingCharacterEntity.IsEquipped(
                    id,
                    out InventoryType inventoryType,
                    out itemIndex,
                    out byte equipWeaponSet,
                    out CharacterItem characterItem))
                {
                    GameInstance.ClientInventoryHandlers.RequestUnEquipItem(
                        inventoryType,
                        itemIndex,
                        equipWeaponSet,
                        -1,
                        ClientInventoryActions.ResponseUnEquipArmor,
                        ClientInventoryActions.ResponseUnEquipWeapon);
                    return true;
                }
                item = characterItem.GetItem();
            }

            if (itemIndex < 0)
                return false;

            if (item == null)
                return false;

            if (item.IsEquipment())
            {
                GameInstance.ClientInventoryHandlers.RequestEquipItem(
                        PlayingCharacterEntity,
                        itemIndex,
                        HotkeyEquipWeaponSet,
                        ClientInventoryActions.ResponseEquipArmor,
                        ClientInventoryActions.ResponseEquipWeapon);
                return true;
            }
            else if (item.IsSkill())
            {
                SetQueueUsingSkill(aimPosition, (item as ISkillItem).SkillData, (item as ISkillItem).SkillLevel, itemIndex);
                return true;
            }
            else if (item.IsBuilding())
            {
                _buildingItemIndex = itemIndex;
                if (showConfirmConstructionUI)
                {
                    // Show confirm UI
                    ShowConstructBuildingDialog();
                }
                else
                {
                    // Build when click
                    // If unable to build, return false
                    if (ConstructingBuildingEntity == null || !ConstructingBuildingEntity.CanBuild())
                        return false;
                    ConfirmBuild();
                }
                _mustReleaseFireKey = true;
                return true;
            }
            else if (item.IsUsable())
            {
                return PlayingCharacterEntity.CallCmdUseItem(itemIndex);
            }

            return false;
        }

        protected bool UseGuildSkill(string id)
        {
            if (GameInstance.JoinedGuild == null)
                return false;
            int dataId = BaseGameData.MakeDataId(id);
            return PlayingCharacterEntity.CallCmdUseGuildSkill(dataId);
        }

        public virtual void Attack(ref bool isLeftHand)
        {
            if (PauseFireInputFrames > 0)
                return;
            WeaponHandlingState weaponHandlingState = GetWeaponHandlingState(isLeftHand);
            if (!PlayingCharacterEntity.Attack(ref weaponHandlingState))
            {
                ReloadUpdater.InterruptByAttacking(weaponHandlingState);
                isLeftHand = weaponHandlingState.Has(WeaponHandlingState.IsLeftHand);
            }
        }

        public virtual void WeaponCharge(ref bool isLeftHand)
        {
            if (PauseFireInputFrames > 0)
                return;
            PlayingCharacterEntity.StartCharge(ref isLeftHand);
        }

        public void Reload()
        {
            ReloadUpdater.Reload();
        }

        public virtual void ChangeWeaponAbility(int index)
        {
            List<BaseWeaponAbility> abilities = PlayingCharacterEntity.GetCaches().RightHandWeaponAbilities;
            bool isSameAbility = WeaponAbility != null && index < abilities.Count && abilities[index] == WeaponAbility;
            if (isSameAbility)
            {
                return;
            }
            if (WeaponAbility != null)
            {
                WeaponAbility.Desetup();
            }
            if (index < abilities.Count)
            {
                WeaponAbilityIndex = index;
            }
            else
            {
                WeaponAbilityIndex = 0;
            }
            WeaponAbility = null;
            if (abilities.Count > 0)
            {
                WeaponAbility = abilities[index];
            }
            if (WeaponAbility != null)
            {
                WeaponAbility.Setup(this, PlayingCharacterEntity.EquipWeapons.rightHand);
            }
            WeaponAbilityState = WeaponAbilityState.Deactivated;
        }

        public virtual void ActivateWeaponAbility()
        {
            if (WeaponAbility == null)
                return;

            if (WeaponAbilityState == WeaponAbilityState.Activated ||
                WeaponAbilityState == WeaponAbilityState.Activating)
                return;

            WeaponAbility.OnPreActivate();
            WeaponAbilityState = WeaponAbilityState.Activating;
        }

        protected virtual void UpdateWeaponAbilityActivation(float deltaTime)
        {
            if (WeaponAbility == null)
                return;

            WeaponAbilityState = WeaponAbility.UpdateActivation(WeaponAbilityState, deltaTime);
        }

        protected virtual void DeactivateWeaponAbility()
        {
            if (WeaponAbility == null)
                return;

            if (WeaponAbilityState == WeaponAbilityState.Deactivated ||
                WeaponAbilityState == WeaponAbilityState.Deactivating)
                return;

            WeaponAbility.OnPreDeactivate();
            WeaponAbilityState = WeaponAbilityState.Deactivating;
        }

        public virtual void HoldActivate()
        {
            if (CanHoldActivate(_holdActivatableEntity))
                _holdActivatableEntity.OnHoldActivate();
        }

        public virtual void Activate()
        {
            if (CanActivate(_activatableEntity))
                _activatableEntity.OnActivate();
        }

        public virtual void UseSkill(bool isLeftHand)
        {
            if (PauseFireInputFrames > 0)
                return;
            if (_queueUsingSkill.skill != null)
            {
                if (_queueUsingSkill.itemIndex >= 0)
                {
                    PlayingCharacterEntity.UseSkillItem(_queueUsingSkill.itemIndex, GetWeaponHandlingState(isLeftHand), SelectedGameEntityObjectId, _queueUsingSkill.aimPosition);
                }
                else
                {
                    PlayingCharacterEntity.UseSkill(_queueUsingSkill.skill.DataId, GetWeaponHandlingState(isLeftHand), SelectedGameEntityObjectId, _queueUsingSkill.aimPosition);
                }
            }
            ClearQueueUsingSkill();
        }

        public int OverlapObjects(Vector3 position, float distance, int layerMask)
        {
            return Physics.OverlapSphereNonAlloc(position, distance, _overlapColliders, layerMask);
        }

        public bool FindTarget(GameObject target, float actDistance, int layerMask)
        {
            int tempCount = OverlapObjects(EntityTransform.position, actDistance, layerMask);
            for (int tempCounter = 0; tempCounter < tempCount; ++tempCounter)
            {
                if (_overlapColliders[tempCounter].gameObject == target)
                    return true;
            }
            return false;
        }

        public bool IsUsingHotkey()
        {
            // Check using hotkey for PC only
            return !InputManager.IsUseMobileInput() && UICharacterHotkeys.UsingHotkey != null;
        }

        public void ForceStandUp()
        {
            _toggleCrouchOn = false;
            _toggleCrawlOn = false;
        }

        public virtual bool GetPrimaryAttackButton()
        {
            return InputManager.GetButton("Fire1") || InputManager.GetButton("Attack");
        }

        public virtual bool GetSecondaryAttackButton()
        {
            return InputManager.GetButton("Fire2");
        }

        public virtual bool GetPrimaryAttackButtonUp()
        {
            return InputManager.GetButtonUp("Fire1") || InputManager.GetButtonUp("Attack");
        }

        public virtual bool GetSecondaryAttackButtonUp()
        {
            return InputManager.GetButtonUp("Fire2");
        }

        public virtual bool GetPrimaryAttackButtonDown()
        {
            return InputManager.GetButtonDown("Fire1") || InputManager.GetButtonDown("Attack");
        }

        public virtual bool GetSecondaryAttackButtonDown()
        {
            return InputManager.GetButtonDown("Fire2");
        }

        public virtual void UpdateViewMode()
        {
            _dirtyViewMode = ActiveViewMode;
            UpdateCameraSettings();
            // Update camera zoom distance when change view mode only, to allow zoom controls
            CurrentCameraZoomDistance = CameraZoomDistance;
        }

        public virtual void UpdateCameraSettings()
        {
            CurrentCameraFov = CameraFov;
            CurrentCameraNearClipPlane = CameraNearClipPlane;
            CurrentCameraFarClipPlane = CameraFarClipPlane;
            if (PlayingCharacterEntity != null && PlayingCharacterEntity.ModelManager != null)
            {
                bool isFps = ActiveViewMode == ShooterControllerViewMode.Fps;
                PlayingCharacterEntity.ModelManager.SetIsFps(isFps);
                if (updateFpsModelContainerActivating && fpsModelContainer.gameObject.activeSelf != isFps)
                    fpsModelContainer.gameObject.SetActive(isFps);
                int i;
                GameObject tempObject;
                for (i = 0; i < tpsObjects.Length; ++i)
                {
                    tempObject = tpsObjects[i];
                    if (tempObject == null || tempObject.activeSelf == !isFps)
                        continue;
                    tempObject.SetActive(!isFps);
                }
                for (i = 0; i < fpsObjects.Length; ++i)
                {
                    tempObject = fpsObjects[i];
                    if (tempObject == null || tempObject.activeSelf == isFps)
                        continue;
                    tempObject.SetActive(isFps);
                }
            }
        }

        public virtual bool IsInFront(Vector3 target)
        {
            // Get aim position direction
            AimPosition aimPosition = PlayingCharacterEntity.GetAttackAimPosition(ref _isLeftHandAttacking, target);
            switch (aimPosition.type)
            {
                case AimPositionType.Direction:
                    // Check that the direction is in front of character or not
                    return Vector3.Angle(aimPosition.direction, EntityTransform.forward) < 115f;
            }
            // 2D mode?
            return true;
        }

        public override void ConfirmBuild()
        {
            base.ConfirmBuild();
            PauseFireInputFrames = PAUSE_FIRE_INPUT_FRAMES_AFTER_CONFIRM_BUILD;
        }

        public WeaponHandlingState GetWeaponHandlingState(bool isLeftHand)
        {
            WeaponHandlingState weaponHandlingState = WeaponHandlingState.None;
            if (isLeftHand)
                weaponHandlingState |= WeaponHandlingState.IsLeftHand;
            if (IsZoomAimming)
                weaponHandlingState |= WeaponHandlingState.IsAiming;
            if (ActiveViewMode == ShooterControllerViewMode.Fps)
                weaponHandlingState |= WeaponHandlingState.IsFpsView;
            if (ActiveViewMode == ShooterControllerViewMode.Shoulder)
                weaponHandlingState |= WeaponHandlingState.IsShoulderView;
            return weaponHandlingState;
        }
    }
}
