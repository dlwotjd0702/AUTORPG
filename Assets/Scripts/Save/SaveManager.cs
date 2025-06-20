using UnityEngine;
using System.Linq;
using Combat;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }
    public static SaveData pendingSaveData = null; // 씬 전환용 임시 저장

    private void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // 씬 전환 전에 세이브데이터를 임시저장
    public static void LoadOnNextScene(SaveData data)
    {
        pendingSaveData = data;
    }

    private void Start()
    {
        // 인게임 씬 진입 시, 대기 중인 세이브데이터를 적용
        if (pendingSaveData != null)
        {
            LoadGame(pendingSaveData);
            pendingSaveData = null;
        }
    }

    public void SaveGame()
    {
        SaveData data = new SaveData();
        data.SetSaveTime();
        data.nickname = FirebaseManager.Instance.Email.Split('@')[0];
        foreach (var s in FindObjectsOfType<MonoBehaviour>().OfType<ISaveable>())
            s.CollectSaveData(data);

        FirebaseManager.Instance.SaveGame(data, (success) => {
            Debug.Log(success ? "저장 완료" : "저장 실패");
        });
    }

    public void LoadGame(SaveData loadedData)
    {
        if (loadedData == null)
        {
            Debug.LogWarning("로드 데이터가 null임.");
            return;
        }
        foreach (var s in FindObjectsOfType<MonoBehaviour>().OfType<ISaveable>())
            s.ApplyLoadedData(loadedData);
    }
}