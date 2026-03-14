using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UtilsComponents
{
    [System.Serializable]
    public class SetupByAspectRatioCanvasScalerSetting : SetupByAspectRatioSetting
    {
        public float matchWidthOrHeight = 0;
        public CanvasScaler.ScreenMatchMode screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
    }

    public class CanvasScalerSetupByAspectRatio : BaseSetupByAspectRatio<SetupByAspectRatioCanvasScalerSetting>
    {
        public List<CanvasScaler> scalers = new List<CanvasScaler>();

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
            for (int i = 0; i < scalers.Count; ++i)
            {
                UpdateCanvasScaler(scalers[i], settings[indexOf]);
            }
        }

        public void UpdateCanvasScaler(CanvasScaler scaler, SetupByAspectRatioCanvasScalerSetting setting)
        {
            if (scaler == null || scaler.uiScaleMode != CanvasScaler.ScaleMode.ScaleWithScreenSize)
                return;
            scaler.screenMatchMode = setting.screenMatchMode;
            scaler.matchWidthOrHeight = setting.matchWidthOrHeight;
#if UNITY_EDITOR
            EditorUtility.SetDirty(scaler);
#endif
        }

#if UNITY_EDITOR
        [ContextMenu("Find All Canvas Scaler And Set To List")]
        public void FindAllCanvasScalerAndSetToList()
        {
            CanvasScaler[] result = FindObjectsOfType<CanvasScaler>(true);
            scalers.Clear();
            scalers.AddRange(result);
            EditorUtility.SetDirty(this);
        }
#endif
    }
}
