using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UtilsComponents
{
    public class ToggleGraphicColor : MonoBehaviour
    {
        [System.Serializable]
        public struct Setting
        {
            public Graphic graphic;
            public Color defaultColor;
            public Color selectedColor;
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
                setting.graphic.color = setting.defaultColor;
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
                    setting.graphic.color = dirtyIsOn ? setting.selectedColor : setting.defaultColor;
                }
            }
        }

        [ContextMenu("Set default color by graphic's color")]
        public void SetDefaultColorByGraphic()
        {
#if UNITY_EDITOR
            for (int i = 0; i < settings.Length; ++i)
            {
                Setting setting = settings[i];
                setting.defaultColor = setting.graphic.color;
                settings[i] = setting;
            }
            EditorUtility.SetDirty(this);
#endif
        }


        [ContextMenu("Swap default color and selected color")]
        public void SwapDefaultColorAndSelectedColor()
        {
#if UNITY_EDITOR
            for (int i = 0; i < settings.Length; ++i)
            {
                Setting setting = settings[i];
                Color defaultColor = setting.defaultColor;
                Color selectedColor = setting.selectedColor;
                setting.defaultColor = selectedColor;
                setting.selectedColor = defaultColor;
                settings[i] = setting;
            }
            EditorUtility.SetDirty(this);
#endif
        }
    }
}
