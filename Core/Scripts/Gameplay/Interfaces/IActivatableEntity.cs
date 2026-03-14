namespace MultiplayerARPG
{
    public interface IActivatableEntity : IBaseActivatableEntity
    {
        /// <summary>
        /// If this returns `TRUE`, when set this entity as a target, character will move to it to attack. Otherwise it will move to it to activate.
        /// </summary>
        /// <returns></returns>
        bool ShouldBeAttackTarget();
        /// <summary>
        /// If this returns `TRUE`, when playing character followed this entity it won't activate, I've set this to `TRUE` for player character entity because I want it to activate player character entities only when press activate button
        /// </summary>
        /// <returns></returns>
        bool ShouldNotActivateAfterFollowed();
        /// <summary>
        /// Can activate or not? return `TRUE` if it can.
        /// </summary>
        /// <returns></returns>
        bool CanActivate();
        /// <summary>
        /// Put anything you want to do when interact the object.
        /// </summary>
        void OnActivate();
    }
}
