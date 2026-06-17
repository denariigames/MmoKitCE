using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    public abstract class BaseDayNightTimeUpdater : ScriptableObject
    {
        public float TimeOfDay { get; protected set; }
        public int CurrentDay { get; protected set; }

        [Header("Time Settings")]
        [SerializeField] [Range(0, 23)] private int defaultHourOfDay = 9;

        /// <summary>
        /// Init day of time, this function will be called at server to init time of day.
        /// For an offline games which may load saved time of day, developer may implement time of day loading in this function
        /// </summary>
        /// <returns>Current time of day (0-24)</returns>
        public abstract void InitTimeOfDay(BaseGameNetworkManager manager);

        /// <summary>
        /// Update time of day, this function will be called at server to update time of day by delta time (or other time system up to how developer will implement)
        /// </summary>
        /// <param name="deltaTime"></param>
        /// <returns>Current time of day (0-24)</returns>
        public abstract void UpdateTimeOfDay(float deltaTime);

        /// <summary>
        /// This function will be called when receive update time of day message from server
        /// </summary>
        /// <param name="timeOfDay"></param>
        /// <param name=""></param>
        public virtual void SetTimeOfDay(float timeOfDay)
        {
            TimeOfDay = timeOfDay;
        }

        /// <summary>
        /// Implements loading from local worldSaveData
        /// </summary>
        /// <param name="worldSaveData"></param>
        public virtual void LoadFromWorldSaveData(WorldSaveData worldSaveData)
        {
            if (worldSaveData == null) return;

            //Set default if day and hour are 0
            if (worldSaveData.currentDay == 0 && worldSaveData.timeOfDay <= 0f)
                worldSaveData.timeOfDay = defaultHourOfDay;

            TimeOfDay = Mathf.Clamp(worldSaveData.timeOfDay, 0f, 24f);
            CurrentDay = worldSaveData.currentDay;
        }
    }
}
