using UnityEngine;
using UnityEngine.Events;

namespace MultiplayerARPG
{
    public class DoActionWhileAliveOnly : MonoBehaviour
    {
        public UnityEvent onAbleToDoAction = new UnityEvent();
        public UnityEvent onNotAbleToDoAction = new UnityEvent();
        [TextArea]
        public string notAbleToDoActionMessage = "Your character is dead, you have to respawn your character before doing it.";
        public LanguageData[] languageSpecificNotAbleToDoActionMessages;

        public void DoAction()
        {
            if (GameInstance.PlayingCharacter != null && GameInstance.PlayingCharacter.CurrentHp > 0)
            {
                onAbleToDoAction.Invoke();
            }
            else
            {
                onNotAbleToDoAction.Invoke();
                UISceneGlobal.Singleton.ShowMessageDialog(LanguageManager.GetText(UITextKeys.UI_LABEL_ERROR.ToString()), Language.GetText(languageSpecificNotAbleToDoActionMessages, notAbleToDoActionMessage));
            }
        }
    }
}
