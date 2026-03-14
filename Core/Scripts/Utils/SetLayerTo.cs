using Insthync.UnityEditorUtils;
using UnityEngine;

namespace UtilsComponents
{
    public class SetLayerTo : MonoBehaviour
    {
        public UnityLayer layer;
        public bool setChildrenLayersRecursively = true;
        public bool includeInactiveLayers = false;

        private void LateUpdate()
        {
            if (setChildrenLayersRecursively)
                gameObject.SetLayerRecursively(layer, includeInactiveLayers);
            else
                gameObject.layer = layer;
        }
    }
}
