using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UtilsComponents
{
    [System.Serializable]
    public class SetupByAspectRatioActivatingSetting : SetupByAspectRatioSetting
    {
        public GameObject gameObject;
        public bool isActivate;
    }

    [System.Serializable]
    public class ActivatingSetupByAspectRatio : BaseSetupByAspectRatio<SetupByAspectRatioActivatingSetting>
    {
        public override void Apply()
        {
            currentWidth = Screen.width;
            currentHeight = Screen.height;
            int aspect = GetCurrentAspectAsInt();
            int indexOf = IndexOfAspect(aspect);
            if (indexOf < 0)
                indexOf = 0;
            if (settings.Count <= 0)
                return;
            if (settings[indexOf].gameObject != null)
                settings[indexOf].gameObject.SetActive(settings[indexOf].isActivate);
        }
    }
}
