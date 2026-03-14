using LiteNetLibManager;

namespace MultiplayerARPG
{
    public interface IClientGachaHandlers
    {
        bool RequestGachaInfo(ResponseDelegate<ResponseGachaInfoMessage> callback);
        bool RequestOpenGacha(RequestOpenGachaMessage data, ResponseDelegate<ResponseOpenGachaMessage> callback);
    }
}
