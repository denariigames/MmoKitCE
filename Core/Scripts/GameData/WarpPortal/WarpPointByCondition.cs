using UnityEngine;
using UnityEngine.Serialization;

namespace MultiplayerARPG
{
    [System.Serializable]
    public struct WarpPointByCondition
    {
        [Tooltip("If this is not empty, character who have the same faction will teleport to this point")]
        public Faction forFaction;
        public WarpPortalType warpPortalType;
        [Tooltip("Map which character will warp to, when use the warp portal, leave this empty to warp character to other position in the same map")]
        [FormerlySerializedAs("respawnMapInfo")]
        public BaseMapInfo warpToMapInfo;
        [FormerlySerializedAs("respawnPosition")]
        public Vector3 warpToPosition;
        [Tooltip("If this is `TRUE` it will change character's rotation when warp")]
        public bool warpOverrideRotation;
        [Tooltip("This will be used if `warpOverrideRotation` is `TRUE` to change character's rotation when warp")]
        public Vector3 warpToRotation;
    }
}
