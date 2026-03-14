using Insthync.ManagedUpdating;
using UnityEngine;

namespace MultiplayerARPG
{
    public class CharacterAlignOnGround : MonoBehaviour, IManagedLateUpdate
    {
        public Transform rootBoneTransform;
        public Vector3 rootBoneRotation;
        public float alignOnGroundDistance = 1f;
        public LayerMask alignOnGroundLayerMask = ~0;
        public float alignSpeed = 20f;

        private Quaternion _aligningQuaternion;

        public Transform CacheTransform { get; private set; }
        private void Start()
        {
            CacheTransform = transform;
            _aligningQuaternion = Quaternion.identity;
        }

        private void OnEnable()
        {
            UpdateManager.Register(DefaultExecutionOrders.CHARACTER_ALIGN_ON_GROUND, this);
        }

        private void OnDisable()
        {
            UpdateManager.Unregister(DefaultExecutionOrders.CHARACTER_ALIGN_ON_GROUND, this);
        }

        public void ManagedLateUpdate()
        {
            if (rootBoneTransform == null)
                return;

            RaycastHit raycastHit;
            if (Physics.Raycast(CacheTransform.position, Vector3.down, out raycastHit, alignOnGroundDistance, alignOnGroundLayerMask, QueryTriggerInteraction.Ignore))
                _aligningQuaternion = Quaternion.Slerp(_aligningQuaternion, Quaternion.FromToRotation(Vector3.up, raycastHit.normal), Time.deltaTime * alignSpeed);
            else
                _aligningQuaternion = Quaternion.Slerp(_aligningQuaternion, Quaternion.identity, Time.deltaTime * alignSpeed);
            rootBoneTransform.rotation = _aligningQuaternion * Quaternion.AngleAxis(CacheTransform.eulerAngles.y, Vector3.up) * Quaternion.Euler(rootBoneRotation);
        }
    }
}
