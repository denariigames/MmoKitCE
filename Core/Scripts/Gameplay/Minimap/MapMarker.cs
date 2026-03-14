using UnityEngine;

namespace MultiplayerARPG
{
    public class MapMarker : MonoBehaviour, IMapMarker
    {
        [SerializeField]
        private string markerType;
        public string MapMarkerType { get => markerType; set => markerType = value; }
        [SerializeField]
        private string markerId;
        public string MapMarkerId { get => markerId; set => markerId = value; }

        private void Start()
        {
            MapMarkerManager.AddMarker(this);
        }

        private void OnDestroy()
        {
            MapMarkerManager.RemoveMarker(this);
        }
    }
}
