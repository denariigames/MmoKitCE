public class UIGroup : UIBase
{
    public UIBase[] subSet;

    protected override void OnDestroy()
    {
        base.OnDestroy();
        subSet.Nullify();
    }

    public override void Show()
    {
        foreach (UIBase entry in subSet)
        {
            entry.Show();
        }
        base.Show();
    }

    public override void Hide()
    {
        foreach (UIBase entry in subSet)
        {
            entry.Hide();
        }
        base.Hide();
    }
}
