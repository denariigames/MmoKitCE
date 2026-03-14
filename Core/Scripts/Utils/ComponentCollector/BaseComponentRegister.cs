using UnityEngine;

public class BaseComponentRegister<T> : MonoBehaviour where T : Component
{
    T[] _components;

    private void Awake()
    {
        _components = GetComponents<T>();
        foreach (T component in _components)
        {
            ComponentCollector.Add(component);
        }
    }

    private void OnDestroy()
    {
        if (_components == null)
            return;
        foreach (T component in _components)
        {
            ComponentCollector.Remove(component);
        }
    }
}
