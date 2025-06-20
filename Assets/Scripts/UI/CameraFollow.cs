using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;       // 따라갈 타겟(플레이어)
    public Vector3 offset = new Vector3(0, 5, -10); // 카메라 오프셋
    public float followSpeed = 10f;

    void LateUpdate()
    {
        if (target == null) return;
        // 부드럽게 따라가기
        Vector3 targetPosition = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);
    }
}