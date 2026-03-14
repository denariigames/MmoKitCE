using UnityEngine;

namespace UtilsComponents
{
    public class CopyTextToClipboard : MonoBehaviour
    {
        public TextWrapper source;
        
        public void CopyToClipboard()
        {
            GUIUtility.systemCopyBuffer = source.text;
        }
    }
}
