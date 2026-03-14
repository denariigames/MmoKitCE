using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerARPG
{
    public class CameraRotationSpeedScaleSetting : MonoBehaviour
    {
        public Slider slider;
        public TextWrapper textScaleValue;
        public string valueFormat = "{0}%";
        public float valueTextMultiplicator = 100f;
        public float minValue = 0.01f;
        public float maxValue = 4f;
        public float defaultValue = 1f;
        public float valueChangeStepOnClick = 0.01f;
        public Button buttonIncrease;
        public Button buttonDecrease;
        public string cameraRotationSpeedScaleSaveKey = "3RD_PERSON_CAMERA_SCALE";
        private readonly static Dictionary<string, float> s_cameraRotationSpeedScales = new Dictionary<string, float>();
        public float CameraRotationSpeedScale
        {
            get
            {
                return GetCameraRotationSpeedScaleByKey(cameraRotationSpeedScaleSaveKey, defaultValue);
            }
            set
            {
                if (!string.IsNullOrEmpty(cameraRotationSpeedScaleSaveKey))
                {
                    s_cameraRotationSpeedScales[cameraRotationSpeedScaleSaveKey] = value;
                    PlayerPrefs.SetFloat(cameraRotationSpeedScaleSaveKey, value);
                }
            }
        }

        public static float GetCameraRotationSpeedScaleByKey(string key, float defaultValue)
        {
            if (string.IsNullOrEmpty(key))
                return defaultValue;
            if (!s_cameraRotationSpeedScales.ContainsKey(key))
                s_cameraRotationSpeedScales[key] = PlayerPrefs.GetFloat(key, defaultValue);
            return s_cameraRotationSpeedScales[key];
        }

        private void Awake()
        {
            slider.minValue = minValue;
            slider.maxValue = maxValue;
            slider.SetValueWithoutNotify(CameraRotationSpeedScale);
            slider.onValueChanged.AddListener(OnValueChanged);
            if (buttonIncrease != null)
                buttonIncrease.onClick.AddListener(OnClickIncrease);
            if (buttonDecrease != null)
                buttonDecrease.onClick.AddListener(OnClickDecrease);
            if (textScaleValue != null)
                textScaleValue.text = string.Format(valueFormat, (slider.value * valueTextMultiplicator).ToString("N2"));
        }

        private void OnDestroy()
        {
            slider.onValueChanged.RemoveListener(OnValueChanged);
            if (buttonIncrease != null)
                buttonIncrease.onClick.RemoveListener(OnClickIncrease);
            if (buttonDecrease != null)
                buttonDecrease.onClick.RemoveListener(OnClickDecrease);
        }

        public void OnValueChanged(float value)
        {
            CameraRotationSpeedScale = value;
            if (textScaleValue != null)
                textScaleValue.text = string.Format(valueFormat, (value * valueTextMultiplicator).ToString("N2"));
        }

        public void OnClickIncrease()
        {
            slider.value += valueChangeStepOnClick;
        }

        public void OnClickDecrease()
        {
            slider.value -= valueChangeStepOnClick;
        }
    }
}
