using LiteNetLibManager;

namespace MultiplayerARPG
{
    public partial interface IClientGameMessageHandlers
    {
        void HandleGameMessage(MessageHandlerData messageHandler);
        void HandleFormattedGameMessage(MessageHandlerData messageHandler);
        void HandleUpdatePartyMember(MessageHandlerData messageHandler);
        void HandleUpdateParty(MessageHandlerData messageHandler);
        void HandleUpdateGuildMember(MessageHandlerData messageHandler);
        void HandleUpdateGuild(MessageHandlerData messageHandler);
        void HandleNotifyRewardExp(MessageHandlerData messageHandler);
        void HandleNotifyRewardGold(MessageHandlerData messageHandler);
        void HandleNotifyRewardItem(MessageHandlerData messageHandler);
        void HandleNotifyRewardCurrency(MessageHandlerData messageHandler);
        void HandleNotifyStorageOpened(MessageHandlerData messageHandler);
        void HandleNotifyStorageClosed(MessageHandlerData messageHandler);
        void HandleNotifyStorageItems(MessageHandlerData messageHandler);
        void HandleNotifyPartyInvitation(MessageHandlerData messageHandler);
        void HandleNotifyGuildInvitation(MessageHandlerData messageHandler);
    }
}
