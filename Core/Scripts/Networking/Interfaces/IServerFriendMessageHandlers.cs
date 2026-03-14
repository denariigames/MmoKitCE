using Cysharp.Threading.Tasks;
using LiteNetLibManager;

namespace MultiplayerARPG
{
    /// <summary>
    /// These properties and functions will be called at server only
    /// </summary>
    public partial interface IServerFriendMessageHandlers
    {
        UniTaskVoid HandleRequestFindCharacters(
            RequestHandlerData requestHandler, RequestFindCharactersMessage request,
            RequestProceedResultDelegate<ResponseSocialCharacterListMessage> result);

        UniTaskVoid HandleRequestGetFriends(
            RequestHandlerData requestHandler, RequestGetFriendsMessage request,
            RequestProceedResultDelegate<ResponseGetFriendsMessage> result);

        UniTaskVoid HandleRequestAddFriend(
            RequestHandlerData requestHandler, RequestAddFriendMessage request,
            RequestProceedResultDelegate<ResponseAddFriendMessage> result);

        UniTaskVoid HandleRequestRemoveFriend(
            RequestHandlerData requestHandler, RequestRemoveFriendMessage request,
            RequestProceedResultDelegate<ResponseRemoveFriendMessage> result);

        UniTaskVoid HandleRequestSendFriendRequest(
            RequestHandlerData requestHandler, RequestSendFriendRequestMessage request,
            RequestProceedResultDelegate<ResponseSendFriendRequestMessage> result);

        UniTaskVoid HandleRequestAcceptFriendRequest(
            RequestHandlerData requestHandler, RequestAcceptFriendRequestMessage request,
            RequestProceedResultDelegate<ResponseAcceptFriendRequestMessage> result);

        UniTaskVoid HandleRequestDeclineFriendRequest(
            RequestHandlerData requestHandler, RequestDeclineFriendRequestMessage request,
            RequestProceedResultDelegate<ResponseDeclineFriendRequestMessage> result);

        UniTaskVoid HandleRequestGetFriendRequests(
            RequestHandlerData requestHandler, RequestGetFriendRequestsMessage request,
            RequestProceedResultDelegate<ResponseGetFriendRequestsMessage> result);

        UniTaskVoid HandleRequestFriendRequestNotification(
            RequestHandlerData requestHandler, EmptyMessage request,
            RequestProceedResultDelegate<ResponseFriendRequestNotificationMessage> result);
    }
}
