using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace MultiplayerARPG
{
    [CreateAssetMenu(fileName = GameDataMenuConsts.NPC_DIALOG_GRAPH_FILE, menuName = GameDataMenuConsts.NPC_DIALOG_GRAPH_MENU, order = GameDataMenuConsts.NPC_DIALOG_GRAPH_ORDER)]
    public class NpcDialogGraph : NodeGraph
    {
        public List<BaseNpcDialog> GetDialogs()
        {
            List<BaseNpcDialog> dialogs = new List<BaseNpcDialog>();
            if (nodes != null && nodes.Count > 0)
            {
                for (int i = 0; i < nodes.Count; ++i)
                {
                    nodes[i].name = name + " " + i;
                    dialogs.Add(nodes[i] as BaseNpcDialog);
                }
            }
            return dialogs;
        }
    }
}
