using System.Collections.Generic;
using System.Linq;
using Combat;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using IdleRPG;

public enum StageProgressMode { Repeat, Advance }

public class StageManager : MonoBehaviour, ISaveable
{
    public Player player;
    public MonsterPool monsterPool;
    public RewardManager rewardManager;

    public int currentStage = 1;
    public int currentWave = 1;
    public int monstersPerWave = 8;
    public int maxStage = 10;
    public int maxWavePerStage = 10;

    public int maxClearedStage = 1;
    public int maxClearedWave = 1;
    public List<StageWaveRecord> clearedStageWave = new List<StageWaveRecord>();

    public StageProgressMode progressMode = StageProgressMode.Advance;
    private int monstersAlive;

    [Header("몬스터 기본 스탯(1마리 기준)")]
    public float baseMaxHp = 10f;
    public int baseGold = 20;
    public int baseExp = 10;
    public float baseAttack = 5f;

    [Header("UI 연결")]
    public Button nextWaveButton;
    public Button prevWaveButton;
    public Button toggleModeButton;
    public TextMeshProUGUI progressModeText;

    void Start()
    {
        if (nextWaveButton) nextWaveButton.onClick.AddListener(OnNextWaveButton);
        if (prevWaveButton) prevWaveButton.onClick.AddListener(OnPrevWaveButton);
        if (toggleModeButton) toggleModeButton.onClick.AddListener(ToggleProgressMode);

        if (SaveManager.pendingSaveData != null)
        {
            ApplyLoadedData(SaveManager.pendingSaveData);
        }
        else
        {
            UpdateProgressModeText();
            StartWave();
        }
    }

    public void StartWave()
    {
        monsterPool.DeactivateAll();
        float spawnRadius = 3.0f;
        Vector3 playerPos = player.transform.position;

        int prefabIdx = Mathf.Clamp(currentStage - 1, 0, monsterPool.monsterPrefabs.Count - 1);

        float hpMultiplier     = 1f + (currentStage - 1) * 0.5f + (currentWave - 1) * 0.2f;
        float goldMultiplier   = 1f + (currentStage - 1) * 0.3f + (currentWave - 1) * 0.1f;
        float expMultiplier    = 1f + (currentStage - 1) * 0.15f + (currentWave - 1) * 0.05f;
        float attackMultiplier = 1f + (currentStage - 1) * 0.2f + (currentWave - 1) * 0.1f;

        float maxHp    = baseMaxHp * hpMultiplier;
        int goldReward = Mathf.RoundToInt(baseGold * goldMultiplier);
        int expReward  = Mathf.RoundToInt(baseExp * expMultiplier);
        float atk      = baseAttack * attackMultiplier;

        for (int i = 0; i < monstersPerWave; i++)
        {
            float angle = i * Mathf.PI * 2 / monstersPerWave;
            Vector3 offset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * spawnRadius;
            Vector3 spawnPos = playerPos + offset;

            Monster monster = monsterPool.Spawn(prefabIdx);
            monster.transform.position = spawnPos;
            monster.ApplyStats(maxHp, expReward, goldReward, atk);

            monster.OnMonsterDeath += OnMonsterDeath;
        }
        UpdateProgressModeText();
    }

    private void OnMonsterDeath(Monster monster)
    {
        rewardManager.GrantReward(player, monster);
        monster.OnMonsterDeath -= OnMonsterDeath;
        monsterPool.ReturnToPool(monster);

        // 모든 몬스터가 죽었는지 풀 기준으로 판정
        if (!monsterPool.HasAliveMonster())
        {
            if (!clearedStageWave.Any(x => x.stage == currentStage && x.wave == currentWave))
                clearedStageWave.Add(new StageWaveRecord { stage = currentStage, wave = currentWave });

            if (currentStage > maxClearedStage || (currentStage == maxClearedStage && currentWave > maxClearedWave))
            {
                maxClearedStage = currentStage;
                maxClearedWave = currentWave;
            }

            if (progressMode == StageProgressMode.Advance)
                NextWave();
            else
                StartWave();
        }
    }

    public void OnNextWaveButton()
    {
        // 버튼에서는 반드시 클리어 조건 체크!
        if (!clearedStageWave.Any(x => x.stage == currentStage && x.wave == currentWave))
        {
            Debug.Log("이전 웨이브를 클리어해야 다음 웨이브로 넘어갈 수 있습니다.");
            return;
        }
        NextWave();
    }

    public void NextWave()
    {
        int nextWave = currentWave;
        int nextStage = currentStage;

        if (currentWave >= maxWavePerStage)
        {
            nextWave = 1;
            nextStage++;
            if (nextStage > maxStage) nextStage = maxStage;
        }
        else
        {
            nextWave++;
        }

        currentStage = nextStage;
        currentWave = nextWave;
        StartWave();
    }


    public void PrevWave()
    {
        // 살아있는 몬스터가 있어도 이전 웨이브로 이동 허용
        if (currentWave > 1)
        {
            currentWave--;
        }
        else if (currentStage > 1)
        {
            currentStage--;
            currentWave = maxWavePerStage;
        }
        else
        {
            currentStage = 1;
            currentWave = 1;
            Debug.Log("최저 웨이브/스테이지입니다.");
            return;
        }
        StartWave();
    }

    public void ToggleProgressMode()
    {
        progressMode = (progressMode == StageProgressMode.Repeat) ? StageProgressMode.Advance : StageProgressMode.Repeat;
        UpdateProgressModeText();
    }

    private void UpdateProgressModeText()
    {
        if (progressModeText)
            progressModeText.text = $"{currentStage} 스테이지 {currentWave}/10 Wave \n모드: {(progressMode == StageProgressMode.Advance ? "돌파" : "반복")}";
    }

    public void OnPlayerDied()
    {
        if (currentWave > 1)
        {
            currentWave--;
        }
        else if (currentStage > 1)
        {
            currentStage--;
            currentWave = maxWavePerStage;
        }
        else
        {
            currentStage = 1;
            currentWave = 1;
            return;
        }
        StartWave();
    }

    

    public void OnPrevWaveButton() => PrevWave();

    // --------- 세이브/로드용 ---------
    public void CollectSaveData(SaveData save)
    {
        save.currentStage = currentStage;
        save.currentWave = currentWave;
        save.maxClearedStage = maxClearedStage;
        save.maxClearedWave = maxClearedWave;
        save.clearedStageWave = clearedStageWave
            .Select(x => new StageWaveRecord { stage = x.stage, wave = x.wave }).ToList();
        save.progressMode = (int)progressMode;
    }

    public void ApplyLoadedData(SaveData save)
    {
        if (save == null) return;
        currentStage = Mathf.Max(1, save.currentStage);
        currentWave = Mathf.Max(1, save.currentWave);
        maxClearedStage = Mathf.Max(currentStage, save.maxClearedStage);
        maxClearedWave = Mathf.Max(currentWave, save.maxClearedWave);
        clearedStageWave = save.clearedStageWave
            .Select(x => new StageWaveRecord { stage = x.stage, wave = x.wave }).ToList();
        progressMode = (StageProgressMode)save.progressMode;
        UpdateProgressModeText();
        StartWave();
    }
}
