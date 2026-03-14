using Insthync.UnityRestClient;
using LiteNetLibManager;

namespace MultiplayerARPG
{
    public static class GameNetworkingReponseUtils
    {
        public static bool ShowUnhandledResponseMessageDialog(this AckResponseCode responseCode, UITextKeys error)
        {
            switch (responseCode)
            {
                case AckResponseCode.Exception:
                    UISceneGlobal.Singleton.ShowMessageDialog(LanguageManager.GetText(UITextKeys.UI_LABEL_ERROR.ToString()), LanguageManager.GetText(UITextKeys.UI_ERROR_INTERNAL_SERVER_ERROR.ToString()));
                    return true;
                case AckResponseCode.Unimplemented:
                    UISceneGlobal.Singleton.ShowMessageDialog(LanguageManager.GetText(UITextKeys.UI_LABEL_ERROR.ToString()), LanguageManager.GetText(UITextKeys.UI_ERROR_SERVICE_NOT_AVAILABLE.ToString()));
                    return true;
                case AckResponseCode.Error:
                    UISceneGlobal.Singleton.ShowMessageDialog(LanguageManager.GetText(UITextKeys.UI_LABEL_ERROR.ToString()), LanguageManager.GetText(error.ToString()));
                    return true;
                case AckResponseCode.Timeout:
                    UISceneGlobal.Singleton.ShowMessageDialog(LanguageManager.GetText(UITextKeys.UI_LABEL_ERROR.ToString()), LanguageManager.GetText(UITextKeys.UI_ERROR_REQUEST_TIMEOUT.ToString()));
                    return true;
            }
            return false;
        }

        public static bool ShowUnhandledResponseMessageDialog(this RestClient.IResult result)
        {
            if (result.IsNetworkError)
            {
                UISceneGlobal.Singleton.ShowMessageDialog(LanguageManager.GetText(UITextKeys.UI_LABEL_ERROR.ToString()), LanguageManager.GetText(UITextKeys.UI_ERROR_CONNECTION_FAILED.ToString()));
                return true;
            }
            if (result.IsHttpError)
            {
                UISceneGlobal.Singleton.ShowMessageDialog(LanguageManager.GetText(UITextKeys.UI_LABEL_ERROR.ToString()), LanguageManager.GetText(UITextKeys.UI_ERROR_INTERNAL_SERVER_ERROR.ToString()));
                return true;
            }
            // TODO: Improve error handling
            return false;
        }

        public static ChatMessage FillChannelId(this ChatMessage message)
        {
            IPlayerCharacterData playerCharacter;
            if (message.channel == ChatChannel.Party || message.channel == ChatChannel.Guild)
            {
                if (!string.IsNullOrEmpty(message.senderName) &&
                    GameInstance.ServerUserHandlers.TryGetPlayerCharacterByName(message.senderName, out playerCharacter))
                {
                    switch (message.channel)
                    {
                        case ChatChannel.Party:
                            message.channelId = playerCharacter.PartyId;
                            break;
                        case ChatChannel.Guild:
                            message.channelId = playerCharacter.GuildId;
                            break;
                    }
                }
            }
            return message;
        }
    }
}
