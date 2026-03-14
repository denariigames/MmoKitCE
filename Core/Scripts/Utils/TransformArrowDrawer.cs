using UnityEngine;

namespace UtilsComponents
{
    public class TransformArrowDrawer : MonoBehaviour
    {
        private void OnDrawGizmos()
        {
            Gizmos.color = new Color(0, 0, 1, 0.5f);
            DrawArrow.ForGizmo(transform.position, transform.forward, 0.5f, 0.1f);
            Gizmos.color = new Color(0, 1, 0, 0.5f);
            DrawArrow.ForGizmo(transform.position, transform.up, 0.5f, 0.1f);
            Gizmos.color = new Color(1, 0, 0, 0.5f);
            DrawArrow.ForGizmo(transform.position, transform.right, 0.5f, 0.1f);
        }
    }
}
