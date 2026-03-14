using Insthync.CameraAndInput;
using UnityEngine;

namespace MultiplayerARPG
{
    public partial class PlayerCharacterController
    {
        public const float MIN_START_MOVE_DISTANCE = 0.01f;

        // Input & control states variables
        protected int _findingEnemyIndex = -1;
        protected bool _getMouseUp;
        protected bool _getMouseDown;
        protected bool _getMouse;
        protected bool _isPointerOverUI;
        protected bool _isMouseDragDetected;
        protected bool _isMouseHoldDetected;
        protected bool _isMouseHoldAndNotDrag;
        protected bool _isSprinting;
        protected bool _isWalking;
        protected Vector3 _mouseDownPosition;
        protected float _mouseDownTime;
        protected bool _isMouseDragOrHoldOrOverUI;
        protected Vector3? _targetPosition;
        protected Vector3 _previousPointClickPosition = Vector3.positiveInfinity;
        protected bool _didActionOnTarget;
        protected bool _isBlockControllerLastFrame = false;
        protected bool _isWASDAttackInputLastFrame = false;
        protected Vector3 _moveDirection;

        public int FindClickObjects(out Vector3 worldPosition2D)
        {
            return _physicFunctions.RaycastPickObjects(CacheGameplayCameraController.Camera, InputManager.MousePosition(), CurrentGameInstance.GetTargetLayerMask(), 100f, out worldPosition2D);
        }

        public bool IsUpdatingMobileMovement()
        {
            return InputManager.IsUpdatingMobileAxis("Horizontal") || InputManager.IsUpdatingMobileAxis("Vertical");
        }

