namespace MultiplayerARPG
{
    public abstract class BaseWeaponAbility : BaseGameData
    {
        public abstract string AbilityKey { get; }
        public virtual bool ShouldDeactivateOnDead { get { return true; } }
        public virtual bool ShouldDeactivateOnReload { get { return true; } }

        [System.NonSerialized]
        protected BasePlayerCharacterController _controller;
        [System.NonSerialized]
        protected CharacterItem _weapon;

        public virtual void Setup(BasePlayerCharacterController controller, CharacterItem weapon)
        {
            _controller = controller;
            _weapon = weapon;
        }

        public virtual void Desetup() { }
        public virtual void ForceDeactivated() { }
        public abstract void OnPreActivate();
        public abstract WeaponAbilityState UpdateActivation(WeaponAbilityState state, float deltaTime);
        public abstract void OnPreDeactivate();
    }
}
