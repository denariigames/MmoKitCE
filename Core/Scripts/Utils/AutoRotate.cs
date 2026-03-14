using UnityEngine;

namespace UtilsComponents
{
    public class AutoRotate : MonoBehaviour
    {
        public Vector3 eulerAngles;
        private void Update()
        {
            transform.eulerAngles += eulerAngles * Time.deltaTime;
        }
    }
}
