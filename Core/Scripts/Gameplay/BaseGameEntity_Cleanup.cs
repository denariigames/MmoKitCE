namespace MultiplayerARPG
{
    public partial class BaseGameEntity
    {
        public virtual void Clean()
        {
            ownerObjects?.DestroyAndNullify();
            ownerObjects = null;
            nonOwnerObjects?.DestroyAndNullify();
            nonOwnerObjects = null;
            model = null;
            cameraTargetTransform = null;
            fpsCameraTargetTransform = null;
            // Events
            onStart = null;
            onEnable = null;
            onDisable = null;
            onUpdate = null;
            onLateUpdate = null;
            onSetup = null;
            onSetupNetElements = null;
            onSetOwnerClient = null;
            onIsUpdateEntityComponentsChanged = null;
            onNetworkDestroy = null;
            onCanMoveValidated = null;
            onCanSprintValidated = null;
            onCanWalkValidated = null;
            onCanCrouchValidated = null;
            onCanCrawlValidated = null;
            onCanJumpValidated = null;
            onCanTurnValidated = null;
            onJumpForceApplied = null;
            // Mount
            PassengingVehicleEntity = null;
            // Move
            Movement = null;
        }
    }
}
