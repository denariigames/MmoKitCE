using LiteNetLibManager;

namespace MultiplayerARPG
{
    public partial class BaseGameNetworkManager
    {
        // Server Handlers
        protected IServerMailHandlers ServerMailHandlers { get; set; }
        protected IServerUserHandlers ServerUserHandlers { get; set; }
        protected IServerBuildingHandlers ServerBuildingHandlers { get; set; }
        protected IServerCharacterHandlers ServerCharacterHandlers { get; set; }
        protected IServerGameMessageHandlers ServerGameMessageHandlers { get; set; }
        protected IServerStorageHandlers ServerStorageHandlers { get; set; }
        protected IServerPartyHandlers ServerPartyHandlers { get; set; }
        protected IServerGuildHandlers ServerGuildHandlers { get; set; }
        protected IServerChatHandlers ServerChatHandlers { get; set; }
        protected IServerUserContentHandlers ServerUserContentHandlers { get; set; }
        protected IServerLogHandlers ServerLogHandlers { get; set; }
        // Server Message Handlers
        protected IServerCashShopMessageHandlers ServerCashShopMessageHandlers { get; set; }
        protected IServerMailMessageHandlers ServerMailMessageHandlers { get; set; }
        protected IServerStorageMessageHandlers ServerStorageMessageHandlers { get; set; }
        protected IServerCharacterMessageHandlers ServerCharacterMessageHandlers { get; set; }
        protected IServerInventoryMessageHandlers ServerInventoryMessageHandlers { get; set; }
        protected IServerPartyMessageHandlers ServerPartyMessageHandlers { get; set; }
        protected IServerGuildMessageHandlers ServerGuildMessageHandlers { get; set; }
        protected IServerGachaMessageHandlers ServerGachaMessageHandlers { get; set; }
        protected IServerFriendMessageHandlers ServerFriendMessageHandlers { get; set; }
        protected IServerBankMessageHandlers ServerBankMessageHandlers { get; set; }
        protected IServerUserContentMessageHandlers ServerUserContentMessageHandlers { get; set; }
        // Client handlers
        protected IClientCashShopHandlers ClientCashShopHandlers { get; set; }
        protected IClientMailHandlers ClientMailHandlers { get; set; }
        protected IClientStorageHandlers ClientStorageHandlers { get; set; }
        protected IClientCharacterHandlers ClientCharacterHandlers { get; set; }
        protected IClientInventoryHandlers ClientInventoryHandlers { get; set; }
        protected IClientPartyHandlers ClientPartyHandlers { get; set; }
        protected IClientGuildHandlers ClientGuildHandlers { get; set; }
        protected IClientGachaHandlers ClientGachaHandlers { get; set; }
        protected IClientFriendHandlers ClientFriendHandlers { get; set; }
        protected IClientBankHandlers ClientBankHandlers { get; set; }
        protected IClientUserContentHandlers ClientUserContentHandlers { get; set; }
        protected IClientOnlineCharacterHandlers ClientOnlineCharacterHandlers { get; set; }
        protected IClientChatHandlers ClientChatHandlers { get; set; }
        protected IClientGameMessageHandlers ClientGameMessageHandlers { get; set; }

