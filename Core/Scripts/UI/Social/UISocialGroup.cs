using System.Collections.Generic;
using Cysharp.Text;
using LiteNetLibManager;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace MultiplayerARPG
{
    public abstract partial class UISocialGroup : UIBase
    {
        public abstract int GetSocialId();
        public abstract int GetMaxMemberAmount();
        public abstract bool IsLeader(string characterId);
        public abstract bool CanKick(string characterId);
        public abstract bool OwningCharacterIsLeader();
        public abstract bool OwningCharacterCanKick();
        public abstract void UpdateOnlineMember(string id, bool isOnline);
    }

    public abstract partial class UISocialGroup<T> : UISocialGroup
        where T : UISocialCharacter
    {
        [Header("String Formats")]
        [Tooltip("Format => {0} = {Social ID}")]
        public UILocaleKeySetting formatKeySocialId = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SIMPLE);
        [Tooltip("Format => {0} = {Current Amount}, {1} = {Max Amount}")]
        public UILocaleKeySetting formatKeyMemberAmount = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SOCIAL_MEMBER_AMOUNT);
        [Tooltip("Format => {0} = {Current Amount}")]
        public UILocaleKeySetting formatKeyMemberAmountNoLimit = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SOCIAL_MEMBER_AMOUNT_NO_LIMIT);
        [Tooltip("Format => {0} = {Current Amount}, {1} = {Max Amount}")]
        public UILocaleKeySetting formatKeyOnlineMemberAmount = new UILocaleKeySetting(UIFormatKeys.UI_FORMAT_SOCIAL_MEMBER_AMOUNT);

        [Header("UI Elements - Group Information")]
        public TextWrapper textSocialId;
        public TextWrapper textMemberAmount;
        public TextWrapper textOnlineMemberAmount;
        [Header("UI Elements - Group Members")]
        [FormerlySerializedAs("listEmptyObject")]
        public GameObject memberListEmptyObject;
        public T uiMemberDialog;
        public T uiMemberPrefab;
        public Transform uiMemberContainer;
        [Header("UI Elements - Signs")]
        [Tooltip("These objects will be activated when owning character is in social group")]
        public GameObject[] owningCharacterIsInGroupObjects;
        [Tooltip("These objects will be activated when owning character is not in social group")]
        public GameObject[] owningCharacterIsNotInGroupObjects;
        [Tooltip("These objects will be activated when owning character is leader")]
        public GameObject[] owningCharacterIsLeaderObjects;
        [Tooltip("These objects will be activated when owning character is not leader")]
        public GameObject[] owningCharacterIsNotLeaderObjects;
        [Tooltip("These objects will be activated when owning character can kick")]
        public GameObject[] owningCharacterCanKickObjects;
        [Tooltip("These objects will be activated when owning character cannot kick")]
        public GameObject[] owningCharacterCannotKickObjects;
        [Tooltip("These objects will be activated when selected character is leader")]
        public GameObject[] selectedCharacterIsLeaderObjects;
        [Tooltip("These objects will be activated when selected character is not leader")]
        public GameObject[] selectedCharacterIsNotLeaderObjects;

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

        protected int currentSocialId = 0;
        public int memberAmount { get; protected set; }
        public HashSet<string> onlineMembers { get; protected set; } = new HashSet<string>();

        private UIList _memberList;
        public UIList MemberList
        {
            get
            {
                if (_memberList == null)
                {
                    _memberList = gameObject.AddComponent<UIList>();
                    _memberList.uiPrefab = uiMemberPrefab.gameObject;
                    _memberList.uiContainer = uiMemberContainer;
                }
                return _memberList;
            }
        }

        private UISocialCharacterSelectionManager _memberSelectionManager;
        public UISocialCharacterSelectionManager MemberSelectionManager
        {
            get
            {
                if (_memberSelectionManager == null)
                    _memberSelectionManager = gameObject.GetOrAddComponent<UISocialCharacterSelectionManager>();
                _memberSelectionManager.selectionMode = UISelectionMode.SelectSingle;
                return _memberSelectionManager;
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            memberListEmptyObject = null;
            uiMemberDialog = null;
            uiMemberPrefab = null;
            uiMemberContainer = null;
            textSocialId = null;
            textMemberAmount = null;
            textOnlineMemberAmount = null;
            owningCharacterIsInGroupObjects.Nullify();
            owningCharacterIsNotInGroupObjects.Nullify();
            owningCharacterIsLeaderObjects.Nullify();
            owningCharacterIsNotLeaderObjects.Nullify();
            owningCharacterCanKickObjects.Nullify();
            owningCharacterCannotKickObjects.Nullify();
            selectedCharacterIsLeaderObjects.Nullify();
            selectedCharacterIsNotLeaderObjects.Nullify();
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
            onlineMembers?.Clear();
            _memberList = null;
            _memberSelectionManager = null;
        }

        protected virtual void Update()
        {
            if (currentSocialId != GetSocialId())
            {
                currentSocialId = GetSocialId();
                UpdateUIs();

                // Refresh guild info
                if (currentSocialId <= 0 && uiMemberPrefab != null)
                    MemberList.HideAll();
            }

            if (currentSocialId > 0)
            {
                if (textSocialId != null)
                {
                    textSocialId.text = ZString.Format(
                            LanguageManager.GetText(formatKeySocialId),
                            GetSocialId().ToString("N0"));
                }

                if (textMemberAmount != null)
                {
                    if (GetMaxMemberAmount() > 0)
                    {
                        textMemberAmount.text = ZString.Format(
                            LanguageManager.GetText(formatKeyMemberAmount),
                            memberAmount.ToString("N0"),
                            GetMaxMemberAmount().ToString("N0"));
                    }
                    else
                    {
                        textMemberAmount.text = ZString.Format(
                            LanguageManager.GetText(formatKeyMemberAmountNoLimit),
                            memberAmount.ToString("N0"));
                    }
                }

                if (textOnlineMemberAmount != null)
                {
                    textOnlineMemberAmount.text = ZString.Format(
                        LanguageManager.GetText(formatKeyOnlineMemberAmount),
                        onlineMembers.Count.ToString("N0"),
                        memberAmount.ToString("N0"));
                }
            }
        }

        protected virtual void UpdateUIs()
        {
            UpdateOwningCharacterSigns();
            UpdateSelectedMemberSigns(MemberSelectionManager.SelectedUI);
        }

        public void UpdateOwningCharacterSigns()
        {
            foreach (GameObject obj in owningCharacterIsInGroupObjects)
            {
                if (obj != null)
                    obj.SetActive(currentSocialId > 0);
            }

            foreach (GameObject obj in owningCharacterIsNotInGroupObjects)
            {
                if (obj != null)
                    obj.SetActive(currentSocialId <= 0);
            }

            foreach (GameObject obj in owningCharacterIsLeaderObjects)
            {
                if (obj != null)
                    obj.SetActive(OwningCharacterIsLeader());
            }

            foreach (GameObject obj in owningCharacterIsNotLeaderObjects)
            {
                if (obj != null)
                    obj.SetActive(!OwningCharacterIsLeader());
            }

            foreach (GameObject obj in owningCharacterCanKickObjects)
            {
                if (obj != null)
                    obj.SetActive(OwningCharacterCanKick());
            }

            foreach (GameObject obj in owningCharacterCannotKickObjects)
            {
                if (obj != null)
                    obj.SetActive(!OwningCharacterCanKick());
            }
        }

        public void UpdateSelectedMemberSigns(UISocialCharacter ui)
        {
            foreach (GameObject obj in selectedCharacterIsLeaderObjects)
            {
                if (obj != null)
                    obj.SetActive(ui != null && IsLeader(ui.Data.id));
            }

            foreach (GameObject obj in selectedCharacterIsNotLeaderObjects)
            {
                if (obj != null)
                    obj.SetActive(ui != null && !IsLeader(ui.Data.id));
            }
        }

        public override void UpdateOnlineMember(string id, bool isOnline)
        {
            if (isOnline)
                onlineMembers.Add(id);
            else
                onlineMembers.Remove(id);
        }

        protected virtual void OnEnable()
        {
            MemberSelectionManager.eventOnSelect.RemoveListener(OnSelectMember);
            MemberSelectionManager.eventOnSelect.AddListener(OnSelectMember);
            MemberSelectionManager.eventOnDeselect.RemoveListener(OnDeselectMember);
            MemberSelectionManager.eventOnDeselect.AddListener(OnDeselectMember);
            if (uiMemberDialog != null)
                uiMemberDialog.onHide.AddListener(OnMemberDialogHide);
            UpdateUIs();
        }

        protected virtual void OnDisable()
        {
            if (uiMemberDialog != null)
                uiMemberDialog.onHide.RemoveListener(OnMemberDialogHide);
            MemberSelectionManager.DeselectSelectedUI();
        }

        protected void OnMemberDialogHide()
        {
            MemberSelectionManager.DeselectSelectedUI();
        }

        protected void OnSelectMember(UISocialCharacter ui)
        {
            if (uiMemberDialog != null)
            {
                uiMemberDialog.selectionManager = MemberSelectionManager;
                ui.CloneTo(uiMemberDialog);
                uiMemberDialog.Show();
            }
            UpdateSelectedMemberSigns(ui);
        }

        protected void OnDeselectMember(UISocialCharacter ui)
        {
            if (uiMemberDialog != null)
            {
                uiMemberDialog.onHide.RemoveListener(OnMemberDialogHide);
                uiMemberDialog.Hide();
                uiMemberDialog.onHide.AddListener(OnMemberDialogHide);
            }
            UpdateSelectedMemberSigns(null);
        }

        public void OnClickAddFriend()
        {
            if (MemberSelectionManager.SelectedUI == null)
                return;

            SocialCharacterData friend = MemberSelectionManager.SelectedUI.Data;
            UISceneGlobal.Singleton.ShowMessageDialog(LanguageManager.GetText(UITextKeys.UI_FRIEND_ADD.ToString()), ZString.Format(LanguageManager.GetText(UITextKeys.UI_FRIEND_ADD_DESCRIPTION.ToString()), friend.characterName), false, true, true, false, null, () =>
            {
                GameInstance.ClientFriendHandlers.RequestAddFriend(new RequestAddFriendMessage()
                {
                    friendId = friend.id,
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
            if (MemberSelectionManager.SelectedUI == null)
                return;

            SocialCharacterData friend = MemberSelectionManager.SelectedUI.Data;
            UISceneGlobal.Singleton.ShowMessageDialog(LanguageManager.GetText(UITextKeys.UI_FRIEND_REMOVE.ToString()), ZString.Format(LanguageManager.GetText(UITextKeys.UI_FRIEND_REMOVE_DESCRIPTION.ToString()), friend.characterName), false, true, true, false, null, () =>
            {
                GameInstance.ClientFriendHandlers.RequestRemoveFriend(new RequestRemoveFriendMessage()
                {
                    friendId = friend.id,
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
            if (MemberSelectionManager.SelectedUI == null)
                return;

            SocialCharacterData friend = MemberSelectionManager.SelectedUI.Data;
            UISceneGlobal.Singleton.ShowMessageDialog(LanguageManager.GetText(UITextKeys.UI_FRIEND_REQUEST.ToString()), ZString.Format(LanguageManager.GetText(UITextKeys.UI_FRIEND_REQUEST_DESCRIPTION.ToString()), friend.characterName), false, true, true, false, null, () =>
            {
                GameInstance.ClientFriendHandlers.RequestSendFriendRequest(new RequestSendFriendRequestMessage()
                {
                    requesteeId = friend.id,
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
            if (MemberSelectionManager.SelectedUI == null)
                return;

            SocialCharacterData friend = MemberSelectionManager.SelectedUI.Data;
            GameInstance.ClientFriendHandlers.RequestAcceptFriendRequest(new RequestAcceptFriendRequestMessage()
            {
                requesterId = friend.id,
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
            if (MemberSelectionManager.SelectedUI == null)
                return;

            SocialCharacterData friend = MemberSelectionManager.SelectedUI.Data;
            GameInstance.ClientFriendHandlers.RequestDeclineFriendRequest(new RequestDeclineFriendRequestMessage()
            {
                requesterId = friend.id,
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
            if (MemberSelectionManager.SelectedUI == null)
                return;

            SocialCharacterData character = MemberSelectionManager.SelectedUI.Data;
            GameInstance.ClientGuildHandlers.RequestAcceptGuildRequest(new RequestAcceptGuildRequestMessage()
            {
                requesterId = character.id,
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
            if (MemberSelectionManager.SelectedUI == null)
                return;

            SocialCharacterData character = MemberSelectionManager.SelectedUI.Data;
            GameInstance.ClientGuildHandlers.RequestDeclineGuildRequest(new RequestDeclineGuildRequestMessage()
            {
                requesterId = character.id,
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
            if (MemberSelectionManager.SelectedUI == null)
                return;

            SocialCharacterData character = MemberSelectionManager.SelectedUI.Data;
            GameInstance.ClientPartyHandlers.RequestSendPartyInvitation(new RequestSendPartyInvitationMessage()
            {
                inviteeId = character.id,
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
            if (MemberSelectionManager.SelectedUI == null)
                return;

            SocialCharacterData character = MemberSelectionManager.SelectedUI.Data;
            GameInstance.ClientGuildHandlers.RequestSendGuildInvitation(new RequestSendGuildInvitationMessage()
            {
                inviteeId = character.id,
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
            if (MemberSelectionManager.SelectedUI == null)
                return;

            SocialCharacterData partyMember = MemberSelectionManager.SelectedUI.Data;
            UISceneGlobal.Singleton.ShowMessageDialog(LanguageManager.GetText(UITextKeys.UI_PARTY_KICK_MEMBER.ToString()), ZString.Format(LanguageManager.GetText(UITextKeys.UI_PARTY_KICK_MEMBER_DESCRIPTION.ToString()), partyMember.characterName), false, true, true, false, null, () =>
            {
                GameInstance.ClientPartyHandlers.RequestKickMemberFromParty(new RequestKickMemberFromPartyMessage()
                {
                    memberId = partyMember.id,
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
            if (MemberSelectionManager.SelectedUI == null)
                return;

            SocialCharacterData guildMember = MemberSelectionManager.SelectedUI.Data;
            UISceneGlobal.Singleton.ShowMessageDialog(LanguageManager.GetText(UITextKeys.UI_GUILD_KICK_MEMBER.ToString()), ZString.Format(LanguageManager.GetText(UITextKeys.UI_GUILD_KICK_MEMBER_DESCRIPTION.ToString()), guildMember.characterName), false, true, true, false, null, () =>
            {
                GameInstance.ClientGuildHandlers.RequestKickMemberFromGuild(new RequestKickMemberFromGuildMessage()
                {
                    memberId = guildMember.id,
                }, KickMemberFromGuildCallback);
            });
        }

        private void KickMemberFromGuildCallback(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseKickMemberFromGuildMessage response)
        {
            ClientGuildActions.ResponseKickMemberFromGuild(requestHandler, responseCode, response);
            if (responseCode.ShowUnhandledResponseMessageDialog(response.message)) return;
            onGuildMemberKicked.Invoke();
        }
    }
}
