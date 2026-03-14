using Insthync.UnityEditorUtils;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    [System.Serializable]
    public class AnimatorLODUpdater
    {
        public List<AnimatorLODSetting> settings = new List<AnimatorLODSetting>();

        [InspectorButton(nameof(SortSettings))]
        public bool sortSettings;

        private float _accumulateDeltaTime;

        public Animator Animator { get; set; }
        public Transform Transform { get; set; }
        public Transform WatcherTransform { get; set; }
        public bool NewFramePlayed { get; private set; }

        public void Update(float deltaTime)
        {
            NewFramePlayed = false;

            if (Animator == null)
            {
                return;
            }

            if (Transform == null || WatcherTransform == null || Transform == WatcherTransform)
            {
                Animator.Update(deltaTime);
                NewFramePlayed = true;
                return;
            }

            if (settings == null || settings.Count <= 0)
            {
                Animator.Update(deltaTime);
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
                    Animator.Update(_accumulateDeltaTime);
                    NewFramePlayed = true;
                    _accumulateDeltaTime = 0;
                    return;
                }
                break;
            }

            if (!found)
            {
                Animator.Update(deltaTime);
                NewFramePlayed = true;
            }
        }

        public void SortSettings()
        {
            settings.Sort();
        }
    }
}
