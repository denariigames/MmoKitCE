using UnityEngine;
using UnityEditor;
using XNode;
using XNodeEditor;

namespace MultiplayerARPG
{
    [CustomNodeEditor(typeof(NpcDialog))]
    public class NpcDialogNodeEditor : NodeEditor
    {
        public override void OnBodyGUI()
        {
            serializedObject.Update();

            NpcDialog node = target as NpcDialog;
            NodeEditorGUILayout.PortField(target.GetInputPort(nameof(node.input)));
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("title"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("titles"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("description"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("descriptions"), true);
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
            EditorGUILayout.PropertyField(serializedObject.FindProperty("icon"));
#endif
#if !DISABLE_ADDRESSABLES
            EditorGUILayout.PropertyField(serializedObject.FindProperty("addressableIcon"));
#endif
#if !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
            EditorGUILayout.PropertyField(serializedObject.FindProperty("voice"));
#endif
#if !DISABLE_ADDRESSABLES
            EditorGUILayout.PropertyField(serializedObject.FindProperty("addressableVoice"));
#endif
            EditorGUILayout.PropertyField(serializedObject.FindProperty("enterDialogActionsOnClient"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("enterDialogActionsOnServer"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(node.type)));
            switch (node.type)
            {
                case NpcDialogType.Normal:
                    NodeEditorGUILayout.DynamicPortList(nameof(node.menus), typeof(NpcDialog), serializedObject, NodePort.IO.Output, Node.ConnectionType.Override);
                    break;
                case NpcDialogType.Quest:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(node.quest)));
                    NodeEditorGUILayout.PortField(target.GetOutputPort(nameof(node.questAcceptedDialog)));
                    NodeEditorGUILayout.PortField(target.GetOutputPort(nameof(node.questDeclinedDialog)));
                    NodeEditorGUILayout.PortField(target.GetOutputPort(nameof(node.questAbandonedDialog)));
                    NodeEditorGUILayout.PortField(target.GetOutputPort(nameof(node.questCompletedDialog)));
                    break;
                case NpcDialogType.Shop:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(node.sellItems)));
                    break;
                case NpcDialogType.CraftItem:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(node.itemCraft)));
                    NodeEditorGUILayout.PortField(target.GetOutputPort(nameof(node.craftDoneDialog)));
                    NodeEditorGUILayout.PortField(target.GetOutputPort(nameof(node.craftItemWillOverwhelmingDialog)));
                    NodeEditorGUILayout.PortField(target.GetOutputPort(nameof(node.craftNotMeetRequirementsDialog)));
                    NodeEditorGUILayout.PortField(target.GetOutputPort(nameof(node.craftCancelDialog)));
                    break;
                case NpcDialogType.SaveRespawnPoint:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(node.saveRespawnMap)));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(node.saveRespawnPosition)));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(node.confirmRequirement)));
                    NodeEditorGUILayout.PortField(target.GetOutputPort(nameof(node.saveRespawnConfirmDialog)));
                    NodeEditorGUILayout.PortField(target.GetOutputPort(nameof(node.saveRespawnCancelDialog)));
                    break;
                case NpcDialogType.Warp:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(node.warpPortalType)));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(node.warpMap)));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(node.warpPosition)));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(node.warpOverrideRotation)));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(node.warpRotation)));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(nameof(node.confirmRequirement)));
                    NodeEditorGUILayout.PortField(target.GetOutputPort(nameof(node.warpCancelDialog)));
                    break;
                case NpcDialogType.RefineItem:
                    NodeEditorGUILayout.PortField(target.GetOutputPort(nameof(node.refineItemCancelDialog)));
                    break;
                case NpcDialogType.PlayerStorage:
                    NodeEditorGUILayout.PortField(target.GetOutputPort(nameof(node.storageCancelDialog)));
                    break;
                case NpcDialogType.GuildStorage:
                    NodeEditorGUILayout.PortField(target.GetOutputPort(nameof(node.storageCancelDialog)));
                    break;
            }

            serializedObject.ApplyModifiedProperties();
        }

        public override Color GetTint()
        {
            if (target != null && target.graph != null && target.graph.nodes[0] == target)
                return new Color(0.3f, 0.6f, 0.3f);
            return base.GetTint();
        }

        public override int GetWidth()
        {
            return 400;
        }
    }
}
