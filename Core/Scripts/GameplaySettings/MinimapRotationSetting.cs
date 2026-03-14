using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerARPG
{
    public class MinimapRotationSetting : MonoBehaviour
    {
        public enum ECameraRotationMode : int
        {
            LockRotation,
            FollowCharacterRotation,
            FollowGameplayCameraRotation,
        }

        public ECameraRotationMode cameraRotationMode;
        public int lockYRotation;
        public Toggle toggle;
        public Button button;

        public const string SETTING_CAMERA_ROTATION_MODE = "SETTING_CAMERA_ROTATION_MODE";
        public const string SETTING_LOCK_Y_ROTATION = "SETTING_LOCK_Y_ROTATION";

        private static ECameraRotationMode? _cameraRotationMode;
        public static ECameraRotationMode CameraRotationMode
        {
            get
            {
                if (!_cameraRotationMode.HasValue)
                    _cameraRotationMode = (ECameraRotationMode)PlayerPrefs.GetInt(SETTING_CAMERA_ROTATION_MODE, (int)ECameraRotationMode.LockRotation);
                return _cameraRotationMode.Value;
            }
            set
            {
                _cameraRotationMode = value;
                PlayerPrefs.SetInt(SETTING_CAMERA_ROTATION_MODE, (int)value);
            }
        }

        private static int? _lockYRotation;
        public static int LockYRotation
        {
            get
            {
                if (!_lockYRotation.HasValue)
                    _lockYRotation = PlayerPrefs.GetInt(SETTING_LOCK_Y_ROTATION, 0);
                return _lockYRotation.Value;
            }
            set
            {
                _lockYRotation = value;
                PlayerPrefs.SetInt(SETTING_LOCK_Y_ROTATION, value);
            }
        }

        private void Start()
        {
            if (toggle != null)
            {
                toggle.SetIsOnWithoutNotify(CameraRotationMode == cameraRotationMode && LockYRotation == lockYRotation);
                toggle.onValueChanged.AddListener(OnToggle);
            }
            if (button != null)
                button.onClick.AddListener(OnClick);
        }

        private void OnDestroy()
        {
            if (toggle != null)
                toggle.onValueChanged.RemoveListener(OnToggle);
            if (button != null)
                button.onClick.RemoveListener(OnClick);
        }

        public void OnToggle(bool isOn)
        {
            OnClick();
        }

        public void OnClick()
        {
            CameraRotationMode = cameraRotationMode;
            LockYRotation = lockYRotation;
        }
    }
}
