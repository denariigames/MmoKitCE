namespace MultiplayerARPG
{
    public interface IJumppableModel
    {
        /// <summary>
        /// Use this to get jump animation duration
        /// </summary>
        /// <returns></returns>
        float GetJumpAnimationDuration();
        /// <summary>
        /// Use this function to play jump animation
        /// </summary>
        void PlayJumpAnimation();
    }
}
