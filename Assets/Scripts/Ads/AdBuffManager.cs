using UnityEngine;
using UnityEngine.UI; // ← 버튼 쓸 때 필요
using System;

public enum AdBuffType { Attack, Speed, Gold }

public class AdBuffManager : MonoBehaviour
{
    public static AdBuffManager Instance { get; private set; }

    public bool AttackBuffActive { get; private set; }
    public bool SpeedBuffActive { get; private set; }
    public bool GoldBuffActive { get; private set; }

    private float attackBuffTimer;
    private float speedBuffTimer;
    private float goldBuffTimer;

    private const float BUFF_DURATION = 300f; // 5분

    // 🔥 광고버프 버튼 연결용
    public Button[] buffButtons; // 0:공격력 1:배속 2:골드
    public RewardedAdTest rewardedAdTest; // 광고 객체 할당

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        // 버튼 이벤트 연결
        if (buffButtons != null && rewardedAdTest != null)
        {
            for (int i = 0; i < buffButtons.Length; i++)
            {
                int idx = i; // 람다 캡처용
                buffButtons[i].onClick.AddListener(() => rewardedAdTest.ShowRewardedAdByIndex(idx));
            }
        }
    }

    void Update()
    {
        if (AttackBuffActive)
        {
            attackBuffTimer -= Time.deltaTime;
            if (attackBuffTimer <= 0)
            {
                AttackBuffActive = false;
                // UI, 효과 종료 알림
            }
        }
        if (SpeedBuffActive)
        {
            speedBuffTimer -= Time.deltaTime;
            if (speedBuffTimer <= 0)
            {
                SpeedBuffActive = false;
            }
        }
        if (GoldBuffActive)
        {
            goldBuffTimer -= Time.deltaTime;
            if (goldBuffTimer <= 0)
            {
                GoldBuffActive = false;
            }
        }
    }

    public void ActivateBuff(AdBuffType type)
    {
        switch (type)
        {
            case AdBuffType.Attack:
                AttackBuffActive = true;
                attackBuffTimer = BUFF_DURATION;
                break;
            case AdBuffType.Speed:
                SpeedBuffActive = true;
                speedBuffTimer = BUFF_DURATION;
                break;
            case AdBuffType.Gold:
                GoldBuffActive = true;
                goldBuffTimer = BUFF_DURATION;
                break;
        }
        // UI 갱신, 효과음 등 추가 가능
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
