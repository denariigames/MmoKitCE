using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class UISelectionEntryImageSprite : MonoBehaviour
{
    [System.Serializable]
    public struct Setting
    {
        public Image image;
        public Sprite defaultSprite;
        public Sprite selectedSprite;
    }

    public Setting[] settings;
    private IUISelectionEntry _entry;
    private bool _dirtySelected;

    private void Awake()
    {
        _entry = GetComponent<IUISelectionEntry>();
    }

    private void OnEnable()
    {
        _dirtySelected = false;
        foreach (Setting setting in settings)
        {
            setting.image.sprite = setting.defaultSprite;
        }
    }

    private void Update()
    {
        if (_entry == null)
            return;

        if (_dirtySelected != _entry.IsSelected)
        {
            _dirtySelected = _entry.IsSelected;
            foreach (Setting setting in settings)
            {
                setting.image.sprite = _dirtySelected ? setting.selectedSprite : setting.defaultSprite;
            }
        }
    }

    [ContextMenu("Set default color by image's color")]
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

    [ContextMenu("Swap default color and selected color")]
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
