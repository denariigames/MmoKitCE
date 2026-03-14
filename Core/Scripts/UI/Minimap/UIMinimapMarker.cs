using UnityEngine;

namespace MultiplayerARPG
{
    public class UIMinimapMarker : MonoBehaviour
    {
        protected UIMinimapRenderer.MarkerData _markerData;

        public virtual void Setup(UIMinimapRenderer.MarkerData markerData)
        {
            _markerData = markerData;
        }
    }
}
