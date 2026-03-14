using UnityEngine;

namespace UtilsComponents
{
    public class FixRotation : MonoBehaviour
    {
        public Vector3 eulerAngles;
        private void LateUpdate()
        {
            transform.eulerAngles = eulerAngles;
        }
    }
}
