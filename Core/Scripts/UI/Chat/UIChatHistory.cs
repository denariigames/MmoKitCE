using System.Collections.Generic;

namespace MultiplayerARPG
{
    public static class UIChatHistory
    {
        public const int MAX_CHAT_HISTORY = 50;
        public static readonly List<ChatMessage> ChatMessages = new List<ChatMessage>();

        static UIChatHistory()
        {
            ClientGenericActions.onClientReceiveChatMessage += OnReceiveChat;
        }

        private static void OnReceiveChat(ChatMessage chatMessage)
        {
            ChatMessages.Add(chatMessage);
            if (ChatMessages.Count > MAX_CHAT_HISTORY)
                ChatMessages.RemoveAt(0);
        }
    }
}
