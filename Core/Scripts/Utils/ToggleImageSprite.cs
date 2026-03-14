using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UtilsComponents
{
    public class ToggleImageSprite : MonoBehaviour
    {
        [System.Serializable]
        public struct Setting
        {
            public Image image;
            public Sprite defaultSprite;
            public Sprite selectedSprite;
        }

        public Setting[] settings;
        private Toggle toggle;
        private bool dirtyIsOn;

        private void Awake()
        {
            toggle = GetComponent<Toggle>();
        }

        private void OnEnable()
        {
            dirtyIsOn = false;
            foreach (Setting setting in settings)
            {
                setting.image.sprite = setting.defaultSprite;
            }
        }

        private void Update()
        {
            if (toggle == null)
                return;

            if (dirtyIsOn != toggle.isOn)
            {
                dirtyIsOn = toggle.isOn;
                foreach (Setting setting in settings)
                {
                    setting.image.sprite = dirtyIsOn ? setting.selectedSprite : setting.defaultSprite;
                }
            }
        }

        [ContextMenu("Set default Sprite by image's Sprite")]
        public void SetDefaultSpriteByImage()
        {
#if UNITY_EDITOR
            for (int i = 0; i < settings.Length; ++i)
            {
                Setting setting = settings[i];
                setting.defaultSprite = setting.image.sprite;
                settings[i] = setting;
            }
            EditorUtility.SetDirty(this);
#endif
        }

        [ContextMenu("Set image's Sprite by default Sprite")]
        public void SetImageByDefaultSprite()
        {
#if UNITY_EDITOR
            for (int i = 0; i < settings.Length; ++i)
            {
                Setting setting = settings[i];
                setting.image.sprite = setting.defaultSprite;
            }
            EditorUtility.SetDirty(this);
#endif
        }


        [ContextMenu("Swap default Sprite and selected Sprite")]
        public void SwapDefaultSpriteAndSelectedSprite()
        {
#if UNITY_EDITOR
            for (int i = 0; i < settings.Length; ++i)
            {
                Setting setting = settings[i];
                Sprite defaultSprite = setting.defaultSprite;
                Sprite selectedSprite = setting.selectedSprite;
                setting.defaultSprite = selectedSprite;
                setting.selectedSprite = defaultSprite;
                settings[i] = setting;
            }
            EditorUtility.SetDirty(this);
#endif
        }
    }
}
