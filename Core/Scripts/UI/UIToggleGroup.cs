using UnityEngine;

public class UIToggleGroup : MonoBehaviour
{
    public UIBase[] uis;

    private void OnEnable()
    {
        foreach (UIBase ui in uis)
        {
            ui.onShowWithObject.AddListener(OnShow);
        }
    }

    private void OnDisable()
    {
        foreach (UIBase ui in uis)
        {
            ui.onShowWithObject.RemoveListener(OnShow);
        }
    }

    private void OnShow(UIBase showUI)
    {
        foreach (UIBase ui in uis)
        {
            if (ui == showUI) continue;
            ui.Hide();
        }
    }
}
