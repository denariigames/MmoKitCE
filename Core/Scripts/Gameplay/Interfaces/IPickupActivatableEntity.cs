namespace MultiplayerARPG
{
    public interface IPickupActivatableEntity : IBaseActivatableEntity
    {
        /// <summary>
        /// Can activate or not? return `TRUE` if it can.
        /// </summary>
        /// <returns></returns>
        bool CanPickupActivate();
        /// <summary>
        /// Put anything you want to do when interact the object.
        /// </summary>
        void OnPickupActivate();
        /// <summary>
        /// This function will be call at server to do something when picking up
        /// For example, add item to inventory, return `FALSE` if character cannot do it
        /// </summary>
        bool ProceedPickingUpAtServer(BaseCharacterEntity characterEntity, out UITextKeys message);
    }
}
