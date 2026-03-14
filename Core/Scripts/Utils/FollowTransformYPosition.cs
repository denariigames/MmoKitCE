using UnityEngine;

namespace UtilsComponents
{
    public class FollowTransformYPosition : MonoBehaviour
    {
        public Transform targetTransform;
        public float yOffsets;
        public float damping = 10f;

        private void Update()
        {
            Vector3 targetPoisiton = new Vector3(transform.position.x, targetTransform.position.y + yOffsets, transform.position.z);
            if (damping <= 0f)
                transform.position = targetPoisiton;
            else
                transform.position = Vector3.Lerp(transform.position, targetPoisiton, Time.deltaTime * damping);
        }
    }
}
