using LiteNetLibManager;

namespace MultiplayerARPG
{
    public partial interface IClientFriendHandlers
    {
        bool RequestFindCharacters(RequestFindCharactersMessage data, ResponseDelegate<ResponseSocialCharacterListMessage> callback);
        bool RequestGetFriends(RequestGetFriendsMessage data, ResponseDelegate<ResponseGetFriendsMessage> callback);
        bool RequestAddFriend(RequestAddFriendMessage data, ResponseDelegate<ResponseAddFriendMessage> callback);
        bool RequestRemoveFriend(RequestRemoveFriendMessage data, ResponseDelegate<ResponseRemoveFriendMessage> callback);
        bool RequestSendFriendRequest(RequestSendFriendRequestMessage data, ResponseDelegate<ResponseSendFriendRequestMessage> callback);
        bool RequestAcceptFriendRequest(RequestAcceptFriendRequestMessage data, ResponseDelegate<ResponseAcceptFriendRequestMessage> callback);
        bool RequestDeclineFriendRequest(RequestDeclineFriendRequestMessage data, ResponseDelegate<ResponseDeclineFriendRequestMessage> callback);
        bool RequestGetFriendRequests(RequestGetFriendRequestsMessage data, ResponseDelegate<ResponseGetFriendRequestsMessage> callback);
        bool RequestFriendRequestNotification(ResponseDelegate<ResponseFriendRequestNotificationMessage> callback);
    }
}
