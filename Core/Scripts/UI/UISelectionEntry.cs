using Insthync.DevExtension;
using Insthync.ManagedUpdating;
using UnityEngine;

public abstract class UISelectionEntry<T> : UIBase, IUISelectionEntry, IManagedUpdate
{
    [Header("UI Selection Elements")]
    public GameObject objectSelected;
    protected T _data;
    public T Data
    {
        get { return _data; }
        set
        {
            _data = value;
            ForceUpdate();
        }
    }
    [Tooltip("UIs set here will be cloned by this UI")]
    public UISelectionEntry<T>[] clones = new UISelectionEntry<T>[0];
    public UISelectionManager selectionManager;
    public float updateUIRepeatRate = 0.5f;
    protected float _updateCountDown;
    private bool _isSelected;
    public bool IsSelected
    {
        get { return _isSelected; }
        protected set
        {
            _isSelected = value;
            if (objectSelected != null)
                objectSelected.SetActive(value);
        }
    }

    public System.Action onUpdateUI;
    public System.Action<T> onUpdateData;

    protected override void Awake()
    {
        base.Awake();
        IsSelected = false;
        _updateCountDown = 0f;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        objectSelected = null;
        clones.Nullify();
        selectionManager = null;
        onUpdateUI = null;
        onUpdateData = null;
    }

    protected virtual void OnEnable()
    {
        UpdateUI();
        UpdateManager.Register(this);
    }

    protected virtual void OnDisable()
    {
        _updateCountDown = 0f;
        UpdateManager.Unregister(this);
    }

    public virtual void ManagedUpdate()
    {
        _updateCountDown -= Time.deltaTime;
        if (_updateCountDown > 0f)
            return;
        _updateCountDown = updateUIRepeatRate;
        UpdateUI();
        if (onUpdateUI != null)
            onUpdateUI.Invoke();
        this.InvokeInstanceDevExtMethods("UpdateUI");
    }

    public void ForceUpdate()
    {
        UpdateData();
        UpdateUI();
        for (int i = 0; i < clones.Length; ++i)
        {
            CloneTo(clones[i]);
        }
        if (onUpdateData != null)
            onUpdateData.Invoke(Data);
        this.InvokeInstanceDevExtMethods("UpdateData");
    }

    public void OnClickSelect()
    {
        if (selectionManager != null)
        {
            UISelectionMode selectionMode = selectionManager.selectionMode;
            if (selectionMode != UISelectionMode.Toggle && IsSelected)
                selectionManager.Deselect(this);
            else if (!IsSelected)
                selectionManager.Select(this);
        }
    }

    public void SelectByManager()
    {
        if (selectionManager != null)
            selectionManager.Select(this);
    }

    public void DeselectByManager()
    {
        if (selectionManager != null)
            selectionManager.Deselect(this);
    }

    public void Select()
    {
        IsSelected = true;
    }

    public void Deselect()
    {
        IsSelected = false;
    }

    public void SetData(object data)
    {
        if (data is T)
            Data = (T)data;
    }

    public object GetData()
    {
        return Data;
    }

    public virtual void CloneTo(UISelectionEntry<T> target)
    {
        if (target != null)
            target.Data = Data;
    }

    protected virtual void UpdateUI() { }
    protected abstract void UpdateData();
}
