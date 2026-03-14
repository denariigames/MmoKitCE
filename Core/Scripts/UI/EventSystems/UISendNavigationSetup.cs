using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MultiplayerARPG
{
    public class UISendNavigationSetup : MonoBehaviour
    {
        internal static List<GameObject> EnabledComponents = new List<GameObject>();
        protected bool cannotFindEventSystem = false;

        public static bool IsBlockController()
        {
            return EnabledComponents.Count > 0;
        }

        private void OnEnable()
        {
            EnabledComponents.Add(gameObject);
            UpdateSendNavigationEvents();
        }

        private void OnDisable()
        {
            EnabledComponents.Remove(gameObject);
            UpdateSendNavigationEvents();
        }

        private void Update()
        {
            if (!cannotFindEventSystem)
                return;
            UpdateSendNavigationEvents();
        }

        public void UpdateSendNavigationEvents()
        {
            if (EventSystem.current == null)
            {
                cannotFindEventSystem = true;
                return;
            }
            for (int i = EnabledComponents.Count - 1; i >= 0; --i)
            {
                if (EnabledComponents[i] == null)
                    EnabledComponents.RemoveAt(i);
            }
            EventSystem.current.sendNavigationEvents = EnabledComponents.Count > 0;
        }
    }
}
