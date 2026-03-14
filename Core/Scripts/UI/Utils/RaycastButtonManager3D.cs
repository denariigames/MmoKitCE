using Insthync.CameraAndInput;
using UnityEngine;

namespace UtilsComponents
{
    public class RaycastButtonManager3D : MonoBehaviour
    {
        private static RaycastButtonManager3D _instance;
        public static RaycastButtonManager3D Instance
        {
            get
            {
                SetupInstance();
                return _instance;
            }
        }

        public static void SetupInstance()
        {
            if (_instance == null)
            {
                GameObject newObject = new GameObject("_RaycastButtonManager3D");
                _instance = newObject.AddComponent<RaycastButtonManager3D>();
            }
        }

        public Camera targetCamera;
        public int allocSize = 64;
        public float maxDistance = 1000f;
        public float clickDetectDuration = 1f;
        public float clickDetectDistance = 10f;

        public Transform CacheTransform { get; private set; }

        private RaycastHit[] _hits;
        private float _pressCountDown;
        private Vector3 _pressPosition;

        private void Awake()
        {
            _hits = new RaycastHit[allocSize];
        }

        private void OnEnable()
        {
            CacheTransform = transform;
            SetupCamera();
        }

        private bool SetupCamera()
        {
            if (targetCamera == null)
                targetCamera = Camera.main;
            return targetCamera != null;
        }

        private void Update()
        {
            if (!SetupCamera())
                return;

            if (InputManager.GetMouseButtonDown(0) && _pressCountDown <= 0f)
            {
                _pressCountDown = clickDetectDuration;
                _pressPosition = InputManager.MousePosition();
            }

            if (_pressCountDown <= 0f)
                return;

            _pressCountDown -= Time.unscaledDeltaTime;
            if (InputManager.GetMouseButtonUp(0) && Vector3.Distance(_pressPosition, InputManager.MousePosition()) < clickDetectDistance)
            {
                _pressCountDown = 0f;
                Ray ray = targetCamera.ScreenPointToRay(InputManager.MousePosition());
                int hitCount = PhysicUtils.SortedRaycastNonAlloc3D(ray, _hits, maxDistance, Physics.AllLayers, QueryTriggerInteraction.Collide);
                if (hitCount <= 0)
                    return;
                RaycastButton3D tempButton;
                for (int i = 0; i < hitCount; ++i)
                {
                    tempButton = _hits[i].collider.GetComponent<RaycastButton3D>();
                    if (tempButton == null)
                        continue;
                    tempButton.InvokeOnClick();
                }
            }
        }
    }
}
