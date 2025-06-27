using System.Collections.Generic;
using UnityEngine;

public class HPBarUIPool : MonoBehaviour
{
    public static HPBarUIPool Instance { get; private set; }
    public HPBarUI prefab;
    public int poolSize = 16;

    private Queue<HPBarUI> pool = new Queue<HPBarUI>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        for (int i = 0; i < poolSize; i++)
            AddNew();
    }

    void AddNew()
    {
        var obj = Instantiate(prefab, transform);
        obj.gameObject.SetActive(false);
        pool.Enqueue(obj);
    }

    public HPBarUI Spawn(GameObject target, float currentHp, float maxHp)
    {
        if (pool.Count == 0) AddNew();
        var obj = pool.Dequeue();
        obj.SetTarget(target);
        obj.SetHP(currentHp, maxHp);
        return obj;
    }

    public void Return(HPBarUI hpBar)
    {
        hpBar.gameObject.SetActive(false);
        pool.Enqueue(hpBar);
    }
}