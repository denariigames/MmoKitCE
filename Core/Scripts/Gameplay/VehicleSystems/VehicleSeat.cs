using UnityEngine;

namespace MultiplayerARPG
{
    [System.Serializable]
    public class VehicleSeat
    {
        [Header("Generic")]
        public VehicleSeatCameraTarget cameraTarget;
        public Transform passengingTransform;
        public Transform exitTransform;

        [Header("Useful for morphing")]
        [Tooltip("If this is not empty, it will used this transform instead of character's one")]
        public Transform meleeDamageTransform;
        [Tooltip("If this is not empty, it will used this transform instead of character's one")]
        public Transform missileDamageTransform;
        public bool canAttack;
        public bool canUseSkill;
        public bool hidePassenger;
        public bool overridePassengerActionAnimations;
        public bool overridePassengerHitBoxes;
    }
}
