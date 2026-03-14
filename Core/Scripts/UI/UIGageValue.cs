using Cysharp.Text;
using Insthync.UnityEditorUtils;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerARPG
{
    [System.Serializable]
    public class UIGageValue
    {
        public enum DisplayType
        {
            CurrentByMax,
            Percentage
        }

        [Header("General Setting")]
        public DisplayType displayType = DisplayType.CurrentByMax;
        public TextWrapper textValue;
        public Image imageGage;
        public Slider sliderGage;
        public float changedStep = 1f;
        [Tooltip("Enable smooth transitions for the gauge.")]
        public bool smoothTransition = false;
        [BoolShowConditional("smoothTransition", true)]
        public float smoothingSpeed = 10.0f;

        [Header("Min By Max Setting")]
        public UILocaleKeySetting formatCurrentByMax = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE_MIN_BY_MAX);
        public string formatCurrentAmount = "N0";
        public string formatMaxAmount = "N0";

        [Header("Percentage Setting")]
        public UILocaleKeySetting formatPercentage = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE_PERCENTAGE);
        public string formatPercentageAmount = "N0";

        private float _targetRate;
        private Coroutine _smoothingCoroutineForImage;
        private Coroutine _smoothingCoroutineForSlider;
        private float _prevCurrent = -1f;
        private float _prevMax = -1f;

        public void SetVisible(bool isVisible)
        {
            if (textValue != null)
                textValue.SetGameObjectActive(isVisible);
            if (imageGage != null)
                imageGage.gameObject.SetActive(isVisible);
            if (sliderGage != null)
                sliderGage.gameObject.SetActive(isVisible);
        }

        public void Update(int current, int max)
        {
            Update((float)current, (float)max);
        }

        public void Update(float current, float max)
        {
            _targetRate = max == 0 ? 1 : current / max;

            if (Mathf.Abs(_prevCurrent - current) >= changedStep ||
                Mathf.Abs(_prevMax - max) >= changedStep)
            {
                _prevCurrent = current;
                _prevMax = max;
                if (textValue != null)
                {
                    switch (displayType)
                    {
                        case DisplayType.Percentage:
                            textValue.text = ZString.Format(
                                LanguageManager.GetText(formatPercentage),
                                (_targetRate * 100f).ToString(formatPercentageAmount));
                            break;
                        default:
                            textValue.text = ZString.Format(
                                LanguageManager.GetText(formatCurrentByMax),
                                current.ToString(formatCurrentAmount),
                                max.ToString(formatMaxAmount));
                            break;
                    }
                }
            }

            if (smoothTransition)
            {
                if (imageGage != null)
                {
                    if (imageGage.isActiveAndEnabled)
                    {
                        if (_smoothingCoroutineForImage != null)
                            imageGage.StopCoroutine(_smoothingCoroutineForImage);
                        _smoothingCoroutineForImage = imageGage.StartCoroutine(SmoothUpdateImageGageRoutine(_targetRate));
                    }
                    else
                    {
                        imageGage.fillAmount = _targetRate;
                    }
                }
                if (sliderGage != null)
                {
                    sliderGage.maxValue = 1f;
                    if (sliderGage.isActiveAndEnabled)
                    {
                        if (_smoothingCoroutineForSlider != null)
                            sliderGage.StopCoroutine(_smoothingCoroutineForSlider);
                        _smoothingCoroutineForSlider = sliderGage.StartCoroutine(SmoothUpdateSliderGageRoutine(_targetRate));
                    }
                    else
                    {
                        sliderGage.value = _targetRate;
                    }
                }
            }
            else
            {
                if (imageGage != null)
                {
                    imageGage.fillAmount = _targetRate;
                }
                if (sliderGage != null)
                {
                    sliderGage.maxValue = 1f;
                    sliderGage.value = _targetRate;
                }
            }
        }

        IEnumerator SmoothUpdateImageGageRoutine(float targetRate)
        {
            while (!Mathf.Approximately(imageGage.fillAmount, targetRate))
            {
                imageGage.fillAmount = Mathf.Lerp(imageGage.fillAmount, targetRate, Time.deltaTime * smoothingSpeed);
                yield return null;
            }
        }

        IEnumerator SmoothUpdateSliderGageRoutine(float targetRate)
        {
            while (!Mathf.Approximately(sliderGage.value, targetRate))
            {
                sliderGage.value = Mathf.Lerp(sliderGage.value, targetRate, Time.deltaTime * smoothingSpeed);
                yield return null;
            }
        }
    }
}
