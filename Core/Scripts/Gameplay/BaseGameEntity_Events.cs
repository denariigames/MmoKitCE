namespace MultiplayerARPG
{
    public partial class BaseGameEntity
    {
        public event System.Action onStart;
        public event System.Action onDestroy;
        public event System.Action onIdentityInitialize;
        public event System.Action onEnable;
        public event System.Action onDisable;
        public event System.Action onUpdate;
        public event System.Action onLateUpdate;
        public event System.Action onSetup;
        public event System.Action onSetupNetElements;
        public event System.Action onSetOwnerClient;
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
