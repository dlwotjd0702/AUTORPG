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
    public int monstersPerWave = 5;
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

        for (int i = 0; i < monstersPerWave; i++)
        {
            // 원주상에 등간격 배치
            float angle = i * Mathf.PI * 2 / monstersPerWave;
            Vector3 offset = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * spawnRadius;
            Vector3 spawnPos = playerPos + offset;

            Monster monster = monsterPool.Spawn();
            monster.transform.position = spawnPos;
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