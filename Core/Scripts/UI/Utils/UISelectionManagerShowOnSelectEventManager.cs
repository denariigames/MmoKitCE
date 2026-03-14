public class UISelectionManagerShowOnSelectEventManager<TData, TUI>
    where TUI : UISelectionEntry<TData>
{
    private UISelectionManager<TData, TUI> _selectionManager;
    private TUI _showingUI;

    /// <summary>
    /// This function should be called on UI component is enable to set events
    /// </summary>
    /// <param name="selectionManager"></param>
    /// <param name="showingUI"></param>
    public void OnEnable(UISelectionManager<TData, TUI> selectionManager, TUI showingUI)
    {
        _selectionManager = selectionManager;
        _showingUI = showingUI;

        // Set events
        if (_selectionManager != null)
        {
            _selectionManager.eventOnSelect.RemoveListener(OnSelect);
            _selectionManager.eventOnSelect.AddListener(OnSelect);
            _selectionManager.eventOnDeselect.RemoveListener(OnDeselect);
            _selectionManager.eventOnDeselect.AddListener(OnDeselect);
        }
        if (_showingUI != null)
        {
            _showingUI.onHide.RemoveListener(OnHide);
            _showingUI.onHide.AddListener(OnHide);
        }
    }

    /// <summary>
    /// This function should be called on UI component is disable to remove and hide UIs
    /// </summary>
    public void OnDisable()
    {
        // Remove events
        if (_showingUI != null)
            _showingUI.onHide.RemoveListener(OnHide);
        if (_selectionManager != null)
            _selectionManager.DeselectSelectedUI();
    }

    private void OnSelect(TUI ui)
    {
        if (_showingUI != null)
        {
            _showingUI.selectionManager = _selectionManager;
            ui.CloneTo(_showingUI);
            _showingUI.Show();
        }
    }

    private void OnDeselect(TUI ui)
    {
        if (_showingUI != null)
        {
            _showingUI.onHide.RemoveListener(OnHide);
            _showingUI.Hide();
            _showingUI.onHide.AddListener(OnHide);
        }
    }

    private void OnHide()
    {
        _selectionManager.DeselectSelectedUI();
    }
}
