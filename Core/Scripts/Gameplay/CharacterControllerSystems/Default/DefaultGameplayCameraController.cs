using Insthync.CameraAndInput;
using UnityEngine;

namespace MultiplayerARPG
{
    public class DefaultGameplayCameraController : MonoBehaviour, IGameplayCameraController
    {
        [SerializeField]
        protected FollowCameraControls gameplayCameraPrefab;
        [SerializeField]
        protected bool aimAssistPlayer = true;
        [SerializeField]
        protected bool aimAssistMonster = true;
        [SerializeField]
        protected bool aimAssistBuilding = false;
        [SerializeField]
        protected bool aimAssistHarvestable = false;
        public BasePlayerCharacterEntity PlayerCharacterEntity { get; protected set; }
        public FollowCameraControls CameraControls { get; protected set; }
        public Camera Camera { get { return CameraControls.CacheCamera; } }
        public Transform CameraTransform { get { return CameraControls.CacheCameraTransform; } }
        public Transform FollowingEntityTransform { get; set; }
        public Vector3 TargetOffset { get { return CameraControls.targetOffset; } set { CameraControls.targetOffset = value; } }
        public float CameraFov { get { return Camera.fieldOfView; } set { Camera.fieldOfView = value; } }
        public float CameraNearClipPlane { get { return Camera.nearClipPlane; } set { Camera.nearClipPlane = value; } }
        public float CameraFarClipPlane { get { return Camera.farClipPlane; } set { Camera.farClipPlane = value; } }
        public float MinZoomDistance { get { return CameraControls.minZoomDistance; } set { CameraControls.minZoomDistance = value; } }
        public float MaxZoomDistance { get { return CameraControls.maxZoomDistance; } set { CameraControls.maxZoomDistance = value; } }
        public float CurrentZoomDistance { get { return CameraControls.zoomDistance; } set { CameraControls.zoomDistance = value; } }
        public bool EnableWallHitSpring { get { return CameraControls.enableWallHitSpring; } set { CameraControls.enableWallHitSpring = value; } }
        public bool UpdateRotation { get { return CameraControls.updateRotation; } set { CameraControls.updateRotation = value; } }
        public bool UpdateRotationX { get { return CameraControls.updateRotationX; } set { CameraControls.updateRotationX = value; } }
        public bool UpdateRotationY { get { return CameraControls.updateRotationY; } set { CameraControls.updateRotationY = value; } }
        public bool UpdateZoom { get { return CameraControls.updateZoom; } set { CameraControls.updateZoom = value; } }

        public virtual void Init()
        {
            if (gameplayCameraPrefab == null)
            {
                Debug.LogWarning("`gameplayCameraPrefab` is empty, `DefaultGameplayCameraController` component is disabling.");
                enabled = false;
            }
            CameraControls = Instantiate(gameplayCameraPrefab);
        }

        public virtual void SetData(FollowCameraControls gameplayCameraPrefab,
            bool aimAssistPlayer = true,
            bool aimAssistMonster = true,
            bool aimAssistBuilding = true,
            bool aimAssistHarvestable = true)
        {
            this.gameplayCameraPrefab = gameplayCameraPrefab;
            this.aimAssistPlayer = aimAssistPlayer;
            this.aimAssistMonster = aimAssistMonster;
            this.aimAssistBuilding = aimAssistBuilding;
            this.aimAssistHarvestable = aimAssistHarvestable;
        }

        protected virtual void OnDestroy()
        {
            if (CameraControls != null)
                Destroy(CameraControls.gameObject);
        }

        protected virtual void Update()
        {
            CameraControls.target = FollowingEntityTransform;
        }

        public virtual void Setup(BasePlayerCharacterEntity characterEntity)
        {
            PlayerCharacterEntity = characterEntity;
        }

        public virtual void Desetup(BasePlayerCharacterEntity characterEntity)
        {
            PlayerCharacterEntity = null;
            FollowingEntityTransform = null;
        }
    }
}
