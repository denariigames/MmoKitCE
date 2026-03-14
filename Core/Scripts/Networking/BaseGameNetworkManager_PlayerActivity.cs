using Cysharp.Threading.Tasks;
using System.Collections.Concurrent;
using UnityEngine;

namespace MultiplayerARPG
{
    public partial class BaseGameNetworkManager
    {
        #region Activity validation functions
        public virtual bool CanWarpCharacter(BasePlayerCharacterEntity playerCharacterEntity)
        {
            if (!IsServer || playerCharacterEntity == null || playerCharacterEntity.IsWarping)
                return false;
            return true;
        }
        #endregion

        public void WarpCharacter(WarpPortalType warpPortalType, BasePlayerCharacterEntity playerCharacterEntity, string mapName, Vector3 position, bool overrideRotation, Vector3 rotation)
        {
            switch (warpPortalType)
            {
                case WarpPortalType.Default:
                    WarpCharacter(playerCharacterEntity, mapName, position, overrideRotation, rotation);
                    break;
                case WarpPortalType.EnterInstance:
                    WarpCharacterToInstance(playerCharacterEntity, mapName, position, overrideRotation, rotation);
                    break;
            }
        }

        /// <summary>
        /// Warp character to other map if `mapName` is not empty
        /// </summary>
        /// <param name="playerCharacterEntity"></param>
        /// <param name="mapName"></param>
        /// <param name="position"></param>
        /// <param name="overrideRotation"></param>
        /// <param name="rotation"></param>
        public abstract UniTask WarpCharacter(BasePlayerCharacterEntity playerCharacterEntity, string mapName, Vector3 position, bool overrideRotation, Vector3 rotation);

        /// <summary>
        /// Warp character to instance map
        /// </summary>
        /// <param name="playerCharacterEntity"></param>
        /// <param name="mapName"></param>
        /// <param name="position"></param>
        /// <param name="overrideRotation"></param>
        /// <param name="rotation"></param>
        public abstract UniTask WarpCharacterToInstance(BasePlayerCharacterEntity playerCharacterEntity, string mapName, Vector3 position, bool overrideRotation, Vector3 rotation);

        /// <summary>
        /// Check if this game network manager is for instance map or not
        /// </summary>
        /// <returns></returns>
        public abstract bool IsInstanceMap();

        /// <summary>
        /// Request player character transform, may use it to get where character will be spawned before ready to play game.
        /// </summary>
        /// <returns></returns>
        public abstract UniTask<ResponsePlayerCharacterTransformMessage> RequestPlayerCharacterTransform(long connectionId);
    }
}
