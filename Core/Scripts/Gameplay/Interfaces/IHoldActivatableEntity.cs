namespace MultiplayerARPG
{
    public interface IHoldActivatableEntity : IBaseActivatableEntity
    {
        /// <summary>
        /// Can activate or not? return `TRUE` if it can.
        /// </summary>
        /// <returns></returns>
        bool CanHoldActivate();
        /// <summary>
        /// Put anything you want to do when interact the object.
        /// </summary>
        void OnHoldActivate();
    }
}
