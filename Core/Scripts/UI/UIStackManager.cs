using Insthync.CameraAndInput;
using System.Collections.Generic;
using UnityEngine;

public class UIStackManager : MonoBehaviour
{
    public string closeButtonName = "CloseUI";
    private static Stack<UIStackEntry> s_entries = new Stack<UIStackEntry>();

    private void Awake()
    {
        Clear();
    }

    private void Update()
    {
        if (InputManager.GetButtonDown(closeButtonName))
        {
            UIStackEntry entry;
            while (s_entries.Count > 0)
            {
                entry = s_entries.Pop();
                if (entry != null)
                {
                    entry.Hide();
                    break;
                }
            }
        }
    }

    public void Clear()
    {
        s_entries.Clear();
    }

    public static void Add(UIStackEntry entry)
    {
        s_entries.Push(entry);
    }
}
