using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerARPG
{
    [System.Serializable]
    public struct UIItemTextPair
    {
        public BaseItem item;
        public TextWrapper uiText;
        public Image imageIcon;
        public GameObject root;
    }
}
