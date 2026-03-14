using Insthync.CameraAndInput;
using UnityEngine;

namespace MultiplayerARPG
{
    public class InputStateManager
    {
        public enum InputType
        {
            Button,
            Mouse,
            Key,
        }

        private InputType inputType;
        private string buttonName;
        private int mouseButton;
        private KeyCode keyCode;
        private float holdDuration;
        private bool isHolding;
        private bool isHolded;
        private float holdTime;
        private float lastDeltaTime;

        public bool IsPress { get; private set; }
        public bool IsRelease { get; private set; }
        public bool IsPressed { get; private set; }
        public bool IsHold { get; private set; }

        public InputStateManager(string buttonName, float holdDuration)
        {
            inputType = InputType.Button;
            this.buttonName = buttonName;
            this.holdDuration = holdDuration;
        }

        public InputStateManager(string buttonName) : this(buttonName, 1f) { }

        public InputStateManager(int mouseButton, float holdDuration)
        {
            inputType = InputType.Mouse;
            this.mouseButton = mouseButton;
            this.holdDuration = holdDuration;
        }

        public InputStateManager(int mouseIndex) : this(mouseIndex, 1f) { }

        public InputStateManager(KeyCode keyCode, float holdDuration)
        {
            inputType = InputType.Key;
            this.keyCode = keyCode;
            this.holdDuration = holdDuration;
        }

        public InputStateManager(KeyCode keyCode) : this(keyCode, 1f) { }

        public void OnUpdate(float deltaTime)
        {
            lastDeltaTime = deltaTime;
            IsPress = false;
            IsRelease = false;
            IsPressed = false;
            switch (inputType)
            {
                case InputType.Button:
                    OnUpdate_Button(deltaTime);
                    break;
                case InputType.Mouse:
                    OnUpdate_Mouse(deltaTime);
                    break;
                case InputType.Key:
                    OnUpdate_Key(deltaTime);
                    break;
            }
            isHolding = IsPress || IsPressed;
            if (holdTime >= holdDuration)
            {
                // Holded, so clear input states
                IsPress = false;
                IsRelease = false;
                IsPressed = false;
                // Set is hold to true just one time, in future frames it will be false
                IsHold = !isHolded;
                if (IsHold && !isHolded)
                    isHolded = true;
            }
        }

        private void OnUpdate_Button(float deltaTime)
        {
            if (InputManager.GetButtonDown(buttonName))
                IsPress = true;
            else if (InputManager.GetButtonUp(buttonName))
                IsRelease = true;
            else if (InputManager.GetButton(buttonName))
                IsPressed = true;
        }

        private void OnUpdate_Mouse(float deltaTime)
        {
            if (InputManager.GetMouseButtonDown(mouseButton))
                IsPress = true;
            else if (InputManager.GetMouseButtonUp(mouseButton))
                IsRelease = true;
            else if (InputManager.GetMouseButton(mouseButton))
                IsPressed = true;
        }

        private void OnUpdate_Key(float deltaTime)
        {
            if (InputManager.GetKeyDown(keyCode))
                IsPress = true;
            else if (InputManager.GetKeyUp(keyCode))
                IsRelease = true;
            else if (InputManager.GetKey(keyCode))
                IsPressed = true;
        }

        public void OnLateUpdate()
        {
            if (isHolding)
            {
                // Update hode time
                holdTime += lastDeltaTime;
            }
            else
            {
                // Reset hold state
                holdTime = 0f;
                isHolded = false;
            }
        }
    }
}