using System.Collections.Generic;
using UnityEngine;

public class FloatingText3DPool : MonoBehaviour
{
    public static FloatingText3DPool Instance { get; private set; }
    public FloatingText3D prefab;
    public int poolSize = 20;

    private Queue<FloatingText3D> pool = new Queue<FloatingText3D>();

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

    public FloatingText3D Spawn(Vector3 worldPos, int amount, Color color/*, bool isCritical = false*/)
    {
        if (pool.Count == 0) AddNew();
        var obj = pool.Dequeue();
        obj.transform.position = worldPos;
        obj.Show(amount, color/*, isCritical*/);
        return obj;
    }

    public void Return(FloatingText3D dt)
    {
        dt.gameObject.SetActive(false);
        pool.Enqueue(dt);
    }
}