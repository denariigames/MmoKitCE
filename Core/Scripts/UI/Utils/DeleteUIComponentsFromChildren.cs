using Insthync.CameraAndInput;
using UnityEngine;

namespace MultiplayerARPG
{
    public class DeleteUIComponentsFromChildren : MonoBehaviour
    {
        [ContextMenu("Delete UI Components")]
        public void DeleteUIComponents()
        {
            UIBase[] uis = gameObject.GetComponentsInChildren<UIBase>(true);
            IMobileInputArea[] mobileInputs = gameObject.GetComponentsInChildren<IMobileInputArea>(true);
            for (int i = uis.Length - 1; i >= 0; --i)
            {
                DestroyImmediate(uis[i]);
            }
            for (int i = mobileInputs.Length - 1; i >= 0; --i)
            {
                if (mobileInputs[i] is Component comp)
                    DestroyImmediate(comp);
            }
        }
    }
}
