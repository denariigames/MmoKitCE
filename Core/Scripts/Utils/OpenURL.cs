using UnityEngine;

namespace UtilsComponents
{
    public class OpenURL : MonoBehaviour
    {
        public string url;
        public void Open()
        {
            Open(url);
        }

        public void Open(string url)
        {
            Application.OpenURL(url);
        }
    }
}
