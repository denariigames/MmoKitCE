using Cysharp.Text;
using LiteNetLibManager;
using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerARPG
{
    public class UIParty : UISocialGroup<UISocialCharacter>
    {
        [Header("UI Elements")]
        public Toggle toggleShareExp;
        public Toggle toggleShareItem;
        public UIPartyCreate uiPartyCreate;
        public UIPartySetting uiPartySetting;

        public PartyData Party { get { return GameInstance.JoinedParty; } }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            toggleShareExp = null;
            toggleShareItem = null;
            uiPartyCreate = null;
            uiPartySetting = null;
        }

        protected override void UpdateUIs()
        {
            if (toggleShareExp != null)
            {
                toggleShareExp.interactable = false;
                toggleShareExp.isOn = Party != null && Party.shareExp;
            }

            if (toggleShareItem != null)
            {
                toggleShareItem.interactable = false;
                toggleShareItem.isOn = Party != null && Party.shareItem;
            }

            base.UpdateUIs();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            UpdatePartyUIs(UpdatePartyMessage.UpdateType.Member, Party);
            ClientPartyActions.onNotifyPartyUpdated += UpdatePartyUIs;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (uiPartyCreate != null)
                uiPartyCreate.Hide();
            if (uiPartySetting != null)
                uiPartySetting.Hide();
            ClientPartyActions.onNotifyPartyUpdated -= UpdatePartyUIs;
        }

        private void UpdatePartyUIs(UpdatePartyMessage.UpdateType updateType, PartyData party)
        {
            if (party == null)
                return;

            memberAmount = party.CountMember();
            UpdateUIs();

            string selectedId = MemberSelectionManager.SelectedUI != null ? MemberSelectionManager.SelectedUI.Data.id : string.Empty;
            MemberSelectionManager.Clear();

            SocialCharacterData[] members;
            party.GetSortedMembers(out members);

            UISocialCharacter selectedUI = null;
            UISocialCharacter tempUI;
            MemberList.Generate(members, (index, data, ui) =>
            {
                tempUI = ui.GetComponent<UISocialCharacter>();
                tempUI.uiSocialGroup = this;
                tempUI.index = index;
                tempUI.Data = data;
                tempUI.Show();
                MemberSelectionManager.Add(tempUI);
                if (selectedId.Equals(data.id))
                    selectedUI = tempUI;
            });

            if (selectedUI == null)
            {
                MemberSelectionManager.DeselectSelectedUI();
            }
            else
            {
                selectedUI.SelectByManager();
            }
        }

        public void OnClickCreateParty()
        {
            // If already in the party, return
            if (currentSocialId > 0)
                return;
            // Show create party dialog
            if (uiPartyCreate != null)
                uiPartyCreate.Show(false, false);
        }

        public void OnClickChangeLeader()
        {
            // If not in the party or not leader, return
            if (!OwningCharacterIsLeader() || MemberSelectionManager.SelectedUI == null)
                return;

            SocialCharacterData partyMember = MemberSelectionManager.SelectedUI.Data;
            UISceneGlobal.Singleton.ShowMessageDialog(LanguageManager.GetText(UITextKeys.UI_PARTY_CHANGE_LEADER.ToString()), ZString.Format(LanguageManager.GetText(UITextKeys.UI_PARTY_CHANGE_LEADER_DESCRIPTION.ToString()), partyMember.characterName), false, true, true, false, null, () =>
            {
                GameInstance.ClientPartyHandlers.RequestChangePartyLeader(new RequestChangePartyLeaderMessage()
                {
                    memberId = partyMember.id,
                }, ChangePartyLeaderCallback);
            });
        }

        private void ChangePartyLeaderCallback(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseChangePartyLeaderMessage response)
        {
            ClientPartyActions.ResponseChangePartyLeader(requestHandler, responseCode, response);
            if (responseCode.ShowUnhandledResponseMessageDialog(response.message)) return;
        }

        public void OnClickSettingParty()
        {
            // If not in the party or not leader, return
            if (!OwningCharacterIsLeader() && Party != null)
                return;

            // Show setup party dialog
            if (uiPartySetting != null)
                uiPartySetting.Show(Party.shareExp, Party.shareItem);
        }

        public void OnClickLeaveParty()
        {
            UISceneGlobal.Singleton.ShowMessageDialog(LanguageManager.GetText(UITextKeys.UI_PARTY_LEAVE.ToString()), LanguageManager.GetText(UITextKeys.UI_PARTY_LEAVE_DESCRIPTION.ToString()), false, true, true, false, null, () =>
            {
                GameInstance.ClientPartyHandlers.RequestLeaveParty(LeavePartyCallback);
            });
        }

        private void LeavePartyCallback(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseLeavePartyMessage response)
        {
            ClientPartyActions.ResponseLeaveParty(requestHandler, responseCode, response);
            if (responseCode.ShowUnhandledResponseMessageDialog(response.message)) return;
        }

        public override int GetSocialId()
        {
            return GameInstance.PlayingCharacter.PartyId;
        }

        public override int GetMaxMemberAmount()
        {
            if (Party == null)
                return 0;
            return Party.MaxMember();
        }

        public override bool IsLeader(string characterId)
        {
            return Party != null && Party.IsLeader(characterId);
        }

        public override bool CanKick(string characterId)
        {
            return Party != null && Party.CanKick(characterId);
        }

        public override bool OwningCharacterIsLeader()
        {
            return IsLeader(GameInstance.PlayingCharacter.Id);
        }

        public override bool OwningCharacterCanKick()
        {
            return CanKick(GameInstance.PlayingCharacter.Id);
        }
    }
}
