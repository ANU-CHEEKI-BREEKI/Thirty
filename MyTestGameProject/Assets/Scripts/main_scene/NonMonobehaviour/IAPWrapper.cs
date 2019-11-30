using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Purchasing;
using static MyIAPManager;

/// <summary>
/// всё что с покупками
/// </summary>
public static class IAPWrapper
{
    static MyIAPManager manager;
    public static event Action<ProductData> OnPurchaseSuccess;

    public static bool Initiate(bool reinit = true)
    {
        if (reinit || manager == null || !manager.IsInitialized)
            manager = new MyIAPManager
            (
                new ProductData() { id = Const.NonConsumable.ID_DISABLE_ADS, type = ProductType.NonConsumable },
                new ProductData() { id = Const.Consumable.ID_GOLD_1, type = ProductType.Consumable },
                new ProductData() { id = Const.Consumable.ID_GOLD_2, type = ProductType.Consumable },
                new ProductData() { id = Const.Consumable.ID_GOLD_3, type = ProductType.Consumable }
            );
        manager.OnPurchasingSuccess += CallOnPurchaseSucess;
        return manager.IsInitialized;
    }

    /// <summary>
    /// обёрточка небольшая
    /// </summary>
    /// <param name="data"></param>
    static void CallOnPurchaseSucess(ProductData data)
    {
        OnPurchaseSuccess?.Invoke(data);
    }

    public static bool BuyProduct(string productId, Action onPurchaseSuccess, Action onPurchaseFails = null, bool disableAds = true)
    {
        if (manager == null || !manager.IsInitialized) return false;

        if (disableAds && !manager.IsOlreadyBought(Const.NonConsumable.ID_DISABLE_ADS))
        {
            Action<ProductData> onSuccess = null;
            onSuccess = (pd) =>
            {
                manager.OnPurchasingSuccess -= onSuccess;

                if (pd.id == productId)
                    manager.BuyProductID(Const.NonConsumable.ID_DISABLE_ADS);
            };
            manager.OnPurchasingSuccess += (pd) => {  };
        }

        manager.BuyProductID(productId, onPurchaseSuccess, onPurchaseFails);

        return true;
    }

    public static class Const
    {
        public static class Consumable
        {
            public const string ID_GOLD_1 = "id_gold_1";
            public const string ID_GOLD_2 = "id_gold_2";
            public const string ID_GOLD_3 = "id_gold_3";
        }

        public static class NonConsumable
        {
            public const string ID_DISABLE_ADS = "id_disable_ads";
        }

        public static class Subscriptions
        {

        }
    }
}