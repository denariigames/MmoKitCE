using UnityEngine;

namespace MultiplayerARPG
{
    public class UIPlayingCharacterSigns : MonoBehaviour
    {
        public UICharacter uiCharacter;
        public GameObject[] isPlayingCharacterObjects = new GameObject[0];
        public GameObject[] isNotPlayingCharacterObjects = new GameObject[0];

        private void Update()
        {
            bool isPlayingCharacter = uiCharacter != null && GameInstance.PlayingCharacter != null &&
                uiCharacter.Data != null && string.Equals(uiCharacter.Data.Id, GameInstance.PlayingCharacter.Id);

            for (int i = 0; i < isPlayingCharacterObjects.Length; ++i)
            {
                isPlayingCharacterObjects[i].SetActive(isPlayingCharacter);
            }

            for (int i = 0; i < isNotPlayingCharacterObjects.Length; ++i)
            {
                isPlayingCharacterObjects[i].SetActive(!isPlayingCharacter);
            }
        }
    }
}