        public virtual void UpdateInput()
        {
            bool isFocusInputField = GenericUtils.IsFocusInputField();
            bool isPointerOverUIObject = UISceneGameplay.IsPointerOverUIObject();
            bool isBlockController = UISceneGameplay.IsBlockController();
            if (Application.isMobilePlatform || GameInstance.IsMobileTestInEditor())
            {
                if (uiNotBlockForMobile)
                    isBlockController = false;
            }
            else if (Application.isConsolePlatform || GameInstance.IsConsoleTestInEditor())
            {
                if (uiNotBlockForConsole)
                    isBlockController = false;
            }
            else
            {
                if (uiNotBlockForStandalone)
                    isBlockController = false;
            }
            CacheGameplayCameraController.UpdateRotationX = false;
            CacheGameplayCameraController.UpdateRotationY = false;
            CacheGameplayCameraController.UpdateRotation = !isFocusInputField && !isBlockController && !_isBlockControllerLastFrame && !isPointerOverUIObject && InputManager.GetButton("CameraRotate");
            CacheGameplayCameraController.UpdateZoom = !isFocusInputField && !isBlockController && !_isBlockControllerLastFrame && !isPointerOverUIObject;

            if (isFocusInputField || isBlockController || _isBlockControllerLastFrame || PlayingCharacterEntity.IsDead())
            {
                _isBlockControllerLastFrame = isBlockController;
                PlayingCharacterEntity.KeyMovement(Vector3.zero, MovementState.None);
                return;
            }

            _isBlockControllerLastFrame = isBlockController;

            // If it's building something, don't allow to activate NPC/Warp/Pickup Item
            if (ConstructingBuildingEntity == null)
            {
                // Activate nearby npcs / players / activable buildings
                if (_activateInput.IsHold)
                {
                    if (ActivatableEntityDetector.holdActivatableEntities.Count > 0)
                    {
                        IHoldActivatableEntity holdActivatable;
                        for (int i = 0; i < ActivatableEntityDetector.holdActivatableEntities.Count; ++i)
                        {
                            holdActivatable = ActivatableEntityDetector.holdActivatableEntities[i];
                            if (holdActivatable.CanHoldActivate())
                            {
                                holdActivatable.OnHoldActivate();
                                break;
                            }
                        }
                    }
                }
                else if (_activateInput.IsRelease)
                {
                    if (ActivatableEntityDetector.activatableEntities.Count > 0)
                    {
                        IActivatableEntity activatable;
                        for (int i = 0; i < ActivatableEntityDetector.activatableEntities.Count; ++i)
                        {
                            activatable = ActivatableEntityDetector.activatableEntities[i];
                            if (activatable.CanActivate())
                            {
                                activatable.OnActivate();
                                break;
                            }
                        }
                    }
                }
                // Pick up nearby items
                if (_pickupItemInput.IsPress)
                {
                    if (ItemDropEntityDetector.pickupActivatableEntities.Count > 0)
                    {
                        IPickupActivatableEntity activatable;
                        for (int i = 0; i < ItemDropEntityDetector.pickupActivatableEntities.Count; ++i)
                        {
                            activatable = ItemDropEntityDetector.pickupActivatableEntities[i];
                            if (activatable.CanPickupActivate())
                            {
                                activatable.OnPickupActivate();
                                break;
                            }
                        }
                    }
                }
                // Reload
                if (_reloadInput.IsPress)
                {
                    // Reload ammo when press the button
                    ReloadAmmo();
                }
                // Find target to attack
                if (_findEnemyInput.IsPress)
                {
                    ++_findingEnemyIndex;
                    if (_findingEnemyIndex < 0 || _findingEnemyIndex >= EnemyEntityDetector.characters.Count)
                        _findingEnemyIndex = 0;
                    if (EnemyEntityDetector.characters.Count > 0)
                    {
                        SetTarget(null, TargetActionType.Attack);
                        if (!EnemyEntityDetector.characters[_findingEnemyIndex].IsDeadOrHideFrom(PlayingCharacterEntity))
                        {
                            SetTarget(EnemyEntityDetector.characters[_findingEnemyIndex], TargetActionType.Attack);
                            if (SelectedGameEntity != null)
                            {
                                // Turn character to enemy but does not move or attack yet.
                                TurnCharacterToEntity(SelectedGameEntity);
                            }
                        }
                    }
                }
                if (_exitVehicleInput.IsPress)
                {
                    // Exit vehicle
                    PlayingCharacterEntity.CallCmdExitVehicle();
                }
                if (_switchEquipWeaponSetInput.IsPress)
                {
                    // Switch equip weapon set
                    GameInstance.ClientInventoryHandlers.RequestSwitchEquipWeaponSet(new RequestSwitchEquipWeaponSetMessage()
                    {
                        equipWeaponSet = (byte)(PlayingCharacterEntity.EquipWeaponSet + 1),
                    }, ClientInventoryActions.ResponseSwitchEquipWeaponSet);
                }
                if (PlayingCharacterEntity.MovementState.Has(MovementState.IsUnderWater))
                {
                    _isSprinting = false;
                    _isWalking = false;
                }
                else if (PlayingCharacterEntity.MovementState.Has(MovementState.IsGrounded))
                {
                    if (InputManager.GetButtonDown("Sprint"))
                    {
                        // Toggles sprint state
                        _isSprinting = !_isSprinting;
                        _isWalking = false;
                    }
                    else if (InputManager.GetButtonDown("Walk"))
                    {
                        // Toggles sprint state
                        _isWalking = !_isWalking;
                        _isSprinting = false;
                    }
                }
                // Auto reload
                if (PlayingCharacterEntity.EquipWeapons.rightHand.IsAmmoEmpty() ||
                    PlayingCharacterEntity.EquipWeapons.leftHand.IsAmmoEmpty())
                {
                    // Reload ammo when empty and not press any keys
                    ReloadAmmo();
                }
            }
            // Update enemy detecting radius to attack distance
            EnemyEntityDetector.detectingRadius = PlayingCharacterEntity.GetAttackDistance(false) + distanceToLockActionTarget;
            // Update inputs
            UpdatePointClickInput();
            UpdateWASDInput();
            UpdateWASDAttack();
            UpdateQueuedSkill();
            UpdateTurnToTargetToDoAction();

            // Set extra movement state
            if (_isSprinting)
                PlayingCharacterEntity.SetExtraMovementState(ExtraMovementState.IsSprinting);
            else if (_isWalking)
                PlayingCharacterEntity.SetExtraMovementState(ExtraMovementState.IsWalking);
            else
                PlayingCharacterEntity.SetExtraMovementState(ExtraMovementState.None);
        }

        protected void ReloadAmmo()
        {
            // Reload ammo at server
            if (!PlayingCharacterEntity.EquipWeapons.rightHand.IsAmmoFull(PlayingCharacterEntity))
                PlayingCharacterEntity.Reload(false);
            else if (!PlayingCharacterEntity.EquipWeapons.leftHand.IsAmmoFull(PlayingCharacterEntity))
                PlayingCharacterEntity.Reload(true);
        }

