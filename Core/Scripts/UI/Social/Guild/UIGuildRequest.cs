using LiteNetLibManager;
using System.Collections.Generic;

namespace MultiplayerARPG
{
    public class UIGuildRequest : UISocialGroup<UISocialCharacter>
    {
        protected override void OnEnable()
        {
            base.OnEnable();
            onGuildRequestAccepted.RemoveListener(Refresh);
            onGuildRequestAccepted.AddListener(Refresh);
            onGuildRequestDeclined.RemoveListener(Refresh);
            onGuildRequestDeclined.AddListener(Refresh);
            Refresh();
        }

        public void Refresh()
        {
            GameInstance.ClientGuildHandlers.RequestGetGuildRequests(new RequestGetGuildRequestsMessage()
            {
                skip = 0,
                limit = 50,
            }, GetGuildRequestsCallback);
            // Update notification count
            UIGuildRequestNotification[] notifications = FindObjectsOfType<UIGuildRequestNotification>();
            for (int i = 0; i < notifications.Length; ++i)
            {
                notifications[i].Refresh();
            }
        }

        private void GetGuildRequestsCallback(ResponseHandlerData responseHandler, AckResponseCode responseCode, ResponseGetGuildRequestsMessage response)
        {
            ClientGuildActions.ResponseGetGuildRequests(responseHandler, responseCode, response);
            if (responseCode.ShowUnhandledResponseMessageDialog(response.message)) return;
            UpdateGuildRequestsUIs(response.guildRequests);
        }

        private void UpdateGuildRequestsUIs(List<SocialCharacterData> friends)
        {
            if (friends == null)
                return;

            memberAmount = friends.Count;
            UpdateUIs();

            string selectedId = MemberSelectionManager.SelectedUI != null ? MemberSelectionManager.SelectedUI.Data.id : string.Empty;
            MemberSelectionManager.Clear();

            UISocialCharacter selectedUI = null;
            UISocialCharacter tempUI;
            MemberList.Generate(friends, (index, data, ui) =>
            {
                tempUI = ui.GetComponent<UISocialCharacter>();
                tempUI.uiSocialGroup = this;
                tempUI.index = index;
                tempUI.Data = data;
                tempUI.Show();
                tempUI.onGuildRequestAccepted.RemoveListener(Refresh);
                tempUI.onGuildRequestAccepted.AddListener(Refresh);
                tempUI.onGuildRequestDeclined.RemoveListener(Refresh);
                tempUI.onGuildRequestDeclined.AddListener(Refresh);
                MemberSelectionManager.Add(tempUI);
                if (selectedId.Equals(data.id))
                    selectedUI = tempUI;
            });

            if (memberListEmptyObject != null)
                memberListEmptyObject.SetActive(friends.Count == 0);

            if (selectedUI == null)
            {
                MemberSelectionManager.DeselectSelectedUI();
            }
            else
            {
                selectedUI.SelectByManager();
            }
        }

        public override bool CanKick(string characterId)
        {
            return false;
        }

        public override int GetMaxMemberAmount()
        {
            return 0;
        }

        public override int GetSocialId()
        {
            return 1;
        }

        public override bool IsLeader(string characterId)
        {
            return false;
        }

        public override bool OwningCharacterCanKick()
        {
            return false;
        }

        public override bool OwningCharacterIsLeader()
        {
            return false;
        }
    }
}
