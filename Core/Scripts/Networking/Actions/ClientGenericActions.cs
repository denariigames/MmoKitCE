using LiteNetLib;
using LiteNetLibManager;
using System.Net.Sockets;

namespace MultiplayerARPG
{
    public static class ClientGenericActions
    {
        public static event System.Action onClientConnected;
        public static event System.Action onClientStopped;
        public static event System.Action onClientWarp;

        public delegate void ClientDisconnectedHandler(DisconnectReason reason, SocketError socketError, UITextKeys message);
        public static event ClientDisconnectedHandler onClientDisconnected;

        public delegate void ChatMessageHandler(ChatMessage message);
        public static event ChatMessageHandler onClientReceiveChatMessage;

        public delegate void GameMessageHandler(UITextKeys message);
        public static event GameMessageHandler onClientReceiveGameMessage;

        public delegate void FormattedGameMessageHandler(UIFormatKeys format, string[] args);
        public static event FormattedGameMessageHandler onClientReceiveFormattedGameMessage;

        public delegate void RewardExpHandler(RewardGivenType givenType, int amount);
        public static event RewardExpHandler onNotifyRewardExp;

        public delegate void RewardGoldHandler(RewardGivenType givenType, int amount);
        public static event RewardGoldHandler onNotifyRewardGold;

        public delegate void RewardItemHandler(RewardGivenType givenType, int dataId, int amount);
        public static event RewardItemHandler onNotifyRewardItem;

        public delegate void RewardCurrencyHandler(RewardGivenType givenType, int dataId, int amount);
        public static event RewardCurrencyHandler onNotifyRewardCurrency;

        public delegate void RewardUnlockableContentHandler(RewardGivenType givenType, UnlockableContentType type, int dataId, int changedProgression, bool unlocked);
        public static event RewardUnlockableContentHandler onNotifyRewardUnlockableContent;

        public delegate void BattlePointsChangedHandler(int amount);
        public static event BattlePointsChangedHandler onNotifyBattlePointsChanged;

        public delegate void EnterGameResponseHandler(AckResponseCode responseCode);
        public static event EnterGameResponseHandler onEnterGameResponse;

        public delegate void ClientReadyResponseHandler(AckResponseCode responseCode);
        public static event ClientReadyResponseHandler onClientReadyResponse;

        public static void Clean()
        {
            onClientConnected = null;
            onClientDisconnected = null;
            onClientStopped = null;
            onClientWarp = null;
            onClientReceiveChatMessage = null;
            onClientReceiveGameMessage = null;
            onClientReceiveFormattedGameMessage = null;
            onNotifyRewardExp = null;
            onNotifyRewardGold = null;
            onNotifyRewardItem = null;
            onNotifyRewardCurrency = null;
            onNotifyRewardUnlockableContent = null;
            onNotifyBattlePointsChanged = null;
            onEnterGameResponse = null;
            onClientReadyResponse = null;
        }

        public static void ClientConnected()
        {
            if (onClientConnected != null)
                onClientConnected.Invoke();
        }

        public static void ClientDisconnected(DisconnectReason reason, SocketError socketError, UITextKeys message)
        {
            if (onClientDisconnected != null)
                onClientDisconnected.Invoke(reason, socketError, message);
        }

        public static void ClientStopped()
        {
            if (onClientStopped != null)
                onClientStopped.Invoke();
        }

        public static void ClientWarp()
        {
            if (onClientWarp != null)
                onClientWarp.Invoke();
        }

        public static void ClientReceiveChatMessage(ChatMessage message)
        {
            if (onClientReceiveChatMessage != null)
                onClientReceiveChatMessage.Invoke(message);
        }

        public static void ClientReceiveGameMessage(UITextKeys message)
        {
            if (message == UITextKeys.NONE)
                return;
            if (onClientReceiveGameMessage != null)
                onClientReceiveGameMessage.Invoke(message);
        }

        public static void ClientReceiveGameMessage(UIFormatKeys format, string[] args)
        {
            if (onClientReceiveFormattedGameMessage != null)
                onClientReceiveFormattedGameMessage.Invoke(format, args);
        }

        public static void NotifyRewardExp(RewardGivenType givenType, int amount)
        {
            if (onNotifyRewardExp != null)
                onNotifyRewardExp.Invoke(givenType, amount);
        }

        public static void NotifyRewardGold(RewardGivenType givenType, int amount)
        {
            if (onNotifyRewardGold != null)
                onNotifyRewardGold.Invoke(givenType, amount);
        }

        public static void NotifyRewardItem(RewardGivenType givenType, int dataId, int amount)
        {
            if (onNotifyRewardItem != null)
                onNotifyRewardItem.Invoke(givenType, dataId, amount);
        }

        public static void NotifyRewardCurrency(RewardGivenType givenType, int dataId, int amount)
        {
            if (onNotifyRewardCurrency != null)
                onNotifyRewardCurrency.Invoke(givenType, dataId, amount);
        }

        public static void NotifyRewardUnlockableContent(RewardGivenType givenType, UnlockableContentType type, int dataId, int changedProgression, bool unlocked)
        {
            if (onNotifyRewardUnlockableContent != null)
                onNotifyRewardUnlockableContent.Invoke(givenType, type, dataId, changedProgression, unlocked);
        }

        public static void NotifyBattlePointsChanged(int amount)
        {
            if (onNotifyBattlePointsChanged != null)
                onNotifyBattlePointsChanged.Invoke(amount);
        }

        public static void OnEnterGameResponse(AckResponseCode responseCode)
        {
            if (onEnterGameResponse != null)
                onEnterGameResponse.Invoke(responseCode);
        }

        public static void OnClientReadyResponse(AckResponseCode responseCode)
        {
            if (onClientReadyResponse != null)
                onClientReadyResponse.Invoke(responseCode);
        }
    }
}
