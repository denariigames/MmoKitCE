using UnityEngine;

namespace UtilsComponents
{
    public class ZRotateLoop : MonoBehaviour
    {
        public float speed = 720;

        private void Update()
        {
            Vector3 eulerAngles = transform.eulerAngles;
            eulerAngles.z += Time.deltaTime * speed;
            transform.eulerAngles = eulerAngles;
        }
    }
}
