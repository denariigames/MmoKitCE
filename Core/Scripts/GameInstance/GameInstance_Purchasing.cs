using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using LiteNetLibManager;
using UnityEngine;
#if ENABLE_PURCHASING && (UNITY_IOS || UNITY_ANDROID)
using UnityEngine.Purchasing;
#endif

namespace MultiplayerARPG
{
#if ENABLE_PURCHASING && (UNITY_IOS || UNITY_ANDROID)
    public partial class GameInstance : IStoreListener
#else
    public partial class GameInstance
#endif
    {
        // NOTE: something about product type
        // -- Consumable product is product such as gold, gem that can be consumed
        // -- Non-Consumable product is product such as special characters/items
        // that player will buy it to unlock ability to use and will not buy it later
        // -- Subscription product is product such as weekly/monthly promotion
        public const string TAG_INIT = "IAP_INIT";
        public const string TAG_PURCHASE = "IAP_PURCHASE";

#if ENABLE_PURCHASING && (UNITY_IOS || UNITY_ANDROID)
        public static IStoreController StoreController { get; private set; }
        public static IExtensionProvider StoreExtensionProvider { get; private set; }
#endif

        public static System.Action<bool, string> PurchaseCallback;
        public static System.Action<bool, string> RestoreCallback;

        [Header("Purchasing")]
        [Tooltip("You can add cash packages / cash shop item here")]
        public CashShopDatabase cashShopDatabase;
        public static readonly Dictionary<int, CashShopItem> CashShopItems = new Dictionary<int, CashShopItem>();
        public static readonly Dictionary<int, CashPackage> CashPackages = new Dictionary<int, CashPackage>();

        private void InitializePurchasing()
        {
            CashShopItems.Clear();
            CashPackages.Clear();

            if (cashShopDatabase != null)
            {
                foreach (CashShopItem cashShopItem in cashShopDatabase.cashShopItems)
                {
                    if (cashShopItem == null || CashShopItems.ContainsKey(cashShopItem.DataId))
                        continue;
                    CashShopItems[cashShopItem.DataId] = cashShopItem;
                }
            }
            // Generate and add cash shop items by items data
            foreach (BaseItem item in Items.Values)
            {
                item.GenerateCashShopItems();
            }

#if ENABLE_PURCHASING && (UNITY_ANDROID || UNITY_IOS)
            // If we have already connected to Purchasing ...
            if (IsPurchasingInitialized())
                return;

            // Create a builder, first passing in a suite of Unity provided stores.
            var module = StandardPurchasingModule.Instance();
            var builder = ConfigurationBuilder.Instance(module);
#endif

            if (cashShopDatabase != null)
            {
                foreach (CashPackage cashPackage in cashShopDatabase.cashPackages)
                {
                    if (cashPackage == null || CashPackages.ContainsKey(cashPackage.DataId))
                        continue;

#if ENABLE_PURCHASING && (UNITY_ANDROID || UNITY_IOS)
                    // Setup IAP package for clients
                    var productCatalogItem = cashPackage.ProductCatalogItem;
                    if (productCatalogItem == null)
                        continue;

                    Logging.Log(LogTag, "[" + TAG_INIT + "]: Adding product " + productCatalogItem.id + " type " + productCatalogItem.type.ToString());
                    if (productCatalogItem.allStoreIDs.Count > 0)
                    {
                        var ids = new IDs();
                        foreach (var storeID in productCatalogItem.allStoreIDs)
                        {
                            ids.Add(storeID.id, storeID.store);
                        }
                        builder.AddProduct(productCatalogItem.id, productCatalogItem.type, ids);
                    }
                    else
                    {
                        builder.AddProduct(productCatalogItem.id, productCatalogItem.type);
                    }
#endif
                    CashPackages[cashPackage.DataId] = cashPackage;
                }
            }

#if ENABLE_PURCHASING && (UNITY_ANDROID || UNITY_IOS)
            Logging.Log(LogTag, "[" + TAG_INIT + "]: Initializing Purchasing...");
            // Kick off the remainder of the set-up with an asynchrounous call, passing the configuration 
            // and this class' instance. Expect a response either in OnInitialized or OnInitializeFailed.
            try
            {
                UnityPurchasing.Initialize(this, builder);
            }
            catch (System.InvalidOperationException ex)
            {
                var errorMessage = "[" + TAG_INIT + "]: Cannot initialize purchasing, the platform may not supports.";
                Logging.LogError(LogTag, errorMessage);
                Logging.LogException(LogTag, ex);
            }
#else
            Logging.Log(LogTag, "[" + TAG_INIT + "]: Initialized without purchasing");
#endif
        }

        public static bool IsPurchasingInitialized()
        {
#if ENABLE_PURCHASING && (UNITY_IOS || UNITY_ANDROID)
            // Only say we are initialized if both the Purchasing references are set.
            return StoreController != null && StoreExtensionProvider != null;
#else
            return false;
#endif
        }

        #region IStoreListener
#if ENABLE_PURCHASING && (UNITY_IOS || UNITY_ANDROID)
        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            // Purchasing has succeeded initializing. Collect our Purchasing references.
            // Overall Purchasing system, configured with products for this application.
            StoreController = controller;
            // Store specific subsystem, for accessing device-specific store features.
            StoreExtensionProvider = extensions;
            var productCount = StoreController.products.all.Length;
            var logMessage = "[" + TAG_INIT + "]: OnInitialized with " + productCount + " products";
            Logging.Log(LogTag, logMessage);
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
            var errorMessage = "[" + TAG_INIT + "]: Fail. InitializationFailureReason: " + error;
            Logging.LogError(LogTag, errorMessage);
            UISceneGlobal.Singleton.ShowMessageDialog("IAP Initialize Failed", string.Concat(error.ToString().Select(c => char.IsUpper(c) ? $" {c}" : c.ToString())).TrimStart(' '));
        }

