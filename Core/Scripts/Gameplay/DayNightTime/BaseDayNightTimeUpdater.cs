using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    public abstract class BaseDayNightTimeUpdater : ScriptableObject
    {
        public float TimeOfDay { get; protected set; }

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
    }
}
