using UnityEngine;

public class UIVisibleFollowShownUIs : MonoBehaviour
{
    public UIBase[] conditionUIs;
    public UIBase[] showingUIs;

    private void Update()
    {
        bool show = false;
        for (int i = 0; i < conditionUIs.Length; ++i)
        {
            if (conditionUIs[i].IsVisible())
            {
                show = true;
                break;
            }
        }
        for (int i = 0; i < showingUIs.Length; ++i)
        {
            showingUIs[i].SetVisible(show);
        }
    }
}
