using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UtilsComponents
{
    public class PivotHighlighter : MonoBehaviour
    {
        public Color color = Color.green;

#if UNITY_EDITOR
        protected virtual void OnDrawGizmos()
        {
            Gizmos.color = color;
            Gizmos.DrawWireSphere(transform.position, 0.1f);
            Gizmos.DrawSphere(transform.position, 0.03f);
            Handles.Label(transform.position, name + "(Pivot)");
        }
#endif
    }
}
