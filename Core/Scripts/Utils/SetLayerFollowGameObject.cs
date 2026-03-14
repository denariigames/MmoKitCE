using UnityEngine;

namespace UtilsComponents
{
    public class SetLayerFollowGameObject : MonoBehaviour
    {
        public GameObject source;
        public bool setChildrenLayersRecursively = true;
        public bool includeInactiveLayers = false;

        private void LateUpdate()
        {
            if (source != null)
            {
                if (setChildrenLayersRecursively)
                    gameObject.SetLayerRecursively(source.layer, includeInactiveLayers);
                else
                    gameObject.layer = source.layer;
            }
        }
    }
}
