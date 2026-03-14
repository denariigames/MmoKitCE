namespace MultiplayerARPG
{
    public interface IVehicleEnterExitModel
    {
        /// <summary>
        /// Use this to get enter vehicle animation duration
        /// </summary>
        /// <returns></returns>
        float GetEnterVehicleAnimationDuration(IVehicleEntity vehicleEntity);
        /// <summary>
        /// Use this function to play enter vehicle animation when entering vehicle
        /// </summary>
        void PlayEnterVehicleAnimation(IVehicleEntity vehicleEntity);
        /// <summary>
        /// Use this to get exit vehicle animation duration
        /// </summary>
        /// <returns></returns>
        float GetExitVehicleAnimationDuration(IVehicleEntity vehicleEntity);
        /// <summary>
        /// Use this function to play exit vehicle animation when exiting vehicle
        /// </summary>
        void PlayExitVehicleAnimation(IVehicleEntity vehicleEntity);
    }
}