using Insthync.UnityEditorUtils;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UtilsComponents
{
    [System.Serializable]
    public class SetupByAspectRatioTransformSetting : SetupByAspectRatioSetting
    {
        public bool setParent = false;
        public Transform parent = null;
        public bool setAsFirstSibling = false;
        public bool setAsLastSibling = false;
        public bool setSiblingIndex = false;
        public int siblingIndex = 0;
        public bool setAnchoredPosition;
        public Vector2 anchoredPosition = Vector2.zero;
        public Vector3 position = Vector3.zero;
        public Vector3 rotation = Vector3.zero;
        public Vector3 scale = Vector3.one;
    }

    [ExecuteInEditMode]
    public class TransformSetupByAspectRatio : BaseSetupByAspectRatio<SetupByAspectRatioTransformSetting>
    {
#if UNITY_EDITOR
        [InspectorButton(nameof(SaveCurrentSetting))]
        public bool saveCurrentSetting;
#endif

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
            if (settings[indexOf].setParent)
                transform.parent = settings[indexOf].parent;
            if (settings[indexOf].setAsFirstSibling)
                transform.SetAsFirstSibling();
            if (settings[indexOf].setAsLastSibling)
                transform.SetAsLastSibling();
            if (settings[indexOf].setSiblingIndex)
                transform.SetSiblingIndex(settings[indexOf].siblingIndex);
            transform.localPosition = settings[indexOf].position;
            transform.localEulerAngles = settings[indexOf].rotation;
            transform.localScale = settings[indexOf].scale;
            if (settings[indexOf].setAnchoredPosition)
                (transform as RectTransform).anchoredPosition = settings[indexOf].anchoredPosition;
        }

#if UNITY_EDITOR
        public void SaveCurrentSetting()
        {
            int aspect = GetCurrentAspectAsInt();
            int indexOf = IndexOfAspect(aspect);
            if (indexOf >= 0)
            {
                settings[indexOf] = new SetupByAspectRatioTransformSetting()
                {
                    width = settings[indexOf].width,
                    height = settings[indexOf].height,
                    setParent = settings[indexOf].setParent,
                    parent = settings[indexOf].parent,
                    setSiblingIndex = settings[indexOf].setSiblingIndex,
                    siblingIndex = settings[indexOf].siblingIndex,
                    setAnchoredPosition = settings[indexOf].setAnchoredPosition,
                    anchoredPosition = transform is RectTransform rect ? rect.anchoredPosition : Vector2.zero,
                    position = transform.localPosition,
                    rotation = transform.localEulerAngles,
                    scale = transform.localScale,
                };
            }
            else
            {
                settings.Add(new SetupByAspectRatioTransformSetting()
                {
                    width = currentWidth,
                    height = currentHeight,
                    setParent = false,
                    parent = transform.parent,
                    setSiblingIndex = false,
                    siblingIndex = transform.GetSiblingIndex(),
                    setAnchoredPosition = false,
                    anchoredPosition = transform is RectTransform rect ? rect.anchoredPosition : Vector2.zero,
                    position = transform.localPosition,
                    rotation = transform.localEulerAngles,
                    scale = transform.localScale,
                });
            }
            settings.Sort();
            EditorUtility.SetDirty(this);
        }
#endif
    }
}