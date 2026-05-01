namespace MultiplayerARPG
{
    public partial class BaseGameEntity
    {
        public event GameEntityDelegate onStart;
        public event GameEntityDelegate onDestroy;
        public event GameEntityDelegate onIdentityInitialize;
        public event GameEntityDelegate onEnable;
        public event GameEntityDelegate onDisable;
        public event GameEntityDelegate onUpdate;
        public event GameEntityDelegate onLateUpdate;
        public event GameEntityDelegate onSetup;
        public event GameEntityDelegate onSetupNetElements;
        public event GameEntityDelegate onSetOwnerClient;
        public event IsUpdateEntityComponentsDelegate onIsUpdateEntityComponentsChanged;
        public event NetworkDestroyDelegate onNetworkDestroy;
        public event CanMoveDelegate onCanMoveValidated;
        public event CanSprintDelegate onCanSprintValidated;
        public event CanWalkDelegate onCanWalkValidated;
        public event CanCrouchDelegate onCanCrouchValidated;
        public event CanCrawlDelegate onCanCrawlValidated;
        public event CanJumpDelegate onCanJumpValidated;
        public event CanDashDelegate onCanDashValidated;
        public event CanTurnDelegate onCanTurnValidated;
        public event JumpForceAppliedDelegate onJumpForceApplied;
    }
}
