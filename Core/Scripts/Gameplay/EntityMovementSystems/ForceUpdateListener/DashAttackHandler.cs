using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MultiplayerARPG
{
    public class DashAttackHandler : MonoBehaviour, IEntityMovementForceUpdateListener
    {
        public ApplyMovementForceSourceType sourceType;
        public int sourceDataId;
#if UNITY_EDITOR
        [SerializeField]
        private BaseGameData sourceData;
        [SerializeField]
        private bool enableTestJumpTime;
        [SerializeField]
        [Range(0f, 1f)]
        private float testJumpTime;
#endif
        public AnimationCurve jumpCurve = new AnimationCurve(
            new Keyframe(0f, 0f), new Keyframe(0.5f, 1f), new Keyframe(1f, 0f));
        public Transform jumpAnimTransform;

        private BaseCharacterEntity _user;
        private bool _isDashing = false;
        private EntityMovementForceApplier _forceApplier;
        private BaseGameData _sourceData;
        private float _jumpMovingTriggerInterval = -1f;
        private float _jumpMovingTriggeredTime = 0f;
        private float _defaultY = 0f;

        private bool _isFinishedJump = false;

        private void Start()
        {
            if (sourceType == ApplyMovementForceSourceType.None || sourceDataId == 0 || jumpAnimTransform == null)
            {
                enabled = false;
                return;
            }
            _user = GetComponent<BaseCharacterEntity>();
            if (_user == null)
                return;
            _defaultY = jumpAnimTransform.localPosition.y;
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (sourceData != null)
            {
                if (sourceData is BaseSkill)
                    sourceType = ApplyMovementForceSourceType.Skill;
                sourceDataId = sourceData.DataId;
                sourceData = null;
                EditorUtility.SetDirty(this);
            }
            if (enableTestJumpTime)
            {
                jumpAnimTransform.transform.localPosition = new Vector3(
                    jumpAnimTransform.localPosition.x,
                    jumpCurve.Evaluate(testJumpTime),
                    jumpAnimTransform.localPosition.z);
            }
        }
#endif

        public void OnPreUpdateForces(IList<EntityMovementForceApplier> forceAppliers)
        {
            if (forceAppliers.Count > 0)
                _forceApplier = forceAppliers.FindBySource(sourceType, sourceDataId);
            _isFinishedJump = false;
            if (_forceApplier == null)
                return;
            // Start of force applying
            if (_isDashing)
            {
                // On Jump Moving
                OnMoving();
                return;
            }
            _isDashing = true;
            OnPreJump();
        }

        private void OnPreJump()
        {
            _sourceData = _forceApplier.GetSourceData();
            // Reset jump anim transform
            jumpAnimTransform.transform.localPosition = new Vector3(
                jumpAnimTransform.localPosition.x,
                _defaultY,
                jumpAnimTransform.localPosition.z);
            // On Pre Jump
            if (_sourceData != null && _sourceData is IDashAttackEventListener eventListener)
            {
                _jumpMovingTriggerInterval = eventListener.DashMovingTriggerInterval;
                eventListener.OnPreDashAttack(_user, _forceApplier.SourceLevel);
            }
        }

        private void OnMoving()
        {
            if (_jumpMovingTriggerInterval <= 0f)
                return;
            if (Time.unscaledTime - _jumpMovingTriggeredTime < _jumpMovingTriggerInterval)
                return;
            _jumpMovingTriggeredTime = Time.unscaledTime;
            if (_sourceData != null && _sourceData is IDashAttackEventListener eventListenerMove)
                eventListenerMove.OnDashMovingToAttack(_user, _forceApplier.SourceLevel);
        }

        public void OnPostUpdateForces(IList<EntityMovementForceApplier> forceAppliers)
        {
            if (_forceApplier == null)
            {
                _isDashing = false;
                return;
            }
            // End of force applying
            if (forceAppliers.FindBySource(sourceType, sourceDataId) != null)
                return;
            _isDashing = false;
            OnPostJump();
        }

        private void OnPostJump()
        {
            // Reset jump anim transform
            jumpAnimTransform.transform.localPosition = new Vector3(
                jumpAnimTransform.localPosition.x,
                _defaultY,
                jumpAnimTransform.localPosition.z);
            // On Post Jump
            if (_sourceData != null && _sourceData is IDashAttackEventListener eventListener)
                eventListener.OnPostDashAttack(_user, _forceApplier.SourceLevel);
        }

        private void LateUpdate()
        {
            if (!_isDashing)
                return;
            if (_forceApplier.Duration <= 0f)
                return;
            jumpAnimTransform.transform.localPosition = new Vector3(
                jumpAnimTransform.localPosition.x,
                jumpCurve.Evaluate(_forceApplier.Elasped / _forceApplier.Duration),
                jumpAnimTransform.localPosition.z);

            if (Mathf.Approximately(_forceApplier.Elasped, _forceApplier.Duration) && !_isFinishedJump)
            {
                _isFinishedJump = true;
                if (_sourceData != null && _sourceData is IDashAttackEventListener eventListener)
                    eventListener.OnPostDashAttack(_user, _forceApplier.SourceLevel);
            }
        }
    }
}
