using UnityEngine;

namespace UtilsComponents
{
    public class SetScaleOnAwake : MonoBehaviour
    {
        public Vector3 scale = Vector3.one;

        private void Awake()
        {
            transform.localScale = scale;
        }
    }
}