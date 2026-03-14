using LiteNetLib;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerARPG
{
    public partial class UISceneGlobal : MonoBehaviour
    {
        public static UISceneGlobal Singleton { get; private set; }
        public UIMessageDialog uiMessageDialog;
        public UIInputDialog uiInputDialog;
        public UIInputDialog uiPasswordDialog;
        public UIRewarding uiRewarding;

        private void Awake()
        {
            if (Singleton != null)
            {
                Destroy(gameObject);
                return;
            }

            Singleton = this;
            DontDestroyOnLoad(gameObject);
            if (uiPasswordDialog == null)
                uiPasswordDialog = uiInputDialog;
        }

        public void ShowMessageDialog(string title,
            string description,
            bool showButtonOkay = true,
            bool showButtonYes = false,
            bool showButtonNo = false,
            bool showButtonCancel = false,
            System.Action onClickOkay = null,
            System.Action onClickYes = null,
            System.Action onClickNo = null,
            System.Action onClickCancel = null)
        {
#if DEBUG_UI
            Debug.Log($"Showing message dialog {title} {description}");
#endif
            uiMessageDialog.Show(title,
                description,
                showButtonOkay,
                showButtonYes,
                showButtonNo,
                showButtonCancel,
                onClickOkay,
                onClickYes,
                onClickNo,
                onClickCancel);
        }

        public void ShowInputDialog(string title,
            string description,
            System.Action<string> onConfirmText,
            string defaultText = "",
            InputField.ContentType contentType = InputField.ContentType.Standard,
            int characterLimit = 0)
        {
#if DEBUG_UI
            Debug.Log($"Showing input dialog (string) {title} {description}");
#endif
            uiInputDialog.Show(title,
                description,
                onConfirmText,
                defaultText,
                contentType,
                characterLimit);
        }

        public void ShowInputDialog(string title,
            string description,
            System.Action<int> onConfirmInteger,
            int? minAmount = null,
            int? maxAmount = null,
            int defaultAmount = 0,
            string placeHolder = "")
        {
#if DEBUG_UI
            Debug.Log($"Showing input dialog (integer) {title} {description}");
#endif
            uiInputDialog.Show(title,
                description,
                onConfirmInteger,
                minAmount,
                maxAmount,
                defaultAmount,
                placeHolder);
        }

        public void ShowInputDialog(string title,
            string description,
            System.Action<float> onConfirmDecimal,
            float? minAmount = null,
            float? maxAmount = null,
            float defaultAmount = 0f,
            string placeHolder = "")
        {
#if DEBUG_UI
            Debug.Log($"Showing input dialog (float) {title} {description}");
#endif
            uiInputDialog.Show(title,
                description,
                onConfirmDecimal,
                minAmount,
                maxAmount,
                defaultAmount,
                placeHolder);
        }

        public void ShowPasswordDialog(string title,
            string description,
            System.Action<string> onConfirmText,
            string defaultText = "",
            InputField.ContentType contentType = InputField.ContentType.Standard,
            int characterLimit = 0,
            string placeHolder = "")
        {
            uiPasswordDialog.Show(title,
                description,
                onConfirmText,
                defaultText,
                contentType,
                characterLimit,
                placeHolder);
        }

        public void ShowDisconnectDialog(DisconnectReason reason, SocketError socketError, UITextKeys message, string source)
        {
            string errorMessage = LanguageManager.GetUnknowTitle();
            string socketErrorCode = socketError == SocketError.Success ? "None" : socketError.ToString();
            if (message.IsError())
            {
                errorMessage = LanguageManager.GetText(message.ToString());
            }
            else
            {
                switch (reason)
                {
                    case DisconnectReason.DisconnectPeerCalled:
                        errorMessage = LanguageManager.GetText(UITextKeys.UI_ERROR_KICKED_FROM_SERVER.ToString());
                        break;
                    case DisconnectReason.ConnectionFailed:
                        errorMessage = LanguageManager.GetText(UITextKeys.UI_ERROR_CONNECTION_FAILED.ToString());
                        break;
                    case DisconnectReason.ConnectionRejected:
                        errorMessage = LanguageManager.GetText(UITextKeys.UI_ERROR_CONNECTION_REJECTED.ToString());
                        break;
                    case DisconnectReason.RemoteConnectionClose:
                        errorMessage = LanguageManager.GetText(UITextKeys.UI_ERROR_REMOTE_CONNECTION_CLOSE.ToString());
                        break;
                    case DisconnectReason.InvalidProtocol:
                        errorMessage = LanguageManager.GetText(UITextKeys.UI_ERROR_INVALID_PROTOCOL.ToString());
                        break;
                    case DisconnectReason.HostUnreachable:
                        errorMessage = LanguageManager.GetText(UITextKeys.UI_ERROR_HOST_UNREACHABLE.ToString());
                        break;
                    case DisconnectReason.Timeout:
                        errorMessage = LanguageManager.GetText(UITextKeys.UI_ERROR_CONNECTION_TIMEOUT.ToString());
                        break;
                }
            }

            if (string.IsNullOrWhiteSpace(source))
                source = LanguageManager.GetUnknowTitle();

            Singleton.ShowMessageDialog(
                LanguageManager.GetText(UITextKeys.UI_LABEL_DISCONNECTED.ToString()),
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                $"{errorMessage}\nRsn: {reason}\nErr: {socketErrorCode}\nMsg: {message}\nSrc: {source}",
#else
                LanguageManager.GetText(errorMessage.ToString()),
#endif
                true, false, false, false);
        }

        public void ShowRewardDialog(UIRewardingData rewardData)
        {
            uiRewarding.Data = rewardData;
            uiRewarding.Show();
        }
    }
}
