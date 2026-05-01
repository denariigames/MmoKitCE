using LiteNetLibManager;
using UnityEngine;

namespace MultiplayerARPG.MMO
{
#if NET || NETCOREAPP || ((UNITY_EDITOR || UNITY_SERVER || !EXCLUDE_SERVER_CODES) && UNITY_STANDALONE)
    public class LoginQueueEntry
    {
        public RequestHandlerData handler;
        public RequestUserLoginMessage request;
        public RequestProceedResultDelegate<ResponseUserLoginMessage> result;
    }
#endif
}
