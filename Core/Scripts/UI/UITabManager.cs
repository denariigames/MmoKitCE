using Insthync.CameraAndInput;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ToggleGroup))]
public class UITabManager : UIBase
{
    [System.Serializable]
    public struct TabData
    {
        public UIBase uiContent;
        public Toggle toggle;
    }

    public TabData[] tabs;
    public string prevTabButtonName = "TabLeft";
    public string nextTabButtonName = "TabRight";
    public int currentTabIndex = 0;
    protected ToggleGroup _toggleGroup;
    protected bool _alreadySetup;

    protected override void Awake()
    {
        SetupToggles();
        base.Awake();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        _toggleGroup = null;
    }

    protected void SetupToggles()
    {
        if (_alreadySetup)
            return;
        _alreadySetup = true;
        _toggleGroup = GetComponent<ToggleGroup>();
        for (int i = 0; i < tabs.Length; ++i)
        {
            int index = i;
            tabs[index].toggle.onValueChanged.AddListener(tabs[index].uiContent.SetVisible);
            tabs[index].toggle.group = _toggleGroup;
        }
    }

    public override void OnShow()
    {
        SetupToggles();
        base.OnShow();
        ShowTab(0, true);
    }

    public void ShowTab(int index, bool force = false)
    {
        if (index < 0)
            index = tabs.Length - 1;
        if (index >= tabs.Length)
            index = 0;
        currentTabIndex = index;
        if (force)
        {
            for (int i = 0; i < tabs.Length; ++i)
            {
                tabs[i].toggle.SetIsOnWithoutNotify(i == index);
                tabs[i].uiContent.SetVisible(i == index);
            }
        }
        else
        {
            tabs[index].toggle.isOn = true;
        }
    }

    protected virtual void Update()
    {
        if (InputManager.GetButtonDown(prevTabButtonName))
        {
            ShowTab(--currentTabIndex);
        }
        if (InputManager.GetButtonDown(nextTabButtonName))
        {
            ShowTab(++currentTabIndex);
        }
    }
}
