using Insthync.CameraAndInput;
using UnityEngine;

namespace MultiplayerARPG
{
    public class ShooterBuildAimController : MonoBehaviour, IShooterBuildAimController
    {
        public BasePlayerCharacterController Controller { get; protected set; }
        public GameInstance CurrentGameInstance { get { return GameInstance.Singleton; } }
        public Transform EntityTransform { get { return Controller.EntityTransform; } }
        public BuildingEntity ConstructingBuildingEntity { get { return Controller.ConstructingBuildingEntity; } }

        [Header("Building Settings")]
        [SerializeField]
        protected bool buildRotationSnap;
        [SerializeField]
        protected float buildRotateAngle = 45f;
        [SerializeField]
        protected float buildRotateSpeed = 200f;

        protected IGameplayCameraController _gameplayCameraController;
        protected float _buildYRotate;
        protected Vector3 _aimTargetPosition;
        protected RaycastHit[] _raycasts = new RaycastHit[100];
        protected Ray _centerRay;
        protected float _centerOriginToCharacterDistance;
        protected Vector3 _cameraForward;

        public virtual void Init()
        {
            Controller = GetComponent<BasePlayerCharacterController>();
            _gameplayCameraController = GetComponent<IGameplayCameraController>();
        }

        public virtual void SetData(
            bool buildRotationSnap = false,
            float buildRotateAngle = 45f,
            float buildRotateSpeed = 200f)
        {
            this.buildRotationSnap = buildRotationSnap;
            this.buildRotateAngle = buildRotateAngle;
            this.buildRotateSpeed = buildRotateSpeed;
        }

        protected virtual void InstantiateConstructingBuilding(BuildingEntity prefab)
        {
            Controller.InstantiateConstructingBuilding(prefab);
        }

        protected virtual void CancelBuild()
        {
            Controller.CancelBuild();
        }

        public void UpdateCameraLookData(Ray centerRay, float centerOriginToCharacterDistance, Vector3 cameraForward, Vector3 cameraRight)
        {
            _centerRay = centerRay;
            _centerOriginToCharacterDistance = centerOriginToCharacterDistance;
            _cameraForward = cameraForward;
        }

        public virtual AimPosition UpdateAimControls(Vector2 aimAxes, BuildingEntity prefab)
        {
            // Instantiate constructing building
            if (ConstructingBuildingEntity == null)
            {
                InstantiateConstructingBuilding(prefab);
                _buildYRotate = 0f;
            }
            // Rotate by keys
            Vector3 buildingAngles = Vector3.zero;
            if (CurrentGameInstance.DimensionType == DimensionType.Dimension3D)
            {
                if (buildRotationSnap)
                {
                    if (InputManager.GetButtonDown("RotateLeft"))
                        _buildYRotate -= buildRotateAngle;
                    if (InputManager.GetButtonDown("RotateRight"))
                        _buildYRotate += buildRotateAngle;
                    // Make Y rotation set to 0, 90, 180
                    buildingAngles.y = _buildYRotate = Mathf.Round(_buildYRotate / buildRotateAngle) * buildRotateAngle;
                }
                else
                {
                    float deltaTime = Time.deltaTime;
                    if (InputManager.GetButton("RotateLeft"))
                        _buildYRotate -= buildRotateSpeed * deltaTime;
                    if (InputManager.GetButton("RotateRight"))
                        _buildYRotate += buildRotateSpeed * deltaTime;
                    // Rotate by set angles
                    buildingAngles.y = _buildYRotate;
                }
                ConstructingBuildingEntity.BuildYRotation = _buildYRotate;
            }
            // Clear area before next find
            ConstructingBuildingEntity.BuildingArea = null;
            // Default aim position (aim to sky/space)
            _aimTargetPosition = _centerRay.origin + _centerRay.direction * (_centerOriginToCharacterDistance + ConstructingBuildingEntity.BuildDistance - BuildingEntity.BUILD_DISTANCE_BUFFER);
            _aimTargetPosition = GameplayUtils.ClampPosition(EntityTransform.position, _aimTargetPosition, ConstructingBuildingEntity.BuildDistance - BuildingEntity.BUILD_DISTANCE_BUFFER);
            // Raycast from camera position to center of screen
            FindConstructingBuildingArea(new Ray(_centerRay.origin, (_aimTargetPosition - _centerRay.origin).normalized), Vector3.Distance(_centerRay.origin, _aimTargetPosition));
            // Not hit ground
            if (!ConstructingBuildingEntity.HitSurface)
            {
                // Find nearest grounded position
                FindConstructingBuildingArea(new Ray(_aimTargetPosition, Vector3.down), 8f);
            }
            // Place constructing building
            if ((ConstructingBuildingEntity.BuildingArea && !ConstructingBuildingEntity.BuildingArea.snapBuildingObject) ||
                !ConstructingBuildingEntity.BuildingArea)
            {
                // Place the building on the ground when the building area is not snapping
                // Or place it anywhere if there is no building area
                // It's also no snapping build area, so set building rotation by camera look direction
                ConstructingBuildingEntity.Position = _aimTargetPosition;
                // Rotate to camera
                Vector3 direction = _aimTargetPosition - _gameplayCameraController.CameraTransform.position;
                direction.y = 0f;
                direction.Normalize();
                ConstructingBuildingEntity.EntityTransform.eulerAngles = Quaternion.LookRotation(direction).eulerAngles + (Vector3.up * _buildYRotate);
            }
            return AimPosition.CreatePosition(ConstructingBuildingEntity.Position);
        }

