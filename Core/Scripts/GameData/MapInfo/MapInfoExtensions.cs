using Insthync.AddressableAssetTools;
using LiteNetLibManager;

namespace MultiplayerARPG
{
    public static class MapInfoExtensions
    {
        public static bool IsAddressableSceneValid(this BaseMapInfo mapInfo)
        {
#if !DISABLE_ADDRESSABLES
            return mapInfo != null && mapInfo.AddressableScene.IsDataValid();
#else
            return false;
#endif
        }

        public static bool IsSceneValid(this BaseMapInfo mapInfo)
        {
            return mapInfo != null && mapInfo.Scene.IsDataValid();
        }

        public static ServerSceneInfo GetSceneInfo(this BaseMapInfo mapInfo)
        {
#if !DISABLE_ADDRESSABLES
            if (mapInfo.IsAddressableSceneValid())
            {
                return mapInfo.AddressableScene.GetServerSceneInfo();
            }
#else
            if (false) { }
#endif
            else if (mapInfo.IsSceneValid())
            {
                return mapInfo.Scene.GetServerSceneInfo();
            }
            return default;
        }
    }
}
