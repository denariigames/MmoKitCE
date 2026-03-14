using Insthync.ManagedUpdating;
using UnityEngine;

namespace MultiplayerARPG
{
    public class MovementColliderAdjustment : BaseGameEntityComponent<BaseGameEntity>, IManagedLateUpdate
    {
        [System.Serializable]
        public struct Settings
        {
            public Vector3 center;
            public float radius;
            public float height;
#if UNITY_EDITOR
            public bool drawGizmos;
            public Color gizmosColor;
            [Header("Editor Tools")]
            public bool applyToComponent;
#endif
        }

        [SerializeField]
        private Settings standSettings = new Settings()
        {
#if UNITY_EDITOR
            gizmosColor = Color.blue
#endif
        };
        public Settings StandSettings => standSettings;

        [SerializeField]
        private Settings crouchSettings = new Settings()
        {
#if UNITY_EDITOR
            gizmosColor = Color.magenta
#endif
        };
        public Settings CrouchSettings => crouchSettings;

        [SerializeField]
        private Settings crawlSettings = new Settings()
        {
#if UNITY_EDITOR
            gizmosColor = Color.red
#endif
        };
        public Settings CrawlSettings => crawlSettings;

        [SerializeField]
        private Settings swimSettings = new Settings()
        {
#if UNITY_EDITOR
            gizmosColor = Color.yellow
#endif
        };
        public Settings SwimSettings => swimSettings;

        private CharacterController _characterController;
        private bool _previousIsUnderWater;
        private ExtraMovementState _previousExtraMovementState;

        private void Awake()
        {
            _characterController = GetComponent<CharacterController>();
        }

        private void OnEnable()
        {
            UpdateManager.Register(this);
        }

        private void OnDisable()
        {
            UpdateManager.Unregister(this);
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            _characterController = GetComponent<CharacterController>();
            ApplyingSettings(ref standSettings);
            ApplyingSettings(ref crouchSettings);
            ApplyingSettings(ref crawlSettings);
            ApplyingSettings(ref swimSettings);
        }

        private void ApplyingSettings(ref Settings settings)
        {
            if (settings.applyToComponent)
            {
                Apply(settings);
                settings.applyToComponent = false;
            }
        }

        private void OnDrawGizmosSelected()
        {
            DrawGizmos(standSettings);
            DrawGizmos(crouchSettings);
            DrawGizmos(crawlSettings);
            DrawGizmos(swimSettings);
        }

        private void DrawGizmos(Settings settings)
        {
            if (!settings.drawGizmos)
                return;
            Gizmos.color = settings.gizmosColor;
            float horizontalScale = transform.localScale.x > transform.localScale.z ? transform.localScale.x : transform.localScale.z;
            float verticalScale = transform.localScale.y;
            Vector3 localPosition = transform.localPosition;
            Vector3 center = settings.center * verticalScale;
            float height = (settings.height - settings.radius * 2) / 2 * verticalScale;
            float radius = settings.radius * horizontalScale;
            Gizmos.DrawWireSphere(localPosition + center + Vector3.up * height, radius);
            Gizmos.DrawWireSphere(localPosition + center + Vector3.down * height, radius);
            Gizmos.DrawLine(localPosition + center + (Vector3.forward * radius) + Vector3.down * height,
                localPosition + center + (Vector3.forward * radius) + Vector3.up * height);
            Gizmos.DrawLine(localPosition + center + (Vector3.back * radius) + Vector3.down * height,
                localPosition + center + (Vector3.back * radius) + Vector3.up * height);
            Gizmos.DrawLine(localPosition + center + (Vector3.right * radius) + Vector3.down * height,
                localPosition + center + (Vector3.right * radius) + Vector3.up * height);
            Gizmos.DrawLine(localPosition + center + (Vector3.left * radius) + Vector3.down * height,
                localPosition + center + (Vector3.left * radius) + Vector3.up * height);
        }
#endif

        public void ManagedLateUpdate()
        {
            if (_characterController == null)
                return;

            bool isUnderWater = Entity.MovementState.Has(MovementState.IsUnderWater);
            if (isUnderWater && isUnderWater != _previousIsUnderWater)
            {
                Apply(swimSettings);
            }
            else if (Entity.ExtraMovementState != _previousExtraMovementState)
            {
                switch (Entity.ExtraMovementState)
                {
                    case ExtraMovementState.IsCrouching:
                        Apply(crouchSettings);
                        break;
                    case ExtraMovementState.IsCrawling:
                        Apply(crawlSettings);
                        break;
                    default:
                        Apply(standSettings);
                        break;
                }
            }
            _previousIsUnderWater = isUnderWater;
            _previousExtraMovementState = Entity.ExtraMovementState;
        }

        private void Apply(Settings settings)
        {
            if (_characterController == null)
                return;

            _characterController.center = settings.center;
            _characterController.radius = settings.radius;
            _characterController.height = settings.height;
        }
    }
}