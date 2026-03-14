using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum UISelectionMode
{
    SelectSingle,
    Toggle,
    SelectMultiple,
}

public abstract class UISelectionManager : MonoBehaviour
{
    public UISelectionMode selectionMode;
    public bool interactable = true;
    public abstract object GetSelectedUI();
    public abstract void Select(int index);
    public abstract void Deselect(int index);
    public abstract void Select(object ui);
    public abstract void Deselect(object ui);
    public abstract void DeselectAll();
    public abstract void DeselectSelectedUI();
    public abstract bool Contains(object ui);
}

public abstract class UISelectionManager<TData, TUI> : UISelectionManager
    where TUI : UISelectionEntry<TData>
{
    [System.Serializable]
    public class DataEvent : UnityEvent<TData>
    {

    }

    [System.Serializable]
    public class UIEvent : UnityEvent<TUI>
    {

    }

    public UIEvent eventOnSelect = new UIEvent();
    public UIEvent eventOnSelected = new UIEvent();
    public UIEvent eventOnDeselect = new UIEvent();
    public UIEvent eventOnDeselected = new UIEvent();

    public DataEvent eventOnDataSelect = new DataEvent();
    public DataEvent eventOnDataSelected = new DataEvent();
    public DataEvent eventOnDataDeselect = new DataEvent();
    public DataEvent eventOnDataDeselected = new DataEvent();

    protected readonly List<TUI> uis = new List<TUI>();
    public TUI SelectedUI { get; protected set; }
    public int SelectedIndex { get { return IndexOf(SelectedUI); } }

    public void Add(TUI ui)
    {
        if (ui == null)
            return;

        ui.selectionManager = this;
        // Select first ui
        if (uis.Count == 0 && selectionMode == UISelectionMode.Toggle)
            Select(ui);
        else
            ui.Deselect();

        uis.Add(ui);
    }

    public int IndexOf(TUI ui)
    {
        if (ui == null)
            return -1;
        return uis.IndexOf(ui);
    }

    public TUI Get(int index)
    {
        if (index < 0 || index >= uis.Count)
            return null;
        return uis[index];
    }

    public bool Remove(TUI ui)
    {
        return uis.Remove(ui);
    }

    public int Count
    {
        get { return uis.Count; }
    }

    public void Clear()
    {
        uis.Clear();
        SelectedUI = null;
    }

    public override sealed object GetSelectedUI()
    {
        return SelectedUI;
    }

    public List<TUI> GetAllUIs()
    {
        List<TUI> result = new List<TUI>();
        foreach (TUI ui in uis)
        {
            result.Add(ui);
        }
        return result;
    }

    public override void Select(int index)
    {
        TUI ui = Get(index);
        Select(ui);
    }

    public override sealed void Select(object ui)
    {
        if (!interactable)
            return;

        if (ui == null)
            return;

        TUI castedUI = ui as TUI;
        castedUI.Select();

        if (eventOnSelect != null)
            eventOnSelect.Invoke(castedUI);

        if (eventOnDataSelect != null)
            eventOnDataSelect.Invoke(castedUI.Data);

        SelectedUI = castedUI;
        if (selectionMode != UISelectionMode.SelectMultiple)
        {
            foreach (TUI deselectUI in uis)
            {
                if (deselectUI != castedUI)
                    deselectUI.Deselect();
            }
        }

        if (eventOnSelected != null)
            eventOnSelected.Invoke(castedUI);

        if (eventOnDataSelected != null)
            eventOnDataSelected.Invoke(castedUI.Data);
    }

    public override void Deselect(int index)
    {
        TUI ui = Get(index);
        Deselect(ui);
    }

    public override sealed void Deselect(object ui)
    {
        if (!interactable)
            return;

        if (ui == null)
            return;

        TUI castedUI = (TUI)ui;

        if (eventOnDeselect != null)
            eventOnDeselect.Invoke(castedUI);

        if (eventOnDataDeselect != null)
            eventOnDataDeselect.Invoke(castedUI.Data);

        SelectedUI = null;
        castedUI.Deselect();

        if (eventOnDeselected != null)
            eventOnDeselected.Invoke(castedUI);

        if (eventOnDataDeselected != null)
            eventOnDataDeselected.Invoke(castedUI.Data);
    }

    public List<TUI> GetSelectedUIs()
    {
        List<TUI> result = new List<TUI>();
        foreach (TUI ui in uis)
        {
            if (ui.IsSelected)
                result.Add(ui);
        }
        return result;
    }

    public override sealed void DeselectAll()
    {
        if (!interactable)
            return;

        SelectedUI = null;
        foreach (TUI deselectUI in uis)
        {
            deselectUI.Deselect();
        }
    }

    public override sealed void DeselectSelectedUI()
    {
        if (!interactable)
            return;

        if (SelectedUI != null)
            Deselect(SelectedUI);
    }

    public override bool Contains(object ui)
    {
        return ui is TUI && uis.Contains(ui as TUI);
    }
}