        public virtual void UpdatePointClickInput()
        {
            if (controllerMode == PlayerCharacterControllerMode.WASD)
                return;

            // If it's building something, not allow point click movement
            if (ConstructingBuildingEntity != null)
                return;

            // If it's aiming skills, not allow point click movement
            if (UICharacterHotkeys.UsingHotkey != null)
                return;

            _getMouseDown = InputManager.GetMouseButtonDown(0);
            _getMouseUp = InputManager.GetMouseButtonUp(0);
            _getMouse = InputManager.GetMouseButton(0);

            if (_getMouseDown)
            {
                _isMouseDragOrHoldOrOverUI = false;
                _mouseDownTime = Time.unscaledTime;
                _mouseDownPosition = InputManager.MousePosition();
            }
            // Read inputs
            _isPointerOverUI = UISceneGameplay.IsPointerOverUIObject();
            _isMouseDragDetected = (InputManager.MousePosition() - _mouseDownPosition).sqrMagnitude > DETECT_MOUSE_DRAG_DISTANCE_SQUARED;
            _isMouseHoldDetected = Time.unscaledTime - _mouseDownTime > DETECT_MOUSE_HOLD_DURATION;
            _isMouseHoldAndNotDrag = !_isMouseDragDetected && _isMouseHoldDetected;
            if (!_isMouseDragOrHoldOrOverUI && (_isMouseDragDetected || _isMouseHoldDetected || _isPointerOverUI))
            {
                // Detected mouse dragging or hold on an UIs
                _isMouseDragOrHoldOrOverUI = true;
            }
            bool updatingMobileMovement = IsUpdatingMobileMovement();
            // Will set move target when pointer isn't point on an UIs 
            if (!updatingMobileMovement && !_isPointerOverUI && !_isWASDAttackInputLastFrame && (_getMouse || _getMouseUp))
            {
                // Clear target
                ClearTarget(true);
                _didActionOnTarget = false;
                // Prepare temp variables
                Transform tempTransform;
                bool tempHasMapPosition = false;
                Vector3 tempMapPosition = Vector3.zero;
                // If mouse up while cursor point to target (character, item, npc and so on)
                bool mouseUpOnTarget = _getMouseUp && !_isMouseDragOrHoldOrOverUI;
                int tempCount = FindClickObjects(out Vector3 tempVector3);
                for (int tempCounter = 0; tempCounter < tempCount; ++tempCounter)
                {
                    tempTransform = _physicFunctions.GetRaycastTransform(tempCounter);
                    // When holding on target, or already enter edit building mode
                    if (_isMouseHoldAndNotDrag)
                    {
                        IHoldActivatableEntity activatable = tempTransform.GetComponent<IHoldActivatableEntity>();
                        if (!activatable.IsNull() && activatable.CanHoldActivate())
                        {
                            SetTarget(activatable, TargetActionType.HoldClickActivate);
                            _isFollowingTarget = true;
                            tempHasMapPosition = false;
                            break;
                        }
                    }
                    else if (mouseUpOnTarget)
                    {
                        ITargetableEntity targetable = tempTransform.GetComponent<ITargetableEntity>();
                        IActivatableEntity activatable = targetable as IActivatableEntity;
                        IPickupActivatableEntity pickupActivatable = targetable as IPickupActivatableEntity;
                        IDamageableEntity damageable = targetable as IDamageableEntity;
                        if (!targetable.IsNull() && !targetable.NotBeingSelectedOnClick())
                        {
                            if (!activatable.IsNull() && activatable.CanActivate())
                            {
                                if (activatable.ShouldBeAttackTarget())
                                    SetTarget(activatable, TargetActionType.Attack);
                                else
                                    SetTarget(activatable, TargetActionType.ClickActivate);
                                _isFollowingTarget = true;
                                tempHasMapPosition = false;
                                break;
                            }
                            else if (!pickupActivatable.IsNull() && pickupActivatable.CanPickupActivate())
                            {
                                SetTarget(pickupActivatable, TargetActionType.ClickActivate);
                                _isFollowingTarget = true;
                                tempHasMapPosition = false;
                                break;
                            }
                            else if (damageable != null && !damageable.IsDeadOrHideFrom(PlayingCharacterEntity))
                            {
                                SetTarget(damageable, TargetActionType.Attack);
                                _isFollowingTarget = true;
                                tempHasMapPosition = false;
                                break;
                            }
                        }
                        if (!_physicFunctions.GetRaycastIsTrigger(tempCounter))
                        {
                            // Set clicked map position, it will be used if no activating entity found
                            tempHasMapPosition = true;
                            tempMapPosition = _physicFunctions.GetRaycastPoint(tempCounter);
                            break;
                        }
                    } // End mouseUpOnTarget
                }
                // When clicked on map (Not touch any game entity)
                // - Clear selected target to hide selected entity UIs
                // - Set target position to position where mouse clicked
                if (tempHasMapPosition)
                {
                    SelectedEntity = null;
                    _targetPosition = tempMapPosition;
                }
                // When clicked on map (any non-collider position)
                // tempVector3 is come from FindClickObjects()
                // - Clear character target to make character stop doing actions
                // - Clear selected target to hide selected entity UIs
                // - Set target position to position where mouse clicked
                if (CurrentGameInstance.DimensionType == DimensionType.Dimension2D && mouseUpOnTarget && tempCount == 0)
                {
                    ClearTarget();
                    tempVector3.z = 0;
                    _targetPosition = tempVector3;
                }

                // Found ground position
                if (_targetPosition.HasValue)
                {
                    // Close NPC dialog, when target changes
                    HideNpcDialog();
                    ClearQueueUsingSkill();
                    _isFollowingTarget = false;
                    if (PlayingCharacterEntity.IsPlayingActionAnimation())
                    {
                        if (pointClickInterruptCastingSkill)
                            PlayingCharacterEntity.InterruptCastingSkill();
                    }
                    else
                    {
                        OnPointClickOnGround(_targetPosition.Value);
                    }
                }
            }
        }

