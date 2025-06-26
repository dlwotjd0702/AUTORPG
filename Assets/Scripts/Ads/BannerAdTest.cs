using GoogleMobileAds.Api;
using UnityEngine;

public class BannerAdTest : MonoBehaviour
{
    private BannerView bannerView;

    void Start()
    {
        // 공식 테스트 배너 광고 단위 ID
        string adUnitId = "ca-app-pub-3940256099942544/6300978111";

        // 화면 하단에 배너 생성
        bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Bottom);

        // 광고 요청 생성
        AdRequest request = new AdRequest();
        bannerView.LoadAd(request);
    }

    void OnDestroy()
    {
        if (bannerView != null)
            bannerView.Destroy();
    }
}