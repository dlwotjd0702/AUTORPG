using UnityEngine;
using System;

public class ProjectileMover : MonoBehaviour
{
    private Transform target;
    private Action onHit;
    public float speed = 12f;

    public void Init(Transform tgt, Action onHitCallback)
    {
        target = tgt;
        onHit = onHitCallback;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }
        var dir = (target.position - transform.position).normalized;
        transform.position += dir * speed * Time.deltaTime;

        if (Vector3.Distance(transform.position, target.position) < 0.4f)
        {
            onHit?.Invoke();
            Destroy(gameObject);
        }
    }
}