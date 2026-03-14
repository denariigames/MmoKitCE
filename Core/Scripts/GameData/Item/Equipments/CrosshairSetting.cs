using UnityEngine;
using UnityEngine.Serialization;

namespace MultiplayerARPG
{
    [System.Serializable]
    public struct CrosshairSetting
    {
        public bool hidden;
        [FormerlySerializedAs("expandPerFrameWhileMoving")]
        public float expandPerFrame;
        public float shrinkPerFrame;
        public float minSpread;
        public float maxSpread;
        public float addSpreadWhileAttackAndMoving;
        [System.Obsolete("Use weapon's recoil instead.")]
        [FormerlySerializedAs("recoil")]
        [FormerlySerializedAs("recoilY")]
        [Tooltip("X axis rotation")]
        public float recoilPitch;
        [System.Obsolete("Use weapon's recoil instead.")]
        [FormerlySerializedAs("recoilX")]
        [Tooltip("Y axis rotation")]
        public float recoilYaw;
        [System.Obsolete("Use weapon's recoil instead.")]
        [Tooltip("Z axis rotation")]
        public float recoilRoll;
    }
}
