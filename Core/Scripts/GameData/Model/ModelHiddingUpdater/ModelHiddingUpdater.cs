using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    public class ModelHiddingUpdater : MonoBehaviour
    {
        public virtual void SetHiddingObjectsAndRenderers(List<GameObject> hiddingObjects, List<Renderer> hiddingRenderers, bool isHidding)
        {
            int i;
            if (hiddingObjects != null && hiddingObjects.Count > 0)
            {
                for (i = 0; i < hiddingObjects.Count; ++i)
                {
                    hiddingObjects[i].SetActive(!isHidding);
                }
            }
            if (hiddingRenderers != null && hiddingRenderers.Count > 0)
            {
                for (i = 0; i < hiddingRenderers.Count; ++i)
                {
                    hiddingRenderers[i].forceRenderingOff = isHidding;
                }
            }
        }
    }
}