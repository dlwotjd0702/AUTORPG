using GoogleMobileAds.Api;
using UnityEngine;
using UnityEngine.UI;

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

    // UI: 광고버프 버튼(순서대로 연결) / 플레이어 스탯 참조
    public Button[] buffButtons; // 0:공격력, 1:배속, 2:골드
    public Stats.PlayerStats playerStats; // 드래그로 연결

    // AdMob 리워드 광고
    private RewardedAd rewardedAd;
    private string adUnitId = "ca-app-pub-3940256099942544/5224354917"; // 테스트 리워드 광고 ID
    private AdBuffType pendingBuffType;

    // 싱글톤 및 DontDestroyOnLoad 처리
    void Awake()
    {
        // 루트가 아니면 강제 루트로 이동 (자식이면 에러남)
        if (transform.parent != null)
            transform.SetParent(null);

        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // 광고 초기화
        MobileAds.Initialize(initStatus => { Debug.Log("AdMob SDK 초기화 완료"); });

        // 리워드 광고 로드
        LoadRewardedAd();

        // 버튼 이벤트 연결
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
                Debug.Log("골드 버프 만료");
            }
        }
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

    // 버튼에서 호출 (OnClick: 0=공격력, 1=배속, 2=골드)
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

    // 광고 시청 및 보상 처리
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

    // 광고 닫힘 시 재로드
    private void HandleAdClosed()
    {
        Debug.Log("광고 닫힘, 새로 광고 로드");
        LoadRewardedAd();
    }

    // 버프 부여 (실제 효과 부여/타이머 세팅)
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
        // (UI/이펙트 등 필요하면 추가)
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
