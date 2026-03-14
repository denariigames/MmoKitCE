using UnityEngine;

namespace MultiplayerARPG
{
    [System.Serializable]
    public struct WarpPortal
    {
#if UNITY_EDITOR || !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
        [Tooltip("The `WarpPortalEntity` prefab which will instantiates on the map scene. If this is not set, it will use the one which set to `GameInstance` → `warpPortalEntityPrefab`")]
        public WarpPortalEntity entityPrefab;
#endif
#if !DISABLE_ADDRESSABLES
        [Tooltip("The `WarpPortalEntity` prefab which will instantiates on the map scene. If this is not set, it will use the one which set to `GameInstance` → `addressableWarpPortalEntityPrefab`")]
        public AssetReferenceWarpPortalEntity addressableEntityPrefab;
#endif
        [Tooltip("Position for the warp portal which will be placed on the map scene")]
        public Vector3 position;
        [Tooltip("Rotation for the warp portal which will be placed on the map scene")]
        public Vector3 rotation;
        public WarpPortalType warpPortalType;
        [Tooltip("Map which character will warp to, when use the warp portal, leave this empty to warp character to other position in the same map")]
        public BaseMapInfo warpToMapInfo;
        [Tooltip("Position which character will warp to when use the warp portal")]
        public Vector3 warpToPosition;
        [Tooltip("If this is `TRUE` it will change character's rotation when warp")]
        public bool warpOverrideRotation;
        [Tooltip("This will be used if `warpOverrideRotation` is `TRUE` to change character's rotation when warp")]
        public Vector3 warpToRotation;
        public WarpPointByCondition[] warpPointsByCondition;
    }
}
