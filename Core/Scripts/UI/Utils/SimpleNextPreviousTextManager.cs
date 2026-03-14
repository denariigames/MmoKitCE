using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace UtilsComponents
{
    public class SimpleNextPreviousTextManager : MonoBehaviour
    {
        [System.Serializable]
        public struct Settings
        {
            [TextArea]
            public string text;
            public UnityEvent onEnter;
        }

        public TextWrapper textComponent;
        public Settings[] settings = new Settings[0];
        public int index = 0;

        private void Awake()
        {
            SetIndex(index);
        }

        public void SetIndex(int index)
        {
            this.index = index;
            textComponent.text = settings[index].text;
            settings[index].onEnter.Invoke();
        }

        public void Next()
        {
            index++;
            if (index > settings.Length - 1)
                index = settings.Length - 1;
            SetIndex(index);
        }

        public void Previous()
        {
            index--;
            if (index < 0)
                index = 0;
            SetIndex(index);
        }
    }
}
