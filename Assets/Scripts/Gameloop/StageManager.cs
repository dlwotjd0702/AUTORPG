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

    [System.Serializable]
    public class StageMonsterPattern
    {
        public int stage;
        public int[] prefabIndices;
    }
    public List<StageMonsterPattern> stagePatterns;

    // --- UI 연결용 ---
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

        UpdateProgressModeText();
        StartWave();
    }

    public void StartWave()
    {
        monsterPool.DeactivateAll();

        monstersAlive = 0;
        float spawnRadius = 3.0f;
        Vector3 playerPos = player.transform.position;

        var pattern = stagePatterns.Find(p => p.stage == currentStage);
        int[] spawnTypes = pattern != null ? pattern.prefabIndices : new int[] { 0 };

        int absoluteWave = (currentStage - 1) * maxWavePerStage + currentWave;
        float hpMultiplier     = 1f + (absoluteWave - 1) * 0.2f;
        float goldMultiplier   = 1f + (absoluteWave - 1) * 0.15f;
        float expMultiplier    = 1f + (absoluteWave - 1) * 0.1f;
        float attackMultiplier = 1f + (absoluteWave - 1) * 0.1f;

        for (int i = 0; i < monstersPerWave; i++)
        {
            float angle = i * Mathf.PI * 2 / monstersPerWave;
            Vector3 offset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * spawnRadius;
            Vector3 spawnPos = playerPos + offset;

            int prefabIdx = spawnTypes[i % spawnTypes.Length];

            Monster monster = monsterPool.Spawn(prefabIdx);
            monster.transform.position = spawnPos;

            monster.maxHp      = Mathf.RoundToInt(monster.maxHp * hpMultiplier);
            monster.currentHp  = monster.maxHp;
            monster.goldReward = Mathf.RoundToInt(monster.goldReward * goldMultiplier);
            monster.expReward  = Mathf.RoundToInt(monster.expReward * expMultiplier);
            monster.attackPower= Mathf.RoundToInt(monster.attackPower * attackMultiplier);

            monster.OnMonsterDeath += OnMonsterDeath;
            monstersAlive++;
        }
    }

    private void OnMonsterDeath(Monster monster)
    {
        rewardManager.GrantReward(player, monster);
        monstersAlive--;
        monster.OnMonsterDeath -= OnMonsterDeath;
        monsterPool.ReturnToPool(monster);

        if (monstersAlive <= 0)
        {
            // 클리어 정보 저장
            if (!clearedStageWave.Any(x => x.stage == currentStage && x.wave == currentWave))
                clearedStageWave.Add(new StageWaveRecord { stage = currentStage, wave = currentWave });

            if (currentStage > maxClearedStage || (currentStage == maxClearedStage && currentWave > maxClearedWave))
            {
                maxClearedStage = currentStage;
                maxClearedWave = currentWave;
            }

            // 모드에 따라 분기
            if (progressMode == StageProgressMode.Advance)
                NextWave();
            else
                StartWave();
        }
    }

    // --------- 반복/돌파 모드 전환 ---------
    public void ToggleProgressMode()
    {
        progressMode = (progressMode == StageProgressMode.Repeat) ? StageProgressMode.Advance : StageProgressMode.Repeat;
        UpdateProgressModeText();
    }

    private void UpdateProgressModeText()
    {
        if (progressModeText)
            progressModeText.text = progressMode == StageProgressMode.Advance ? "모드: 돌파" : "모드: 반복";
    }

    // --------- 플레이어 죽음 시 처리 ---------
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
        StartWave();
    }

    public void NextWave()
    {
        if (currentWave >= maxWavePerStage)
        {
            currentWave = 1;
            currentStage++;
            if (currentStage > maxStage) currentStage = maxStage;
        }
        else
        {
            currentWave++;
        }
        StartWave();
    }

    public void PrevWave()
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
        StartWave();
    }

    public void MoveToStageWave(int stage, int wave)
    {
        if (clearedStageWave.Any(x => x.stage == stage && x.wave == wave))
        {
            currentStage = stage;
            currentWave = wave;
            StartWave();
        }
        else
        {
            Debug.LogWarning($"아직 클리어하지 않은 스테이지/웨이브: {stage}-{wave}");
        }
    }

    // 버튼용 OnClick
    public void OnNextWaveButton() => NextWave();
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
        currentStage = save.currentStage;
        currentWave = save.currentWave;
        maxClearedStage = save.maxClearedStage;
        maxClearedWave = save.maxClearedWave;
        clearedStageWave = save.clearedStageWave
            .Select(x => new StageWaveRecord { stage = x.stage, wave = x.wave }).ToList();
        progressMode = (StageProgressMode)save.progressMode;
        UpdateProgressModeText();
        StartWave();
    }
}
