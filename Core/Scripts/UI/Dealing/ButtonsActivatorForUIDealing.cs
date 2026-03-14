using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerARPG
{
    [RequireComponent(typeof(UIDealing))]
    public class ButtonsActivatorForUIDealing : MonoBehaviour
    {
        public Button buttonLock;
        public Button buttonConfirm;
        public Button buttonCancel;
        public GameObject owningLockSign;
        public GameObject anotherLockSign;
        public GameObject owningConfirmSign;
        public GameObject anotherConfirmSign;
        private UIDealing ui;

        private void Awake()
        {
            ui = GetComponent<UIDealing>();
            ui.onStateChangeToDealing.AddListener(OnStateChangeToDealing);
            ui.onStateChangeToLock.AddListener(OnStateChangeToLock);
            ui.onStateChangeToConfirm.AddListener(OnStateChangeToConfirm);
            ui.onAnotherStateChangeToLock.AddListener(OnAnotherStateChangeToLock);
            ui.onAnotherStateChangeToConfirm.AddListener(OnAnotherStateChangeToConfirm);
            ui.onBothStateChangeToLock.AddListener(OnBothStateChangeToLock);
            // Refresh UI data to applies events
            ui.ForceUpdate();
        }

        public void DeactivateAllButtons()
        {
            if (buttonLock)
                buttonLock.gameObject.SetActive(false);
            if (buttonConfirm)
                buttonConfirm.gameObject.SetActive(false);
            if (buttonCancel)
                buttonCancel.gameObject.SetActive(false);
            if (owningLockSign)
                owningLockSign.SetActive(false);
            if (anotherLockSign)
                anotherLockSign.SetActive(false);
            if (owningConfirmSign)
                owningConfirmSign.SetActive(false);
            if (anotherConfirmSign)
                anotherConfirmSign.SetActive(false);
        }

        public void OnStateChangeToDealing()
        {
            DeactivateAllButtons();
            if (buttonLock)
            {
                buttonLock.gameObject.SetActive(true);
                buttonLock.interactable = true;
            }
            if (buttonCancel)
                buttonCancel.gameObject.SetActive(true);
        }

        public void OnStateChangeToLock()
        {
            DeactivateAllButtons();
            if (buttonLock)
            {
                buttonLock.gameObject.SetActive(true);
                buttonLock.interactable = false;
            }
            if (buttonCancel)
                buttonCancel.gameObject.SetActive(true);
            if (owningLockSign)
                owningLockSign.SetActive(true);
        }

        public void OnStateChangeToConfirm()
        {
            if (owningConfirmSign)
                owningConfirmSign.SetActive(true);
        }

        public void OnAnotherStateChangeToLock()
        {
            if (anotherLockSign)
                anotherLockSign.SetActive(true);
        }

        public void OnAnotherStateChangeToConfirm()
        {
            if (anotherConfirmSign)
                anotherConfirmSign.SetActive(true);
        }

        public void OnBothStateChangeToLock()
        {
            DeactivateAllButtons();
            if (buttonConfirm)
                buttonConfirm.gameObject.SetActive(true);
            if (buttonCancel)
                buttonCancel.gameObject.SetActive(true);
            if (owningLockSign)
                owningLockSign.SetActive(true);
            if (anotherLockSign)
                anotherLockSign.SetActive(true);
        }
    }
}
