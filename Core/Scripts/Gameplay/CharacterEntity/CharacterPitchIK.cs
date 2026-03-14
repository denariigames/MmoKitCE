using Insthync.ManagedUpdating;
using UnityEngine;

namespace MultiplayerARPG
{
    public class CharacterPitchIK : MonoBehaviour, IManagedLateUpdate
    {
        public enum Axis
        {
            X, Y, Z
        }

        public Axis axis = Axis.Z;
        public bool enableWhileStanding = true;
        public bool enableWhileCrouching = true;
        public bool enableWhileCrawling = true;
        public bool enableWhileSwiming = true;
        public HumanBodyBones pitchBone = HumanBodyBones.UpperChest;
        public Vector3 rotateOffset;
        public bool inversePitch = true;
        public float lerpDamping = 25f;
        [Range(0f, 180f)]
        public float maxAngle = 0f;
        public Quaternion PitchRotation { get; private set; }
        public BaseCharacterEntity CharacterEntity { get; private set; }
        public Animator Animator { get; private set; }

        public bool Enabling
        {
            get
            {
                if (!enabled)
                    return false;
                if (!CharacterEntity || CharacterEntity.IsDead())
                    return false;
                if (CharacterEntity.MovementState == MovementState.IsUnderWater)
                {
                    if (!enableWhileSwiming)
                        return false;
                    return true;
                }
                switch (CharacterEntity.ExtraMovementState)
                {
                    case ExtraMovementState.IsCrouching:
                        if (!enableWhileCrouching)
                            return false;
                        break;
                    case ExtraMovementState.IsCrawling:
                        if (!enableWhileCrawling)
                            return false;
                        break;
                    default:
                        if (!enableWhileStanding)
                            return false;
                        break;
                }
                return true;
            }
        }

        private void Awake()
        {
            CharacterEntity = GetComponent<BaseCharacterEntity>();
            if (CharacterEntity == null)
                CharacterEntity = GetComponentInParent<BaseCharacterEntity>();
            if (CharacterEntity == null)
            {
                enabled = false;
                return;
            }
            Animator = CharacterEntity.GetComponent<Animator>();
            if (Animator == null)
                Animator = CharacterEntity.GetComponentInChildren<Animator>();
        }

        private void OnEnable()
        {
            UpdateManager.Register(DefaultExecutionOrders.PITCH_IK, this);
        }

        private void OnDisable()
        {
            UpdateManager.Unregister(DefaultExecutionOrders.PITCH_IK, this);
        }

        public void ManagedLateUpdate()
        {
            if (!Enabling)
                return;
            PitchRotation = CalculatePitchRotation(CharacterEntity.Pitch, Time.deltaTime, PitchRotation, axis, rotateOffset, inversePitch, lerpDamping, maxAngle);
            Transform tempTransform = Animator.GetBoneTransform(pitchBone);
            tempTransform.localRotation = PitchRotation;
        }

        public static Quaternion CalculatePitchRotation(
            float characterPitch,
            float deltaTime,
            Quaternion oldRotation,
            Axis axis,
            Vector3 rotateOffset,
            bool inversePitch,
            float lerpDamping,
            float maxAngle)
        {
            // Clamp pitch
            if (maxAngle > 0f)
            {
                if (characterPitch >= 180f && characterPitch < 360f - maxAngle)
                {
                    characterPitch = 360f - maxAngle;
                }
                else if (characterPitch < 180f && characterPitch > maxAngle)
                {
                    characterPitch = maxAngle;
                }
            }
            // Find pitch rotation
            Quaternion tempRotation = Quaternion.identity;
            switch (axis)
            {
                case Axis.X:
                    tempRotation = Quaternion.Euler(Vector3.left * characterPitch * (inversePitch ? -1 : 1));
                    break;
                case Axis.Y:
                    tempRotation = Quaternion.Euler(Vector3.up * characterPitch * (inversePitch ? -1 : 1));
                    break;
                case Axis.Z:
                    tempRotation = Quaternion.Euler(Vector3.forward * characterPitch * (inversePitch ? -1 : 1));
                    break;
            }
            tempRotation = tempRotation * Quaternion.Euler(rotateOffset);
            if (lerpDamping > 0f)
                return Quaternion.Lerp(oldRotation, tempRotation, lerpDamping * deltaTime);
            return tempRotation;
        }
    }
}
