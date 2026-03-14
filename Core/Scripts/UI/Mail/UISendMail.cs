using LiteNetLibManager;
using UnityEngine.UI;

namespace MultiplayerARPG
{
    public class UISendMail : UIBase
    {
        public InputFieldWrapper inputReceiverName;
        public InputFieldWrapper inputTitle;
        public InputFieldWrapper inputContent;
        public InputFieldWrapper inputGold;

        public string ReceiverName
        {
            get { return inputReceiverName == null ? string.Empty : inputReceiverName.text; }
        }

        public string Title
        {
            get { return inputTitle == null ? string.Empty : inputTitle.text; }
        }

        public string Content
        {
            get { return inputContent == null ? string.Empty : inputContent.text; }
        }

        public int Gold
        {
            get
            {
                try
                {
                    return int.Parse(inputGold.text);
                }
                catch
                {
                    return 0;
                }
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            inputReceiverName = null;
            inputTitle = null;
            inputContent = null;
            inputGold = null;
        }

        private void OnEnable()
        {
            if (inputReceiverName != null)
                inputReceiverName.text = string.Empty;
            if (inputTitle != null)
                inputTitle.text = string.Empty;
            if (inputContent != null)
                inputContent.text = string.Empty;
            if (inputGold != null)
            {
                inputGold.text = "0";
                inputGold.contentType = InputField.ContentType.IntegerNumber;
            }
        }

        public void OnClickSend()
        {
            if (inputReceiverName != null)
                inputReceiverName.interactable = false;
            if (inputTitle != null)
                inputTitle.interactable = false;
            if (inputContent != null)
                inputContent.interactable = false;
            if (inputGold != null)
                inputGold.interactable = false;
            GameInstance.ClientMailHandlers.RequestSendMail(new RequestSendMailMessage()
            {
                receiverName = ReceiverName,
                title = Title,
                content = Content,
                gold = Gold,
            }, MailSendCallback);
        }

        private void MailSendCallback(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseSendMailMessage response)
        {
            if (inputReceiverName != null)
                inputReceiverName.interactable = true;
            if (inputTitle != null)
                inputTitle.interactable = true;
            if (inputContent != null)
                inputContent.interactable = true;
            if (inputGold != null)
                inputGold.interactable = true;
            ClientMailActions.ResponseSendMail(requestHandler, responseCode, response);
            if (responseCode.ShowUnhandledResponseMessageDialog(response.message)) return;
            // Success, hide this dialog
            UISceneGlobal.Singleton.ShowMessageDialog(LanguageManager.GetText(UITextKeys.UI_LABEL_SUCCESS.ToString()), LanguageManager.GetText(UITextKeys.UI_MAIL_SENT.ToString()));
            Hide();
        }
    }
}
