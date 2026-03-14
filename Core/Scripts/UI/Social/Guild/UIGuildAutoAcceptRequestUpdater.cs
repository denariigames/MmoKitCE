using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerARPG
{
    public class UIGuildAutoAcceptRequestUpdater : MonoBehaviour
    {
        public Toggle toggle;
        public Toggle toggleOff;

        private void OnEnable()
        {
            if (GameInstance.JoinedGuild == null) return;
            if (toggle != null)
                toggle.isOn = GameInstance.JoinedGuild.autoAcceptRequests;
            if (toggleOff != null)
                toggleOff.isOn = !GameInstance.JoinedGuild.autoAcceptRequests;
        }

        public void UpdateData()
        {
            if (toggle == null) return;
            GameInstance.ClientGuildHandlers.RequestChangeGuildAutoAcceptRequests(new RequestChangeGuildAutoAcceptRequestsMessage()
            {
                autoAcceptRequests = toggle.isOn,
            }, ClientGuildActions.ResponseChangeGuildAutoAcceptRequests);
        }
    }
}
