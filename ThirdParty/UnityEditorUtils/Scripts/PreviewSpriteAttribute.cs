using UnityEngine;

namespace Insthync.UnityEditorUtils
{
    public class PreviewSpriteAttribute : PropertyAttribute
    {
        public int height { get; private set; }
        public PreviewSpriteAttribute(int height = 100)
        {
            this.height = height;
        }
    }
}
