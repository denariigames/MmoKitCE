using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerARPG
{
    public class ImageBasedOnEquippedWeaponType : MonoBehaviour
    {
        public Image image;
        public Sprite defaultSprite;
        public WeaponTypeAndSprite[] variesSprites;

        private readonly Dictionary<WeaponType, Sprite> variesSpritesDict = new Dictionary<WeaponType, Sprite>();

        private void Awake()
        {
            if (image == null)
                image = GetComponent<Image>();
            if (variesSprites != null && variesSprites.Length > 0)
            {
                foreach (var entry in variesSprites)
                {
                    variesSpritesDict[entry.weaponType] = entry.sprite;
                }
            }
        }

        private void Update()
        {
            if (image == null || GameInstance.PlayingCharacter == null)
                return;
            bool isLeftHand = false;
            Sprite usingSprite;
            WeaponType currentWeaponType = GameInstance.PlayingCharacter.GetAvailableWeapon(ref isLeftHand).GetWeaponItem().WeaponType;
            if (currentWeaponType == null || !variesSpritesDict.TryGetValue(currentWeaponType, out usingSprite))
                usingSprite = defaultSprite;
            image.sprite = usingSprite;
        }

        [System.Serializable]
        public struct WeaponTypeAndSprite
        {
            public WeaponType weaponType;
            public Sprite sprite;
        }
    }
}
