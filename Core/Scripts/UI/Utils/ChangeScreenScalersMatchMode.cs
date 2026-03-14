using Insthync.UnityEditorUtils;
using UnityEngine;
using UnityEngine.UI;

namespace UtilsComponents
{
    public class ChangeScreenScalersMatchMode : MonoBehaviour
    {
        public CanvasScaler.ScreenMatchMode matchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        public float matchWidthOrHeight = 1f;

        [InspectorButton(nameof(Apply))]
        public bool apply;

        public void Apply()
        {
            CanvasScaler[] scalers = GetComponentsInChildren<CanvasScaler>(true);
            for (int i = 0; i < scalers.Length; ++i)
            {
                scalers[i].screenMatchMode = matchMode;
                scalers[i].matchWidthOrHeight = matchWidthOrHeight;
            }
        }
    }
}