        protected virtual void RegisterHandlerMessages()
        {
            // Client messages
            RegisterClientMessage(GameNetworkingConsts.Warp, HandleWarpAtClient);
            RegisterClientMessage(GameNetworkingConsts.Chat, HandleChatAtClient);
            RegisterClientMessage(GameNetworkingConsts.UpdateTimeOfDay, HandleUpdateDayNightTimeAtClient);
            RegisterClientMessage(GameNetworkingConsts.UpdateMapInfo, HandleUpdateMapInfoAtClient);
            RegisterClientMessage(GameNetworkingConsts.EntityState, HandleServerEntityStateAtClient);
            if (ClientOnlineCharacterHandlers != null)
            {
                RegisterClientMessage(GameNetworkingConsts.NotifyOnlineCharacter, ClientOnlineCharacterHandlers.HandleNotifyOnlineCharacter);
            }
            if (ClientGameMessageHandlers != null)
            {
                RegisterClientMessage(GameNetworkingConsts.GameMessage, ClientGameMessageHandlers.HandleGameMessage);
                RegisterClientMessage(GameNetworkingConsts.FormattedGameMessage, ClientGameMessageHandlers.HandleFormattedGameMessage);
                RegisterClientMessage(GameNetworkingConsts.UpdatePartyMember, ClientGameMessageHandlers.HandleUpdatePartyMember);
                RegisterClientMessage(GameNetworkingConsts.UpdateParty, ClientGameMessageHandlers.HandleUpdateParty);
                RegisterClientMessage(GameNetworkingConsts.UpdateGuildMember, ClientGameMessageHandlers.HandleUpdateGuildMember);
                RegisterClientMessage(GameNetworkingConsts.UpdateGuild, ClientGameMessageHandlers.HandleUpdateGuild);
                RegisterClientMessage(GameNetworkingConsts.NotifyRewardExp, ClientGameMessageHandlers.HandleNotifyRewardExp);
                RegisterClientMessage(GameNetworkingConsts.NotifyRewardGold, ClientGameMessageHandlers.HandleNotifyRewardGold);
                RegisterClientMessage(GameNetworkingConsts.NotifyRewardItem, ClientGameMessageHandlers.HandleNotifyRewardItem);
                RegisterClientMessage(GameNetworkingConsts.NotifyRewardCurrency, ClientGameMessageHandlers.HandleNotifyRewardCurrency);
                RegisterClientMessage(GameNetworkingConsts.NotifyStorageOpened, ClientGameMessageHandlers.HandleNotifyStorageOpened);
                RegisterClientMessage(GameNetworkingConsts.NotifyStorageClosed, ClientGameMessageHandlers.HandleNotifyStorageClosed);
                RegisterClientMessage(GameNetworkingConsts.NotifyStorageItemsUpdated, ClientGameMessageHandlers.HandleNotifyStorageItems);
                RegisterClientMessage(GameNetworkingConsts.NotifyPartyInvitation, ClientGameMessageHandlers.HandleNotifyPartyInvitation);
                RegisterClientMessage(GameNetworkingConsts.NotifyGuildInvitation, ClientGameMessageHandlers.HandleNotifyGuildInvitation);
            }
            // Server messages
            RegisterServerMessage(GameNetworkingConsts.Chat, HandleChatAtServer);
            RegisterServerMessage(GameNetworkingConsts.EntityState, HandleClientEntityStateAtServer);
            if (ServerCharacterHandlers != null)
            {
                RegisterServerMessage(GameNetworkingConsts.NotifyOnlineCharacter, ServerCharacterHandlers.HandleRequestOnlineCharacter);
            }
            // Request to server (response to client)
            // Cash shop
            if (ServerCashShopMessageHandlers != null)
            {
                RegisterRequestToServer<EmptyMessage, ResponseCashShopInfoMessage>(GameNetworkingConsts.CashShopInfo, ServerCashShopMessageHandlers.HandleRequestCashShopInfo);
                RegisterRequestToServer<EmptyMessage, ResponseCashPackageInfoMessage>(GameNetworkingConsts.CashPackageInfo, ServerCashShopMessageHandlers.HandleRequestCashPackageInfo);
                RegisterRequestToServer<RequestCashShopBuyMessage, ResponseCashShopBuyMessage>(GameNetworkingConsts.CashShopBuy, ServerCashShopMessageHandlers.HandleRequestCashShopBuy);
                RegisterRequestToServer<RequestCashPackageBuyValidationMessage, ResponseCashPackageBuyValidationMessage>(GameNetworkingConsts.CashPackageBuyValidation, ServerCashShopMessageHandlers.HandleRequestCashPackageBuyValidation);
            }
            // Mail
            if (ServerMailMessageHandlers != null)
            {
                RegisterRequestToServer<RequestMailListMessage, ResponseMailListMessage>(GameNetworkingConsts.MailList, ServerMailMessageHandlers.HandleRequestMailList);
                RegisterRequestToServer<RequestReadMailMessage, ResponseReadMailMessage>(GameNetworkingConsts.ReadMail, ServerMailMessageHandlers.HandleRequestReadMail);
                RegisterRequestToServer<RequestClaimMailItemsMessage, ResponseClaimMailItemsMessage>(GameNetworkingConsts.ClaimMailItems, ServerMailMessageHandlers.HandleRequestClaimMailItems);
                RegisterRequestToServer<RequestDeleteMailMessage, ResponseDeleteMailMessage>(GameNetworkingConsts.DeleteMail, ServerMailMessageHandlers.HandleRequestDeleteMail);
                RegisterRequestToServer<RequestSendMailMessage, ResponseSendMailMessage>(GameNetworkingConsts.SendMail, ServerMailMessageHandlers.HandleRequestSendMail);
                RegisterRequestToServer<EmptyMessage, ResponseMailNotificationMessage>(GameNetworkingConsts.MailNotification, ServerMailMessageHandlers.HandleRequestMailNotification);
                RegisterRequestToServer<EmptyMessage, ResponseClaimAllMailsItemsMessage>(GameNetworkingConsts.ClaimAllMailsItems, ServerMailMessageHandlers.HandleRequestClaimAllMailsItems);
                RegisterRequestToServer<EmptyMessage, ResponseDeleteAllMailsMessage>(GameNetworkingConsts.DeleteAllMails, ServerMailMessageHandlers.HandleRequestDeleteAllMails);
            }
            // Storage
            if (ServerStorageMessageHandlers != null)
            {
                RegisterRequestToServer<RequestOpenStorageMessage, ResponseOpenStorageMessage>(GameNetworkingConsts.OpenStorage, ServerStorageMessageHandlers.HandleRequestOpenStorage);
                RegisterRequestToServer<EmptyMessage, ResponseCloseStorageMessage>(GameNetworkingConsts.CloseStorage, ServerStorageMessageHandlers.HandleRequestCloseStorage);
                RegisterRequestToServer<RequestMoveItemFromStorageMessage, ResponseMoveItemFromStorageMessage>(GameNetworkingConsts.MoveItemFromStorage, ServerStorageMessageHandlers.HandleRequestMoveItemFromStorage);
                RegisterRequestToServer<RequestMoveItemToStorageMessage, ResponseMoveItemToStorageMessage>(GameNetworkingConsts.MoveItemToStorage, ServerStorageMessageHandlers.HandleRequestMoveItemToStorage);
                RegisterRequestToServer<RequestSwapOrMergeStorageItemMessage, ResponseSwapOrMergeStorageItemMessage>(GameNetworkingConsts.SwapOrMergeStorageItem, ServerStorageMessageHandlers.HandleRequestSwapOrMergeStorageItem);
            }
            // Character
            if (ServerCharacterMessageHandlers != null)
            {
                RegisterRequestToServer<RequestIncreaseAttributeAmountMessage, ResponseIncreaseAttributeAmountMessage>(GameNetworkingConsts.IncreaseAttributeAmount, ServerCharacterMessageHandlers.HandleRequestIncreaseAttributeAmount);
                RegisterRequestToServer<RequestIncreaseSkillLevelMessage, ResponseIncreaseSkillLevelMessage>(GameNetworkingConsts.IncreaseSkillLevel, ServerCharacterMessageHandlers.HandleRequestIncreaseSkillLevel);
                RegisterRequestToServer<RequestRespawnMessage, ResponseRespawnMessage>(GameNetworkingConsts.Respawn, ServerCharacterMessageHandlers.HandleRequestRespawn);
                RegisterRequestToServer<RequestSetIconMessage, ResponseSetIconMessage>(GameNetworkingConsts.SetIcon, ServerCharacterMessageHandlers.HandleRequestSetIcon);
                RegisterRequestToServer<RequestSetFrameMessage, ResponseSetFrameMessage>(GameNetworkingConsts.SetFrame, ServerCharacterMessageHandlers.HandleRequestSetFrame);
                RegisterRequestToServer<RequestSetBackgroundMessage, ResponseSetBackgroundMessage>(GameNetworkingConsts.SetBackground, ServerCharacterMessageHandlers.HandleRequestSetBackground);
                RegisterRequestToServer<RequestSetTitleMessage, ResponseSetTitleMessage>(GameNetworkingConsts.SetTitle, ServerCharacterMessageHandlers.HandleRequestSetTitle);
                RegisterRequestToServer<EmptyMessage, ResponsePlayerCharacterTransformMessage>(GameNetworkingConsts.PlayerCharacterTransform, ServerCharacterMessageHandlers.HandleRequestPlayerCharacterTransform);
            }
            // Inventory
            if (ServerInventoryMessageHandlers != null)
            {
                RegisterRequestToServer<RequestSwapOrMergeItemMessage, ResponseSwapOrMergeItemMessage>(GameNetworkingConsts.SwapOrMergeItem, ServerInventoryMessageHandlers.HandleRequestSwapOrMergeItem);
                RegisterRequestToServer<RequestEquipWeaponMessage, ResponseEquipWeaponMessage>(GameNetworkingConsts.EquipWeapon, ServerInventoryMessageHandlers.HandleRequestEquipWeapon);
                RegisterRequestToServer<RequestEquipArmorMessage, ResponseEquipArmorMessage>(GameNetworkingConsts.EquipArmor, ServerInventoryMessageHandlers.HandleRequestEquipArmor);
                RegisterRequestToServer<RequestUnEquipWeaponMessage, ResponseUnEquipWeaponMessage>(GameNetworkingConsts.UnEquipWeapon, ServerInventoryMessageHandlers.HandleRequestUnEquipWeapon);
                RegisterRequestToServer<RequestUnEquipArmorMessage, ResponseUnEquipArmorMessage>(GameNetworkingConsts.UnEquipArmor, ServerInventoryMessageHandlers.HandleRequestUnEquipArmor);
                RegisterRequestToServer<RequestSwitchEquipWeaponSetMessage, ResponseSwitchEquipWeaponSetMessage>(GameNetworkingConsts.SwitchEquipWeaponSet, ServerInventoryMessageHandlers.HandleRequestSwitchEquipWeaponSet);
                RegisterRequestToServer<RequestDismantleItemMessage, ResponseDismantleItemMessage>(GameNetworkingConsts.DismantleItem, ServerInventoryMessageHandlers.HandleRequestDismantleItem);
                RegisterRequestToServer<RequestDismantleItemsMessage, ResponseDismantleItemsMessage>(GameNetworkingConsts.DismantleItems, ServerInventoryMessageHandlers.HandleRequestDismantleItems);
                RegisterRequestToServer<RequestEnhanceSocketItemMessage, ResponseEnhanceSocketItemMessage>(GameNetworkingConsts.EnhanceSocketItem, ServerInventoryMessageHandlers.HandleRequestEnhanceSocketItem);
                RegisterRequestToServer<RequestRefineItemMessage, ResponseRefineItemMessage>(GameNetworkingConsts.RefineItem, ServerInventoryMessageHandlers.HandleRequestRefineItem);
                RegisterRequestToServer<RequestRemoveEnhancerFromItemMessage, ResponseRemoveEnhancerFromItemMessage>(GameNetworkingConsts.RemoveEnhancerFromItem, ServerInventoryMessageHandlers.HandleRequestRemoveEnhancerFromItem);
                RegisterRequestToServer<RequestRepairItemMessage, ResponseRepairItemMessage>(GameNetworkingConsts.RepairItem, ServerInventoryMessageHandlers.HandleRequestRepairItem);
                RegisterRequestToServer<EmptyMessage, ResponseRepairEquipItemsMessage>(GameNetworkingConsts.RepairEquipItems, ServerInventoryMessageHandlers.HandleRequestRepairEquipItems);
                RegisterRequestToServer<RequestSellItemMessage, ResponseSellItemMessage>(GameNetworkingConsts.SellItem, ServerInventoryMessageHandlers.HandleRequestSellItem);
                RegisterRequestToServer<RequestSellItemsMessage, ResponseSellItemsMessage>(GameNetworkingConsts.SellItems, ServerInventoryMessageHandlers.HandleRequestSellItems);
                RegisterRequestToServer<RequestSortItemsMessage, ResponseSortItemsMessage>(GameNetworkingConsts.SortItems, ServerInventoryMessageHandlers.HandleRequestSortItems);
                RegisterRequestToServer<RequestChangeAmmoItemMessage, ResponseChangeAmmoItemMessage>(GameNetworkingConsts.ChangeAmmoItem, ServerInventoryMessageHandlers.HandleRequestChangeAmmoItem);
                RegisterRequestToServer<RequestRemoveAmmoFromItemMessage, ResponseRemoveAmmoFromItemMessage>(GameNetworkingConsts.RemoveAmmoFromItem, ServerInventoryMessageHandlers.HandleRequestRemoveAmmoFromItem);
            }
            // Party
            if (ServerPartyMessageHandlers != null)
            {
                RegisterRequestToServer<RequestCreatePartyMessage, ResponseCreatePartyMessage>(GameNetworkingConsts.CreateParty, ServerPartyMessageHandlers.HandleRequestCreateParty);
                RegisterRequestToServer<RequestChangePartyLeaderMessage, ResponseChangePartyLeaderMessage>(GameNetworkingConsts.ChangePartyLeader, ServerPartyMessageHandlers.HandleRequestChangePartyLeader);
                RegisterRequestToServer<RequestChangePartySettingMessage, ResponseChangePartySettingMessage>(GameNetworkingConsts.ChangePartySetting, ServerPartyMessageHandlers.HandleRequestChangePartySetting);
                RegisterRequestToServer<RequestSendPartyInvitationMessage, ResponseSendPartyInvitationMessage>(GameNetworkingConsts.SendPartyInvitation, ServerPartyMessageHandlers.HandleRequestSendPartyInvitation);
                RegisterRequestToServer<RequestAcceptPartyInvitationMessage, ResponseAcceptPartyInvitationMessage>(GameNetworkingConsts.AcceptPartyInvitation, ServerPartyMessageHandlers.HandleRequestAcceptPartyInvitation);
                RegisterRequestToServer<RequestDeclinePartyInvitationMessage, ResponseDeclinePartyInvitationMessage>(GameNetworkingConsts.DeclinePartyInvitation, ServerPartyMessageHandlers.HandleRequestDeclinePartyInvitation);
                RegisterRequestToServer<RequestKickMemberFromPartyMessage, ResponseKickMemberFromPartyMessage>(GameNetworkingConsts.KickMemberFromParty, ServerPartyMessageHandlers.HandleRequestKickMemberFromParty);
                RegisterRequestToServer<EmptyMessage, ResponseLeavePartyMessage>(GameNetworkingConsts.LeaveParty, ServerPartyMessageHandlers.HandleRequestLeaveParty);
            }
            // Guild
            if (ServerGuildMessageHandlers != null)
            {
                RegisterRequestToServer<RequestCreateGuildMessage, ResponseCreateGuildMessage>(GameNetworkingConsts.CreateGuild, ServerGuildMessageHandlers.HandleRequestCreateGuild);
                RegisterRequestToServer<RequestChangeGuildLeaderMessage, ResponseChangeGuildLeaderMessage>(GameNetworkingConsts.ChangeGuildLeader, ServerGuildMessageHandlers.HandleRequestChangeGuildLeader);
                RegisterRequestToServer<RequestChangeGuildMessageMessage, ResponseChangeGuildMessageMessage>(GameNetworkingConsts.ChangeGuildMessage, ServerGuildMessageHandlers.HandleRequestChangeGuildMessage);
                RegisterRequestToServer<RequestChangeGuildMessageMessage, ResponseChangeGuildMessageMessage>(GameNetworkingConsts.ChangeGuildMessage2, ServerGuildMessageHandlers.HandleRequestChangeGuildMessage2);
                RegisterRequestToServer<RequestChangeGuildOptionsMessage, ResponseChangeGuildOptionsMessage>(GameNetworkingConsts.ChangeGuildOptions, ServerGuildMessageHandlers.HandleRequestChangeGuildOptions);
                RegisterRequestToServer<RequestChangeGuildAutoAcceptRequestsMessage, ResponseChangeGuildAutoAcceptRequestsMessage>(GameNetworkingConsts.ChangeGuildAutoAcceptRequests, ServerGuildMessageHandlers.HandleRequestChangeGuildAutoAcceptRequests);
                RegisterRequestToServer<RequestChangeGuildRoleMessage, ResponseChangeGuildRoleMessage>(GameNetworkingConsts.ChangeGuildRole, ServerGuildMessageHandlers.HandleRequestChangeGuildRole);
                RegisterRequestToServer<RequestChangeMemberGuildRoleMessage, ResponseChangeMemberGuildRoleMessage>(GameNetworkingConsts.ChangeMemberGuildRole, ServerGuildMessageHandlers.HandleRequestChangeMemberGuildRole);
                RegisterRequestToServer<RequestSendGuildInvitationMessage, ResponseSendGuildInvitationMessage>(GameNetworkingConsts.SendGuildInvitation, ServerGuildMessageHandlers.HandleRequestSendGuildInvitation);
                RegisterRequestToServer<RequestAcceptGuildInvitationMessage, ResponseAcceptGuildInvitationMessage>(GameNetworkingConsts.AcceptGuildInvitation, ServerGuildMessageHandlers.HandleRequestAcceptGuildInvitation);
                RegisterRequestToServer<RequestDeclineGuildInvitationMessage, ResponseDeclineGuildInvitationMessage>(GameNetworkingConsts.DeclineGuildInvitation, ServerGuildMessageHandlers.HandleRequestDeclineGuildInvitation);
                RegisterRequestToServer<RequestKickMemberFromGuildMessage, ResponseKickMemberFromGuildMessage>(GameNetworkingConsts.KickMemberFromGuild, ServerGuildMessageHandlers.HandleRequestKickMemberFromGuild);
                RegisterRequestToServer<EmptyMessage, ResponseLeaveGuildMessage>(GameNetworkingConsts.LeaveGuild, ServerGuildMessageHandlers.HandleRequestLeaveGuild);
                RegisterRequestToServer<RequestIncreaseGuildSkillLevelMessage, ResponseIncreaseGuildSkillLevelMessage>(GameNetworkingConsts.IncreaseGuildSkillLevel, ServerGuildMessageHandlers.HandleRequestIncreaseGuildSkillLevel);
                RegisterRequestToServer<RequestSendGuildRequestMessage, ResponseSendGuildRequestMessage>(GameNetworkingConsts.SendGuildRequest, ServerGuildMessageHandlers.HandleRequestSendGuildRequest);
                RegisterRequestToServer<RequestAcceptGuildRequestMessage, ResponseAcceptGuildRequestMessage>(GameNetworkingConsts.AcceptGuildRequest, ServerGuildMessageHandlers.HandleRequestAcceptGuildRequest);
                RegisterRequestToServer<RequestDeclineGuildRequestMessage, ResponseDeclineGuildRequestMessage>(GameNetworkingConsts.DeclineGuildRequest, ServerGuildMessageHandlers.HandleRequestDeclineGuildRequest);
                RegisterRequestToServer<RequestGetGuildRequestsMessage, ResponseGetGuildRequestsMessage>(GameNetworkingConsts.GetGuildRequests, ServerGuildMessageHandlers.HandleRequestGetGuildRequests);
                RegisterRequestToServer<RequestFindGuildsMessage, ResponseFindGuildsMessage>(GameNetworkingConsts.FindGuilds, ServerGuildMessageHandlers.HandleRequestFindGuilds);
                RegisterRequestToServer<RequestGetGuildInfoMessage, ResponseGetGuildInfoMessage>(GameNetworkingConsts.GetGuildInfo, ServerGuildMessageHandlers.HandleRequestGetGuildInfo);
                RegisterRequestToServer<EmptyMessage, ResponseGuildRequestNotificationMessage>(GameNetworkingConsts.GuildRequestNotification, ServerGuildMessageHandlers.HandleRequestGuildRequestNotification);
            }
            // Gacha
            if (ServerGachaMessageHandlers != null)
            {
                RegisterRequestToServer<EmptyMessage, ResponseGachaInfoMessage>(GameNetworkingConsts.GachaInfo, ServerGachaMessageHandlers.HandleRequestGachaInfo);
                RegisterRequestToServer<RequestOpenGachaMessage, ResponseOpenGachaMessage>(GameNetworkingConsts.OpenGacha, ServerGachaMessageHandlers.HandleRequestOpenGacha);
            }
            // Friend
            if (ServerFriendMessageHandlers != null)
            {
                RegisterRequestToServer<RequestFindCharactersMessage, ResponseSocialCharacterListMessage>(GameNetworkingConsts.FindCharacters, ServerFriendMessageHandlers.HandleRequestFindCharacters);
                RegisterRequestToServer<RequestGetFriendsMessage, ResponseGetFriendsMessage>(GameNetworkingConsts.GetFriends, ServerFriendMessageHandlers.HandleRequestGetFriends);
                RegisterRequestToServer<RequestAddFriendMessage, ResponseAddFriendMessage>(GameNetworkingConsts.AddFriend, ServerFriendMessageHandlers.HandleRequestAddFriend);
                RegisterRequestToServer<RequestRemoveFriendMessage, ResponseRemoveFriendMessage>(GameNetworkingConsts.RemoveFriend, ServerFriendMessageHandlers.HandleRequestRemoveFriend);
                RegisterRequestToServer<RequestSendFriendRequestMessage, ResponseSendFriendRequestMessage>(GameNetworkingConsts.SendFriendRequest, ServerFriendMessageHandlers.HandleRequestSendFriendRequest);
                RegisterRequestToServer<RequestAcceptFriendRequestMessage, ResponseAcceptFriendRequestMessage>(GameNetworkingConsts.AcceptFriendRequest, ServerFriendMessageHandlers.HandleRequestAcceptFriendRequest);
                RegisterRequestToServer<RequestDeclineFriendRequestMessage, ResponseDeclineFriendRequestMessage>(GameNetworkingConsts.DeclineFriendRequest, ServerFriendMessageHandlers.HandleRequestDeclineFriendRequest);
                RegisterRequestToServer<RequestGetFriendRequestsMessage, ResponseGetFriendRequestsMessage>(GameNetworkingConsts.GetFriendRequests, ServerFriendMessageHandlers.HandleRequestGetFriendRequests);
                RegisterRequestToServer<EmptyMessage, ResponseFriendRequestNotificationMessage>(GameNetworkingConsts.FriendRequestNotification, ServerFriendMessageHandlers.HandleRequestFriendRequestNotification);
            }
            // Bank
            if (ServerBankMessageHandlers != null)
            {
                RegisterRequestToServer<RequestDepositUserGoldMessage, ResponseDepositUserGoldMessage>(GameNetworkingConsts.DepositUserGold, ServerBankMessageHandlers.HandleRequestDepositUserGold);
                RegisterRequestToServer<RequestWithdrawUserGoldMessage, ResponseWithdrawUserGoldMessage>(GameNetworkingConsts.WithdrawUserGold, ServerBankMessageHandlers.HandleRequestWithdrawUserGold);
                RegisterRequestToServer<RequestDepositGuildGoldMessage, ResponseDepositGuildGoldMessage>(GameNetworkingConsts.DepositGuildGold, ServerBankMessageHandlers.HandleRequestDepositGuildGold);
                RegisterRequestToServer<RequestWithdrawGuildGoldMessage, ResponseWithdrawGuildGoldMessage>(GameNetworkingConsts.WithdrawGuildGold, ServerBankMessageHandlers.HandleRequestWithdrawGuildGold);
            }
            // User Content
            if (ServerUserContentMessageHandlers != null)
            {
                RegisterRequestToServer<RequestUnlockContentProgressionMessage, ResponseUnlockContentProgressionMessage>(GameNetworkingConsts.UnlockContentProgression, ServerUserContentMessageHandlers.HandleRequestUnlockContentProgression);
                RegisterRequestToServer<RequestAvailableContentsMessage, ResponseAvailableContentsMessage>(GameNetworkingConsts.AvailableContents, ServerUserContentMessageHandlers.HandleRequestAvailableContents);
                RegisterRequestToServer<RequestUnlockContentMessage, ResponseUnlockContentMessage>(GameNetworkingConsts.UnlockContent, ServerUserContentMessageHandlers.HandleRequestUnlockContent);
            }
        }

