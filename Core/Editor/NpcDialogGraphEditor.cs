using XNodeEditor;

namespace MultiplayerARPG
{
    [CustomNodeGraphEditor(typeof(NpcDialogGraph))]
    public class NpcDialogGraphEditor : NodeGraphEditor
    {
        public override string GetNodeMenuName(System.Type type)
        {
            if (type == typeof(NpcDialog))
                return base.GetNodeMenuName(type).Replace("Dialog/", "");
            else
                return null;
        }
    }
}
