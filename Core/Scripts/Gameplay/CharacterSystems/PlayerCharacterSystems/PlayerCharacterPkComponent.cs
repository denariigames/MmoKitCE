using LiteNetLib;
using LiteNetLibManager;
using UnityEngine;

namespace MultiplayerARPG
{
    [DisallowMultipleComponent]
    public class PlayerCharacterPkComponent : BaseNetworkedGameEntityComponent<BasePlayerCharacterEntity>
    {
        public void TogglePkMode()
        {
#if !DISABLE_CLASSIC_PK
            RPC(CmdTogglePkMode, Identity.DefaultRpcChannelId, DeliveryMethod.ReliableUnordered);
#endif
        }

#if !DISABLE_CLASSIC_PK
        [ServerRpc]
        protected void CmdTogglePkMode()
        {
            if (Entity.IsPkOn)
            {
                if (!CurrentGameplayRule.CanTurnPkOff(Entity))
                    return;
                // Turn off
                Entity.IsPkOn = false;
                Entity.PkPoint = 0;
                Entity.ConsecutivePkKills = 0;
            }
            else
            {
                if (!CurrentGameplayRule.CanTurnPkOn(Entity))
                    return;
                // Turn on
                Entity.IsPkOn = true;
                Entity.LastPkOnTime = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            }
        }
#endif
    }
}