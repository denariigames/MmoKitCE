using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerARPG
{
    public class SetScaleBySlider : MonoBehaviour
    {
        public Slider slider;
        public Transform targetTransform;
        public bool changeScaleX;
        public bool changeScaleY;
        public bool changeScaleZ;

        private void Start()
        {
            slider.onValueChanged.AddListener(OnValueChange);
        }

        private void OnDestroy()
        {
            slider.onValueChanged.RemoveListener(OnValueChange);
        }

        public void OnValueChange(float value)
        {
            Vector3 scale = targetTransform.transform.localScale;
            if (changeScaleX)
                scale.x = value;
            if (changeScaleY)
                scale.y = value;
            if (changeScaleZ)
                scale.z = value;
            targetTransform.transform.localScale = scale;
        }
    }
}
