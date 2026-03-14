using UnityEngine;

namespace UtilsComponents
{
    public class ActivateObjectByNumber : MonoBehaviour
    {
        [System.Serializable]
        public struct Data
        {
            public int number;
            public GameObject obj;
        }

        public Data[] list = new Data[0];

        public void Activate(int number)
        {
            foreach (var entry in list)
            {
                if (entry.obj != null)
                    entry.obj.SetActive(entry.number == number);
            }
        }
    }
}
