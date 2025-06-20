using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveManagerUIController : MonoBehaviour
{
    public Button saveButton;
    public Button loadButton;
    public GameObject warningPanel;   // 경고용 팝업 패널(비활성화 상태여야 함)
    public TextMeshProUGUI warningText;
    public Button warningYesButton;
    public Button warningNoButton;

    private void Start()
    {

        // 버튼 리스너 등록
        saveButton.onClick.AddListener(OnClickSave);
        loadButton.onClick.AddListener(OnClickLoad);

        // 경고창 리스너 등록
        warningYesButton.onClick.AddListener(OnWarningYes);
        warningNoButton.onClick.AddListener(OnWarningNo);

        warningPanel.SetActive(false);
    }

    private void OnClickSave()
    {
        SaveManager.Instance.SaveGame();
    }

    private void OnClickLoad()
    {
        warningText.text = "로드 시 현재 진행상황이 덮어써집니다.\n정말 로드하시겠습니까?";
        warningPanel.SetActive(true);
    }

    public void OnWarningYes()
    {
        // 경고창 Yes: 즉시 세이브매니저에 로드
        SaveData data = SaveManager.pendingSaveData;
        if (data != null)
        {
            SaveManager.Instance.LoadGame(data);
            Debug.Log("진행상황이 로드되어 덮어써졌습니다.");
        }
        else
        {
            Debug.LogWarning("로드할 데이터가 없습니다.");
        }
        warningPanel.SetActive(false);
    }

    private void OnWarningNo()
    {
        // 경고창 No: 닫기
        warningPanel.SetActive(false);
    }
}