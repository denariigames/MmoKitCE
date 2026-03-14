using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerARPG
{
    public static class UIComponentExtensions
    {
        public static void SetImageGameDataIcon(this Image image, BaseGameData gameData)
        {
            SetImageGameDataIcon(image, gameData, true);
        }

        public static async void SetImageGameDataIcon(this Image image, BaseGameData gameData, bool deactivateIfNoContent, params GameObject[] placeHolders)
        {
#if UNITY_EDITOR || !UNITY_SERVER
            Sprite sprite = null;
            if (gameData)
                sprite = await gameData.GetIcon();
            if (!image)
                return;
            image.SetImageSprite(sprite, deactivateIfNoContent, placeHolders);
#endif
        }

        public static async void SetRawImageExternalTexture(this RawImage rawImage, string url, Texture2D defaultTexture = null, bool deactivateIfNoContent = true)
        {
#if UNITY_EDITOR || !UNITY_SERVER
            if (!rawImage)
                return;
            Texture2D texture = null;
            // TODO: May improve it by change URL format before load
            if (!string.IsNullOrWhiteSpace(url))
            {
                texture = await ExternalTextureManager.Load(url);
                if (texture == null)
                    texture = defaultTexture;
            }
            if (!rawImage)
                return;
            rawImage.gameObject.SetActive(!deactivateIfNoContent || texture != null);
            rawImage.texture = texture;
#endif
        }

        public static void SetImageNpcDialogIcon(this Image image, BaseNpcDialog npcDialog)
        {
            SetImageNpcDialogIcon(image, npcDialog, true);
        }

        public static async void SetImageNpcDialogIcon(this Image image, BaseNpcDialog npcDialog, bool deactivateIfNoContent, params GameObject[] placeHolders)
        {
#if UNITY_EDITOR || !UNITY_SERVER
            Sprite sprite = null;
            if (npcDialog)
                sprite = await npcDialog.GetIcon();
            if (!image)
                return;
            image.SetImageSprite(sprite, deactivateIfNoContent, placeHolders);
#endif
        }

        public static void SetImageSprite(this Image image, Sprite sprite, bool deactivateIfNoContent, params GameObject[] placeHolders)
        {
#if UNITY_EDITOR || !UNITY_SERVER
            if (!image)
                return;
            bool isNull = sprite == null;
            image.gameObject.SetActive(!deactivateIfNoContent || !isNull);
            image.sprite = sprite;
            if (placeHolders != null && placeHolders.Length > 0)
            {
                for (int i = 0; i < placeHolders.Length; ++i)
                {
                    placeHolders[i].SetActive(isNull);
                }
            }
#endif
        }

        public static async void PlayNpcDialogVoice(this AudioSource source, MonoBehaviour uiRoot, BaseNpcDialog npcDialog)
        {
#if UNITY_EDITOR || !UNITY_SERVER
            if (!source)
                return;
            source.Stop();
            AudioClip clip = await npcDialog.GetVoice();
            if (!source)
                return;
            source.clip = clip;
            if (clip != null && uiRoot && uiRoot.enabled)
                source.Play();
#endif
        }
    }
}
