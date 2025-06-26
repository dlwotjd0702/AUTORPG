using GoogleMobileAds.Api;
using UnityEngine;

public class AdMobManager : MonoBehaviour
{
    public static AdMobManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        DontDestroyOnLoad(gameObject);

        MobileAds.Initialize(initStatus =>
        {
            Debug.Log("AdMob SDK 초기화 완료");
        });
    }
}