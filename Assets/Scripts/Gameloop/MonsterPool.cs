using System.Collections.Generic;
using IdleRPG;
using UnityEngine;

public class MonsterPool : MonoBehaviour
{
    public GameObject monsterPrefab;
    public List<Monster> pool = new List<Monster>();

    public Monster Spawn()
    {
        Monster monster = pool.Find(m => !m.gameObject.activeInHierarchy);
        if (monster == null)
        {
            monster = Instantiate(monsterPrefab, gameObject.transform).GetComponent<Monster>();
            pool.Add(monster);
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
}