using Insthync.UnityEditorUtils;
using System.Collections.Generic;
using UnityEngine;

namespace UtilsComponents
{
    public class SetupByAspectRatioSetting : System.IComparable
    {
        public int width = 16;
        public int height = 9;

        public int GetAspectAsInt()
        {
            return Mathf.RoundToInt((float)width / (float)height * 100f);
        }

        public int CompareTo(object obj)
        {
            if (!(obj is SetupByAspectRatioSetting))
                return 0;
            return GetAspectAsInt().CompareTo(((SetupByAspectRatioSetting)obj).GetAspectAsInt());
        }
    }

    public abstract class BaseSetupByAspectRatio<T> : MonoBehaviour
        where T : SetupByAspectRatioSetting, new()
    {
        public List<T> settings = new List<T>();
        public int currentWidth = 16;
        public int currentHeight = 9;
#if UNITY_EDITOR
        [InspectorButton(nameof(Apply))]
        public bool apply;
#endif

        protected int _prevAspect = -1;

        protected virtual void Update()
        {
            currentWidth = Screen.width;
            currentHeight = Screen.height;
            if (!Application.isPlaying)
                return;
            int aspect = GetCurrentAspectAsInt();
            if (aspect != _prevAspect)
            {
                _prevAspect = aspect;
                Apply();
            }
        }

        public int GetCurrentAspectAsInt()
        {
            return Mathf.RoundToInt((float)currentWidth / (float)currentHeight * 100f);
        }

        public int IndexOfAspect(int aspectAsInt)
        {
            settings.Sort();
            int index = -1;
            for (int i = 0; i < settings.Count; ++i)
            {
                if (aspectAsInt >= settings[i].GetAspectAsInt())
                    index = i;
            }
            if (index < 0 && settings.Count > 0)
                index = 0;
            return index;
        }

        [ContextMenu("Apply")]
        public abstract void Apply();
    }
}
