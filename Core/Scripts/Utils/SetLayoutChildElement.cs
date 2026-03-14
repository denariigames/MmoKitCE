using UnityEngine;
using UnityEngine.UI;

namespace UtilsComponents
{
    public class SetLayoutChildElement : MonoBehaviour
    {
        public LayoutGroup layoutGroup;

        public void SetChildElement(TextAnchor value)
        {
            layoutGroup.childAlignment = value;
        }

        public void SetUpperLeft()
        {
            SetChildElement(TextAnchor.UpperLeft);
        }
        public void SetUpperCenter()
        {
            SetChildElement(TextAnchor.UpperCenter);
        }
        public void SetUpperRight()
        {
            SetChildElement(TextAnchor.UpperRight);
        }
        public void SetMiddleLeft()
        {
            SetChildElement(TextAnchor.MiddleLeft);
        }
        public void SetMiddleCenter()
        {
            SetChildElement(TextAnchor.MiddleCenter);
        }
        public void SetMiddleRight()
        {
            SetChildElement(TextAnchor.MiddleRight);
        }
        public void SetLowerLeft()
        {
            SetChildElement(TextAnchor.LowerLeft);
        }
        public void SetLowerCenter()
        {
            SetChildElement(TextAnchor.LowerCenter);
        }
        public void SetLowerRight()
        {
            SetChildElement(TextAnchor.LowerRight);
        }
    }
}
