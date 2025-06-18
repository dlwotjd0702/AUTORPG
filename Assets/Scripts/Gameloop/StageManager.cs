using System.Collections.Generic;
using IdleRPG;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public Player player;
    public MonsterPool monsterPool;
    public RewardManager rewardManager;

    public int currentStage = 1;
    public int currentWave = 1;
    public int monstersPerWave = 8;
    public float spawnInterval = 1.5f;

    private int monstersAlive;

    void Start()
    {
        StartWave();
    }

    public void StartWave()
    {
        monstersAlive = 0;
        float spawnRadius = 3.0f; // 플레이어로부터 거리
        Vector3 playerPos = player.transform.position;

        // 강화 배율, 난이도에 따라 수정
        float hpMultiplier = 1f + (currentWave - 1) * 0.2f; // 20%씩 증가
        float goldMultiplier = 1f + (currentWave - 1) * 0.15f; // 15%씩 증가
        float expMultiplier = 1f + (currentWave - 1) * 0.1f; // 10%씩 증가
        float attackMultiplier = 1f + (currentWave - 1) * 0.1f; // 10%씩 증가

        for (int i = 0; i < monstersPerWave; i++)
        {
            float angle = i * Mathf.PI * 2 / monstersPerWave;
            Vector3 offset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * spawnRadius;
            Vector3 spawnPos = playerPos + offset;

            Monster monster = monsterPool.Spawn();
            monster.transform.position = spawnPos;

            // --- [여기서 스탯 강화 적용] ---
            float baseHp = monster.maxHp; // 원본
            monster.maxHp = Mathf.RoundToInt(baseHp * hpMultiplier);
            monster.currentHp = monster.maxHp;
            monster.goldReward = Mathf.RoundToInt(monster.goldReward * goldMultiplier);
            monster.expReward = Mathf.RoundToInt(monster.expReward * expMultiplier);
            monster.attackPower = Mathf.RoundToInt(monster.attackPower * attackMultiplier);
            // 공격력, 공격속도 등 다른 수치도 여기에 추가 가능

            monster.OnMonsterDeath += OnMonsterDeath;
            monstersAlive++;
        }
    }

    private void OnMonsterDeath(Monster monster)
    {
        rewardManager.GrantReward(player, monster); // 플레이어에게 몬스터 보상 지급
        monstersAlive--;
        monster.OnMonsterDeath -= OnMonsterDeath;
        monsterPool.ReturnToPool(monster);

        if (monstersAlive <= 0)
        {
            // 웨이브 종료 처리, 다음 웨이브로 진행 등
            NextWave();
        }
    }

    public void NextWave()
    {
        currentWave++;
        // 난이도/몬스터 수 증가 등 확장 가능
        StartWave();
    }
}