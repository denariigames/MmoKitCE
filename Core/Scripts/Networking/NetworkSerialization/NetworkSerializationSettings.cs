// ce scalabilty: #15

namespace MultiplayerARPG
{
    /// <summary>
    /// Shared runtime toggles for additive network serialization upgrades.
    /// Keep all options default-off for strict backward compatibility.
    /// </summary>
    public static class NetworkSerializationSettings
    {
        public static bool UseQuantizedMovementVectors = false;
    }
}
