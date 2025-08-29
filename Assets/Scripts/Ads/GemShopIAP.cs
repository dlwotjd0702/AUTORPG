using System;
using UnityEngine;
using UnityEngine.Purchasing; // IAP 네임스페이스!
using Unity.Services.Core;    // UGS 네임스페이스
using System.Threading.Tasks; // Task

public class GemShopIAP : MonoBehaviour, IStoreListener
{
    public PlayerGemManager gemManager;
    private static IStoreController s_storeController;
    private static IExtensionProvider s_storeExtensionProvider;

    public const string PRODUCT_100_GEM = "test_gem_100";

    // Start를 async로 변경
    [Obsolete("Obsolete")]
    async void Start()
    {
        await UnityServices.InitializeAsync(); // UGS 먼저 초기화
        Debug.Log("Unity Gaming Services Initialized!");

        if (s_storeController == null)
        {
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
            builder.AddProduct(PRODUCT_100_GEM, ProductType.Consumable);
            UnityPurchasing.Initialize(this, builder);
        }
    }

    // 상품 구매 버튼에서 호출
    public void BuyGems100()
    {
        if (s_storeController != null)
            s_storeController.InitiatePurchase(PRODUCT_100_GEM);
        else
            Debug.LogWarning("IAP 초기화 안됨");
    }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        s_storeController = controller;
        s_storeExtensionProvider = extensions;
        Debug.Log("IAP 초기화 완료");
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.LogError("IAP 초기화 실패: " + error);
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message) // 2022버전
    {
        Debug.LogError("IAP 초기화 실패: " + error + " " + message);
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
    {
        if (e.purchasedProduct.definition.id == PRODUCT_100_GEM)
        {
            Debug.Log("[IAP] 100젬 구매 성공(시뮬)");
            if (gemManager != null)
                gemManager.AddGems(100);
        }
        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason reason)
    {
        Debug.LogError("[IAP] 구매 실패: " + reason);
    }
}
