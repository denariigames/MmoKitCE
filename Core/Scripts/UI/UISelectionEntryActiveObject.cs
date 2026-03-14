using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class UISelectionEntryActiveObject : MonoBehaviour
{
    [System.Serializable]
    public struct Setting
    {
        public GameObject defaultObject;
        public GameObject selectedObject;
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
            setting.defaultObject.SetActive(true);
            setting.selectedObject.SetActive(false);
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
                setting.defaultObject.SetActive(!_dirtySelected);
                setting.selectedObject.SetActive(_dirtySelected);
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
