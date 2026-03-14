using Cysharp.Text;
using LiteNetLibManager;
using UnityEngine;
using UnityEngine.Events;

namespace MultiplayerARPG
{
    public class UIMail : UIBase
    {
        [Header("String Formats")]
        [Tooltip("Format => {0} = {Sender Name}")]
        public UILocaleKeySetting formatSenderName = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_MAIL_SENDER_NAME);
        [Tooltip("Format => {0} = {Title}")]
        public UILocaleKeySetting formatTitle = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_MAIL_TITLE);
        [Tooltip("Format => {0} = {Content}")]
        public UILocaleKeySetting formatContent = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_MAIL_CONTENT);
        [Tooltip("Format => {0} = {Gold}")]
        public UILocaleKeySetting formatGold = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_GOLD);
        [Tooltip("Format => {0} = {Cash}")]
        public UILocaleKeySetting formatCash = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_CASH);
        [Tooltip("Format => {0} = {Sent Date}")]
        public UILocaleKeySetting formatSentDate = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_MAIL_SENT_DATE);

        [Header("UI Elements")]
        public TextWrapper textSenderName;
        public TextWrapper textTitle;
        public TextWrapper textContent;
        public GameObject textGoldRoot;
        public TextWrapper textGold;
        public GameObject textCashRoot;
        public TextWrapper textCash;
        public UICharacterCurrencies uiCurrencies;
        public UICharacterItems uiItems;
        public TextWrapper textSentDate;
        public UIMailList uiMailList;
        public GameObject[] noRewardObjects;
        public GameObject[] hasGoldObjects;
        public GameObject[] hasCashObjects;
        public GameObject[] readObjects;
        public GameObject[] unreadObjects;
        public GameObject[] claimObjects;
        public GameObject[] unclaimObjects;

        [Header("Events")]
        public UnityEvent onMailEmpty = new UnityEvent();
        public UnityEvent onReadMail = new UnityEvent();
        public UnityEvent onClaimMailItems = new UnityEvent();
        public UnityEvent onDeleteMail = new UnityEvent();

        [Header("Default Messages")]
        public LanguageTextSetting noSelectedMailSender = new LanguageTextSetting()
        {
            defaultText = "N/A",
        };
        public LanguageTextSetting noSelectedMailTitle = new LanguageTextSetting()
        {
            defaultText = "NO SELECTED MAIL",
        };
        public LanguageTextSetting noSelectedMailContent = new LanguageTextSetting()
        {
            defaultText = "Please select a mail to read",
        };

        private string _mailId;
        public string MailId
        {
            get { return _mailId; }
            set
            {
                _mailId = value;
                ReadMail();
            }
        }

        public Mail Mail { get; protected set; }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            textSenderName = null;
            textTitle = null;
            textContent = null;
            textGoldRoot = null;
            textGold = null;
            textCashRoot = null;
            textCash = null;
            uiCurrencies = null;
            uiItems = null;
            textSentDate = null;
            uiMailList = null;
            noRewardObjects.Nullify();
            hasGoldObjects.Nullify();
            hasCashObjects.Nullify();
            readObjects.Nullify();
            unreadObjects.Nullify();
            claimObjects.Nullify();
            unclaimObjects.Nullify();
            onMailEmpty?.RemoveAllListeners();
            onMailEmpty = null;
            onReadMail?.RemoveAllListeners();
            onReadMail = null;
            onClaimMailItems?.RemoveAllListeners();
            onClaimMailItems = null;
            onDeleteMail?.RemoveAllListeners();
            onDeleteMail = null;
        }

        private void ReadMail()
        {
            GameInstance.ClientMailHandlers.RequestReadMail(new RequestReadMailMessage()
            {
                id = MailId,
            }, ReadMailCallback);
        }

        private void ReadMailCallback(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseReadMailMessage response)
        {
            ClientMailActions.ResponseReadMail(requestHandler, responseCode, response);
            if (responseCode.ShowUnhandledResponseMessageDialog(response.message)) return;
            Mail = response.mail;
            UpdateData(Mail);
            onReadMail.Invoke();
        }

        public void OnClickClaimItems()
        {
            GameInstance.ClientMailHandlers.RequestClaimMailItems(new RequestClaimMailItemsMessage()
            {
                id = MailId
            }, ClaimMailItemsCallback);
        }

        private void ClaimMailItemsCallback(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseClaimMailItemsMessage response)
        {
            ClientMailActions.ResponseClaimMailItems(requestHandler, responseCode, response);
            if (responseCode.ShowUnhandledResponseMessageDialog(response.message)) return;
            UISceneGlobal.Singleton.ShowMessageDialog(LanguageManager.GetText(UITextKeys.UI_LABEL_SUCCESS.ToString()), LanguageManager.GetText(UITextKeys.UI_MAIL_CLAIMED.ToString()));
            Mail.IsClaim = true;
            UpdateData(Mail);
            if (uiMailList)
                uiMailList.Refresh();
            onClaimMailItems.Invoke();
        }

        public void OnClickDelete()
        {
            GameInstance.ClientMailHandlers.RequestDeleteMail(new RequestDeleteMailMessage()
            {
                id = MailId
            }, DeleteMailCallback);
        }

        private void DeleteMailCallback(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseDeleteMailMessage response)
        {
            ClientMailActions.ResponseDeleteMail(requestHandler, responseCode, response);
            if (responseCode.ShowUnhandledResponseMessageDialog(response.message)) return;
            UISceneGlobal.Singleton.ShowMessageDialog(LanguageManager.GetText(UITextKeys.UI_LABEL_SUCCESS.ToString()), LanguageManager.GetText(UITextKeys.UI_MAIL_DELETED.ToString()));
            Hide();
            if (uiMailList)
                uiMailList.Refresh();
            onDeleteMail.Invoke();
        }

        protected virtual void UpdateData(Mail mail)
        {
            if (textSenderName != null)
            {
                textSenderName.text = ZString.Format(
                    LanguageManager.GetText(formatSenderName),
                    mail == null ? noSelectedMailSender.Text : mail.SenderName);
                textSenderName.SetGameObjectActive(mail != null);
            }

            if (textTitle != null)
            {
                textTitle.text = ZString.Format(
                    LanguageManager.GetText(formatTitle),
                    mail == null ? noSelectedMailTitle.Text : mail.Title);
                textTitle.SetGameObjectActive(mail != null);
            }

            if (textContent != null)
            {
                textContent.text = ZString.Format(
                    LanguageManager.GetText(formatContent),
                    mail == null ? noSelectedMailContent.Text : mail.Content);
                textContent.SetGameObjectActive(mail != null);
            }

            if (textGoldRoot != null)
                textGoldRoot.SetActive(mail != null && mail.Gold != 0);

            if (textGold != null)
            {
                textGold.text = ZString.Format(
                    LanguageManager.GetText(formatGold),
                    mail == null ? "0" : mail.Gold.ToString("N0"));
                textGold.SetGameObjectActive(mail != null && mail.Gold != 0);
            }

            if (textCashRoot != null)
                textCashRoot.SetActive(mail != null && mail.Cash != 0);

            if (textCash != null)
            {
                textCash.text = ZString.Format(
                    LanguageManager.GetText(formatCash),
                    mail == null ? "0" : mail.Cash.ToString("N0"));
                textCash.SetGameObjectActive(mail != null && mail.Cash != 0);
            }

            if (uiCurrencies != null)
            {
                if (mail != null && mail.Currencies.Count > 0)
                {
                    uiCurrencies.NotForOwningCharacter = true;
                    uiCurrencies.UpdateData(GameInstance.PlayingCharacter, mail.Currencies);
                    uiCurrencies.Show();
                }
                else
                {
                    uiCurrencies.Hide();
                }
            }

            if (uiItems != null)
            {
                if (mail != null && mail.Items.Count > 0)
                {
                    uiItems.UpdateData(GameInstance.PlayingCharacter, mail.Items);
                    uiItems.Show();
                }
                else
                {
                    uiItems.Hide();
                }
            }

            if (textSentDate != null)
            {
                System.DateTime dateTime = GenericUtils.GetStartDateTime();
                if (mail != null)
                    dateTime = dateTime.AddSeconds(mail.SentTimestamp).ToLocalTime();
                textSentDate.text = ZString.Format(
                    LanguageManager.GetText(formatSentDate),
                    (System.DateTime.Now - new System.DateTime(dateTime.Ticks)).GetPrettyDate());
                textSentDate.SetGameObjectActive(mail != null);
            }

            if (noRewardObjects != null && noRewardObjects.Length > 0)
            {
                for (int i = 0; i < noRewardObjects.Length; ++i)
                {
                    noRewardObjects[i].SetActive(mail == null ||
                        (mail.Gold == 0 && mail.Cash == 0 &&
                        (mail.Items == null || mail.Items.Count == 0) &&
                        (mail.Currencies == null || mail.Currencies.Count == 0)));
                }
            }

            if (hasGoldObjects != null && hasGoldObjects.Length > 0)
            {
                for (int i = 0; i < hasGoldObjects.Length; ++i)
                {
                    hasGoldObjects[i].SetActive(mail != null && mail.Gold != 0);
                }
            }

            if (hasCashObjects != null && hasCashObjects.Length > 0)
            {
                for (int i = 0; i < hasCashObjects.Length; ++i)
                {
                    hasCashObjects[i].SetActive(mail != null && mail.Cash != 0);
                }
            }

            if (readObjects != null && readObjects.Length > 0)
            {
                for (int i = 0; i < readObjects.Length; ++i)
                {
                    readObjects[i].SetActive(mail != null && mail.IsRead);
                }
            }

            if (unreadObjects != null && unreadObjects.Length > 0)
            {
                for (int i = 0; i < unreadObjects.Length; ++i)
                {
                    unreadObjects[i].SetActive(mail == null || !mail.IsRead);
                }
            }

            if (claimObjects != null && claimObjects.Length > 0)
            {
                for (int i = 0; i < claimObjects.Length; ++i)
                {
                    claimObjects[i].SetActive(mail != null && mail.HaveItemsToClaim() && mail.IsClaim);
                }
            }

            if (unclaimObjects != null && unclaimObjects.Length > 0)
            {
                for (int i = 0; i < unclaimObjects.Length; ++i)
                {
                    unclaimObjects[i].SetActive(mail != null && mail.HaveItemsToClaim() && !mail.IsClaim);
                }
            }

            if (mail == null || string.IsNullOrWhiteSpace(mail.Id))
            {
                onMailEmpty.Invoke();
            }
        }
    }
}
