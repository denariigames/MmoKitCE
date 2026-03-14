using System.Collections.Generic;

namespace MultiplayerARPG
{
    public class MapMarkerManager
    {
        private static readonly Dictionary<string, IMapMarker> s_allMarkers = new Dictionary<string, IMapMarker>();
        public static IReadOnlyDictionary<string, IMapMarker> AllMarkers => s_allMarkers;
        private static bool s_markersUpdated = false;
        private static readonly List<IMapMarker> s_markerValues = new List<IMapMarker>();
        public static List<IMapMarker> MarkerValues
        {
            get
            {
                if (s_markersUpdated)
                {
                    s_markersUpdated = false;
                    s_markerValues.Clear();
                    s_markerValues.AddRange(s_allMarkers.Values);
                }
                return s_markerValues;
            }
        }
        public static event System.Action<IMapMarker> OnAdded;
        public static event System.Action<IMapMarker> OnRemoved;

        public static void AddMarker(IMapMarker mapMarker)
        {
            if (!s_allMarkers.ContainsKey(mapMarker.MapMarkerId))
            {
                s_allMarkers.Add(mapMarker.MapMarkerId, mapMarker);
                s_markersUpdated = true;
                OnAdded?.Invoke(mapMarker);
            }
        }

        public static void RemoveMarker(IMapMarker mapMarker)
        {
            if (s_allMarkers.Remove(mapMarker.MapMarkerId))
            {
                s_markersUpdated = true;
                OnRemoved?.Invoke(mapMarker);
            }
        }
    }
}
