using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;

namespace MultiplayerARPG
{
    public class CharacterActionComponentManager : MonoBehaviour
    {
        public delegate void PrepareActionDurationsResultDelegate(float[] triggerDurations, float totalDuration, float remainsDuration, float endTime);
        public delegate void PrepareActionDurationsDelegate(float[] triggerDurations, float totalDuration, bool useAnimSpeedRate);

        public const float DEFAULT_TOTAL_DURATION = 1f;
        public const float DEFAULT_TRIGGER_DURATION = 0.5f;
        public const float STATE_PREPARING_DURATION = 1f;
        private PrepareActionDurationsDelegate _onSetPreparingActionDurations = null;

        /// <summary>
        /// Calculate times for animation playing
        /// <param name="triggerDurations"></param>
        /// <param name="totalDuration"></param>
        /// <param name="animSpeedRate"></param>
        /// <param name="cancellationToken"></param>
        /// <param name="resultCallback"></param>
        /// <returns></returns>
        /// </summary>
        public async UniTask PrepareActionDurations(float[] triggerDurations, float totalDuration, float animSpeedRate, float changedDuration, CancellationToken cancellationToken, PrepareActionDurationsResultDelegate resultCallback)
        {
            float remainsDuration = BaseCharacterModel.GetAnimationDuration(totalDuration, animSpeedRate, changedDuration);
            float setupDuration = 0f;
            // Try setup state data (maybe by animation clip events or state machine behaviours), if it was not set up
            if (triggerDurations == null || triggerDurations.Length == 0 || totalDuration < 0f)
            {
                // `_onSetPreparingActionDurations` will be invoked in `PrepareActionDurations` to setup `triggerDurations`, `totalDuration`
                _onSetPreparingActionDurations += (__triggerDurations, __totalDuration, __useAnimSpeedRate) =>
                {
                    triggerDurations = __triggerDurations;
                    totalDuration = __totalDuration;
                    if (!__useAnimSpeedRate)
                        animSpeedRate = 1f;
                };
                // Wait some components to setup proper `triggerDurations` and `totalDuration` within `STATE_PREPARING_DURATION`
                do
                {
                    await UniTask.Yield(cancellationToken);
                    setupDuration += Time.unscaledDeltaTime;
                } while (setupDuration < STATE_PREPARING_DURATION && (triggerDurations == null || triggerDurations.Length == 0 || totalDuration < 0f));
                if (setupDuration < STATE_PREPARING_DURATION)
                {
                    remainsDuration = BaseCharacterModel.GetAnimationDuration(totalDuration, animSpeedRate, changedDuration);
                }
                else
                {
                    // Can't setup properly, so try to setup manually to make it still workable
                    remainsDuration = BaseCharacterModel.GetAnimationDuration(DEFAULT_TOTAL_DURATION, animSpeedRate, changedDuration);
                    triggerDurations = new float[1]
                    {
                        DEFAULT_TRIGGER_DURATION,
                    };
                }
            }
            resultCallback?.Invoke(triggerDurations, totalDuration, remainsDuration, Time.unscaledTime + remainsDuration);
        }

        public bool ShouldPrepareActionDurations()
        {
            return _onSetPreparingActionDurations != null;
        }

        public void PrepareActionDurations(float[] triggerDurations, float totalDuration, bool useAnimSpeedRate)
        {
            _onSetPreparingActionDurations?.Invoke(triggerDurations, totalDuration, useAnimSpeedRate);
            _onSetPreparingActionDurations = null;
        }
    }
}
