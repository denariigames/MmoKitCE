using LiteNetLibManager;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

namespace MultiplayerARPG
{
    public class UIFriendRequest : UISocialGroup<UISocialCharacter>
    {
        // TODO: should send character name to find at server
        public InputFieldWrapper inputCharacterName;
        public Button buttonRefresh;

        protected override void OnDestroy()
        {
            base.OnDestroy();
            buttonRefresh = null;
            inputCharacterName = null;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            onFriendRequestAccepted.RemoveListener(Refresh);
            onFriendRequestAccepted.AddListener(Refresh);
            onFriendRequestDeclined.RemoveListener(Refresh);
            onFriendRequestDeclined.AddListener(Refresh);
            if (buttonRefresh)
            {
                buttonRefresh.onClick.RemoveListener(Refresh);
                buttonRefresh.onClick.AddListener(Refresh);
            }
            if (inputCharacterName)
                inputCharacterName.text = string.Empty;
            Refresh();
        }

        public void Refresh()
        {
            GameInstance.ClientFriendHandlers.RequestGetFriendRequests(new RequestGetFriendRequestsMessage()
            {
                skip = 0,
                limit = 50,
            }, GetFriendRequestsCallback);
            // Update notification count
            UIFriendRequestNotification[] notifications = FindObjectsOfType<UIFriendRequestNotification>();
            for (int i = 0; i < notifications.Length; ++i)
            {
                notifications[i].Refresh();
            }
        }

        private void GetFriendRequestsCallback(ResponseHandlerData responseHandler, AckResponseCode responseCode, ResponseGetFriendRequestsMessage response)
        {
            ClientFriendActions.ResponseGetFriendRequests(responseHandler, responseCode, response);
            if (responseCode.ShowUnhandledResponseMessageDialog(response.message)) return;
            List<SocialCharacterData> friendRequests = new List<SocialCharacterData>();
            if (response.friendRequests != null && response.friendRequests.Count > 0)
                friendRequests.AddRange(response.friendRequests);
            if (inputCharacterName != null && !string.IsNullOrWhiteSpace(inputCharacterName.text))
                friendRequests = friendRequests.Where(Fillter).ToList();
            UpdateFriendRequestsUIs(friendRequests);
        }

        private bool Fillter(SocialCharacterData data)
        {
            string filterText = inputCharacterName.text;
            return (!string.IsNullOrWhiteSpace(data.characterName) && data.characterName.ToLower().Contains(filterText.ToLower())) ||
                (!string.IsNullOrWhiteSpace(data.id) && data.id.ToLower().Contains(filterText.ToLower()));
        }

        private void UpdateFriendRequestsUIs(List<SocialCharacterData> friends)
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
                tempUI.onFriendRequestAccepted.RemoveListener(Refresh);
                tempUI.onFriendRequestAccepted.AddListener(Refresh);
                tempUI.onFriendRequestDeclined.RemoveListener(Refresh);
                tempUI.onFriendRequestDeclined.AddListener(Refresh);
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
