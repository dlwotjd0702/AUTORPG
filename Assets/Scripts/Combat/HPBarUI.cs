using UnityEngine;
using UnityEngine.UI;

public class HPBarUI : MonoBehaviour
{
    public Slider hpSlider;
    public Image fillImage;      // Fill Area > Fill 이미지 연결
    public Color normalColor = Color.green;  // 기본: 초록 (플레이어)
    public Color disabledColor = Color.red;  // disable 시: 빨강

    private Transform target;
    private Vector3 offset = Vector3.up * 2.0f;

    public void SetTarget(Transform t)
    {
        target = t;
        gameObject.SetActive(true);
    }

    public void SetHP(float current, float max)
    {
        hpSlider.value = current / max;
    }

    void LateUpdate()
    {
        if (target == null) return;
        Vector3 pos = Camera.main.WorldToScreenPoint(target.position + offset);
        transform.position = pos;
    }

    public void SetColor(bool enabled)
    {
        if (fillImage != null)
            fillImage.color = enabled ? normalColor : disabledColor;
    }

    public void Release()
    {
        target = null;
        SetColor(false); // Release 시 색상(빨강)
        gameObject.SetActive(false);
        HPBarUIPool.Instance.Return(this);
    }
}