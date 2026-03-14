using UnityEngine;

namespace MultiplayerARPG
{
    public class UIGuildMessageUpdater : MonoBehaviour
    {
        public InputFieldWrapper inputField;

        private void OnEnable()
        {
            if (inputField == null || GameInstance.JoinedGuild == null) return;
            inputField.text = GameInstance.JoinedGuild.guildMessage;
        }

        public void UpdateData()
        {
            if (inputField == null) return;
            GameInstance.ClientGuildHandlers.RequestChangeGuildMessage(new RequestChangeGuildMessageMessage()
            {
                message = inputField.text,
            }, ClientGuildActions.ResponseChangeGuildMessage);
        }
    }
}
