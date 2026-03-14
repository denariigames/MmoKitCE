using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerARPG
{
    public class UIBlockController : MonoBehaviour
    {
        internal static HashSet<GameObject> EnabledControllerBlockers = new HashSet<GameObject>();

        public static bool IsBlockController()
        {
            return EnabledControllerBlockers.Count > 0;
        }

        private void OnEnable()
        {
            EnabledControllerBlockers.Add(gameObject);
        }

        private void OnDisable()
        {
            EnabledControllerBlockers.Remove(gameObject);
        }
    }
}
