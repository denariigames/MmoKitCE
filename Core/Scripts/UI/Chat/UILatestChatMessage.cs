namespace MultiplayerARPG
{
    public partial class UILatestChatMessage : UIChatMessage
    {
        public ChatChannel chatChannel = ChatChannel.Local;
        public bool showAllChannels = true;

        protected override void Awake()
        {
            base.Awake();
            SetOnClientReceiveChatMessage();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            RemoveOnClientReceiveChatMessage();
        }

        public void SetOnClientReceiveChatMessage()
        {
            RemoveOnClientReceiveChatMessage();
            if (UIChatHistory.ChatMessages.Count > 0)
                OnReceiveChat(UIChatHistory.ChatMessages[UIChatHistory.ChatMessages.Count - 1]);
            ClientGenericActions.onClientReceiveChatMessage += OnReceiveChat;
        }

        public void RemoveOnClientReceiveChatMessage()
        {
            ClientGenericActions.onClientReceiveChatMessage -= OnReceiveChat;
        }

        private void OnReceiveChat(ChatMessage chatMessage)
        {
            if (showAllChannels)
            {
                Data = chatMessage;
            }
            else if (chatMessage.channel == ChatChannel.Local &&
                chatChannel == ChatChannel.Local)
            {
                Data = chatMessage;
            }
            else if (chatMessage.channel == ChatChannel.Global &&
                chatChannel == ChatChannel.Global)
            {
                Data = chatMessage;
            }
            else if (chatMessage.channel == ChatChannel.Whisper &&
                chatChannel == ChatChannel.Whisper)
            {
                Data = chatMessage;
            }
            else if (chatMessage.channel == ChatChannel.Party &&
                chatChannel == ChatChannel.Party)
            {
                Data = chatMessage;
            }
            else if (chatMessage.channel == ChatChannel.Guild &&
                chatChannel == ChatChannel.Guild)
            {
                Data = chatMessage;
            }
            else if (chatMessage.channel == ChatChannel.System &&
                chatChannel == ChatChannel.System)
            {
                Data = chatMessage;
            }
        }
    }
}
