using Insthync.ManagedUpdating;
using UnityEngine;

namespace MultiplayerARPG
{
    public class MovementTransformAdjustment : BaseGameEntityComponent<BaseGameEntity>, IManagedUpdate, IManagedLateUpdate
    {
        [System.Serializable]
        public struct Settings
        {
            public Vector3 localPosition;
#if UNITY_EDITOR
            public bool drawGizmos;
            public Color gizmosColor;
            [Header("Editor Tools")]
            public bool applyToTransform;
#endif
        }

        [SerializeField]
        private Transform targetTransform = null;
        [SerializeField]
        private float translateSpeed = 5f;
        [SerializeField]
        private Settings standSettings = new Settings()
        {
#if UNITY_EDITOR
            gizmosColor = Color.blue
#endif
        };
        [SerializeField]
        private Settings crouchSettings = new Settings()
        {
#if UNITY_EDITOR
            gizmosColor = Color.magenta
#endif
        };
        [SerializeField]
        private Settings crawlSettings = new Settings()
        {
#if UNITY_EDITOR
            gizmosColor = Color.red
#endif
        };
        [SerializeField]
        private Settings swimSettings = new Settings()
        {
#if UNITY_EDITOR
            gizmosColor = Color.yellow
#endif
        };

        private Vector3 _targetPosition;

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
            if (targetTransform == null)
            {
                Debug.LogError("`Target Transform` is empty");
                return;
            }
            ApplyingSettings(ref standSettings);
            ApplyingSettings(ref crouchSettings);
            ApplyingSettings(ref crawlSettings);
            ApplyingSettings(ref swimSettings);
        }

        private void ApplyingSettings(ref Settings settings)
        {
            if (settings.applyToTransform)
            {
                targetTransform.localPosition = settings.localPosition;
                settings.applyToTransform = false;
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
            Gizmos.DrawWireSphere(targetTransform.parent.position + new Vector3(settings.localPosition.x * targetTransform.parent.lossyScale.x, settings.localPosition.y * targetTransform.parent.lossyScale.y, settings.localPosition.z * targetTransform.parent.lossyScale.z), 0.1f);
        }
#endif

        public void ManagedUpdate()
        {
            targetTransform.localPosition = Vector3.MoveTowards(targetTransform.localPosition, _targetPosition, translateSpeed * Time.deltaTime);
        }

        public void ManagedLateUpdate()
        {
            if (targetTransform == null)
                return;

            if (Entity.MovementState.Has(MovementState.IsUnderWater))
            {
                Apply(swimSettings);
            }
            else
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
        }

        private void Apply(Settings settings)
        {
            _targetPosition = settings.localPosition;
        }
    }
}
