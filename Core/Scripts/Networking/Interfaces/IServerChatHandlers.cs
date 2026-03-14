namespace MultiplayerARPG
{
    /// <summary>
    /// These properties and functions will be called at server only
    /// </summary>
    public partial interface IServerChatHandlers
    {
        void OnChatMessage(ChatMessage message);
        bool CanSendSystemAnnounce(string senderId);
        bool ChatTooFast(string senderId);
        void ChatFlooded(string senderId);
    }
}
