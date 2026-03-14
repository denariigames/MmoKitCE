using Insthync.CameraAndInput;
using UnityEngine;

namespace MultiplayerARPG
{
    public class DefaultBuildAimController : MonoBehaviour, IBuildAimController
    {
        public const float BUILDING_CONSTRUCTING_GROUND_FINDING_DISTANCE = 100f;

        public BasePlayerCharacterController Controller { get; protected set; }
        public GameInstance CurrentGameInstance { get { return GameInstance.Singleton; } }
        public Transform EntityTransform { get { return GameInstance.PlayingCharacterEntity.EntityTransform; } }
        public BuildingEntity ConstructingBuildingEntity { get { return Controller.ConstructingBuildingEntity; } }

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
        [SerializeField]
        protected float consoleDistanceRate = 0.75f;

        protected IGameplayCameraController _gameplayCameraController;
        protected IPhysicFunctions _physicFunctions;
        protected float _buildYRotate;

        public virtual void Init()
        {
            Controller = GetComponent<BasePlayerCharacterController>();
            _gameplayCameraController = GetComponent<IGameplayCameraController>();
            if (CurrentGameInstance.DimensionType == DimensionType.Dimension3D)
                _physicFunctions = new PhysicFunctions(128);
            else
                _physicFunctions = new PhysicFunctions2D(128);
        }

