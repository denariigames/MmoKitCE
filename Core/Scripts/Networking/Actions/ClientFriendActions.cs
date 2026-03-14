using LiteNetLibManager;

namespace MultiplayerARPG
{
    public static class ClientFriendActions
    {
        public static event System.Action<ResponseHandlerData, AckResponseCode, ResponseSocialCharacterListMessage> onResponseFindCharacters;
        public static event System.Action<ResponseHandlerData, AckResponseCode, ResponseGetFriendsMessage> onResponseGetFriends;
        public static event System.Action<ResponseHandlerData, AckResponseCode, ResponseAddFriendMessage> onResponseAddFriend;
        public static event System.Action<ResponseHandlerData, AckResponseCode, ResponseRemoveFriendMessage> onResponseRemoveFriend;
        public static event System.Action<ResponseHandlerData, AckResponseCode, ResponseSendFriendRequestMessage> onResponseSendFriendRequest;
        public static event System.Action<ResponseHandlerData, AckResponseCode, ResponseAcceptFriendRequestMessage> onResponseAcceptFriendRequest;
        public static event System.Action<ResponseHandlerData, AckResponseCode, ResponseDeclineFriendRequestMessage> onResponseDeclineFriendRequest;
        public static event System.Action<ResponseHandlerData, AckResponseCode, ResponseGetFriendRequestsMessage> onResponseGetFriendRequests;
        public static event System.Action<ResponseHandlerData, AckResponseCode, ResponseFriendRequestNotificationMessage> onResponseFriendRequestNotification;

        public static void Clean()
        {
            onResponseFindCharacters = null;
            onResponseGetFriends = null;
            onResponseAddFriend = null;
            onResponseRemoveFriend = null;
            onResponseSendFriendRequest = null;
            onResponseAcceptFriendRequest = null;
            onResponseDeclineFriendRequest = null;
            onResponseGetFriendRequests = null;
            onResponseFriendRequestNotification = null;
            FriendRequestNotificationCacheManager.ClearCache();
        }

        public static void ResponseFindCharacters(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseSocialCharacterListMessage response)
        {
            ClientGenericActions.ClientReceiveGameMessage(response.message);
            if (onResponseFindCharacters != null)
                onResponseFindCharacters.Invoke(requestHandler, responseCode, response);
        }

        public static void ResponseGetFriends(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseGetFriendsMessage response)
        {
            ClientGenericActions.ClientReceiveGameMessage(response.message);
            if (onResponseGetFriends != null)
                onResponseGetFriends.Invoke(requestHandler, responseCode, response);
        }

        public static void ResponseAddFriend(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseAddFriendMessage response)
        {
            ClientGenericActions.ClientReceiveGameMessage(response.message);
            if (onResponseAddFriend != null)
                onResponseAddFriend.Invoke(requestHandler, responseCode, response);
        }

        public static void ResponseRemoveFriend(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseRemoveFriendMessage response)
        {
            ClientGenericActions.ClientReceiveGameMessage(response.message);
            if (onResponseRemoveFriend != null)
                onResponseRemoveFriend.Invoke(requestHandler, responseCode, response);
        }

        public static void ResponseSendFriendRequest(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseSendFriendRequestMessage response)
        {
            ClientGenericActions.ClientReceiveGameMessage(response.message);
            if (onResponseSendFriendRequest != null)
                onResponseSendFriendRequest.Invoke(requestHandler, responseCode, response);
        }

        public static void ResponseAcceptFriendRequest(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseAcceptFriendRequestMessage response)
        {
            ClientGenericActions.ClientReceiveGameMessage(response.message);
            if (onResponseAcceptFriendRequest != null)
                onResponseAcceptFriendRequest.Invoke(requestHandler, responseCode, response);
            if (responseCode == AckResponseCode.Success)
                FriendRequestNotificationCacheManager.LoadOrGetFriendRequestNotificationFromCache(null, true);
        }

        public static void ResponseDeclineFriendRequest(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseDeclineFriendRequestMessage response)
        {
            ClientGenericActions.ClientReceiveGameMessage(response.message);
            if (onResponseDeclineFriendRequest != null)
                onResponseDeclineFriendRequest.Invoke(requestHandler, responseCode, response);
            if (responseCode == AckResponseCode.Success)
                FriendRequestNotificationCacheManager.LoadOrGetFriendRequestNotificationFromCache(null, true);
        }

        public static void ResponseGetFriendRequests(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseGetFriendRequestsMessage response)
        {
            ClientGenericActions.ClientReceiveGameMessage(response.message);
            if (onResponseGetFriendRequests != null)
                onResponseGetFriendRequests.Invoke(requestHandler, responseCode, response);
        }

        public static void ResponseFriendRequestNotification(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseFriendRequestNotificationMessage response)
        {
            ClientGenericActions.ClientReceiveGameMessage(response.message);
            if (onResponseFriendRequestNotification != null)
                onResponseFriendRequestNotification.Invoke(requestHandler, responseCode, response);
        }
    }
}
