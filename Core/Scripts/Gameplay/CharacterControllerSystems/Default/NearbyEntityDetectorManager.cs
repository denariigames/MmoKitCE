using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    public class NearbyEntityDetectorManager : MonoBehaviour
    {
        private static NearbyEntityDetectorManager _instance;
        public static NearbyEntityDetectorManager Instance => _instance != null ? _instance : (_instance = CreateInstance());
        private static HashSet<NearbyEntityDetector> _detectors = new HashSet<NearbyEntityDetector>();
        private static float _latestDetectTime = -1f;

        public float delay = 1f;

        private static NearbyEntityDetectorManager CreateInstance()
        {
            var gameObject = new GameObject(nameof(NearbyEntityDetectorManager))
            {
                hideFlags = HideFlags.DontSave,
            };
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                gameObject.hideFlags = HideFlags.HideAndDontSave;
            }
            else
#endif
            {
                DontDestroyOnLoad(gameObject);
            }
            return gameObject.AddComponent<NearbyEntityDetectorManager>();
        }

        public static void Register(NearbyEntityDetector detector)
        {
            Instance.Register_Implementation(detector);
        }

        private void Register_Implementation(NearbyEntityDetector detector)
        {
            _detectors.Add(detector);
        }

        public static void Unregister(NearbyEntityDetector detector)
        {
            Instance.Unregister_Implementation(detector);
        }

        private void Unregister_Implementation(NearbyEntityDetector detector)
        {
            _detectors.Remove(detector);
        }

        private void Awake()
        {
            if (_instance == null)
                _instance = this;
        }

        private void Update()
        {
            if (_instance != this)
            {
                Destroy(gameObject);
                return;
            }

            if (GameInstance.PlayingCharacterEntity == null)
                return;

            float currentTime = Time.unscaledTime;
            if (currentTime - _latestDetectTime > delay)
            {
                _latestDetectTime = currentTime;
                foreach (NearbyEntityDetector entityDetector in _detectors)
                {
                    entityDetector.DetectEntities();
                    entityDetector.SortNearestAllEntity();
                }
            }
            foreach (NearbyEntityDetector entityDetector in _detectors)
            {
                entityDetector.RemoveInactiveAllEntity();
            }
        }
    }
}