        /// <summary>
        /// When point click on ground, move target to the position
        /// </summary>
        /// <param name="targetPosition"></param>
        protected virtual void OnPointClickOnGround(Vector3 targetPosition)
        {
            if (Vector3.Distance(MovementTransform.position, targetPosition) > MIN_START_MOVE_DISTANCE)
            {
                _destination = targetPosition;
                PlayingCharacterEntity.PointClickMovement(targetPosition);
            }
        }

        protected virtual void SetTarget(ITargetableEntity entity, TargetActionType targetActionType, bool checkControllerMode = true)
        {
            _targetPosition = null;
            if (checkControllerMode && controllerMode == PlayerCharacterControllerMode.WASD)
            {
                if (_targetActionType != targetActionType)
                    _previousTargetActionType = _targetActionType;
                _targetActionType = targetActionType;
                _destination = null;
                SelectedEntity = entity;
                return;
            }
            if (pointClickSetTargetImmediately ||
                (entity != null && SelectedEntity != null && entity.EntityGameObject == SelectedEntity.EntityGameObject) ||
                (entity != null && entity.SetAsTargetInOneClick()))
            {
                if (_targetActionType != targetActionType)
                    _previousTargetActionType = _targetActionType;
                _targetActionType = targetActionType;
                _destination = null;
                TargetEntity = entity;
                if (entity is IGameEntity gameEntity)
                    PlayingCharacterEntity.SetTargetEntity(gameEntity.Entity);
            }
            SelectedEntity = entity;
        }

        protected virtual void ClearTarget(bool exceptSelectedTarget = false)
        {
            if (!exceptSelectedTarget)
                SelectedEntity = null;
            TargetEntity = null;
            PlayingCharacterEntity.SetTargetEntity(null);
            _targetPosition = null;
            _targetActionType = _previousTargetActionType = TargetActionType.ClickActivate;
        }

        public override void DeselectBuilding()
        {
            base.DeselectBuilding();
            ClearTarget();
        }

        public virtual void UpdateWASDInput()
        {
            if (controllerMode == PlayerCharacterControllerMode.PointClick)
                return;

            // If mobile platforms, don't receive input raw to make it smooth
            bool raw = !GameInstance.IsMobileTestInEditor() && !Application.isMobilePlatform && !GameInstance.IsConsoleTestInEditor() && !Application.isConsolePlatform;
            _moveDirection = GetMoveDirection(InputManager.GetAxis("Horizontal", raw), InputManager.GetAxis("Vertical", raw)).normalized;

            // Set movement state to be forward only when it is having moving direction
            MovementState movementState = MovementState.None;
            // Move
            if (_moveDirection.sqrMagnitude > 0f)
            {
                HideNpcDialog();
                _destination = null;
                _isFollowingTarget = false;
                if (TargetGameEntity != null && Vector3.Distance(EntityTransform.position, TargetGameEntity.EntityTransform.position) >= wasdClearTargetDistance)
                {
                    // Clear target when character moved far from target
                    ClearTarget();
                }
                if (_turnToTargetActionType == TargetActionType.None && !PlayingCharacterEntity.IsPlayingAttackOrUseSkillAnimation())
                    PlayingCharacterEntity.SetLookRotation(Quaternion.LookRotation(_moveDirection), false);
                movementState = MovementState.Forward;
            }

            if (PlayingCharacterEntity.MovementState.Has(MovementState.IsUnderWater))
            {
                if (InputManager.GetButton("SwimUp"))
                {
                    movementState |= MovementState.Up;
                }
                else if (InputManager.GetButton("SwimDown"))
                {
                    movementState |= MovementState.Down;
                }
            }
            else if (PlayingCharacterEntity.MovementState.Has(MovementState.IsGrounded))
            {
                if (InputManager.GetButtonDown("Jump"))
                {
                    movementState |= MovementState.IsJump;
                }
                else if (InputManager.GetButtonDown("Dash"))
                {
                    movementState |= MovementState.IsDash;
                }
            }
            if (_moveDirection.sqrMagnitude > 0 && PlayingCharacterEntity.LadderComponent != null && PlayingCharacterEntity.LadderComponent.ClimbingLadder)
            {
                movementState = GameplayUtils.GetMovementStateByDirection(_moveDirection, PlayingCharacterEntity.LadderComponent.ClimbingLadder);
            }
            PlayingCharacterEntity.KeyMovement(_moveDirection, movementState);
        }

