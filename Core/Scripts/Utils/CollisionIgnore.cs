using Insthync.UnityEditorUtils;
using UnityEngine;

namespace UtilsComponents
{
    public class CollisionIgnore : MonoBehaviour
    {
        public CollisionIgnoreOption[] ignoreOptions;
        private void Awake()
        {
            foreach (CollisionIgnoreOption ignoreOption in ignoreOptions)
            {
                Physics.IgnoreLayerCollision(ignoreOption.layer1.LayerIndex, ignoreOption.layer2.LayerIndex, ignoreOption.ignore);
                Physics2D.IgnoreLayerCollision(ignoreOption.layer1.LayerIndex, ignoreOption.layer2.LayerIndex, ignoreOption.ignore);
            }
        }
    }

    [System.Serializable]
    public struct CollisionIgnoreOption
    {
        public UnityLayer layer1;
        public UnityLayer layer2;
        public bool ignore;
    }
}
