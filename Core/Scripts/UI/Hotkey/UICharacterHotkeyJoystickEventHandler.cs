using Insthync.CameraAndInput;
using UnityEngine;

namespace MultiplayerARPG
{
    [DefaultExecutionOrder(DefaultExecutionOrders.UI_CHARACTER_HOTKEY_JOYSTICK)]
    [RequireComponent(typeof(UICharacterHotkey))]
    public class UICharacterHotkeyJoystickEventHandler : MonoBehaviour, IHotkeyJoystickEventHandler
    {
        public const string HOTKEY_AXIS_X = "HotkeyAxisX";
        public const string HOTKEY_AXIS_Y = "HotkeyAxisY";
        public UICharacterHotkey UICharacterHotkey { get; private set; }
        public bool Interactable { get { return UICharacterHotkey.IsAssigned(); } }
        public bool IsDragging { get; private set; }
        public AimPosition AimPosition { get; private set; }

        private MobileMovementJoystick _joystick;
        private string _hotkeyAxisNameX;
        private string _hotkeyAxisNameY;
        private RectTransform _hotkeyCancelArea;
        private Vector2 _hotkeyAxes;
        private bool _hotkeyCancel;

        private void Awake()
        {
            UICharacterHotkey = GetComponent<UICharacterHotkey>();
            if (TryGetComponent(out UICharacterHotkeys hotkeys))
                Debug.LogWarning("[UICharacterHotkeyJoystickEventHandler] this should not be in the same game object with `UICharacterHotkeys` component");
            UICharacterHotkeys.RegisterHotkeyJoystick(this);
        }

        private void Start()
        {
            _joystick = Instantiate(UICharacterHotkey.UICharacterHotkeys.hotkeyAimJoyStickPrefab, UICharacterHotkey.transform.parent);
            _joystick.gameObject.SetActive(true);
            _joystick.transform.localPosition = UICharacterHotkey.transform.localPosition;
            _joystick.axisXName = _hotkeyAxisNameX = HOTKEY_AXIS_X + "_" + UICharacterHotkey.HotkeyId;
            _joystick.axisYName = _hotkeyAxisNameY = HOTKEY_AXIS_Y + "_" + UICharacterHotkey.HotkeyId;
            _joystick.setAsLastSiblingOnDrag = true;
            _joystick.hideWhileIdle = true;
            _joystick.Interactable = true;
            _hotkeyCancelArea = UICharacterHotkey.UICharacterHotkeys.hotkeyCancelArea;
        }

        private void OnEnable()
        {
            if (_joystick != null)
                _joystick.gameObject.SetActive(true);
        }

        private void OnDisable()
        {
            if (_joystick != null)
                _joystick.gameObject.SetActive(false);
        }

        public void UpdateEvent()
        {
            if (UICharacterHotkey == null || _joystick == null)
                return;
            _joystick.transform.localPosition = UICharacterHotkey.transform.localPosition;
            _joystick.Interactable = Interactable;

            if (!IsDragging && _joystick.IsDragging)
            {
                UICharacterHotkeys.SetUsingHotkey(UICharacterHotkey);
                IsDragging = true;
            }

            // Can dragging only 1 hotkey each time, so check with latest dragging hotkey
            // If it's not this hotkey then set dragging state to false 
            // To check joystick's started dragging state next time
            if (UICharacterHotkeys.UsingHotkey == null || UICharacterHotkeys.UsingHotkey != UICharacterHotkey)
            {
                IsDragging = false;
                return;
            }

            _hotkeyAxes = new Vector2(InputManager.GetAxis(_hotkeyAxisNameX, false), InputManager.GetAxis(_hotkeyAxisNameY, false));
            _hotkeyCancel = false;

            if (_hotkeyCancelArea != null)
            {
                if (_hotkeyCancelArea.rect.Contains(_hotkeyCancelArea.InverseTransformPoint(_joystick.CurrentPosition)))
                {
                    // Cursor position is inside hotkey cancel area so cancel the hotkey
                    _hotkeyCancel = true;
                }
            }

            if (IsDragging && _joystick.IsDragging)
            {
                AimPosition = UICharacterHotkey.UpdateAimControls(_hotkeyAxes);
            }

            if (IsDragging && !_joystick.IsDragging)
            {
                UICharacterHotkeys.FinishHotkeyAimControls(_hotkeyCancel);
                IsDragging = false;
            }
        }
    }
}