        protected void UpdateWASDAttack()
        {
            _isWASDAttackInputLastFrame = false;

            if (controllerMode == PlayerCharacterControllerMode.PointClick)
                return;

            if (PlayingCharacterEntity.IsPlayingActionAnimation())
                return;

            if (!InputManager.GetButton("Attack"))
                return;

            _isWASDAttackInputLastFrame = true;

            _destination = null;

            BaseCharacterEntity targetEntity;
            if (TryGetSelectedTargetAsAttackingEntity(out targetEntity))
                SetTarget(targetEntity, TargetActionType.Attack, false);

            int overlapMask = CurrentGameInstance.playerLayer.Mask | CurrentGameInstance.monsterLayer.Mask;
            if (wasdLockAttackTarget)
            {
                if (!TryGetAttackingEntity(out targetEntity) || targetEntity.IsDeadOrHideFrom(PlayingCharacterEntity))
                {
                    // Find nearest target and move to the target
                    targetEntity = PlayingCharacterEntity
                        .FindNearestAliveEntity<BaseCharacterEntity>(
                        PlayingCharacterEntity.GetAttackDistance(_isLeftHandAttacking) + distanceToLockActionTarget,
                        false,
                        true,
                        false,
                        overlapMask);
                }
                if (targetEntity != null && !targetEntity.IsDeadOrHideFrom(PlayingCharacterEntity))
                {
                    // Set target, then attack later when moved nearby target
                    SelectedEntity = targetEntity;
                    SetTarget(targetEntity, TargetActionType.Attack, false);
                    _isFollowingTarget = true;
                }
                else
                {
                    // No nearby target, so attack immediately
                    RequestAttack();
                    _isFollowingTarget = false;
                }
            }
            else
            {
                // Find nearest target and set selected target to show character hp/mp UIs
                SelectedEntity = PlayingCharacterEntity
                    .FindNearestAliveEntity<BaseCharacterEntity>(
                    PlayingCharacterEntity.GetAttackDistance(_isLeftHandAttacking),
                    false,
                    true,
                    false,
                    overlapMask);
                if (SelectedGameEntity != null)
                {
                    // Look at target and attack
                    TurnCharacterToEntityToAttack(SelectedGameEntity);
                }
                else
                {
                    // Not lock target, so not finding target and attack immediately
                    RequestAttack();
                }
                _isFollowingTarget = false;
            }
        }

        protected void UpdateQueuedSkill()
        {
            if (_queueUsingSkill.skill == null || _queueUsingSkill.level <= 0)
                return;

            _destination = null;
            BaseSkill skill = _queueUsingSkill.skill;
            int skillLevel = _queueUsingSkill.level;
            BaseCharacterEntity targetEntity;
            // Point click mode always lock on target
            bool wasdLockAttackTarget = this.wasdLockAttackTarget || controllerMode == PlayerCharacterControllerMode.PointClick;

            if (skill.HasCustomAimControls() && _queueUsingSkill.aimPosition.type == AimPositionType.Position)
            {
                // Target not required, use skill immediately
                TurnCharacterToPositionToUsePendingSkill(_queueUsingSkill.aimPosition.position);
                _isFollowingTarget = false;
                return;
            }

            if (skill.IsAttack)
            {
                int overlapMask = CurrentGameInstance.playerLayer.Mask | CurrentGameInstance.monsterLayer.Mask;
                if (wasdLockAttackTarget)
                {
                    if (!TryGetSelectedTargetAsAttackingEntity(out targetEntity) || targetEntity.IsDeadOrHideFrom(PlayingCharacterEntity))
                    {
                        // Try find nearby enemy if no selected target or selected taget is not enemy or target is hide or dead
                        targetEntity = PlayingCharacterEntity
                            .FindNearestAliveEntity<BaseCharacterEntity>(
                            skill.GetCastDistance(PlayingCharacterEntity, skillLevel, _isLeftHandAttacking) + distanceToLockActionTarget,
                            false,
                            true,
                            false,
                            overlapMask);
                    }
                    if (targetEntity != null && !targetEntity.IsDeadOrHideFrom(PlayingCharacterEntity))
                    {
                        // Set target, then use skill later when moved nearby target
                        SelectedEntity = targetEntity;
                        if (!skill.CanUse(PlayingCharacterEntity, skillLevel, false, SelectedGameEntityObjectId, out UITextKeys gameMessage, _queueUsingSkill.itemIndex >= 0))
                        {
                            ClientGenericActions.ClientReceiveGameMessage(gameMessage);
                            ClearQueueUsingSkill();
                            return;
                        }
                        SetTarget(targetEntity, TargetActionType.UseSkill, false);
                        _isFollowingTarget = true;
                    }
                    else
                    {
                        // No nearby target, so use skill immediately
                        RequestUsePendingSkill();
                        _isFollowingTarget = false;
                    }
                }
                else
                {
                    // Find nearest target and set selected target to show character hp/mp UIs
                    SelectedEntity = PlayingCharacterEntity
                        .FindNearestAliveEntity<BaseCharacterEntity>(
                        skill.GetCastDistance(PlayingCharacterEntity, skillLevel, _isLeftHandAttacking),
                        false,
                        true,
                        false,
                        overlapMask);
                    if (!skill.CanUse(PlayingCharacterEntity, skillLevel, false, SelectedGameEntityObjectId, out UITextKeys gameMessage, _queueUsingSkill.itemIndex >= 0))
                    {
                        ClientGenericActions.ClientReceiveGameMessage(gameMessage);
                        ClearQueueUsingSkill();
                        return;
                    }
                    if (SelectedGameEntity != null)
                    {
                        // Look at target and use skill
                        TurnCharacterToEntityToUsePendingSkill(SelectedGameEntity);
                    }
                    else
                    {
                        // Not lock target, so use skill immediately
                        RequestUsePendingSkill();
                    }
                    _isFollowingTarget = false;
                }
            }
            else
            {
                // Not attack skill, so use skill immediately
                if (skill.RequiredTarget)
                {
                    if (SelectedGameEntity == null)
                    {
                        RequestUsePendingSkill();
                        _isFollowingTarget = false;
                        return;
                    }
                    if (wasdLockAttackTarget)
                    {
                        // Set target, then use skill later when moved nearby target
                        if (SelectedGameEntity is BaseCharacterEntity)
                        {
                            if (!skill.CanUse(PlayingCharacterEntity, skillLevel, false, SelectedGameEntityObjectId, out UITextKeys gameMessage, _queueUsingSkill.itemIndex >= 0))
                            {
                                ClientGenericActions.ClientReceiveGameMessage(gameMessage);
                                ClearQueueUsingSkill();
                                return;
                            }
                            SetTarget(SelectedGameEntity, TargetActionType.UseSkill, false);
                            _isFollowingTarget = true;
                        }
                        else
                        {
                            ClearQueueUsingSkill();
                            _isFollowingTarget = false;
                        }
                    }
                    else
                    {
                        // Try apply skill to selected entity immediately, it will fail if selected entity is far from the character
                        if (SelectedGameEntity is BaseCharacterEntity)
                        {
                            if (!skill.CanUse(PlayingCharacterEntity, skillLevel, false, SelectedGameEntityObjectId, out UITextKeys gameMessage, _queueUsingSkill.itemIndex >= 0))
                            {
                                ClientGenericActions.ClientReceiveGameMessage(gameMessage);
                                ClearQueueUsingSkill();
                                return;
                            }
                            if (SelectedGameEntity != null && SelectedGameEntity != PlayingCharacterEntity)
                            {
                                // Look at target and use skill
                                TurnCharacterToEntityToUsePendingSkill(SelectedGameEntity);
                            }
                            else
                            {
                                // Not lock target, so use skill immediately
                                RequestUsePendingSkill();
                            }
                            _isFollowingTarget = false;
                        }
                        else
                        {
                            ClearQueueUsingSkill();
                            _isFollowingTarget = false;
                        }
                    }
                }
                else
                {
                    // Target not required, use skill immediately
                    RequestUsePendingSkill();
                }
            }
        }