        protected int FindConstructingBuildingArea(Ray ray, float distance)
        {
            ConstructingBuildingEntity.BuildingArea = null;
            ConstructingBuildingEntity.HitSurface = false;
            ConstructingBuildingEntity.HitSurfaceNormal = Vector3.down;
            int tempCount = PhysicUtils.SortedRaycastNonAlloc3D(ray.origin, ray.direction, _raycasts, distance, CurrentGameInstance.GetBuildLayerMask());
            RaycastHit tempHitInfo;
            BuildingEntity buildingEntity;
            BuildingArea buildingArea;
            for (int tempCounter = 0; tempCounter < tempCount; ++tempCounter)
            {
                tempHitInfo = _raycasts[tempCounter];
                if (ConstructingBuildingEntity.EntityTransform.root == tempHitInfo.transform.root)
                {
                    // Hit collider which is part of constructing building entity, skip it
                    continue;
                }

                _aimTargetPosition = tempHitInfo.point;

                if (!IsInFront(tempHitInfo.point))
                {
                    // Skip because this position is not allowed to build the building
                    continue;
                }

                buildingEntity = tempHitInfo.transform.root.GetComponent<BuildingEntity>();
                buildingArea = tempHitInfo.transform.GetComponent<BuildingArea>();
                if (buildingArea == null && tempHitInfo.collider.isTrigger)
                {
                    // Skip because it is trigger collider without building area
                    continue;
                }

                if ((buildingArea == null || !ConstructingBuildingEntity.BuildingTypes.Contains(buildingArea.buildingType))
                    && buildingEntity == null)
                {
                    // Hit surface which is not building area or building entity
                    ConstructingBuildingEntity.BuildingArea = null;
                    ConstructingBuildingEntity.HitSurface = true;
                    ConstructingBuildingEntity.HitSurfaceNormal = tempHitInfo.normal;
                    break;
                }

                if (buildingArea == null || !ConstructingBuildingEntity.BuildingTypes.Contains(buildingArea.buildingType))
                {
                    // Skip because this area is not allowed to build the building that you are going to build
                    continue;
                }

                // Found building area which can construct the building
                ConstructingBuildingEntity.BuildingArea = buildingArea;
                ConstructingBuildingEntity.HitSurface = true;
                ConstructingBuildingEntity.HitSurfaceNormal = tempHitInfo.normal;
                break;
            }
            return tempCount;
        }

        public virtual void FinishAimControls(bool isCancel)
        {
            if (isCancel)
                CancelBuild();
        }

        public virtual bool IsInFront(Vector3 position)
        {
            return Vector3.Angle(_cameraForward, position - EntityTransform.position) < 115f;
        }
    }
}
