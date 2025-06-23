using System.Collections.Generic;
using System.Linq;
using IdleRPG;
using UnityEngine;

public class MonsterPool : MonoBehaviour
{
    public List<GameObject> monsterPrefabs;
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
        monster.ResetMonster();
        return monster;
    }

    public void ReturnToPool(Monster monster)
    {
        monster.gameObject.SetActive(false);
    }

    public void DeactivateAll()
    {
        foreach (var m in pool)
            m.gameObject.SetActive(false);
    }

    // ★ 살아있는 몬스터 체크
    public bool HasAliveMonster()
    {
        return pool.Any(m => m.gameObject.activeInHierarchy);
    }
}