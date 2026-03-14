using UnityEngine;

namespace UtilsComponents
{
    public class SortSpriteOrderY : MonoBehaviour
    {
        private Renderer cacheRenderer;
        public Renderer CacheRenderer
        {
            get
            {
                if (cacheRenderer == null)
                    cacheRenderer = GetComponent<Renderer>();
                return cacheRenderer;
            }
        }

        void Update()
        {
            CacheRenderer.sortingOrder = -(int)(transform.position.y * 100);
        }
    }
}
