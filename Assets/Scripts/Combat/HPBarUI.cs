using System;
using IdleRPG;
using UnityEngine;
using UnityEngine.UI;

public class HPBarUI : MonoBehaviour
{
    public Slider hpSlider;
    public Image fillImage;      // Fill Area > Fill 이미지 연결
    public Color normalColor = Color.green;  // 기본: 초록 (플레이어)
    public Color disabledColor = Color.red;  // disable 시: 빨강

    private GameObject target;
    private Vector3 offset = Vector3.up * 2.0f;
    private Player  player;
    private Monster monster;
    public void SetTarget(GameObject t)
    {
        target = t;
        gameObject.SetActive(true);
        player = t.GetComponent<Player>();
        if (player != null)
        {
            SetColor(true);
            return;
        }

        // 2. 몬스터 스크립트가 있으면 참조
        monster = t.GetComponent<Monster>();
        if (monster != null)
        {
            SetColor(false);
            return;
        }
    }

    public void SetHP(float current, float max)
    {
        //hpSlider.value = current / max;
    }

    private void Update()
    {
        if (player != null)hpSlider.value = player.currentHp / player.MaxHp;
        
        if (monster != null)hpSlider.value = monster.currentHp / monster.maxHp;
    }


    void LateUpdate()
    {
        if (target == null) return;
        Vector3 pos = Camera.main.WorldToScreenPoint(target.transform.position + offset);
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