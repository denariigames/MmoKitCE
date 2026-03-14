using UnityEngine;

namespace MultiplayerARPG
{
    [CreateAssetMenu(fileName = GameDataMenuConsts.NPC_DATABASE_FILE, menuName = GameDataMenuConsts.NPC_DATABASE_MENU, order = GameDataMenuConsts.NPC_DATABASE_ORDER)]
    public class NpcDatabase : ScriptableObject
    {
        public Npcs[] maps;
    }
}