public abstract class UISelectionManager<TData, TUI, TDataEvent, TUIEvent> : UISelectionManager
    where TUI : UISelectionEntry<TData>
    where TDataEvent : UnityEvent<TData>, new()
    where TUIEvent : UnityEvent<TUI>, new()
{
    public TUIEvent eventOnSelect = new TUIEvent();
    public TUIEvent eventOnSelected = new TUIEvent();
    public TUIEvent eventOnDeselect = new TUIEvent();
    public TUIEvent eventOnDeselected = new TUIEvent();

    public TDataEvent eventOnDataSelect = new TDataEvent();
    public TDataEvent eventOnDataSelected = new TDataEvent();
    public TDataEvent eventOnDataDeselect = new TDataEvent();
    public TDataEvent eventOnDataDeselected = new TDataEvent();

    protected readonly List<TUI> uis = new List<TUI>();
    public TUI SelectedUI { get; protected set; }
    public int SelectedIndex { get { return IndexOf(SelectedUI); } }

    public void Add(TUI ui)
    {
        if (ui == null)
            return;

        ui.selectionManager = this;
        // Select first ui
        if (uis.Count == 0 && selectionMode == UISelectionMode.Toggle)
            Select(ui);
        else
            ui.Deselect();

        uis.Add(ui);
    }

    public int IndexOf(TUI ui)
    {
        if (ui == null)
            return -1;
        return uis.IndexOf(ui);
    }

    public TUI Get(int index)
    {
        if (index < 0 || index >= uis.Count)
            return null;
        return uis[index];
    }

    public bool Remove(TUI ui)
    {
        return uis.Remove(ui);
    }

    public int Count
    {
        get { return uis.Count; }
    }

    public void Clear()
    {
        uis.Clear();
        SelectedUI = null;
    }

    public override sealed object GetSelectedUI()
    {
        return SelectedUI;
    }

    public override void Select(int index)
    {
        TUI ui = Get(index);
        Select(ui);
    }

    public override sealed void Select(object ui)
    {
        if (!interactable)
            return;

        if (ui == null)
            return;

        TUI castedUI = ui as TUI;
        castedUI.Select();

        if (eventOnSelect != null)
            eventOnSelect.Invoke(castedUI);

        if (eventOnDataSelect != null)
            eventOnDataSelect.Invoke(castedUI.Data);

        SelectedUI = castedUI;
        if (selectionMode != UISelectionMode.SelectMultiple)
        {
            foreach (TUI deselectUI in uis)
            {
                if (deselectUI != castedUI)
                    deselectUI.Deselect();
            }
        }

        if (eventOnSelected != null)
            eventOnSelected.Invoke(castedUI);

        if (eventOnDataSelected != null)
            eventOnDataSelected.Invoke(castedUI.Data);
    }

    public override void Deselect(int index)
    {
        TUI ui = Get(index);
        Deselect(ui);
    }

    public override sealed void Deselect(object ui)
    {
        if (!interactable)
            return;

        if (ui == null)
            return;

        TUI castedUI = (TUI)ui;

        if (eventOnDeselect != null)
            eventOnDeselect.Invoke(castedUI);

        if (eventOnDataDeselect != null)
            eventOnDataDeselect.Invoke(castedUI.Data);

        SelectedUI = null;
        castedUI.Deselect();

        if (eventOnDeselected != null)
            eventOnDeselected.Invoke(castedUI);

        if (eventOnDataDeselected != null)
            eventOnDataDeselected.Invoke(castedUI.Data);
    }

    public List<TUI> GetSelectedUIs()
    {
        List<TUI> result = new List<TUI>();
        foreach (TUI ui in uis)
        {
            if (ui.IsSelected)
                result.Add(ui);
        }
        return result;
    }

    public List<TUI> GetAllUIs()
    {
        List<TUI> result = new List<TUI>();
        foreach (TUI ui in uis)
        {
            result.Add(ui);
        }
        return result;
    }

    public override sealed void DeselectAll()
    {
        if (!interactable)
            return;

        SelectedUI = null;
        foreach (TUI deselectUI in uis)
        {
            deselectUI.Deselect();
        }
    }

    public override sealed void DeselectSelectedUI()
    {
        if (!interactable)
            return;

        if (SelectedUI != null)
            Deselect(SelectedUI);
    }

    public override bool Contains(object ui)
    {
        return ui is TUI && uis.Contains(ui as TUI);
    }
}
