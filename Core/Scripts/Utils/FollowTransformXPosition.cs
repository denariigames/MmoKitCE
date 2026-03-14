using UnityEngine;

namespace UtilsComponents
{
    public class FollowTransformXPosition : MonoBehaviour
    {
        public Transform targetTransform;
        public float xOffsets;
        public float damping = 10f;

        private void Update()
        {
            Vector3 targetPoisiton = new Vector3(targetTransform.position.x + xOffsets, transform.position.y, transform.position.z);
            if (damping <= 0f)
                transform.position = targetPoisiton;
            else
                transform.position = Vector3.Lerp(transform.position, targetPoisiton, Time.deltaTime * damping);
        }
    }
}
