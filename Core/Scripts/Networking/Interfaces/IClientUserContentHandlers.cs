using LiteNetLibManager;

namespace MultiplayerARPG
{
    public partial interface IClientUserContentHandlers
    {
        bool RequestUnlockContentProgression(RequestUnlockContentProgressionMessage data, ResponseDelegate<ResponseUnlockContentProgressionMessage> callback);
        bool RequestAvailableContents(RequestAvailableContentsMessage data, ResponseDelegate<ResponseAvailableContentsMessage> callback);
        bool RequestUnlockContent(RequestUnlockContentMessage data, ResponseDelegate<ResponseUnlockContentMessage> callback);
    }
}