        public void OnInitializeFailed(InitializationFailureReason error, string message)
        {
            // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
            var errorMessage = "[" + TAG_INIT + "]: Fail. InitializationFailureReason: " + error + ", " + message;
            Logging.LogError(LogTag, errorMessage);
            UISceneGlobal.Singleton.ShowMessageDialog("IAP Initialize Failed", message);
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
        {
            var dataId = args.purchasedProduct.definition.id.GenerateHashId();

            if (!args.purchasedProduct.hasReceipt)
                return PurchaseProcessingResult.Complete;

            CashPackage package;
            if (CashPackages.TryGetValue(dataId, out package))
            {
                // Connect to server to precess purchasing
                ClientCashShopHandlers.RequestCashPackageBuyValidation(new RequestCashPackageBuyValidationMessage()
                    {
                        dataId = dataId, 
                        platform = Application.platform,
                        receipt = args.purchasedProduct.receipt,
                    }, ResponseCashPackageBuyValidation);
            }
            else
            {
                PurchaseResult(false, LanguageManager.GetText(UITextKeys.UI_ERROR_CASH_PACKAGE_NOT_FOUND.ToString()));
            }

            // Return a flag indicating whether this product has completely been received, or if the application needs 
            // to be reminded of this purchase at next app launch. Use PurchaseProcessingResult.Pending when still 
            // saving purchased products to the cloud, and when that save is delayed.
            return PurchaseProcessingResult.Pending;
        }

        private void ResponseCashPackageBuyValidation(ResponseHandlerData requestHandler, AckResponseCode responseCode, ResponseCashPackageBuyValidationMessage response)
        {
            ClientCashShopActions.ResponseCashPackageBuyValidation(requestHandler, responseCode, response);
            if (responseCode == AckResponseCode.Unimplemented)
            {
                PurchaseResult(false, LanguageManager.GetText(UITextKeys.UI_ERROR_SERVICE_NOT_AVAILABLE.ToString()));
                return;
            }
            if (responseCode == AckResponseCode.Timeout)
            {
                PurchaseResult(false, LanguageManager.GetText(UITextKeys.UI_ERROR_CONNECTION_TIMEOUT.ToString()));
                return;
            }
            switch (responseCode)
            {
                case AckResponseCode.Error:
                    PurchaseResult(false, LanguageManager.GetText(response.message.ToString()));
                    break;
                default:
                    CashPackage package;
                    if (CashPackages.TryGetValue(response.dataId, out package))
                    {
                        StoreController.ConfirmPendingPurchase(package.ProductData);
                        PurchaseResult(true);
                    }
                    else
                    {
                        PurchaseResult(false, LanguageManager.GetText(UITextKeys.UI_ERROR_CASH_PACKAGE_NOT_FOUND.ToString()));
                    }
                    break;
            }
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            // A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing 
            // this reason with the user to guide their troubleshooting actions.
            Logging.LogError(LogTag, "[" + TAG_PURCHASE + "]: FAIL. Product: " + product.definition.storeSpecificId + ", PurchaseFailureReason: " + failureReason);
            string errorMessage = string.Empty;
            switch (failureReason)
            {
                case PurchaseFailureReason.PurchasingUnavailable:
                    errorMessage = LanguageManager.GetText(UITextKeys.UI_ERROR_IAP_PURCHASING_UNAVAILABLE.ToString());
                    break;
                case PurchaseFailureReason.ExistingPurchasePending:
                    errorMessage = LanguageManager.GetText(UITextKeys.UI_ERROR_IAP_EXISTING_PURCHASE_PENDING.ToString());
                    break;
                case PurchaseFailureReason.ProductUnavailable:
                    errorMessage = LanguageManager.GetText(UITextKeys.UI_ERROR_IAP_PRODUCT_UNAVAILABLE.ToString());
                    break;
                case PurchaseFailureReason.SignatureInvalid:
                    errorMessage = LanguageManager.GetText(UITextKeys.UI_ERROR_IAP_SIGNATURE_INVALID.ToString());
                    break;
                case PurchaseFailureReason.UserCancelled:
                    errorMessage = LanguageManager.GetText(UITextKeys.UI_ERROR_IAP_USER_CANCELLED.ToString());
                    break;
                case PurchaseFailureReason.PaymentDeclined:
                    errorMessage = LanguageManager.GetText(UITextKeys.UI_ERROR_IAP_PAYMENT_DECLINED.ToString());
                    break;
                case PurchaseFailureReason.DuplicateTransaction:
                    errorMessage = LanguageManager.GetText(UITextKeys.UI_ERROR_IAP_DUPLICATE_TRANSACTION.ToString());
                    break;
                default:
                    errorMessage = LanguageManager.GetText(UITextKeys.UI_ERROR_IAP_UNKNOW.ToString());
                    break;
            }
            PurchaseResult(false, errorMessage);
        }
#endif
        #endregion

        #region IAP Actions
        public void Purchase(string productId)
        {
#if ENABLE_PURCHASING && (UNITY_IOS || UNITY_ANDROID)
            // If Purchasing has not yet been set up ...
            if (!IsPurchasingInitialized())
            {
                // ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
                Logging.LogError(LogTag, "[" + TAG_PURCHASE + "]: FAIL. Not initialized.");
                PurchaseResult(false, LanguageManager.GetText(UITextKeys.UI_ERROR_IAP_NOT_INITIALIZED.ToString()));
                return;
            }

            var product = StoreController.products.WithID(productId);
            if (product != null && product.availableToPurchase)
            {
                Logging.Log(LogTag, string.Format("[" + TAG_PURCHASE + "] Purchasing product asychronously: '{0}'", product.definition.id));
                StoreController.InitiatePurchase(product);
            }
            else
            {
                Logging.LogError(LogTag, "[" + TAG_PURCHASE + "]: FAIL. Not purchasing product, either is not found or is not available for purchase.");
                PurchaseResult(false, LanguageManager.GetText(UITextKeys.UI_ERROR_IAP_PRODUCT_UNAVAILABLE.ToString()));
            }
#else
            Logging.LogWarning(LogTag, "Cannot purchase product, Unity Purchasing is not enabled.");
#endif
        }
        #endregion

        #region Callback Events
        private static void PurchaseResult(bool success, string errorMessage = "")
        {
            if (PurchaseCallback != null)
            {
                PurchaseCallback(success, errorMessage);
                PurchaseCallback = null;
            }
        }
        #endregion
    }
}
