using UnityEngine;

[RequireComponent(typeof(UIBase))]
public class UIStackEntry : MonoBehaviour
{
    private UIBase _ui;

    private void Awake()
    {
        _ui = GetComponent<UIBase>();
    }

    private void OnEnable()
    {
        UIStackManager.Add(this);
    }

    public void Hide()
    {
        _ui.Hide();
    }
}
