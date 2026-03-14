using UtilsComponents;

namespace MultiplayerARPG
{
    public class OnEnableEventOrEventSystemReady : OnEnableEvent
    {
        protected override void OnEnable()
        {
            base.OnEnable();
            EventSystemManager.onEventSystemReady += Instance_onEventSystemReady;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            EventSystemManager.onEventSystemReady -= Instance_onEventSystemReady;
        }

        private void Instance_onEventSystemReady()
        {
            Trigger();
        }
    }
}
