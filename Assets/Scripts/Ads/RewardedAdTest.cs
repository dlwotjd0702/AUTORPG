using GoogleMobileAds.Api;
using UnityEngine;
using System;


public class RewardedAdTest : MonoBehaviour
{
    private RewardedAd rewardedAd;
    private string adUnitId = "ca-app-pub-3940256099942544/5224354917"; // 테스트 리워드 광고 ID
    private AdBuffType pendingBuffType;

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
            rewardedAd.OnAdFullScreenContentClosed += HandleAdClosed;
        });
    }

    /// <summary>
    /// 배열 인덱스로 버프 타입 분기
    /// 0=공격력, 1=배속, 2=골드
    /// </summary>
    public void ShowRewardedAdByIndex(int idx)
    {
        AdBuffType type = AdBuffType.Attack;
        switch (idx)
        {
            case 0: type = AdBuffType.Attack; break;
            case 1: type = AdBuffType.Speed; break;
            case 2: type = AdBuffType.Gold; break;
            default: type = AdBuffType.Attack; break;
        }
        ShowRewardedAd(type);
    }

    public void ShowRewardedAd(AdBuffType buffType)
    {
        if (rewardedAd != null)
        {
            pendingBuffType = buffType;
            rewardedAd.Show((Reward reward) =>
            {
                Debug.Log($"광고 보상 지급! Type: {reward.Type}, Amount: {reward.Amount}");
                AdBuffManager.Instance.ActivateBuff(pendingBuffType);
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