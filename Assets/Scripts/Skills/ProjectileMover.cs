using UnityEngine;
using System;
using Object = UnityEngine.Object;

public class ProjectileMover : MonoBehaviour
{
    private Transform target;
    private Action onHit;
    private GameObject hitEffectPrefab;
    public float speed = 12f;
    public float hitEffectDestroyDelay = 2f; // 이펙트 유지 시간

    public void Init(Transform tgt, Action onHitCallback, GameObject hitEffect = null)
    {
        target = tgt;
        onHit = onHitCallback;
        hitEffectPrefab = hitEffect;
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject);
            return;
        }
        var dir = (target.position - transform.position).normalized;
        transform.position += dir * (speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target.position) < 0.4f)
        {
            // 도착시 히트 이펙트 생성
            if (hitEffectPrefab != null)
            {
                var effect = Object.Instantiate(hitEffectPrefab, target.position, Quaternion.identity);
                Destroy(effect, hitEffectDestroyDelay); // 2초 뒤에 이펙트만 파괴
            }

            onHit?.Invoke();
            Destroy(gameObject, hitEffectDestroyDelay); // ★ 2초 뒤에 발사체 오브젝트도 파괴
        }
    }
}