        public void UpdateFollowTarget()
        {
            if (!_isFollowingTarget)
                return;

            IDamageableEntity targetDamageable;
            IActivatableEntity activatableEntity;
            IHoldActivatableEntity holdActivatableEntity;
            IPickupActivatableEntity pickupActivatableEntity;
            if (TryGetAttackingEntity(out targetDamageable))
            {
                if (targetDamageable.IsDeadOrHideFrom(PlayingCharacterEntity))
                {
                    ClearQueueUsingSkill();
                    PlayingCharacterEntity.StopMove();
                    ClearTarget();
                    return;
                }
                float attackDistance;
                float attackFov;
                GetAttackDistanceAndFov(_isLeftHandAttacking, out attackDistance, out attackFov);
                AttackOrMoveToEntity(targetDamageable, attackDistance, CurrentGameInstance.playerLayer.Mask | CurrentGameInstance.monsterLayer.Mask);
            }
            else if (TryGetUsingSkillEntity(out targetDamageable))
            {
                if (_queueUsingSkill.skill.IsAttack && targetDamageable.IsDeadOrHideFrom(PlayingCharacterEntity))
                {
                    ClearQueueUsingSkill();
                    PlayingCharacterEntity.StopMove();
                    ClearTarget();
                    return;
                }
                float castDistance;
                float castFov;
                GetUseSkillDistanceAndFov(_isLeftHandAttacking, out castDistance, out castFov);
                UseSkillOrMoveToEntity(targetDamageable, castDistance);
            }
            else if (TryGetDoActionEntity(out activatableEntity, TargetActionType.ClickActivate))
            {
                DoActionOrMoveToEntity(activatableEntity, activatableEntity.GetActivatableDistance(), () =>
                {
                    if (activatableEntity.ShouldNotActivateAfterFollowed())
                        return;
                    if (!_didActionOnTarget)
                    {
                        _didActionOnTarget = true;
                        if (activatableEntity.CanActivate())
                            activatableEntity.OnActivate();
                        if (activatableEntity.ShouldClearTargetAfterActivated())
                            ClearTarget();
                    }
                });
            }
            else if (TryGetDoActionEntity(out holdActivatableEntity, TargetActionType.HoldClickActivate))
            {
                DoActionOrMoveToEntity(holdActivatableEntity, holdActivatableEntity.GetActivatableDistance(), () =>
                {
                    if (!_didActionOnTarget)
                    {
                        _didActionOnTarget = true;
                        if (holdActivatableEntity.CanHoldActivate())
                            holdActivatableEntity.OnHoldActivate();
                        if (holdActivatableEntity.ShouldClearTargetAfterActivated())
                            ClearTarget();
                    }
                });
            }
            else if (TryGetDoActionEntity(out pickupActivatableEntity, TargetActionType.ClickActivate))
            {
                DoActionOrMoveToEntity(pickupActivatableEntity, pickupActivatableEntity.GetActivatableDistance(), () =>
                {
                    if (!_didActionOnTarget)
                    {
                        _didActionOnTarget = true;
                        if (pickupActivatableEntity.CanPickupActivate())
                            pickupActivatableEntity.OnPickupActivate();
                        if (pickupActivatableEntity.ShouldClearTargetAfterActivated())
                            ClearTarget();
                    }
                });
            }
        }

