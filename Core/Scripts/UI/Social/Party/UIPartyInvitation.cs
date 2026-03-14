using Cysharp.Text;
using LiteNetLibManager;
using UnityEngine;

namespace MultiplayerARPG
{
    public partial class UIPartyInvitation : UISelectionEntry<PartyInvitationData>
    {
        [Header("String Formats")]
        [Tooltip("Format => {0} = {Character Name}")]
        public UILocaleKeySetting formatKeyName = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE);
        [Tooltip("Format => {0} = {Level}")]
        public UILocaleKeySetting formatKeyLevel = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_LEVEL);

        [Header("UI Elements")]
        public TextWrapper uiTextName;
        public TextWrapper uiTextLevel;
        public UIGageValue uiTimeoutGage;
        public GameObject[] timeoutSigns = new GameObject[0];
        public GameObject[] notTimeoutSigns = new GameObject[0];

        private float _showedTime;

        protected override void OnEnable()
        {
            base.OnEnable();
            _showedTime = Time.unscaledTime;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            uiTextName = null;
            uiTextLevel = null;
            uiTimeoutGage = null;
            timeoutSigns.Nullify();
            notTimeoutSigns.Nullify();
        }

        public override void ManagedUpdate()
        {
            base.ManagedUpdate();
            float timeout = GameInstance.Singleton.SocialSystemSetting.PartyInvitationTimeout;
            float takenTime = Time.unscaledTime - _showedTime;
            bool alreadyTimedout = takenTime > timeout;
            if (uiTimeoutGage != null)
                uiTimeoutGage.Update(timeout - takenTime, timeout);
            foreach (GameObject obj in timeoutSigns)
            {
                obj.SetActive(alreadyTimedout);
            }
            foreach (GameObject obj in notTimeoutSigns)
            {
                obj.SetActive(!alreadyTimedout);
            }
        }

        protected override void UpdateData()
        {
            if (uiTextName != null)
                uiTextName.text = ZString.Format(LanguageManager.GetText(formatKeyName), Data.InviterName);

            if (uiTextLevel != null)
                uiTextLevel.text = ZString.Format(LanguageManager.GetText(formatKeyLevel), Data.InviterLevel.ToString("N0"));
        }

        public void OnClickAccept()
        {
            GameInstance.ClientPartyHandlers.RequestAcceptPartyInvitation(new RequestAcceptPartyInvitationMessage()
            {
                partyId = Data.PartyId,
                inviterId = Data.InviterId,
            }, AcceptPartyInvitationCallback);
        }

        private void AcceptPartyInvitationCallback(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseAcceptPartyInvitationMessage response)
        {
            ClientPartyActions.ResponseAcceptPartyInvitation(requestHandler, responseCode, response);
            if (responseCode.ShowUnhandledResponseMessageDialog(response.message)) return;
            Hide();
        }

        public void OnClickDecline()
        {
            GameInstance.ClientPartyHandlers.RequestDeclinePartyInvitation(new RequestDeclinePartyInvitationMessage()
            {
                partyId = Data.PartyId,
                inviterId = Data.InviterId,
            }, DeclinePartyInvitationCallback);
            Hide();
        }

        private void DeclinePartyInvitationCallback(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseDeclinePartyInvitationMessage response)
        {
            ClientPartyActions.ResponseDeclinePartyInvitation(requestHandler, responseCode, response);
            if (responseCode.ShowUnhandledResponseMessageDialog(response.message)) return;
            Hide();
        }
    }
}
