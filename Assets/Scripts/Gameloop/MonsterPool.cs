using System.Collections.Generic;
using IdleRPG;
using UnityEngine;

public class MonsterPool : MonoBehaviour
{
    public List<GameObject> monsterPrefabs; // 0,1,2... 각 프리팹 인덱스
    private List<Monster> pool = new List<Monster>();

    public Monster Spawn(int prefabIndex = 0)
    {
        Monster monster = pool.Find(m => !m.gameObject.activeInHierarchy && m.prefabIndex == prefabIndex);
        if (monster == null)
        {
            var prefab = monsterPrefabs[Mathf.Clamp(prefabIndex, 0, monsterPrefabs.Count - 1)];
            monster = Instantiate(prefab, transform).GetComponent<Monster>();
            monster.prefabIndex = prefabIndex;
            pool.Add(monster);
        }
        monster.currentHp = monster.maxHp;
        monster.gameObject.SetActive(true);
        return monster;
    }

    public void ReturnToPool(Monster monster)
    {
        monster.gameObject.SetActive(false);
    }

    // 현재 활성화된 몬스터 모두 끄기
    public void DeactivateAll()
    {
        foreach (var m in pool)
            m.gameObject.SetActive(false);
    }
}