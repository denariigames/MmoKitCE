namespace MultiplayerARPG
{
    public partial class LanRpgNetworkManager
    {
        private void PrepareLanRpgHandlers()
        {
            // Server Handlers
            ServerUserHandlers = gameObject.GetOrAddComponent<IServerUserHandlers, DefaultServerUserHandlers>();
            ServerBuildingHandlers = gameObject.GetOrAddComponent<IServerBuildingHandlers, DefaultServerBuildingHandlers>();
            ServerCharacterHandlers = gameObject.GetOrAddComponent<IServerCharacterHandlers, DefaultServerCharacterHandlers>();
            ServerGameMessageHandlers = gameObject.GetOrAddComponent<IServerGameMessageHandlers, DefaultServerGameMessageHandlers>();
            ServerStorageHandlers = gameObject.GetOrAddComponent<IServerStorageHandlers, LanRpgServerStorageHandlers>();
            ServerPartyHandlers = gameObject.GetOrAddComponent<IServerPartyHandlers, DefaultServerPartyHandlers>();
            ServerGuildHandlers = gameObject.GetOrAddComponent<IServerGuildHandlers, DefaultServerGuildHandlers>();
            ServerChatHandlers = gameObject.GetOrAddComponent<IServerChatHandlers, DefaultServerChatHandlers>();
            ServerLogHandlers = gameObject.GetOrAddComponent<IServerLogHandlers, DefaultServerLogHandlers>();
            // Server Message Handlers
            ServerCashShopMessageHandlers = gameObject.GetOrAddComponent<IServerCashShopMessageHandlers, LanRpgServerCashShopMessageHandlers>();
            ServerStorageMessageHandlers = gameObject.GetOrAddComponent<IServerStorageMessageHandlers, LanRpgServerStorageMessageHandlers>();
            ServerCharacterMessageHandlers = gameObject.GetOrAddComponent<IServerCharacterMessageHandlers, DefaultServerCharacterMessageHandlers>();
            ServerInventoryMessageHandlers = gameObject.GetOrAddComponent<IServerInventoryMessageHandlers, DefaultServerInventoryMessageHandlers>();
            ServerPartyMessageHandlers = gameObject.GetOrAddComponent<IServerPartyMessageHandlers, LanRpgServerPartyMessageHandlers>();
            ServerGuildMessageHandlers = gameObject.GetOrAddComponent<IServerGuildMessageHandlers, LanRpgServerGuildMessageHandlers>();
            ServerGachaMessageHandlers = gameObject.GetOrAddComponent<IServerGachaMessageHandlers, LanRpgServerGachaMessageHandlers>();
            ServerBankMessageHandlers = gameObject.GetOrAddComponent<IServerBankMessageHandlers, LanRpgServerBankMessageHandlers>();
            ServerUserContentMessageHandlers = gameObject.GetOrAddComponent<IServerUserContentMessageHandlers, LanRpgServerUserContentMessageHandlers>();
            // Client handlers
            ClientCashShopHandlers = gameObject.GetOrAddComponent<IClientCashShopHandlers, DefaultClientCashShopHandlers>();
            ClientMailHandlers = gameObject.GetOrAddComponent<IClientMailHandlers, DefaultClientMailHandlers>();
            ClientStorageHandlers = gameObject.GetOrAddComponent<IClientStorageHandlers, DefaultClientStorageHandlers>();
            ClientCharacterHandlers = gameObject.GetOrAddComponent<IClientCharacterHandlers, DefaultClientCharacterHandlers>();
            ClientInventoryHandlers = gameObject.GetOrAddComponent<IClientInventoryHandlers, DefaultClientInventoryHandlers>();
            ClientPartyHandlers = gameObject.GetOrAddComponent<IClientPartyHandlers, DefaultClientPartyHandlers>();
            ClientGuildHandlers = gameObject.GetOrAddComponent<IClientGuildHandlers, DefaultClientGuildHandlers>();
            ClientGachaHandlers = gameObject.GetOrAddComponent<IClientGachaHandlers, DefaultClientGachaHandlers>();
            ClientFriendHandlers = gameObject.GetOrAddComponent<IClientFriendHandlers, DefaultClientFriendHandlers>();
            ClientBankHandlers = gameObject.GetOrAddComponent<IClientBankHandlers, DefaultClientBankHandlers>();
            ClientUserContentHandlers = gameObject.GetOrAddComponent<IClientUserContentHandlers, DefaultClientUserContentHandlers>();
            ClientOnlineCharacterHandlers = gameObject.GetOrAddComponent<IClientOnlineCharacterHandlers, DefaultClientOnlineCharacterHandlers>();
            ClientChatHandlers = gameObject.GetOrAddComponent<IClientChatHandlers, DefaultClientChatHandlers>();
            ClientGameMessageHandlers = gameObject.GetOrAddComponent<IClientGameMessageHandlers, DefaultClientGameMessageHandlers>();
        }
    }
}
