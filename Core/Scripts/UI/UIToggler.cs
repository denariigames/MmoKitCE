using Insthync.CameraAndInput;
using UnityEngine;
using UnityEngine.Serialization;

public class UIToggler : MonoBehaviour
{
    public UIBase ui;
    [Tooltip("It will toggle `ui` when key with `keyCode` pressed or button with `buttonName` pressed.")]
    [FormerlySerializedAs("key")]
    public KeyCode keyCode;
    [Tooltip("It will toggle `ui` when key with `keyCode` pressed or button with `buttonName` pressed.")]
    public string buttonName;

    private void Update()
    {
        if (GenericUtils.IsFocusInputField())
            return;
        if (InputManager.GetButtonDown(buttonName) || InputManager.GetKeyDown(keyCode))
            ui.Toggle();
    }
}
