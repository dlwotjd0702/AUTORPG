using UnityEngine;
using TMPro;

public class FloatingText3D : MonoBehaviour
{
    public TextMeshPro text;
    public float floatSpeed = 2f;
    public float duration = 0.8f;
    public float fadeStart = 0.4f; // 페이드 시작 시간(남은시간)
    private float timer;
    private Color originColor;

    // 애니메이션 방향
    Vector3 moveDir = Vector3.up;
    // bool isCritical; // 크리티컬용 (미구현)

    void Awake()
    {
        if (text == null)
            text = GetComponent<TextMeshPro>();
        originColor = text.color;
    }

    public void Show(int amount, Color color/*, bool isCritical = false*/)
    {
        text.text = amount.ToString();
        text.color = color;
        // this.isCritical = isCritical;
        // if (isCritical) text.fontSize *= 1.3f; // 크리티컬이면 폰트 크기 증가 등 (미구현)
        timer = duration;
        gameObject.SetActive(true);
    }

    void Update()
    {
        // Y축으로 부드럽게 이동
        transform.position += moveDir * (floatSpeed * Time.deltaTime);

        // 페이드 아웃 (잔여 시간 fadeStart보다 작으면)
        if (timer < fadeStart)
        {
            float alpha = Mathf.Clamp01(timer / fadeStart);
            var c = text.color;
            c.a = alpha;
            text.color = c;
        }

        timer -= Time.deltaTime;
        if (timer < 0f)
            ReturnToPool();
    }

    void ReturnToPool()
    {
        text.color = originColor;
        // text.fontSize = 원래값; // 크리티컬일 때만 조절 필요
        gameObject.SetActive(false);
        FloatingText3DPool.Instance.Return(this);
    }
}