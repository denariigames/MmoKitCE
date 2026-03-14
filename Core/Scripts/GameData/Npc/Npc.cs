using UnityEngine;

namespace MultiplayerARPG
{
    [System.Serializable]
    public struct Npc
    {
#if UNITY_EDITOR || !EXCLUDE_PREFAB_REFS || DISABLE_ADDRESSABLES
        public NpcEntity entityPrefab;
#endif
#if !DISABLE_ADDRESSABLES
        public AssetReferenceNpcEntity addressableEntityPrefab;
#endif
        public Vector3 position;
        public Vector3 rotation;
        public string title;
        [Tooltip("It will use `startDialog` if `graph` is empty")]
        public BaseNpcDialog startDialog;
        [Tooltip("It will use `graph` start dialog if this is not empty")]
        public NpcDialogGraph graph;
    }
}
