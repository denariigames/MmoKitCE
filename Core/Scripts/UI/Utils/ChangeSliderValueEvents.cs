using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace MultiplayerARPG
{
    public class ChangeSliderValueEvents : MonoBehaviour
    {
        public Slider slider;
        public UnityEvent onEnterMin = new UnityEvent();
        public UnityEvent onExitMin = new UnityEvent();
        public UnityEvent onEnterMax = new UnityEvent();
        public UnityEvent onExitMax = new UnityEvent();
        public float stepPerFrame = 0.1f;
        private bool _isBtnIncrease = false;
        private bool _isBtnDecrease = false;
        private bool _isReachedMin = false;
        private bool _isReachedMax = false;

        private void Start()
        {
            slider.onValueChanged.AddListener(OnValueChanged);
            OnValueChanged(slider.value);
        }

        private void OnDestroy()
        {
            slider.onValueChanged.RemoveListener(OnValueChanged);
        }

        public void OnValueChanged(float value)
        {
            if (Mathf.Approximately(slider.maxValue, value))
            {
                if (!_isReachedMax)
                {
                    _isReachedMax = true;
                    onEnterMax.Invoke();
                }
            }
            else if (Mathf.Approximately(slider.minValue, value))
            {
                if (!_isReachedMin)
                {
                    _isReachedMin = true;
                    onEnterMin.Invoke();
                }
            }
            else
            {
                if (_isReachedMax)
                {
                    _isReachedMax = false;
                    onExitMax.Invoke();
                }
                if (_isReachedMin)
                {
                    _isReachedMin = false;
                    onExitMin.Invoke();
                }
            }
        }

        private void Update()
        {
            if (_isBtnIncrease)
            {
                slider.value += stepPerFrame * Time.deltaTime;
            }
            else if (_isBtnDecrease)
            {
                slider.value -= stepPerFrame * Time.deltaTime;
            }
        }

        public void OnIncreaseButtonDown()
        {
            _isBtnIncrease = true;
        }

        public void OnIncreaseButtonUp()
        {
            _isBtnIncrease = false;
        }

        public void OnDecreaseButtonDown()
        {
            _isBtnDecrease = true;
        }

        public void OnDecreaseButtonUp()
        {
            _isBtnDecrease = false;
        }
    }
}
