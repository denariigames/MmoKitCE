using Insthync.DevExtension;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MultiplayerARPG
{
    public partial class BaseEquipmentEntity
    {
        [Header("Support-Hand IK Settings")]
        public Transform mainHandSupportTransform;
        [Tooltip("Use this improve character proportion while playing reload animation, to make it play properly")]
        public Vector3 reloadAnimPosOffsets;

        [Header("ADS IK Settings")]
        public Transform adsTransform;

#if UNITY_EDITOR
        [DevExtMethods("OnDrawGizmos")]
        protected void OnDrawGizmos_IK()
        {
            if (adsTransform != null)
            {
                Gizmos.color = new Color(1, 0, 0, 0.5f);
                Gizmos.DrawSphere(adsTransform.position, 0.03f);
                Handles.Label(adsTransform.position, name + "(ADS)");
                Gizmos.color = new Color(0, 0, 1, 0.5f);
                DrawArrow.ForGizmo(adsTransform.position, adsTransform.forward, 0.5f, 0.1f);
                Gizmos.color = new Color(0, 1, 0, 0.5f);
                DrawArrow.ForGizmo(adsTransform.position, -adsTransform.up, 0.5f, 0.1f);
            }
        }
#endif
    }
}
