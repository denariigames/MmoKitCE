using LiteNetLibManager;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

namespace MultiplayerARPG
{
    public class UIFriend : UISocialGroup<UISocialCharacter>
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
            onFriendRemoved.RemoveListener(Refresh);
            onFriendRemoved.AddListener(Refresh);
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
            GameInstance.ClientFriendHandlers.RequestGetFriends(new RequestGetFriendsMessage()
            {
                skip = 0,
                limit = 50,
            }, GetFriendsCallback);
        }

        private void GetFriendsCallback(ResponseHandlerData responseHandler, AckResponseCode responseCode, ResponseGetFriendsMessage response)
        {
            ClientFriendActions.ResponseGetFriends(responseHandler, responseCode, response);
            if (responseCode.ShowUnhandledResponseMessageDialog(response.message)) return;
            List<SocialCharacterData> friends = new List<SocialCharacterData>();
            if (response.friends != null && response.friends.Count > 0)
                friends.AddRange(response.friends);
            if (inputCharacterName != null && !string.IsNullOrWhiteSpace(inputCharacterName.text))
                friends = friends.Where(Fillter).ToList();
            UpdateFriendsUIs(friends);
        }

        private bool Fillter(SocialCharacterData data)
        {
            string filterText = inputCharacterName.text;
            return (!string.IsNullOrWhiteSpace(data.characterName) && data.characterName.ToLower().Contains(filterText.ToLower())) ||
                (!string.IsNullOrWhiteSpace(data.id) && data.id.ToLower().Contains(filterText.ToLower()));
        }

        private void UpdateFriendsUIs(List<SocialCharacterData> friends)
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
                tempUI.onFriendRemoved.RemoveListener(Refresh);
                tempUI.onFriendRemoved.AddListener(Refresh);
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
