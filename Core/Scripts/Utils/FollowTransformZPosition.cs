using UnityEngine;

namespace UtilsComponents
{
    public class FollowTransformZPosition : MonoBehaviour
    {
        public Transform targetTransform;
        public float zOffsets;
        public float damping = 10f;

        private void Update()
        {
            Vector3 targetPoisiton = new Vector3(transform.position.x, transform.position.y, targetTransform.position.z + zOffsets);
            if (damping <= 0f)
                transform.position = targetPoisiton;
            else
                transform.position = Vector3.Lerp(transform.position, targetPoisiton, Time.deltaTime * damping);
        }
    }
}
