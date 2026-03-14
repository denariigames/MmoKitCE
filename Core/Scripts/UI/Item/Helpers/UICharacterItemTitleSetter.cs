using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MultiplayerARPG
{
    public class UICharacterItemTitleSetter : MonoBehaviour
    {
#if UNITY_EDITOR
        [ContextMenu("Set Title")]
        public void SetTitle()
        {
            TextWrapper text = GetComponentInChildren<TextWrapper>();
            UICharacterItem ui = GetComponentInParent<UICharacterItem>();
            if (text && ui)
            {
                ui.uiTextTitle = text;
                EditorUtility.SetDirty(ui);
            }
        }
#endif
    }
}
