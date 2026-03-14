using LiteNetLibManager;
using System.Collections.Generic;
using UnityEngine.UI;

namespace MultiplayerARPG
{
    public class UIFindCharacters : UISocialGroup<UISocialCharacter>
    {
        public InputFieldWrapper inputCharacterName;
        public Button buttonFind;

        protected override void OnDestroy()
        {
            base.OnDestroy();
            buttonFind = null;
            inputCharacterName = null;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            onFriendRequested.RemoveListener(OnClickFindCharacters);
            onFriendRequested.AddListener(OnClickFindCharacters);
            onFriendAdded.RemoveListener(OnClickFindCharacters);
            onFriendAdded.AddListener(OnClickFindCharacters);
            if (buttonFind)
            {
                buttonFind.onClick.RemoveListener(OnClickFindCharacters);
                buttonFind.onClick.AddListener(OnClickFindCharacters);
            }
            if (inputCharacterName)
                inputCharacterName.text = string.Empty;
            OnClickFindCharacters();
        }

        private void UpdateFoundCharactersUIs(List<SocialCharacterData> foundCharacters)
        {
            if (foundCharacters == null)
                return;

            memberAmount = foundCharacters.Count;
            UpdateUIs();

            string selectedId = MemberSelectionManager.SelectedUI != null ? MemberSelectionManager.SelectedUI.Data.id : string.Empty;
            MemberSelectionManager.Clear();

            UISocialCharacter selectedUI = null;
            UISocialCharacter tempUI;
            MemberList.Generate(foundCharacters, (index, data, ui) =>
            {
                tempUI = ui.GetComponent<UISocialCharacter>();
                tempUI.uiSocialGroup = this;
                tempUI.index = index;
                tempUI.Data = data;
                tempUI.Show();
                tempUI.onFriendRequested.RemoveListener(OnClickFindCharacters);
                tempUI.onFriendRequested.AddListener(OnClickFindCharacters);
                tempUI.onFriendAdded.RemoveListener(OnClickFindCharacters);
                tempUI.onFriendAdded.AddListener(OnClickFindCharacters);
                MemberSelectionManager.Add(tempUI);
                if (selectedId.Equals(data.id))
                    selectedUI = tempUI;
            });

            if (memberListEmptyObject != null)
                memberListEmptyObject.SetActive(foundCharacters.Count == 0);

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

        public void OnClickFindCharacters()
        {
            string characterName = string.Empty;
            if (inputCharacterName != null)
                characterName = inputCharacterName.text;
            GameInstance.ClientFriendHandlers.RequestFindCharacters(new RequestFindCharactersMessage()
            {
                characterName = characterName,
                skip = 0,
                limit = 50,
            }, FindCharactersCallback);
        }

        private void FindCharactersCallback(ResponseHandlerData responseHandler, AckResponseCode responseCode, ResponseSocialCharacterListMessage response)
        {
            ClientFriendActions.ResponseFindCharacters(responseHandler, responseCode, response);
            if (responseCode.ShowUnhandledResponseMessageDialog(response.message)) return;
            UpdateFoundCharactersUIs(response.characters);
        }
    }
}
