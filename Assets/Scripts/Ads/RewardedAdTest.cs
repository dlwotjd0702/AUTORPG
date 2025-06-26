using GoogleMobileAds.Api;
using UnityEngine;
using System;

public class RewardedAdTest : MonoBehaviour
{
    private RewardedAd rewardedAd;
    private string adUnitId = "ca-app-pub-3940256099942544/5224354917"; // 테스트 리워드 광고 ID

    void Start()
    {
        LoadRewardedAd();
    }

    public void LoadRewardedAd()
    {
        AdRequest request = new AdRequest();
        RewardedAd.Load(adUnitId, request, (RewardedAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogError($"리워드 광고 로드 실패: {error}");
                return;
            }

            Debug.Log("리워드 광고 로드 성공");
            rewardedAd = ad;
            // 광고 닫힘/보상 이벤트 등록
            rewardedAd.OnAdFullScreenContentClosed += HandleAdClosed;
        });
    }

    // 광고 버튼에 연결!
    public void ShowRewardedAd()
    {
        if (rewardedAd != null)
        {
            rewardedAd.Show((Reward reward) =>
            {
                Debug.Log($"광고 보상 지급! Type: {reward.Type}, Amount: {reward.Amount}");
                // 실제 보상 지급 로직 작성
            });
        }
        else
        {
            Debug.Log("광고가 아직 준비되지 않았음");
        }
    }

    private void HandleAdClosed()
    {
        Debug.Log("광고 닫힘, 새로 광고 로드");
        LoadRewardedAd();
    }
}