using UnityEngine;

namespace MultiplayerARPG
{
    public class UIUpdateCurrentCharacter : MonoBehaviour
    {
        public UICharacter uiCharacter;

        private void Awake()
        {
            GameInstance.OnSetPlayingCharacterEvent += GameInstance_onSetPlayingCharacter;
            GameInstance_onSetPlayingCharacter(GameInstance.PlayingCharacterEntity);
        }

        private void OnDestroy()
        {
            GameInstance.OnSetPlayingCharacterEvent -= GameInstance_onSetPlayingCharacter;
            GameInstance_onSetPlayingCharacter(null);
            uiCharacter = null;
        }

        private void GameInstance_onSetPlayingCharacter(IPlayerCharacterData data)
        {
            uiCharacter.Data = data;
        }
    }
}
