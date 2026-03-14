namespace MultiplayerARPG
{
    public interface ILadderEnterExitModel
    {
        /// <summary>
        /// Use this to get enter ladder animation duration
        /// </summary>
        /// <returns></returns>
        float GetEnterLadderAnimationDuration(LadderEntranceType entranceType);
        /// <summary>
        /// Use this function to play enter ladder animation when entering ladder
        /// </summary>
        void PlayEnterLadderAnimation(LadderEntranceType entranceType);
        /// <summary>
        /// Use this to get exit ladder animation duration
        /// </summary>
        /// <returns></returns>
        float GetExitLadderAnimationDuration(LadderEntranceType entranceType);
        /// <summary>
        /// Use this function to play exit ladder animation when exiting ladder
        /// </summary>
        void PlayExitLadderAnimation(LadderEntranceType entranceType);
    }
}