using Insthync.DevExtension;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class UIBaseEvent : UnityEvent<UIBase>
{

}

public partial class UIBase : MonoBehaviour
{
    public bool hideOnAwake = false;
    public bool moveToLastSiblingOnShow = false;
    public GameObject root;
    public UnityEvent onShow = new UnityEvent();
    public UnityEvent onHide = new UnityEvent();
    public UIBaseEvent onShowWithObject = new UIBaseEvent();
    public UIBaseEvent onHideWithObject = new UIBaseEvent();
    public System.Action<UIBase> overrideShow;
    public System.Action<UIBase> overrideHide;
    public System.Func<UIBase, bool> overrideIsVisible;

    private bool _isAwaken;

    public GameObject CacheRoot
    {
        get
        {
            CacheComponents();
            return root;
        }
    }

    public bool AlreadyCachedComponents { get; private set; }

    protected virtual void Awake()
    {
        if (_isAwaken)
            return;
        _isAwaken = true;

        // Force call events
        bool isVisible = IsVisible();
        if (hideOnAwake && !isVisible)
            CallHideEvents();
        else if (!hideOnAwake && isVisible)
            CallShowEvents();

        if (hideOnAwake)
            Hide();
        else
            Show();
    }

    protected virtual void OnDestroy()
    {
        root = null;
        onShow?.RemoveAllListeners();
        onShow = null;
        onHide?.RemoveAllListeners();
        onHide = null;
        onShowWithObject?.RemoveAllListeners();
        onShowWithObject = null;
        onHideWithObject?.RemoveAllListeners();
        onHideWithObject = null;
    }

    protected virtual void CacheComponents()
    {
        if (AlreadyCachedComponents)
            return;
        if (root == null)
            root = gameObject;
        AlreadyCachedComponents = true;
    }

    public virtual bool IsVisible()
    {
        if (overrideIsVisible != null)
            return overrideIsVisible.Invoke(this);
        return CacheRoot.activeSelf;
    }

    public virtual void Show()
    {
        if (IsVisible())
            return;
        if (!_isAwaken)
            CacheRoot.SetActive(true);
        CacheComponents();
        if (overrideShow != null)
            overrideShow.Invoke(this);
        else
            CacheRoot.SetActive(true);
        if (moveToLastSiblingOnShow)
            CacheRoot.transform.SetAsLastSibling();
        CallShowEvents();
    }

    private void CallShowEvents()
    {
        onShow.Invoke();
        onShowWithObject.Invoke(this);
        OnShow();
        this.InvokeInstanceDevExtMethods("Show");
    }

    public virtual void OnShow()
    {

    }

    public virtual void Hide()
    {
        if (!IsVisible())
            return;
        if (!_isAwaken)
            CacheRoot.SetActive(true);
        CacheComponents();
        if (overrideHide != null)
            overrideHide.Invoke(this);
        else
            CacheRoot.SetActive(false);
        CallHideEvents();
    }

    private void CallHideEvents()
    {
        onHide.Invoke();
        onHideWithObject.Invoke(this);
        OnHide();
        this.InvokeInstanceDevExtMethods("Hide");
    }

    public virtual void OnHide()
    {

    }

    public void SetVisible(bool isVisible)
    {
        if (!isVisible && IsVisible())
            Hide();
        if (isVisible && !IsVisible())
            Show();
    }

    public void Toggle()
    {
        if (IsVisible())
            Hide();
        else
            Show();
    }
}
