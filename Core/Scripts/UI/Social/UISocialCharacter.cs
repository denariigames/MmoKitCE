using Cysharp.Text;
using LiteNetLibManager;
using UnityEngine;
using UnityEngine.Events;

namespace MultiplayerARPG
{
    public partial class UISocialCharacter : UISelectionEntry<SocialCharacterData>
    {
        [Header("String Formats")]
        [Tooltip("Format => {0} = {Character Name}")]
        public UILocaleKeySetting formatKeyName = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE);
        [Tooltip("Format => {0} = {Level}")]
        public UILocaleKeySetting formatKeyLevel = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_LEVEL);
        [Tooltip("Format => {0} = {Index}")]
        public UILocaleKeySetting formatKeyIndex = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE);

        [Header("UI Elements")]
        public UISocialGroup uiSocialGroup;
        public TextWrapper uiTextName;
        public TextWrapper uiTextLevel;
        public TextWrapper uiTextIndex;
        public UIGageValue uiGageHp;
        public UIGageValue uiGageMp;
        public UIPlayerIcon uiPlayerIcon;
        public UIPlayerFrame uiPlayerFrame;
        public UIPlayerBackground uiPlayerBackground;
        public UIPlayerTitle uiPlayerTitle;
        public TextWrapper uiTextOnlineStatus;
        public UICharacterBuffs uiCharacterBuffs;
        [Header("Member states objects")]
        [Tooltip("These objects will be activated when social member -> isOnline is true")]
        public GameObject[] memberIsOnlineObjects;
        [Tooltip("These objects will be activated when social member -> isOnline is false")]
        public GameObject[] memberIsNotOnlineObjects;
        [Tooltip("These objects will be activated when this social member is leader")]
        public GameObject[] memberIsLeaderObjects;
        [Tooltip("These objects will be activated when this social member is not leader")]
        public GameObject[] memberIsNotLeaderObjects;
        [Tooltip("These objects will be activated when owning character is leader")]
        public GameObject[] owningCharacterIsLeaderObjects;
        [Tooltip("These objects will be activated when owning character is not leader")]
        public GameObject[] owningCharacterIsNotLeaderObjects;
        [Tooltip("These objects will be activated when owning character can kick")]
        public GameObject[] owningCharacterCanKickObjects;
        [Tooltip("These objects will be activated when owning character cannot kick")]
        public GameObject[] owningCharacterCannotKickObjects;
        public UICharacterClass uiCharacterClass;
        public UIPlayerActivateMenu uiPlayerActivateMenu;
        public int index = 0;

        [Header("Events")]
        public UnityEvent onFriendAdded = new UnityEvent();
        public UnityEvent onFriendRemoved = new UnityEvent();
        public UnityEvent onFriendRequested = new UnityEvent();
        public UnityEvent onFriendRequestAccepted = new UnityEvent();
        public UnityEvent onFriendRequestDeclined = new UnityEvent();
        public UnityEvent onGuildRequestAccepted = new UnityEvent();
        public UnityEvent onGuildRequestDeclined = new UnityEvent();
        public UnityEvent onPartyInvitationSent = new UnityEvent();
        public UnityEvent onGuildInvitationSent = new UnityEvent();
        public UnityEvent onPartyMemberKicked = new UnityEvent();
        public UnityEvent onGuildMemberKicked = new UnityEvent();

        private IPlayerCharacterData _characterEntity;

        protected override void OnDestroy()
        {
            base.OnDestroy();
            uiSocialGroup = null;
            uiTextName = null;
            uiTextLevel = null;
            uiTextIndex = null;
            uiGageHp = null;
            uiGageMp = null;
            uiPlayerIcon = null;
            uiPlayerFrame = null;
            uiPlayerBackground = null;
            uiPlayerTitle = null;
            uiTextOnlineStatus = null;
            uiCharacterBuffs = null;
            memberIsOnlineObjects.Nullify();
            memberIsNotOnlineObjects.Nullify();
            memberIsLeaderObjects.Nullify();
            memberIsNotLeaderObjects.Nullify();
            owningCharacterIsLeaderObjects.Nullify();
            owningCharacterIsNotLeaderObjects.Nullify();
            owningCharacterCanKickObjects.Nullify();
            owningCharacterCannotKickObjects.Nullify();
            uiCharacterClass = null;
            onFriendAdded?.RemoveAllListeners();
            onFriendAdded = null;
            onFriendRemoved?.RemoveAllListeners();
            onFriendRemoved = null;
            onFriendRequested?.RemoveAllListeners();
            onFriendRequested = null;
            onFriendRequestAccepted?.RemoveAllListeners();
            onFriendRequestAccepted = null;
            onFriendRequestDeclined?.RemoveAllListeners();
            onFriendRequestDeclined = null;
            onGuildRequestAccepted?.RemoveAllListeners();
            onGuildRequestAccepted = null;
            onGuildRequestDeclined?.RemoveAllListeners();
            onGuildRequestDeclined = null;
            onPartyInvitationSent?.RemoveAllListeners();
            onPartyInvitationSent = null;
            onGuildInvitationSent?.RemoveAllListeners();
            onGuildInvitationSent = null;
            onPartyMemberKicked?.RemoveAllListeners();
            onPartyMemberKicked = null;
            onGuildMemberKicked?.RemoveAllListeners();
            onGuildMemberKicked = null;
            _characterEntity = null;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            if (uiSocialGroup != null)
                uiSocialGroup.UpdateOnlineMember(Data.id, false);
        }

        protected override void UpdateUI()
        {
            base.UpdateUI();

            bool isOnline = GameInstance.ClientOnlineCharacterHandlers.IsCharacterOnline(Data.id);
            if (uiSocialGroup != null)
                uiSocialGroup.UpdateOnlineMember(Data.id, isOnline);
            int offlineOffsets = GameInstance.ClientOnlineCharacterHandlers.GetCharacterOfflineOffsets(Data.id);

            if (uiTextOnlineStatus != null)
                uiTextOnlineStatus.text = GetOnlineStatusText(isOnline, offlineOffsets);

            // Member online status
            foreach (GameObject obj in memberIsOnlineObjects)
            {
                if (obj != null)
                    obj.SetActive(isOnline);
            }

            foreach (GameObject obj in memberIsNotOnlineObjects)
            {
                if (obj != null)
                    obj.SetActive(!isOnline);
            }

            // Hp
            int currentValue = isOnline ? Data.currentHp : 0;
            int maxValue = isOnline ? Data.maxHp : 0;
            if (uiGageHp != null)
            {
                uiGageHp.Update(currentValue, maxValue);
                if (uiGageHp.textValue != null)
                    uiGageHp.textValue.SetGameObjectActive(maxValue > 0);
            }

            // Mp
            currentValue = isOnline ? Data.currentMp : 0;
            maxValue = isOnline ? Data.maxMp : 0;
            if (uiGageMp != null)
            {
                uiGageMp.Update(currentValue, maxValue);
                if (uiGageMp.textValue != null)
                    uiGageMp.textValue.SetGameObjectActive(maxValue > 0);
            }

            // Icon
            if (uiPlayerIcon != null)
                uiPlayerIcon.SetDataByDataId(Data.iconDataId);

            // Frame
            if (uiPlayerFrame != null)
                uiPlayerFrame.SetDataByDataId(Data.frameDataId);

            // Background
            if (uiPlayerBackground != null)
                uiPlayerBackground.SetDataByDataId(Data.backgroundDataId);

            // Title
            if (uiPlayerTitle != null)
                uiPlayerTitle.SetDataByDataId(Data.titleDataId);

            if (!string.IsNullOrEmpty(Data.id) && (_characterEntity == null || !string.Equals(_characterEntity.Id, Data.id)))
            {
                // Try looking for the character entity
                GameInstance.ClientCharacterHandlers.TryGetSubscribedPlayerCharacterById(Data.id, out _characterEntity);
            }

            // Buffs
            if (uiCharacterBuffs != null)
            {
                if (_characterEntity != null)
                {
                    uiCharacterBuffs.UpdateData(_characterEntity);
                    uiCharacterBuffs.Show();
                }
                else
                {
                    uiCharacterBuffs.Hide();
                }
            }

            GameInstance.ClientOnlineCharacterHandlers.RequestOnlineCharacter(Data.id);
        }

        protected override void UpdateData()
        {
            if (uiTextName != null)
            {
                uiTextName.text = ZString.Format(
                    LanguageManager.GetText(formatKeyName),
                    string.IsNullOrEmpty(Data.characterName) ? LanguageManager.GetUnknowTitle() : Data.characterName);
            }

            if (uiTextLevel != null)
            {
                uiTextLevel.text = ZString.Format(
                    LanguageManager.GetText(formatKeyLevel),
                    Data.level.ToString("N0"));
            }

            if (uiTextIndex != null)
            {
                uiTextIndex.text = ZString.Format(
                    LanguageManager.GetText(formatKeyIndex),
                    (index + 1).ToString("N0"));
            }

            foreach (GameObject obj in memberIsLeaderObjects)
            {
                if (obj != null)
                    obj.SetActive(!string.IsNullOrEmpty(Data.id) && uiSocialGroup.IsLeader(Data.id));
            }

            foreach (GameObject obj in memberIsNotLeaderObjects)
            {
                if (obj != null)
                    obj.SetActive(string.IsNullOrEmpty(Data.id) || !uiSocialGroup.IsLeader(Data.id));
            }

            foreach (GameObject obj in owningCharacterIsLeaderObjects)
            {
                if (obj != null)
                    obj.SetActive(uiSocialGroup.OwningCharacterIsLeader());
            }

            foreach (GameObject obj in owningCharacterIsNotLeaderObjects)
            {
                if (obj != null)
                    obj.SetActive(!uiSocialGroup.OwningCharacterIsLeader());
            }

            foreach (GameObject obj in owningCharacterCanKickObjects)
            {
                if (obj != null)
                    obj.SetActive(uiSocialGroup.OwningCharacterCanKick());
            }

            foreach (GameObject obj in owningCharacterCannotKickObjects)
            {
                if (obj != null)
                    obj.SetActive(!uiSocialGroup.OwningCharacterCanKick());
            }

            // Character class data
            PlayerCharacter character;
            GameInstance.PlayerCharacters.TryGetValue(Data.dataId, out character);
            if (uiCharacterClass != null)
                uiCharacterClass.Data = character;

            if (uiPlayerActivateMenu != null)
            {
                uiPlayerActivateMenu.Data = new PlayerCharacterData()
                {
                    Id = Data.id,
                    UserId = Data.userId,
                    CharacterName = Data.characterName,
                    DataId = Data.dataId,
                    Level = Data.level,
                    FactionId = Data.factionId,
                    PartyId = Data.partyId,
                    GuildId = Data.guildId,
                    GuildRole = Data.guildRole,
                    IconDataId = Data.iconDataId,
                    FrameDataId = Data.frameDataId,
                    TitleDataId = Data.titleDataId,
                };
            }
        }

        public virtual void OnClickShowPlayerActivateMenu()
        {
            if (uiPlayerActivateMenu == null)
                return;
            if (string.Equals(Data.id, GameInstance.PlayingCharacter.Id))
                return;
            uiPlayerActivateMenu.Show();
        }

        protected virtual string GetOnlineStatusText(bool isOnline, int offlineOffsets)
        {
            if (isOnline)
                return "Online";
            return (System.DateTime.Now - System.DateTime.Now.AddSeconds(-offlineOffsets)).GetPrettyDate();
        }

        public void OnClickAddFriend()
        {
            UISceneGlobal.Singleton.ShowMessageDialog(LanguageManager.GetText(UITextKeys.UI_FRIEND_ADD.ToString()), ZString.Format(LanguageManager.GetText(UITextKeys.UI_FRIEND_ADD_DESCRIPTION.ToString()), Data.characterName), false, true, true, false, null, () =>
            {
                GameInstance.ClientFriendHandlers.RequestAddFriend(new RequestAddFriendMessage()
                {
                    friendId = Data.id,
                }, AddFriendCallback);
            });
        }

        public void AddFriendCallback(ResponseHandlerData responseHandler, AckResponseCode responseCode, ResponseAddFriendMessage response)
        {
            ClientFriendActions.ResponseAddFriend(responseHandler, responseCode, response);
            if (responseCode.ShowUnhandledResponseMessageDialog(response.message)) return;
            onFriendAdded.Invoke();
        }

        public void OnClickRemoveFriend()
        {
            UISceneGlobal.Singleton.ShowMessageDialog(LanguageManager.GetText(UITextKeys.UI_FRIEND_REMOVE.ToString()), ZString.Format(LanguageManager.GetText(UITextKeys.UI_FRIEND_REMOVE_DESCRIPTION.ToString()), Data.characterName), false, true, true, false, null, () =>
            {
                GameInstance.ClientFriendHandlers.RequestRemoveFriend(new RequestRemoveFriendMessage()
                {
                    friendId = Data.id,
                }, RemoveFriendCallback);
            });
        }

        private void RemoveFriendCallback(ResponseHandlerData responseHandler, AckResponseCode responseCode, ResponseRemoveFriendMessage response)
        {
            ClientFriendActions.ResponseRemoveFriend(responseHandler, responseCode, response);
            if (responseCode.ShowUnhandledResponseMessageDialog(response.message)) return;
            onFriendRemoved.Invoke();
        }

        public void OnClickSendFriendRequest()
        {
            UISceneGlobal.Singleton.ShowMessageDialog(LanguageManager.GetText(UITextKeys.UI_FRIEND_REMOVE.ToString()), ZString.Format(LanguageManager.GetText(UITextKeys.UI_FRIEND_REMOVE_DESCRIPTION.ToString()), Data.characterName), false, true, true, false, null, () =>
            {
                GameInstance.ClientFriendHandlers.RequestSendFriendRequest(new RequestSendFriendRequestMessage()
                {
                    requesteeId = Data.id,
                }, SendFriendRequestCallback);
            });
        }

        private void SendFriendRequestCallback(ResponseHandlerData responseHandler, AckResponseCode responseCode, ResponseSendFriendRequestMessage response)
        {
            ClientFriendActions.ResponseSendFriendRequest(responseHandler, responseCode, response);
            if (responseCode.ShowUnhandledResponseMessageDialog(response.message)) return;
            onFriendRequested.Invoke();
        }

        public void OnClickAcceptFriendRequest()
        {
            GameInstance.ClientFriendHandlers.RequestAcceptFriendRequest(new RequestAcceptFriendRequestMessage()
            {
                requesterId = Data.id,
            }, AcceptFriendRequestCallback);
        }

        private void AcceptFriendRequestCallback(ResponseHandlerData responseHandler, AckResponseCode responseCode, ResponseAcceptFriendRequestMessage response)
        {
            ClientFriendActions.ResponseAcceptFriendRequest(responseHandler, responseCode, response);
            if (responseCode.ShowUnhandledResponseMessageDialog(response.message)) return;
            onFriendRequestAccepted.Invoke();
        }

        public void OnClickDeclineFriendRequest()
        {
            GameInstance.ClientFriendHandlers.RequestDeclineFriendRequest(new RequestDeclineFriendRequestMessage()
            {
                requesterId = Data.id,
            }, DeclineFriendRequestCallback);
        }

        private void DeclineFriendRequestCallback(ResponseHandlerData responseHandler, AckResponseCode responseCode, ResponseDeclineFriendRequestMessage response)
        {
            ClientFriendActions.ResponseDeclineFriendRequest(responseHandler, responseCode, response);
            if (responseCode.ShowUnhandledResponseMessageDialog(response.message)) return;
            onFriendRequestDeclined.Invoke();
        }

        public void OnClickAcceptGuildRequest()
        {
            GameInstance.ClientGuildHandlers.RequestAcceptGuildRequest(new RequestAcceptGuildRequestMessage()
            {
                requesterId = Data.id,
            }, AcceptGuildRequestCallback);
        }

        private void AcceptGuildRequestCallback(ResponseHandlerData responseHandler, AckResponseCode responseCode, ResponseAcceptGuildRequestMessage response)
        {
            ClientGuildActions.ResponseAcceptGuildRequest(responseHandler, responseCode, response);
            if (responseCode.ShowUnhandledResponseMessageDialog(response.message)) return;
            onGuildRequestAccepted.Invoke();
        }

        public void OnClickDeclineGuildRequest()
        {
            GameInstance.ClientGuildHandlers.RequestDeclineGuildRequest(new RequestDeclineGuildRequestMessage()
            {
                requesterId = Data.id,
            }, DeclineGuildRequestCallback);
        }

        private void DeclineGuildRequestCallback(ResponseHandlerData responseHandler, AckResponseCode responseCode, ResponseDeclineGuildRequestMessage response)
        {
            ClientGuildActions.ResponseDeclineGuildRequest(responseHandler, responseCode, response);
            if (responseCode.ShowUnhandledResponseMessageDialog(response.message)) return;
            onGuildRequestDeclined.Invoke();
        }

        public void OnClickSendPartyInvitation()
        {
            GameInstance.ClientPartyHandlers.RequestSendPartyInvitation(new RequestSendPartyInvitationMessage()
            {
                inviteeId = Data.id,
            }, SendPartyInvitationCallback);
        }

        private void SendPartyInvitationCallback(ResponseHandlerData responseHandler, AckResponseCode responseCode, ResponseSendPartyInvitationMessage response)
        {
            ClientPartyActions.ResponseSendPartyInvitation(responseHandler, responseCode, response);
            if (responseCode.ShowUnhandledResponseMessageDialog(response.message)) return;
            onPartyInvitationSent.Invoke();
        }

        public void OnClickSendGuildInvitation()
        {
            GameInstance.ClientGuildHandlers.RequestSendGuildInvitation(new RequestSendGuildInvitationMessage()
            {
                inviteeId = Data.id,
            }, SendGuildInvitationCallback);
        }

        private void SendGuildInvitationCallback(ResponseHandlerData responseHandler, AckResponseCode responseCode, ResponseSendGuildInvitationMessage response)
        {
            ClientGuildActions.ResponseSendGuildInvitation(responseHandler, responseCode, response);
            if (responseCode.ShowUnhandledResponseMessageDialog(response.message)) return;
            onGuildInvitationSent.Invoke();
        }

        public void OnClickKickFromParty()
        {
            UISceneGlobal.Singleton.ShowMessageDialog(LanguageManager.GetText(UITextKeys.UI_PARTY_KICK_MEMBER.ToString()), ZString.Format(LanguageManager.GetText(UITextKeys.UI_PARTY_KICK_MEMBER_DESCRIPTION.ToString()), Data.characterName), false, true, true, false, null, () =>
            {
                GameInstance.ClientPartyHandlers.RequestKickMemberFromParty(new RequestKickMemberFromPartyMessage()
                {
                    memberId = Data.id,
                }, KickMemberFromPartyCallback);
            });
        }

        private void KickMemberFromPartyCallback(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseKickMemberFromPartyMessage response)
        {
            ClientPartyActions.ResponseKickMemberFromParty(requestHandler, responseCode, response);
            if (responseCode.ShowUnhandledResponseMessageDialog(response.message)) return;
            onPartyMemberKicked.Invoke();
        }

        public void OnClickKickFromGuild()
        {
            UISceneGlobal.Singleton.ShowMessageDialog(LanguageManager.GetText(UITextKeys.UI_GUILD_KICK_MEMBER.ToString()), ZString.Format(LanguageManager.GetText(UITextKeys.UI_GUILD_KICK_MEMBER_DESCRIPTION.ToString()), Data.characterName), false, true, true, false, null, () =>
            {
                GameInstance.ClientGuildHandlers.RequestKickMemberFromGuild(new RequestKickMemberFromGuildMessage()
                {
                    memberId = Data.id,
                }, KickMemberFromGuildCallback);
            });
        }

        private void KickMemberFromGuildCallback(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseKickMemberFromGuildMessage response)
        {
            ClientGuildActions.ResponseKickMemberFromGuild(requestHandler, responseCode, response);
            if (responseCode.ShowUnhandledResponseMessageDialog(response.message)) return;
            onGuildMemberKicked.Invoke();
        }

        public void OnClickCopyCharacterId()
        {
            GUIUtility.systemCopyBuffer = Data.id;
            ClientGenericActions.ClientReceiveGameMessage(UITextKeys.UI_CHARACTER_ID_COPIED);
        }

        public void OnClickCopyUserId()
        {
            GUIUtility.systemCopyBuffer = Data.userId;
            ClientGenericActions.ClientReceiveGameMessage(UITextKeys.UI_USER_ID_COPIED);
        }
    }
}
