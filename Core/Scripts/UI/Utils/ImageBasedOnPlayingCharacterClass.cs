using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerARPG
{
    public class ImageBasedOnPlayingCharacterClass : MonoBehaviour
    {
        public Image image;
        public Sprite defaultSprite;
        public CharacterClassAndSprite[] variesSprites;

        private readonly Dictionary<PlayerCharacter, Sprite> variesSpritesDict = new Dictionary<PlayerCharacter, Sprite>();

        private void Awake()
        {
            if (image == null)
                image = GetComponent<Image>();
            if (variesSprites != null && variesSprites.Length > 0)
            {
                foreach (var entry in variesSprites)
                {
                    variesSpritesDict[entry.characterClass] = entry.sprite;
                }
            }
        }

        private void Update()
        {
            if (image == null || GameInstance.PlayingCharacter == null)
                return;
            Sprite usingSprite;
            PlayerCharacter currentClass = GameInstance.PlayingCharacter.GetDatabase() as PlayerCharacter;
            if (currentClass == null || !variesSpritesDict.TryGetValue(currentClass, out usingSprite))
                usingSprite = defaultSprite;
            image.sprite = usingSprite;
        }

        [System.Serializable]
        public struct CharacterClassAndSprite
        {
            public PlayerCharacter characterClass;
            public Sprite sprite;
        }
    }
}
