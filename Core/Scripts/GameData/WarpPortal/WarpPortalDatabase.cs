using UnityEngine;

namespace MultiplayerARPG
{
    [CreateAssetMenu(fileName = GameDataMenuConsts.WARP_PORTAL_DATABASE_FILE, menuName = GameDataMenuConsts.WARP_PORTAL_DATABASE_MENU, order = GameDataMenuConsts.WARP_PORTAL_DATABASE_ORDER)]
    public class WarpPortalDatabase : ScriptableObject
    {
        public WarpPortals[] maps;
    }
}
