using LiteNetLibManager;
using UnityEngine;

namespace MultiplayerARPG
{
    public partial class DefaultClientFriendHandlers : MonoBehaviour, IClientFriendHandlers
    {
        public LiteNetLibManager.LiteNetLibManager Manager { get; private set; }

        private void Awake()
        {
            Manager = GetComponent<LiteNetLibManager.LiteNetLibManager>();
        }

        public bool RequestFindCharacters(RequestFindCharactersMessage data, ResponseDelegate<ResponseSocialCharacterListMessage> callback)
        {
            return Manager.ClientSendRequest(GameNetworkingConsts.FindCharacters, data, responseDelegate: callback);
        }

        public bool RequestGetFriends(RequestGetFriendsMessage data, ResponseDelegate<ResponseGetFriendsMessage> callback)
        {
            return Manager.ClientSendRequest(GameNetworkingConsts.GetFriends, data, responseDelegate: callback);
        }

        public bool RequestAddFriend(RequestAddFriendMessage data, ResponseDelegate<ResponseAddFriendMessage> callback)
        {
            return Manager.ClientSendRequest(GameNetworkingConsts.AddFriend, data, responseDelegate: callback);
        }

        public bool RequestRemoveFriend(RequestRemoveFriendMessage data, ResponseDelegate<ResponseRemoveFriendMessage> callback)
        {
            return Manager.ClientSendRequest(GameNetworkingConsts.RemoveFriend, data, responseDelegate: callback);
        }

        public bool RequestSendFriendRequest(RequestSendFriendRequestMessage data, ResponseDelegate<ResponseSendFriendRequestMessage> callback)
        {
            return Manager.ClientSendRequest(GameNetworkingConsts.SendFriendRequest, data, responseDelegate: callback);
        }

        public bool RequestAcceptFriendRequest(RequestAcceptFriendRequestMessage data, ResponseDelegate<ResponseAcceptFriendRequestMessage> callback)
        {
            return Manager.ClientSendRequest(GameNetworkingConsts.AcceptFriendRequest, data, responseDelegate: callback);
        }

        public bool RequestDeclineFriendRequest(RequestDeclineFriendRequestMessage data, ResponseDelegate<ResponseDeclineFriendRequestMessage> callback)
        {
            return Manager.ClientSendRequest(GameNetworkingConsts.DeclineFriendRequest, data, responseDelegate: callback);
        }

        public bool RequestGetFriendRequests(RequestGetFriendRequestsMessage data, ResponseDelegate<ResponseGetFriendRequestsMessage> callback)
        {
            return Manager.ClientSendRequest(GameNetworkingConsts.GetFriendRequests, data, responseDelegate: callback);
        }

        public bool RequestFriendRequestNotification(ResponseDelegate<ResponseFriendRequestNotificationMessage> callback)
        {
            return Manager.ClientSendRequest(GameNetworkingConsts.FriendRequestNotification, EmptyMessage.Value, responseDelegate: callback);
        }
    }
}
