public interface IUISelectionEntry
{
    bool IsSelected { get; }
    void ForceUpdate();
    void SetData(object data);
    object GetData();
}
