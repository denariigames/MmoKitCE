using Insthync.UnityEditorUtils;
using LiteNetLibManager;
using UnityEngine;
#if ENABLE_PURCHASING && (UNITY_IOS || UNITY_ANDROID)
using UnityEngine.Purchasing;
#endif

namespace MultiplayerARPG
{
    [CreateAssetMenu(fileName = GameDataMenuConsts.CASH_PACKAGE_FILE, menuName = GameDataMenuConsts.CASH_PACKAGE_MENU, order = GameDataMenuConsts.CASH_PACKAGE_ORDER)]
    public partial class CashPackage : BaseGameData
    {
        [Category("Cash Package Settings")]
        [SerializeField]
        private string externalIconUrl = string.Empty;
        public string ExternalIconUrl { get { return externalIconUrl; } }

        [SerializeField]
        private int cashAmount = 0;
        public int CashAmount { get { return cashAmount; } }

        [HideInInspector]
        [SerializeField]
        private string productId = string.Empty;
        public string ProductId { get { return productId; } }

        public override string Id { get { return productId; } }

#if ENABLE_PURCHASING && (UNITY_IOS || UNITY_ANDROID)
        public ProductCatalogItem ProductCatalogItem
        {
            get
            {
                var catalog = ProductCatalog.LoadDefaultCatalog();
                foreach (var item in catalog.allProducts)
                {
                    if (item.id.Equals(productId))
                        return item;
                }
                return null;
            }
        }

        public Product ProductData
        {
            get
            {
                if (GameInstance.StoreController == null || GameInstance.StoreController.products == null)
                    return null;
                return GameInstance.StoreController.products.WithID(productId);
            }
        }

        public ProductMetadata Metadata
        {
            get
            {
                if (ProductData == null)
                    return null;
                return ProductData.metadata;
            }
        }
#endif

        public string GetTitle()
        {
#if ENABLE_PURCHASING && (UNITY_IOS || UNITY_ANDROID)
            if (ProductCatalogItem == null)
                return LanguageManager.GetUnknowTitle();
            var title = ProductCatalogItem.defaultDescription.Title;
            if (Metadata != null && !string.IsNullOrEmpty(Metadata.localizedTitle))
                title = Metadata.localizedTitle;
            return title;
#else
            Logging.LogWarning(ToString(), "Cannot get IAP product title, Unity Purchasing is not enabled.");
            return LanguageManager.GetUnknowTitle();
#endif
        }

        public string GetDescription()
        {
#if ENABLE_PURCHASING && (UNITY_IOS || UNITY_ANDROID)
            if (ProductCatalogItem == null)
                return "";
            var description = ProductCatalogItem.defaultDescription.Description;
            if (Metadata != null && !string.IsNullOrEmpty(Metadata.localizedDescription))
                description = Metadata.localizedDescription;
            return description;
#else
            Logging.LogWarning(ToString(), "Cannot get IAP product description, Unity Purchasing is not enabled.");
            return "";
#endif
        }

        public string GetSellPrice()
        {
#if ENABLE_PURCHASING && (UNITY_IOS || UNITY_ANDROID)
            if (ProductCatalogItem == null || Metadata == null)
                return LanguageManager.GetUnknowDescription();
            return Metadata.localizedPrice.ToString("N0") + " " + Metadata.isoCurrencyCode;
#else
            Logging.LogWarning(ToString(), "Cannot get IAP product price, Unity Purchasing is not enabled.");
            return LanguageManager.GetUnknowDescription();
#endif
        }
    }
}
