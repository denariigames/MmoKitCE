using UnityEngine;

namespace MultiplayerARPG
{
    public class UIVendingSign : MonoBehaviour
    {
        public GameObject uiRoot;
        public TextWrapper textTitle;

        private BasePlayerCharacterEntity _entity;
        private bool _isStarted = false;

        private void Start()
        {
            _isStarted = true;
            _entity = GetComponentInParent<BasePlayerCharacterEntity>();
            if (_entity == null || _entity.VendingComponent == null)
                return;
            _entity.VendingComponent.onVendingDataChange += UpdateUI;
            UpdateUI(_entity.VendingComponent.Data);
        }

        private void OnEnable()
        {
            if (!_isStarted)
                return;
            if (_entity == null || _entity.VendingComponent == null)
                return;
            _entity.VendingComponent.onVendingDataChange += UpdateUI;
            UpdateUI(_entity.VendingComponent.Data);
        }

        private void OnDisable()
        {
            if (_entity == null || _entity.VendingComponent == null)
                return;
            _entity.VendingComponent.onVendingDataChange -= UpdateUI;
        }

        public void UpdateUI(VendingData data)
        {
            if (uiRoot != null)
                uiRoot.SetActive(data.isStarted);
            if (textTitle != null)
                textTitle.text = data.title;
        }

        public void OnClickVending()
        {
            BaseUISceneGameplay.Singleton.ShowVending(_entity);
        }
    }
}