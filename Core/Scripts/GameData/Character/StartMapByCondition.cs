using System.Linq;
using UnityEngine;

namespace MultiplayerARPG
{
    [System.Serializable]
    public struct StartMapByCondition
    {
        [Tooltip("If this is not empty, character who have the same faction will start at this point")]
        public Faction forFaction;
        public BaseMapInfo startMap;

        [Tooltip("If this is `TRUE` it will uses `overrideStartPosition` as start position, instead of `startMap` -> `startPosition`")]
        public bool useOverrideStartPosition;
        public Vector3 overrideStartPosition;

        [Tooltip("If this is `TRUE` it will uses `overrideStartRotation` as start position, instead of `startMap` -> `startRotation`")]
        public bool useOverrideStartRotation;
        public Vector3 overrideStartRotation;

        public BaseMapInfo StartMap
        {
            get
            {
                if (startMap == null)
                    return GameInstance.MapInfos.FirstOrDefault().Value;
                return startMap;
            }
        }

        public Vector3 StartPosition
        {
            get
            {
                return useOverrideStartPosition ? overrideStartPosition : StartMap.StartPosition;
            }
        }

        public Vector3 StartRotation
        {
            get
            {
                return useOverrideStartRotation ? overrideStartRotation : StartMap.StartRotation;
            }
        }
    }
}
