// CE security: #31

namespace MultiplayerARPG.MMO
{
    public enum CentralServerPeerType : byte
    {
        MapSpawnServer,
        MapServer,
        InstanceMapServer,
        AllocateMapServer,
        LoginServer,
    }
}