        public virtual void SetData(
            bool buildGridSnap = false,
            Vector3 buildGridOffsets = new Vector3(),
            float buildGridSize = 4f,
            bool buildRotationSnap = false,
            float buildRotateAngle = 45f,
            float buildRotateSpeed = 200f)
        {
            this.buildGridSnap = buildGridSnap;
            this.buildGridOffsets = buildGridOffsets;
            this.buildGridSize = buildGridSize;
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

        public virtual AimPosition UpdateAimControls(Vector2 aimAxes, BuildingEntity prefab)
        {
            // Instantiate constructing building
            if (ConstructingBuildingEntity == null)
            {
                InstantiateConstructingBuilding(prefab);
                _buildYRotate = 0;
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
            ConstructingBuildingEntity.Rotation = buildingAngles;
            // Find position to place building
            if (GameInstance.UseMobileInput())
                FindAndSetBuildingAreaByAxes(aimAxes);
            else if (GameInstance.UseConsoleInput())
                FindAndSetBuildingAreaByForwardAxes();
            else
                FindAndSetBuildingAreaByMousePosition();
            return AimPosition.CreatePosition(ConstructingBuildingEntity.Position);
        }

        public virtual void FinishAimControls(bool isCancel)
        {
            if (isCancel)
                CancelBuild();
        }

        public void FindAndSetBuildingAreaByAxes(Vector2 aimAxes)
        {
            if (UIBlockController.IsBlockController() || UIBlockActionController.IsBlockController())
                return;
            Vector3 raycastPosition = EntityTransform.position + (GameplayUtils.GetDirectionByAxes(_gameplayCameraController.CameraTransform, aimAxes.x, aimAxes.y) * ConstructingBuildingEntity.BuildDistance);
            if (CurrentGameInstance.DimensionType == DimensionType.Dimension3D)
                raycastPosition += Vector3.up;
            LoopSetBuildingArea(_physicFunctions.RaycastDown(raycastPosition, CurrentGameInstance.GetBuildLayerMask(), 100f, QueryTriggerInteraction.Collide));
        }

        public void FindAndSetBuildingAreaByForwardAxes()
        {
            if (UIBlockController.IsBlockController() || UIBlockActionController.IsBlockController())
                return;
            Vector3 raycastPosition = EntityTransform.position + (EntityTransform.forward * ConstructingBuildingEntity.BuildDistance * consoleDistanceRate);
            if (CurrentGameInstance.DimensionType == DimensionType.Dimension3D)
                raycastPosition += Vector3.up;
            LoopSetBuildingArea(_physicFunctions.RaycastDown(raycastPosition, CurrentGameInstance.GetBuildLayerMask(), 100f, QueryTriggerInteraction.Collide));
        }

        public void FindAndSetBuildingAreaByMousePosition()
        {
            if (UIBlockController.IsBlockController() || UIBlockActionController.IsBlockController())
                return;
            LoopSetBuildingArea(_physicFunctions.RaycastPickObjects(_gameplayCameraController.Camera, InputManager.MousePosition(), CurrentGameInstance.GetBuildLayerMask(), 1000f, out _, QueryTriggerInteraction.Collide));
        }

        /// <summary>
        /// Return true if found building area
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        private bool LoopSetBuildingArea(int count)
        {
            ConstructingBuildingEntity.BuildingArea = null;
            ConstructingBuildingEntity.HitSurface = false;
            ConstructingBuildingEntity.HitSurfaceNormal = Vector3.down;
            BuildingEntity buildingEntity;
            BuildingArea buildingArea;
            Transform tempTransform;
            Vector3 tempRaycastPoint;
            Vector3 snappedPosition = GetBuildingPlacePosition(ConstructingBuildingEntity.Position);
            for (int tempCounter = 0; tempCounter < count; ++tempCounter)
            {
                tempTransform = _physicFunctions.GetRaycastTransform(tempCounter);
                if (ConstructingBuildingEntity.EntityTransform.root == tempTransform.root)
                {
                    // Hit collider which is part of constructing building entity, skip it
                    continue;
                }

                tempRaycastPoint = _physicFunctions.GetRaycastPoint(tempCounter);
                snappedPosition = GetBuildingPlacePosition(tempRaycastPoint);

                if (CurrentGameInstance.DimensionType == DimensionType.Dimension3D)
                {
                    // Find ground position from upper position
                    bool hitAimmingObject = false;
                    Vector3 raycastOrigin = tempRaycastPoint + Vector3.up * BUILDING_CONSTRUCTING_GROUND_FINDING_DISTANCE * 0.5f;
                    RaycastHit[] groundHits = Physics.RaycastAll(raycastOrigin, Vector3.down, BUILDING_CONSTRUCTING_GROUND_FINDING_DISTANCE, CurrentGameInstance.GetBuildLayerMask());
                    for (int j = 0; j < groundHits.Length; ++j)
                    {
                        if (groundHits[j].transform == tempTransform)
                        {
                            tempRaycastPoint = groundHits[j].point;
                            snappedPosition = GetBuildingPlacePosition(tempRaycastPoint);
                            ConstructingBuildingEntity.Position = snappedPosition;
                            hitAimmingObject = true;
                            break;
                        }
                    }
                    if (!hitAimmingObject)
                        continue;
                }
                else
                {
                    ConstructingBuildingEntity.Position = snappedPosition;
                }

                buildingEntity = tempTransform.root.GetComponent<BuildingEntity>();
                buildingArea = tempTransform.GetComponent<BuildingArea>();
                if ((buildingArea == null || !ConstructingBuildingEntity.BuildingTypes.Contains(buildingArea.buildingType))
                    && buildingEntity == null)
                {
                    // Hit surface which is not building area or building entity
                    ConstructingBuildingEntity.BuildingArea = null;
                    ConstructingBuildingEntity.HitSurface = true;
                    ConstructingBuildingEntity.HitSurfaceNormal = _physicFunctions.GetRaycastNormal(tempCounter);
                    break;
                }

                if (buildingArea == null || !ConstructingBuildingEntity.BuildingTypes.Contains(buildingArea.buildingType))
                {
                    // Skip because this area is not allowed to build the building that you are going to build
                    continue;
                }

                ConstructingBuildingEntity.BuildingArea = buildingArea;
                ConstructingBuildingEntity.HitSurface = true;
                ConstructingBuildingEntity.HitSurfaceNormal = _physicFunctions.GetRaycastNormal(tempCounter);
                return true;
            }
            ConstructingBuildingEntity.Position = snappedPosition;
            return false;
        }

        private Vector3 GetBuildingPlacePosition(Vector3 position)
        {
            if (CurrentGameInstance.DimensionType == DimensionType.Dimension3D)
            {
                if (buildGridSnap)
                    position = new Vector3(Mathf.Round(position.x / buildGridSize) * buildGridSize, position.y, Mathf.Round(position.z / buildGridSize) * buildGridSize) + buildGridOffsets;
            }
            else
            {
                if (buildGridSnap)
                    position = new Vector3(Mathf.Round(position.x / buildGridSize) * buildGridSize, Mathf.Round(position.y / buildGridSize) * buildGridSize) + buildGridOffsets;
            }
            return position;
        }
    }
}
