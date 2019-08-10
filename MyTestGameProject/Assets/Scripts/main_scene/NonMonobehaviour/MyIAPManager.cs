using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Purchasing;

public class MyIAPManager : IStoreListener
{
    private IStoreController controller;
    private IExtensionProvider extensions;

    static List<ProductData> products = new List<ProductData>();

    public event Action<ProductData, PurchaseFailureReason> OnPurchasingFailed;
    public event Action<ProductData> OnPurchasingSuccess;

    public MyIAPManager(params ProductData[] productsData)
    {
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        foreach (var prod in productsData)
        {
            if (products.Contains(prod)) continue;

            builder.AddProduct(prod.id, prod.type);
            products.Add(prod);
        }

        UnityPurchasing.Initialize(this, builder);
    }

    #region IStoreListener

    /// <summary>
    /// Called when Unity IAP is ready to make purchases.
    /// </summary>
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        this.controller = controller;
        this.extensions = extensions;
    }

    /// <summary>
    /// Called when Unity IAP encounters an unrecoverable initialization error.
    ///
    /// Note that this will not be called if Internet is unavailable; Unity IAP
    /// will attempt initialization until it becomes available.
    /// </summary>
    public void OnInitializeFailed(InitializationFailureReason error)
    {
        this.controller = null;
        this.extensions = null;
    }

    /// <summary>
    /// Called when a purchase completes.
    ///
    /// May be called at any time after OnInitialized().
    /// </summary>
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
    {
        var id = e.purchasedProduct.definition.id;
        var data = products.Where(pr => pr.id == id).SingleOrDefault();

        data?.onPurchaseSuccess?.Invoke();
        OnPurchasingSuccess?.Invoke(data);

        return PurchaseProcessingResult.Complete;
    }

    /// <summary>
    /// Called when a purchase fails.
    /// </summary>
    public void OnPurchaseFailed(Product i, PurchaseFailureReason p)
    {
        var id = i.definition.id;
        var data = products.Where(pr => pr.id == id).SingleOrDefault();

        data?.onPurchaseFails?.Invoke();
        OnPurchasingFailed?.Invoke(data, p);
    }

    #endregion

    /// <summary>
    /// Проверить, куплен ли товар.
    /// </summary>
    public bool IsOlreadyBought(string productId)
    {
        var data = products.Where(pr => pr.id == productId).SingleOrDefault();
        if (data == null) return false;

        if (data.type == ProductType.Consumable) return false;

        Product product = controller.products.WithID(productId);
        return product.hasReceipt;
    }

    public bool IsInitialized { get { return controller != null && extensions != null; } }

    public void BuyProductID(string productId, Action onPurchaseSuccess = null, Action onPurchaseFails = null)
    {
        if (IsInitialized && !IsOlreadyBought(productId))
        {
            var data = products.Where(pr => pr.id == productId).SingleOrDefault();
            if (data == null) return;

            data.onPurchaseSuccess = onPurchaseSuccess;
            data.onPurchaseFails = onPurchaseFails;

            Product product = controller.products.WithID(productId);

            if (product != null && product.availableToPurchase)
            {
                Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                controller.InitiatePurchase(product);
            }
            else
            {
                Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
                OnPurchaseFailed(product, PurchaseFailureReason.ProductUnavailable);
            }
        }
    }

    public class ProductData
    {
        public string id;
        public ProductType type;

        public Action onPurchaseSuccess;
        public Action onPurchaseFails;

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (!(obj is ProductData)) return false;

            var other = obj as ProductData;

            return id == other.id && type == other.type;
        }

        /// <summary>
        /// это автоматически сгенерированный метод (если чо)
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            var hashCode = -1056084179;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(id);
            hashCode = hashCode * -1521134295 + type.GetHashCode();
            return hashCode;
        }
    }
}