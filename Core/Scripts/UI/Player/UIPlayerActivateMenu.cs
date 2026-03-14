using LiteNetLibManager;
using UnityEngine;
using UnityEngine.Serialization;

namespace MultiplayerARPG
{
    public partial class UIPlayerActivateMenu : UISelectionEntry<IPlayerCharacterData>
    {
        [FormerlySerializedAs("uiCharacter")]
        public UICharacter uiAnotherCharacter;
        [Tooltip("These objects will be activated when owning character can invite to join party")]
        public GameObject[] partyInviteObjects = new GameObject[0];
        public bool autoCreatePartyIfNotCreated = true;
        [Tooltip("These objects will be activated when owning character can invite to join guild")]
        public GameObject[] guildInviteObjects = new GameObject[0];
        [Tooltip("These objects will be activated when owning character can invite to deal")]
        public GameObject[] dealObjects = new GameObject[0];
        [Tooltip("These objects will be activated when owning character can invite to duel")]
        public GameObject[] duelObjects = new GameObject[0];
        [Tooltip("These objects will be activated when owning character can see vending")]
        public GameObject[] vendingObjects = new GameObject[0];

        protected override void OnEnable()
        {
            base.OnEnable();
            UIPlayerActivateMenu[] comps = FindObjectsByType<UIPlayerActivateMenu>(FindObjectsSortMode.None);
            for (int i = 0; i < comps.Length; ++i)
            {
                if (comps[i] == this)
                    continue;
                comps[i].Hide();
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            uiAnotherCharacter = null;
            partyInviteObjects.Nullify();
            guildInviteObjects.Nullify();
            vendingObjects.Nullify();
            _data = null;
        }

        protected override void UpdateUI()
        {
            if (Data == null)
            {
                Hide();
                return;
            }
            base.UpdateUI();
            bool canInviteParty = Data.PartyId <= 0 && GameInstance.JoinedParty != null && GameInstance.JoinedParty.CanInvite(GameInstance.PlayingCharacter.Id);
            if (!canInviteParty)
                canInviteParty = autoCreatePartyIfNotCreated;
            foreach (GameObject obj in partyInviteObjects)
            {
                if (obj != null)
                    obj.SetActive(canInviteParty);
            }
            bool canInviteGuild = Data.GuildId <= 0 && GameInstance.JoinedGuild != null && GameInstance.JoinedGuild.CanInvite(GameInstance.PlayingCharacter.Id);
            foreach (GameObject obj in guildInviteObjects)
            {
                if (obj != null)
                    obj.SetActive(canInviteGuild);
            }
            if (Data is BasePlayerCharacterEntity entity)
            {
                foreach (GameObject obj in dealObjects)
                {
                    if (obj != null)
                        obj.SetActive(entity.DealingComponent != null);
                }
                foreach (GameObject obj in duelObjects)
                {
                    if (obj != null)
                        obj.SetActive(entity.DuelingComponent != null);
                }
                foreach (GameObject obj in vendingObjects)
                {
                    if (obj != null)
                        obj.SetActive(entity.IsVendingStarted);
                }
            }
            else
            {
                foreach (GameObject obj in dealObjects)
                {
                    if (obj != null)
                        obj.SetActive(false);
                }
                foreach (GameObject obj in duelObjects)
                {
                    if (obj != null)
                        obj.SetActive(false);
                }
                foreach (GameObject obj in vendingObjects)
                {
                    if (obj != null)
                        obj.SetActive(false);
                }
            }
        }

        protected override void UpdateData()
        {
            if (uiAnotherCharacter != null)
            {
                uiAnotherCharacter.NotForOwningCharacter = true;
                uiAnotherCharacter.Data = Data;
            }
        }

        public void OnClickSendDealingRequest()
        {
            if (Data is BasePlayerCharacterEntity entity)
                GameInstance.PlayingCharacterEntity.DealingComponent.CallCmdSendDealingRequest(entity.ObjectId);
            Hide();
        }

        public void OnClickSendDuelingRequest()
        {
            if (Data is BasePlayerCharacterEntity entity)
                GameInstance.PlayingCharacterEntity.DuelingComponent.CallCmdSendDuelingRequest(entity.ObjectId);
            Hide();
        }

        public void OnClickSendPartyInvitation()
        {
            if (GameInstance.JoinedParty == null)
            {
                if (!autoCreatePartyIfNotCreated)
                    return;
                // Create a party before proceed invitation
                ProceedPartyCreateBeforeInvite();
                return;
            }
            ProceedPartyInvitation();
        }

        public void ProceedPartyCreateBeforeInvite()
        {
            GameInstance.ClientPartyHandlers.RequestCreateParty(new RequestCreatePartyMessage()
            {
                shareExp = true,
                shareItem = true,
            }, SendPartyCreateCallback);
        }

        public void SendPartyCreateCallback(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseCreatePartyMessage response)
        {
            ClientPartyActions.ResponseCreateParty(requestHandler, responseCode, response);
            if (responseCode.ShowUnhandledResponseMessageDialog(response.message))
                return;
            ProceedPartyInvitation();
        }

        public void ProceedPartyInvitation()
        {
            GameInstance.ClientPartyHandlers.RequestSendPartyInvitation(new RequestSendPartyInvitationMessage()
            {
                inviteeId = Data.Id,
            }, SendPartyInvitationCallback);
        }

        public void SendPartyInvitationCallback(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseSendPartyInvitationMessage response)
        {
            ClientPartyActions.ResponseSendPartyInvitation(requestHandler, responseCode, response);
            if (responseCode.ShowUnhandledResponseMessageDialog(response.message))
                return;
            Hide();
        }

        public void OnClickSendGuildInvitation()
        {
            GameInstance.ClientGuildHandlers.RequestSendGuildInvitation(new RequestSendGuildInvitationMessage()
            {
                inviteeId = Data.Id,
            }, SendGuildInvitationCallback);
        }

        public void SendGuildInvitationCallback(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseSendGuildInvitationMessage response)
        {
            ClientGuildActions.ResponseSendGuildInvitation(requestHandler, responseCode, response);
            if (responseCode.ShowUnhandledResponseMessageDialog(response.message))
                return;
            Hide();
        }

        public void OnClickVending()
        {
            if (Data is BasePlayerCharacterEntity entity)
                BaseUISceneGameplay.Singleton.ShowVending(entity);
        }
    }
}
