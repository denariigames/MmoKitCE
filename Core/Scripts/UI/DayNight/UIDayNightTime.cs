using UnityEngine;

namespace MultiplayerARPG
{
    public class UIDayNightTime : MonoBehaviour
    {
        public UITimer uiTimerDayNightTime;

        private void Update()
        {
            float timeOfDay = GameInstance.Singleton != null && GameInstance.Singleton.DayNightTimeUpdater != null ? GameInstance.Singleton.DayNightTimeUpdater.TimeOfDay : 0f;
            float timeOfDayInSeconds = timeOfDay / 24f * (60f * 60f * 24f);
            if (uiTimerDayNightTime != null)
                uiTimerDayNightTime.UpdateTime(timeOfDayInSeconds);
        }
    }
}