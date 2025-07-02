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

    // 문자열 메시지용
    public FloatingText3D Spawn(Vector3 worldPos, string message, Color color)
    {
        if (pool.Count == 0) AddNew();
        var obj = pool.Dequeue();
        obj.transform.position = worldPos;
        obj.Show(message, color);
        return obj;
    }

    // 숫자 전용(기존 사용 방식 유지)
    public FloatingText3D Spawn(Vector3 worldPos, int amount, Color color)
    {
        return Spawn(worldPos, amount.ToString(), color);
    }

    public void Return(FloatingText3D dt)
    {
        dt.gameObject.SetActive(false);
        pool.Enqueue(dt);
    }
}