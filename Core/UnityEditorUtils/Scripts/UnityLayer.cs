using UnityEngine;

namespace Insthync.UnityEditorUtils
{
    [System.Serializable]
    public struct UnityLayer
    {
        [SerializeField]
        private int layerIndex;
        public int LayerIndex
        {
            get { return layerIndex; }
        }

        public UnityLayer(int layerIndex)
        {
            this.layerIndex = layerIndex;
        }

        public static implicit operator int(UnityLayer unityLayer)
        {
            return unityLayer.LayerIndex;
        }

        public void Set(int layerIndex)
        {
            if (layerIndex > 0 && layerIndex < 32)
                this.layerIndex = layerIndex;
        }

        public int Mask
        {
            get { return 1 << layerIndex; }
        }
    }
}
