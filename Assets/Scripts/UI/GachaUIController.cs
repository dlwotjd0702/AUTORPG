using UnityEngine;

public class GachaUIController : MonoBehaviour
{
    [Header("각 가챠 패널을 등록하세요!")]
    public GameObject weaponPanel;
    public GameObject armorPanel;
    public GameObject accessoryPanel;
    public GameObject skillPanel;

    // 열려있는 패널 추적 (옵션)
    private GameObject currentPanel;

    void Start()
    {
        // 처음에 무기 패널만 열기
        ShowPanel(ItemType.Weapon);
    }

    public void ShowPanel(ItemType type)
    {
        // 모두 닫기
        if (weaponPanel) weaponPanel.SetActive(false);
        if (armorPanel) armorPanel.SetActive(false);
        if (accessoryPanel) accessoryPanel.SetActive(false);
        if (skillPanel) skillPanel.SetActive(false);

        // 해당 패널만 열기
        switch (type)
        {
            case ItemType.Weapon:
                if (weaponPanel) weaponPanel.SetActive(true); currentPanel = weaponPanel;
                break;
            case ItemType.Armor:
                if (armorPanel) armorPanel.SetActive(true); currentPanel = armorPanel;
                break;
            case ItemType.Accessory:
                if (accessoryPanel) accessoryPanel.SetActive(true); currentPanel = accessoryPanel;
                break;
            case ItemType.Skill:
                if (skillPanel) skillPanel.SetActive(true); currentPanel = skillPanel;
                break;
        }
    }

    // UI버튼에 이 함수 연결: Weapon 탭 버튼에 호출
    public void OnWeaponButton() => ShowPanel(ItemType.Weapon);
    public void OnArmorButton() => ShowPanel(ItemType.Armor);
    public void OnAccessoryButton() => ShowPanel(ItemType.Accessory);
    public void OnSkillButton() => ShowPanel(ItemType.Skill);
    public void OnCloseButton() => gameObject.SetActive(false);
    public void OnOpenButton() => gameObject.SetActive(true);
    
}