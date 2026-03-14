using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MultiplayerARPG
{
    [RequireComponent(typeof(Selectable))]
    public class NavigationChild : MonoBehaviour
    {
        public NavigationGroup group;
        public bool shouldBeFirstSelected;
        public bool vertical = true;
        public bool horizontal = true;
        protected int _dirtySiblingIndex;

        public Selectable Selectable { get; private set; }

        private void Awake()
        {
            Selectable = GetComponent<Selectable>();
        }

        private void Start()
        {
            if (group == null)
                group = GetComponentInParent<NavigationGroup>();
            if (group != null)
                group.AddChild(this);
        }

        private void Update()
        {
            if (group != null)
            {
                if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject == gameObject)
                    group.SetLastSelectedChild(this);
                if (_dirtySiblingIndex != transform.GetSiblingIndex())
                {
                    _dirtySiblingIndex = transform.GetSiblingIndex();
                    group.forceUpdateChildNavigation = true;
                }
            }
        }

        private void OnDestroy()
        {
            if (group != null)
                group.RemoveChild(this);
            group = null;
            Selectable = null;
        }
    }
}
