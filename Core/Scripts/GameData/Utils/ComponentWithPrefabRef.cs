using Insthync.UnityEditorUtils;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MultiplayerARPG
{
    public interface IComponentWithPrefabRef
    {
        void SetupRefToPrefab(GameObject prefab);
    }

    [DisallowMultipleComponent]
    public abstract class ComponentWithPrefabRef<T> : MonoBehaviour, IComponentWithPrefabRef
        where T : MonoBehaviour
    {
#if UNITY_EDITOR
        public T refToPrefab;
        [InspectorButton(nameof(ReplacePrefab))]
        public bool repacePrefab;
#endif

        public void SetupRefToPrefab(GameObject prefab)
        {
#if UNITY_EDITOR
            if (prefab == null)
            {
                refToPrefab = null;
                return;
            }
            refToPrefab = prefab.GetComponent<T>();
#endif
        }

#if UNITY_EDITOR
        public void ReplacePrefab()
        {
            string path = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(refToPrefab);
            PrefabUtility.SaveAsPrefabAsset(gameObject, path, out bool success);
            Debug.Log($"Replaced {gameObject} to {path} success?: {success}");
        }
#endif
    }
}
