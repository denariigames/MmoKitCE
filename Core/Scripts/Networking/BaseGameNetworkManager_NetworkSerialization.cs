// cf scalabilty: #10

using UnityEngine;

namespace MultiplayerARPG
{
    /// <summary>
    /// Extension to BaseGameNetworkManager that adds message batching capabilities
    /// </summary>
    public partial class BaseGameNetworkManager
    {
        [Header("Serialization Optimization")]
        [Tooltip("Use quantized movement vectors for movement input/sync payloads. Enable only when both client and server use this build/setting.")]
        [SerializeField] private bool useQuantizedMovementVectors = false;

        [Tooltip("Quantization precision for movement vectors when quantization is enabled. 100 = 0.01 units, 10 = 0.1 units, 1000 = 0.001 units.")]
        [SerializeField] private float quantizedMovementVectorPrecision = 100f;

        // Initialize components and register messages after network initialization
        protected new virtual void Start()
        {
            ApplySerializationOptions();
        }

        private void OnValidate()
        {
            ApplySerializationOptions();
        }

        private void ApplySerializationOptions()
        {
            EntityMovementFunctions.UseQuantizedMovementVectors = useQuantizedMovementVectors;
            EntityMovementFunctions.QuantizedMovementPrecision = Mathf.Max(1f, quantizedMovementVectorPrecision);
            NetworkSerializationSettings.UseQuantizedMovementVectors = useQuantizedMovementVectors;
        }
    }
}
