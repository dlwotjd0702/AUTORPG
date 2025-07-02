using UnityEngine;
using TMPro;

public class FloatingText3D : MonoBehaviour
{
    public TextMeshPro text;
    public float floatSpeed = 2f;
    public float duration = 0.8f;
    public float fadeStart = 0.4f;
    private float timer;
    private Color originColor;

    Vector3 moveDir = Vector3.up;

    void Awake()
    {
        if (text == null)
            text = GetComponent<TextMeshPro>();
        originColor = text.color;
    }

    public void Show(string message, Color color)
    {
        text.text = message;
        text.color = color;
        timer = duration;
        gameObject.SetActive(true);
    }

    public void Show(int amount, Color color)
    {
        Show(amount.ToString(), color);
    }

    void Update()
    {
        transform.position += moveDir * (floatSpeed * Time.deltaTime);

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
        gameObject.SetActive(false);
        FloatingText3DPool.Instance.Return(this);
    }
}