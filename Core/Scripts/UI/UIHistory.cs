using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIHistory : MonoBehaviour
{
    /// <summary>
    /// args: Which UI?, Is First UI?
    /// </summary>
    [System.Serializable]
    public class BackEvent : UnityEvent<UIBase, bool>
    {
    }

    /// <summary>
    /// args: Which UI?
    /// </summary>
    [System.Serializable]
    public class NextEvent : UnityEvent<UIBase>
    {
    }

    public UIBase firstUI;
    public BackEvent onBack = new BackEvent();
    public NextEvent onNext = new NextEvent();
    protected readonly Stack<UIBase> _uiStack = new Stack<UIBase>();

    protected virtual void Awake()
    {
        if (firstUI != null)
            firstUI.Show();
    }

    public void Next(UIBase ui)
    {
        if (ui == null)
            return;
        // Hide latest ui
        if (_uiStack.Count > 0)
            _uiStack.Peek().Hide();
        else if (firstUI != null)
            firstUI.Hide();

        _uiStack.Push(ui);
        ui.Show();
        onNext.Invoke(ui);
    }

    public void Back()
    {
        // Remove current ui from stack
        if (_uiStack.Count > 0)
        {
            UIBase ui = _uiStack.Pop();
            ui.Hide();
        }
        // Show recent ui
        if (_uiStack.Count > 0)
        {
            UIBase ui = _uiStack.Peek();
            ui.Show();
            onBack.Invoke(ui, false);
        }
        else if (firstUI != null)
        {
            firstUI.Show();
            onBack.Invoke(firstUI, true);
        }
    }

    public void ClearHistory()
    {
        while (_uiStack.Count > 0)
        {
            UIBase ui = _uiStack.Pop();
            ui.Hide();
        }
        _uiStack.Clear();
        if (firstUI != null)
            firstUI.Show();
    }
}