        protected virtual void CleanHandlers()
        {
            // Server components
            if (ServerUserHandlers != null)
                ServerUserHandlers.ClearUsersAndPlayerCharacters();
            if (ServerBuildingHandlers != null)
                ServerBuildingHandlers.ClearBuildings();
            if (ServerCharacterHandlers != null)
                ServerCharacterHandlers.ClearOnlineCharacters();
            if (ServerStorageHandlers != null)
                ServerStorageHandlers.ClearStorage();
            if (ServerPartyHandlers != null)
                ServerPartyHandlers.ClearParty();
            if (ServerGuildHandlers != null)
                ServerGuildHandlers.ClearGuild();
            // Client components
            if (ClientCharacterHandlers != null)
                ClientCharacterHandlers.ClearSubscribedPlayerCharacters();
            if (ClientOnlineCharacterHandlers != null)
                ClientOnlineCharacterHandlers.ClearOnlineCharacters();
        }

        protected virtual void SetServerHandlersRef()
        {
            GameInstance.ServerMailHandlers = ServerMailHandlers;
            GameInstance.ServerUserHandlers = ServerUserHandlers;
            GameInstance.ServerBuildingHandlers = ServerBuildingHandlers;
            GameInstance.ServerCharacterHandlers = ServerCharacterHandlers;
            GameInstance.ServerGameMessageHandlers = ServerGameMessageHandlers;
            GameInstance.ServerStorageHandlers = ServerStorageHandlers;
            GameInstance.ServerPartyHandlers = ServerPartyHandlers;
            GameInstance.ServerGuildHandlers = ServerGuildHandlers;
            GameInstance.ServerChatHandlers = ServerChatHandlers;
            GameInstance.ServerUserContentHandlers = ServerUserContentHandlers;
            GameInstance.ServerLogHandlers = ServerLogHandlers;
        }

        protected virtual void SetClientHandlersRef()
        {
            GameInstance.ClientCashShopHandlers = ClientCashShopHandlers;
            GameInstance.ClientMailHandlers = ClientMailHandlers;
            GameInstance.ClientStorageHandlers = ClientStorageHandlers;
            GameInstance.ClientCharacterHandlers = ClientCharacterHandlers;
            GameInstance.ClientInventoryHandlers = ClientInventoryHandlers;
            GameInstance.ClientPartyHandlers = ClientPartyHandlers;
            GameInstance.ClientGuildHandlers = ClientGuildHandlers;
            GameInstance.ClientGachaHandlers = ClientGachaHandlers;
            GameInstance.ClientFriendHandlers = ClientFriendHandlers;
            GameInstance.ClientBankHandlers = ClientBankHandlers;
            GameInstance.ClientUserContentHandlers = ClientUserContentHandlers;
            GameInstance.ClientOnlineCharacterHandlers = ClientOnlineCharacterHandlers;
            GameInstance.ClientChatHandlers = ClientChatHandlers;
        }
    }
}