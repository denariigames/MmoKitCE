using Cysharp.Threading.Tasks;
using Insthync.AddressableAssetTools;
using Insthync.CameraAndInput;
using UnityEngine;

namespace MultiplayerARPG
{
    public class DefaultAreaSkillAimController : MonoBehaviour, IAreaSkillAimController
    {
        public const float GROUND_DETECTION_DISTANCE = 30f;
        private readonly RaycastHit[] findGroundRaycastHits = new RaycastHit[32];
        public bool IsAiming { get { return Time.frameCount - _lastUpdateFrame <= 1; } }
        public Transform EntityTransform { get { return GameInstance.PlayingCharacterEntity.EntityTransform; } }

        [SerializeField]
        protected float consoleDistanceRate = 0.75f;

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
            if (GameInstance.UseMobileInput())
                return UpdateAimControls_Mobile(aimAxes, skill, skillLevel);
            else if (GameInstance.UseConsoleInput())
                return UpdateAimControls_Console(skill, skillLevel);
            return UpdateAimControls_PC(InputManager.MousePosition(), skill, skillLevel);
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
            position = GameplayUtils.ClampPosition(EntityTransform.position, position, castDistance);
            position = PhysicUtils.FindGroundedPosition(position, findGroundRaycastHits, GROUND_DETECTION_DISTANCE, skill.GroundDetectionLayerMask);
            if (_targetObject != null)
                _targetObject.transform.position = position;
            return AimPosition.CreatePosition(position);
        }

        public AimPosition UpdateAimControls_Mobile(Vector2 aimAxes, BaseAreaSkill skill, int skillLevel)
        {
            float castDistance = skill.castDistance.GetAmount(skillLevel);
            Vector3 position = EntityTransform.position + (GameplayUtils.GetDirectionByAxes(Camera.main.transform, aimAxes.x, aimAxes.y) * castDistance);
            position = PhysicUtils.FindGroundedPosition(position, findGroundRaycastHits, GROUND_DETECTION_DISTANCE, skill.GroundDetectionLayerMask);
            if (_targetObject != null)
                _targetObject.transform.position = position;
            return AimPosition.CreatePosition(position);
        }

        public AimPosition UpdateAimControls_Console(BaseAreaSkill skill, int skillLevel)
        {
            float castDistance = skill.castDistance.GetAmount(skillLevel);
            Vector3 position = EntityTransform.position + (EntityTransform.forward * castDistance * consoleDistanceRate);
            position = PhysicUtils.FindGroundedPosition(position, findGroundRaycastHits, GROUND_DETECTION_DISTANCE, skill.GroundDetectionLayerMask);
            if (_targetObject != null)
                _targetObject.transform.position = position;
            return AimPosition.CreatePosition(position);
        }
    }
}