        protected void UpdateTurnToTargetToDoAction()
        {
            if (TargetGameEntity == null && !_turnToTargetPosition.HasValue)
            {
                _turnToTargetActionType = TargetActionType.None;
                return;
            }
            if (_turnToTargetActionType != TargetActionType.None)
            {
                Vector3 targetPosition = _turnToTargetPosition.HasValue ? _turnToTargetPosition.Value : TargetGameEntity.EntityTransform.position;
                Vector3 lookAtDir = (targetPosition - EntityTransform.position).normalized;
                Quaternion lookAtRot = Quaternion.LookRotation(lookAtDir);
                PlayingCharacterEntity.SetLookRotation(lookAtRot, false);
                float currentAngle = Quaternion.Angle(Quaternion.LookRotation(PlayingCharacterEntity.EntityTransform.forward), lookAtRot);
                if (currentAngle <= 15f)
                {
                    switch (_turnToTargetActionType)
                    {
                        case TargetActionType.Attack:
                            RequestAttack();
                            OnAttackOnEntity();
                            _turnToTargetActionType = TargetActionType.ActionRequested;
                            break;
                        case TargetActionType.UseSkill:
                            RequestUsePendingSkill();
                            OnUseSkillOnEntity();
                            _turnToTargetActionType = TargetActionType.ActionRequested;
                            break;
                        case TargetActionType.ActionRequested:
                            if (!PlayingCharacterEntity.IsPlayingAttackOrUseSkillAnimation())
                            {
                                _turnToTargetActionType = TargetActionType.None;
                                _turnToTargetPosition = null;
                            }
                            break;
                    }
                }
            }
        }

        protected virtual bool TargetIsInActivateDistance(Vector3 sourcePosition, Vector3 targetPosition, float distance)
        {
            distance -= activateDistanceBuffer;
            return Vector3.Distance(sourcePosition, targetPosition) <= distance;
        }

        protected virtual bool OverlappedEntity(ITargetableEntity entity, Vector3 sourcePosition, Vector3 targetPosition, float distance)
        {
            if (TargetIsInActivateDistance(sourcePosition, targetPosition, distance))
                return true;
            // Target is far from controlling entity, try overlap the entity
            if (entity == null)
                return false;
            return _physicFunctions.IsGameEntityInDistance(entity, sourcePosition, distance, false);
        }

        protected virtual bool OverlappedEntityHitBox<T>(T entity, Vector3 sourcePosition, Vector3 targetPosition, float distance)
            where T : BaseGameEntity
        {
            if (TargetIsInActivateDistance(sourcePosition, targetPosition, distance))
                return true;
            // Target is far from controlling entity, try overlap the entity
            if (entity == null)
                return false;
            return _physicFunctions.IsGameEntityHitBoxInDistance(entity, sourcePosition, distance, false);
        }

        protected virtual void DoActionOrMoveToEntity(ITargetableEntity entity, float distance, System.Action action)
        {
            if (entity.IsNull())
                return;
            Vector3 sourcePosition = EntityTransform.position;
            Vector3 targetPosition = entity.EntityTransform.position;
            if (OverlappedEntity(entity, sourcePosition, targetPosition, distance))
            {
                // Stop movement to do action
                PlayingCharacterEntity.StopMove();
                // Do action
                action.Invoke();
                // This function may be used by extending classes
                OnDoActionOnEntity();
            }
            else
            {
                // Move to target entity
                UpdateTargetEntityPosition(sourcePosition, targetPosition, distance);
            }
        }

        protected virtual void OnDoActionOnEntity()
        {

        }

