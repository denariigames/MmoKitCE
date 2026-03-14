using UnityEngine;
using UnityEngine.UI;

public class UIShowHideButtons : MonoBehaviour
{
    public UIBase ui;
    public Button btnShow;
    public Button btnHide;

    void Awake()
    {
        if (ui != null && btnShow != null)
            btnShow.onClick.AddListener(ui.Show);

        if (ui != null && btnHide != null)
            btnHide.onClick.AddListener(ui.Hide);
    }

    void OnDestroy()
    {
        if (ui != null && btnShow != null)
            btnShow.onClick.RemoveListener(ui.Show);

        if (ui != null && btnHide != null)
            btnHide.onClick.RemoveListener(ui.Hide);
    }
}
