using GoogleMobileAds.Api;
using UnityEngine;

public class BannerAdTest : MonoBehaviour
{
    private BannerView bannerView;

    void Start()
    {
        string adUnitId = "ca-app-pub-3940256099942544/6300978111"; // 테스트 배너 ID

        // ⭐️ AdPosition.TopLeft로 설정!
        bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.TopLeft);

        AdRequest request = new AdRequest();
        bannerView.LoadAd(request);
    }

    void OnDestroy()
    {
        if (bannerView != null)
            bannerView.Destroy();
    }
}