        protected virtual void AttackOrMoveToEntity(IDamageableEntity entity, float distance, int layerMask)
        {
            Transform damageTransform = PlayingCharacterEntity.GetAvailableWeaponDamageInfo(ref _isLeftHandAttacking).GetDamageTransform(PlayingCharacterEntity, _isLeftHandAttacking);
            Vector3 sourcePosition = damageTransform.position;
            Vector3 targetPosition = entity.OpponentAimTransform.position;
            if (OverlappedEntityHitBox(entity.Entity, sourcePosition, targetPosition, distance))
            {
                // Stop movement to attack
                PlayingCharacterEntity.StopMove();
                // Turn character to attacking target
                TurnCharacterToEntityToAttack(entity.Entity);
            }
            else
            {
                // Move to target entity
                UpdateTargetEntityPosition(sourcePosition, targetPosition, distance);
            }
        }

        protected virtual void OnAttackOnEntity()
        {

        }

        protected virtual void UseSkillOrMoveToEntity(IDamageableEntity entity, float distance)
        {
            if (_queueUsingSkill.skill != null)
            {
                Transform applyTransform = _queueUsingSkill.skill.GetApplyTransform(PlayingCharacterEntity, _isLeftHandAttacking);
                Vector3 sourcePosition = applyTransform.position;
                Vector3 targetPosition = entity.OpponentAimTransform.position;
                if (entity.GetObjectId() == PlayingCharacterEntity.ObjectId /* Applying skill to user? */ ||
                    OverlappedEntityHitBox(entity.Entity, sourcePosition, targetPosition, distance))
                {
                    // Set next frame target action type
                    _targetActionType = _queueUsingSkill.skill.IsAttack ? TargetActionType.Attack : _previousTargetActionType;
                    // Stop movement to use skill
                    PlayingCharacterEntity.StopMove();
                    // Turn character to attacking target
                    TurnCharacterToEntityToUsePendingSkill(entity.Entity);
                }
                else
                {
                    // Move to target entity
                    UpdateTargetEntityPosition(sourcePosition, targetPosition, distance);
                }
            }
            else
            {
                // Can't use skill
                _targetActionType = _previousTargetActionType;
                ClearQueueUsingSkill();
                return;
            }
        }

        protected virtual void OnUseSkillOnEntity()
        {

        }

        protected virtual void UpdateTargetEntityPosition(Vector3 sourcePosition, Vector3 targetPosition, float distance)
        {
            Vector3 direction = (targetPosition - sourcePosition).normalized;
            float activateDistance = distance - StoppingDistance - activateDistanceBuffer;
            if (activateDistance <= 0f)
                activateDistance = StoppingDistance * 0.5f;
            Vector3 position = targetPosition - (direction * activateDistance);
            if (Vector3.Distance(MovementTransform.position, position) > MIN_START_MOVE_DISTANCE &&
                Vector3.Distance(_previousPointClickPosition, position) > MIN_START_MOVE_DISTANCE)
            {
                PlayingCharacterEntity.PointClickMovement(position);
                _previousPointClickPosition = position;
            }
            _turnToTargetActionType = TargetActionType.None;
            _turnToTargetPosition = null;
        }

        protected void TurnCharacterToEntity(BaseGameEntity entity)
        {
            if (entity == null)
                return;
            TurnCharacterToPosition(entity.EntityTransform.position);
        }

        protected void TurnCharacterToPosition(Vector3 position)
        {
            Vector3 lookAtDirection = (position - EntityTransform.position).normalized;
            if (lookAtDirection.sqrMagnitude > 0)
                PlayingCharacterEntity.SetLookRotation(Quaternion.LookRotation(lookAtDirection), false);
        }

        protected void TurnCharacterToEntityToAttack(BaseGameEntity entity)
        {
            _turnToTargetActionType = TargetActionType.Attack;
            _turnToTargetPosition = null;
            SelectedEntity = entity;
        }

        protected void TurnCharacterToPositionToAttack(Vector3 position)
        {
            _turnToTargetActionType = TargetActionType.Attack;
            _turnToTargetPosition = position;
        }

        protected void TurnCharacterToEntityToUsePendingSkill(BaseGameEntity entity)
        {
            _turnToTargetActionType = TargetActionType.UseSkill;
            _turnToTargetPosition = null;
            SelectedEntity = entity;
        }

        protected void TurnCharacterToPositionToUsePendingSkill(Vector3 position)
        {
            _turnToTargetActionType = TargetActionType.UseSkill;
            _turnToTargetPosition = position;
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

        protected bool UseSkill(string id, AimPosition aimPosition)
        {
            int dataId = BaseGameData.MakeDataId(id);
            if (!GameInstance.Skills.TryGetValue(dataId, out BaseSkill skill) || skill == null ||
                !PlayingCharacterEntity.CachedData.Skills.TryGetValue(skill, out int skillLevel))
                return false;
            SetQueueUsingSkill(aimPosition, skill, skillLevel);
            return true;
        }

        protected bool UseItem(string id, AimPosition aimPosition)
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
                _destination = null;
                PlayingCharacterEntity.StopMove();
                _buildingItemIndex = itemIndex;
                ShowConstructBuildingDialog();
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
    }
}
