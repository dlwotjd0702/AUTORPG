using GoogleMobileAds.Api;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum AdBuffType { Attack, Speed, Gold }

public class AdBuffManager : MonoBehaviour
{
    public static AdBuffManager Instance { get; private set; }

    // 버프 상태
    public bool AttackBuffActive { get; private set; }
    public bool SpeedBuffActive { get; private set; }
    public bool GoldBuffActive { get; private set; }

    // 버프 타이머
    private float attackBuffTimer;
    private float speedBuffTimer;
    private float goldBuffTimer;

    private const float BUFF_DURATION = 300f; // 5분

    // UI: 광고버프 버튼/플레이어 스탯
    public Button[] buffButtons; // 0:공격력, 1:배속, 2:골드
    public Stats.PlayerStats playerStats;

    // ★ 버프 남은시간 표시 TMP 텍스트 (드래그로 각각 연결)
    public TextMeshProUGUI attackBuffTimeText;
    public TextMeshProUGUI speedBuffTimeText;
    public TextMeshProUGUI goldBuffTimeText;

    // AdMob 리워드 광고
    private RewardedAd rewardedAd;
    private string adUnitId = "ca-app-pub-3940256099942544/5224354917"; // 테스트 리워드 광고 ID
    private AdBuffType pendingBuffType;

    void Awake()
    {
        if (transform.parent != null)
            transform.SetParent(null);

        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        MobileAds.Initialize(initStatus => { Debug.Log("AdMob SDK 초기화 완료"); });
        LoadRewardedAd();

        if (buffButtons != null)
        {
            for (int i = 0; i < buffButtons.Length; i++)
            {
                int idx = i;
                buffButtons[i].onClick.AddListener(() => ShowRewardedAdByIndex(idx));
            }
        }
    }

    void Update()
    {
        // 공격력 버프
        if (AttackBuffActive)
        {
            attackBuffTimer -= Time.deltaTime;
            if (attackBuffTimer <= 0)
            {
                AttackBuffActive = false;
                attackBuffTimer = 0;
                Debug.Log("공격력 버프 만료");
                if (playerStats != null)
                    playerStats.RefreshStats();
            }
        }
        // 배속 버프
        if (SpeedBuffActive)
        {
            speedBuffTimer -= Time.deltaTime;
            if (speedBuffTimer <= 0)
            {
                SpeedBuffActive = false;
                speedBuffTimer = 0;
                Debug.Log("배속 버프 만료");
            }
        }
        Time.timeScale = SpeedBuffActive ? 2.0f : 1.0f;
        // 골드 버프
        if (GoldBuffActive)
        {
            goldBuffTimer -= Time.deltaTime;
            if (goldBuffTimer <= 0)
            {
                GoldBuffActive = false;
                goldBuffTimer = 0;
                Debug.Log("골드 버프 만료");
            }
        }

        // ★ TMP 텍스트로 남은 시간 UI 실시간 업데이트
        UpdateBuffTimeUI();
    }

    // 남은시간을 "mm:ss" 형식으로 표시
    private void UpdateBuffTimeUI()
    {
        attackBuffTimeText.text = AttackBuffActive ? FormatTime(attackBuffTimer) : "";
        speedBuffTimeText.text = SpeedBuffActive ? FormatTime(speedBuffTimer) : "";
        goldBuffTimeText.text = GoldBuffActive ? FormatTime(goldBuffTimer) : "";
    }

    private string FormatTime(float time)
    {
        if (time < 0) time = 0;
        int min = (int)(time / 60);
        int sec = (int)(time % 60);
        return $"{min:00}:{sec:00}";
    }

    // 광고 로드
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

    public void ShowRewardedAdByIndex(int idx)
    {
        AdBuffType type = AdBuffType.Attack;
        switch (idx)
        {
            case 0: type = AdBuffType.Attack; break;
            case 1: type = AdBuffType.Speed; break;
            case 2: type = AdBuffType.Gold; break;
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
                ActivateBuff(pendingBuffType);
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

    public void ActivateBuff(AdBuffType type)
    {
        switch (type)
        {
            case AdBuffType.Attack:
                AttackBuffActive = true;
                attackBuffTimer = BUFF_DURATION;
                Debug.Log("공격력 버프 적용 (5분)");
                if (playerStats != null)
                    playerStats.RefreshStats();
                break;
            case AdBuffType.Speed:
                SpeedBuffActive = true;
                speedBuffTimer = BUFF_DURATION;
                Debug.Log("배속 버프 적용 (5분)");
                break;
            case AdBuffType.Gold:
                GoldBuffActive = true;
                goldBuffTimer = BUFF_DURATION;
                Debug.Log("골드 버프 적용 (5분)");
                break;
        }
    }

    public float GetBuffRemainTime(AdBuffType type)
    {
        return type switch
        {
            AdBuffType.Attack => attackBuffTimer,
            AdBuffType.Speed => speedBuffTimer,
            AdBuffType.Gold => goldBuffTimer,
            _ => 0
        };
    }
}
