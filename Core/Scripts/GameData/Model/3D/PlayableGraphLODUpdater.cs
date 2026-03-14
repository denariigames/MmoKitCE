using Insthync.UnityEditorUtils;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

namespace MultiplayerARPG.GameData.Model.Playables
{
    [System.Serializable]
    public class PlayableGraphLODUpdater
    {
        public List<AnimatorLODSetting> settings = new List<AnimatorLODSetting>();

        [InspectorButton(nameof(SortSettings))]
        public bool sortSettings;

        private float _accumulateDeltaTime;

        public PlayableGraph Graph { get; set; }
        public Transform Transform { get; set; }
        public Transform WatcherTransform { get; set; }
        public bool NewFramePlayed { get; private set; }

        public void Update(float deltaTime)
        {
            NewFramePlayed = false;

            if (!Graph.IsValid())
            {
                return;
            }

            if (Transform == null || WatcherTransform == null || Transform == WatcherTransform)
            {
                Graph.Evaluate(deltaTime);
                NewFramePlayed = true;
                return;
            }

            if (settings == null || settings.Count <= 0)
            {
                Graph.Evaluate(deltaTime);
                NewFramePlayed = true;
                return;
            }

            _accumulateDeltaTime += deltaTime;
            bool found = false;
            for (int i = settings.Count - 1; i >= 0; --i)
            {
                if (Vector3.Distance(Transform.position, WatcherTransform.position) < settings[i].distance)
                    continue;
                found = true;
                if (_accumulateDeltaTime >= 1f / settings[i].framesPerSecond)
                {
                    Graph.Evaluate(_accumulateDeltaTime);
                    NewFramePlayed = true;
                    _accumulateDeltaTime = 0;
                    return;
                }
                break;
            }

            if (!found)
            {
                Graph.Evaluate(deltaTime);
                NewFramePlayed = true;
            }
        }

        public void SortSettings()
        {
            settings.Sort();
        }
    }
}
