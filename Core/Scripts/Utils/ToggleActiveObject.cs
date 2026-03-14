using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UtilsComponents
{
    public class ToggleActiveObject : MonoBehaviour
    {
        [System.Serializable]
        public struct Setting
        {
            public GameObject defaultObject;
            public GameObject selectedObject;
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
                setting.defaultObject.SetActive(true);
                setting.selectedObject.SetActive(false);
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
                    setting.defaultObject.SetActive(!dirtyIsOn);
                    setting.selectedObject.SetActive(dirtyIsOn);
                }
            }
        }

        [ContextMenu("Set Active Default Object")]
        public void SetActiveDefaultObject()
        {
#if UNITY_EDITOR
            for (int i = 0; i < settings.Length; ++i)
            {
                Setting setting = settings[i];
                setting.defaultObject.SetActive(true);
                setting.selectedObject.SetActive(false);
                settings[i] = setting;
            }
            EditorUtility.SetDirty(this);
#endif
        }


        [ContextMenu("Swap Default Object and Selected Object")]
        public void SwapDefaultObjectAndSelectedObject()
        {
#if UNITY_EDITOR
            for (int i = 0; i < settings.Length; ++i)
            {
                Setting setting = settings[i];
                GameObject defaultObject = setting.defaultObject;
                GameObject selectedObject = setting.selectedObject;
                setting.defaultObject = selectedObject;
                setting.selectedObject = defaultObject;
                settings[i] = setting;
            }
            EditorUtility.SetDirty(this);
#endif
        }
    }
}
