using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UtilsComponents
{
    public class SimplePageManager : MonoBehaviour
    {
        public UIBase[] pages = new UIBase[0];
        private int previousIndex;

        public void HideAll()
        {
            foreach (UIBase page in pages)
            {
                page?.Hide();
            }
        }

        public void GoTo(int index)
        {
            HideAll();
            previousIndex = index;
            pages[index].Show();
        }

        public void GoBack()
        {
            GoTo(previousIndex);
        }
    }
}
