using UnityEngine;

public class VersionText : MonoBehaviour
{
    public TextWrapper uiText;
    public string format = "Ver.{0}";

    private void Start()
    {
        uiText.text = string.Format(format, Application.version);
    }
}
