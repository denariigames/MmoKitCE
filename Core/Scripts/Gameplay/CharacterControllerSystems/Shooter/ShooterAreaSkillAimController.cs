using Cysharp.Threading.Tasks;
using Insthync.AddressableAssetTools;
using Insthync.CameraAndInput;
using UnityEngine;

namespace MultiplayerARPG
{
    public class ShooterAreaSkillAimController : MonoBehaviour, IAreaSkillAimController
    {
        public const float GROUND_DETECTION_DISTANCE = 30f;
        private readonly RaycastHit[] findGroundRaycastHits = new RaycastHit[32];
        public bool IsAiming { get { return Time.frameCount - _lastUpdateFrame <= 1; } }
        public bool IsMobile { get { return InputManager.IsUseMobileInput(); } }

        private int _lastUpdateFrame;
        private bool _beginDragged;
        private GameObject _targetObject;

        private async void InstantiateTargetObject(BaseAreaSkill skill)
        {
            if (_targetObject != null)
                Destroy(_targetObject);
            GameObject prefab;
#if !DISABLE_ADDRESSABLES
            prefab = await skill.AddressableTargetObjectPrefab.GetOrLoadAssetAsyncOrUsePrefab(skill.TargetObjectPrefab);
#else
            await UniTask.Yield();
            prefab = skill.TargetObjectPrefab;
#endif
            if (prefab != null)
            {
                _targetObject = Instantiate(prefab);
                _targetObject.SetActive(true);
            }
        }

        public AimPosition UpdateAimControls(Vector2 aimAxes, BaseAreaSkill skill, int skillLevel)
        {
            _lastUpdateFrame = Time.frameCount;
            if (!_beginDragged)
            {
                _beginDragged = true;
                InstantiateTargetObject(skill);
            }
            if (IsMobile)
                return UpdateAimControls_Mobile(aimAxes, skill, skillLevel);
            return UpdateAimControls_PC(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f), skill, skillLevel);
        }

        public void FinishAimControls(bool isCancel)
        {
            _beginDragged = false;
            if (_targetObject != null)
                Destroy(_targetObject);
        }

        public AimPosition UpdateAimControls_PC(Vector3 cursorPosition, BaseAreaSkill skill, int skillLevel)
        {
            float castDistance = skill.castDistance.GetAmount(skillLevel);
            Vector3 position = GameplayUtils.CursorWorldPosition(Camera.main, cursorPosition, 100f, skill.TargetDetectionLayerMask);
            position = GameplayUtils.ClampPosition(GameInstance.PlayingCharacterEntity.EntityTransform.position, position, castDistance);
            position = PhysicUtils.FindGroundedPosition(position, findGroundRaycastHits, GROUND_DETECTION_DISTANCE, skill.GroundDetectionLayerMask);
            if (_targetObject != null)
                _targetObject.transform.position = position;
            return AimPosition.CreatePosition(position);
        }

        public AimPosition UpdateAimControls_Mobile(Vector2 aimAxes, BaseAreaSkill skill, int skillLevel)
        {
            float castDistance = skill.castDistance.GetAmount(skillLevel);
            Vector3 position = GameInstance.PlayingCharacterEntity.EntityTransform.position + (GameplayUtils.GetDirectionByAxes(Camera.main.transform, aimAxes.x, aimAxes.y) * castDistance);
            position = PhysicUtils.FindGroundedPosition(position, findGroundRaycastHits, GROUND_DETECTION_DISTANCE, skill.GroundDetectionLayerMask);
            if (_targetObject != null)
                _targetObject.transform.position = position;
            return AimPosition.CreatePosition(position);
        }
    }
}
