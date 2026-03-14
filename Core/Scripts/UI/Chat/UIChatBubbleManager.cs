using UnityEngine;

namespace MultiplayerARPG
{
    public partial class UIChatBubbleManager : MonoBehaviour
    {
        public UIChatMessage uiLocalChatMessagePrefab;
        public UIChatMessage uiGlobalChatMessagePrefab;
        public UIChatMessage uiWhisperChatMessagePrefab;
        public UIChatMessage uiPartyChatMessagePrefab;
        public UIChatMessage uiGuildChatMessagePrefab;
        public float visibleDuration = 3f;

        private void Awake()
        {
            SetOnClientReceiveChatMessage();
        }

        private void OnDestroy()
        {
            RemoveOnClientReceiveChatMessage();
        }

        public void SetOnClientReceiveChatMessage()
        {
            RemoveOnClientReceiveChatMessage();
            ClientGenericActions.onClientReceiveChatMessage += OnReceiveChat;
        }

        public void RemoveOnClientReceiveChatMessage()
        {
            ClientGenericActions.onClientReceiveChatMessage -= OnReceiveChat;
        }

        private void OnReceiveChat(ChatMessage chatMessage)
        {
            switch (chatMessage.channel)
            {
                case ChatChannel.Local:
                    InstantiateChatMessage(uiLocalChatMessagePrefab, chatMessage);
                    break;
                case ChatChannel.Global:
                    InstantiateChatMessage(uiGlobalChatMessagePrefab, chatMessage);
                    break;
                case ChatChannel.Whisper:
                    InstantiateChatMessage(uiWhisperChatMessagePrefab, chatMessage);
                    break;
                case ChatChannel.Party:
                    InstantiateChatMessage(uiPartyChatMessagePrefab, chatMessage);
                    break;
                case ChatChannel.Guild:
                    InstantiateChatMessage(uiGuildChatMessagePrefab, chatMessage);
                    break;
            }
        }

        public void InstantiateChatMessage(UIChatMessage prefab, ChatMessage chatMessage)
        {
            if (prefab == null)
                return;
            if (string.IsNullOrEmpty(chatMessage.senderName) || string.IsNullOrEmpty(chatMessage.message))
                return;
            IPlayerCharacterData senderCharacter;
            if (!GameInstance.ClientCharacterHandlers.TryGetSubscribedPlayerCharacterByName(chatMessage.senderName, out senderCharacter) && !(senderCharacter is BasePlayerCharacterEntity))
                return;
            (senderCharacter as BasePlayerCharacterEntity).InstantiateChatBubble(prefab, chatMessage, visibleDuration);
        }
    }
}
