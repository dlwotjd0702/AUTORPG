using System;
using System.Collections.Generic;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Firestore;
using UnityEngine;

// --- 랭킹용 엔트리 데이터 구조 ---
[FirestoreData]
public class RankingEntry
{
    [FirestoreProperty] public string nickname { get; set; }
    [FirestoreProperty] public int maxClearedStage { get; set; }
    [FirestoreProperty] public int maxClearedWave { get; set; }
    [FirestoreProperty] public string userId { get; set; }
  
}

public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager Instance { get; private set; }

    public FirebaseAuth Auth { get; private set; }
    public FirebaseUser User { get; private set; }
    public FirebaseFirestore Firestore { get; private set; }

    private const string SaveDataCollection = "SaveData";
    private const string SaveDataDocPrefix = "User_";

    // 랭킹 관련
    private const string RankingCollection = "StageRankings"; // 랭킹 전용 컬렉션

    public bool IsReady { get; private set; } = false;
    public string Email { get; private set; }

    private void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
        InitializeFirebase();
    }

    private void InitializeFirebase()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            var status = task.Result;
            if (status == DependencyStatus.Available)
            {
                Auth = FirebaseAuth.DefaultInstance;
                Firestore = FirebaseFirestore.DefaultInstance;
                Debug.Log("Firebase 초기화 완료");
                IsReady = true;
            }
            else
            {
                Debug.LogError($"Firebase 초기화 실패: {status}");
            }
        });
    }

    #region --------- Auth (이메일/익명) ---------
    public void SignInWithEmail(string email, string password, Action<bool, string> callback)
    {
        Auth.SignInWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            Email = email;
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogError("Firebase 로그인 실패: " + task.Exception);
                callback?.Invoke(false, "로그인 실패");
                return;
            }
            User = task.Result.User;
            Debug.Log($"로그인 성공: {User.UserId}");
            callback?.Invoke(true, User.UserId);
        });
    }

    public void SignUpWithEmail(string email, string password, Action<bool, string> callback)
    {
        Auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogError("Firebase 회원가입 실패: " + task.Exception);
                callback?.Invoke(false, "회원가입 실패");
                return;
            }
            User = task.Result.User;
            Debug.Log($"회원가입 성공: {User.UserId}");
            callback?.Invoke(true, User.UserId);
        });
    }

    public void SignInAnonymously(Action<bool, string> callback)
    {
        Auth.SignInAnonymouslyAsync().ContinueWithOnMainThread(task =>
        {
            Email = User.UserId;
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogError("Firebase 익명 로그인 실패: " + task.Exception);
                callback?.Invoke(false, "익명 로그인 실패");
                return;
            }
            User = task.Result.User;
            Debug.Log($"익명 로그인 성공: {User.UserId}");
            callback?.Invoke(true, User.UserId);
        });
    }
    #endregion

    #region --------- Save/Load ---------
    public void SaveGame(SaveData saveData, Action<bool> callback = null)
    {
        if (User == null)
        {
            Debug.LogError("로그인 필요");
            callback?.Invoke(false);
            return;
        }

        var docRef = Firestore.Collection(SaveDataCollection).Document(SaveDataDocPrefix + User.UserId);
        docRef.SetAsync(saveData).ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("Firestore 저장 실패: " + task.Exception);
                callback?.Invoke(false);
            }
            else
            {
                Debug.Log("Firestore 저장 성공");
                callback?.Invoke(true);
            }
        });
    }

    public void LoadGame(Action<SaveData> onLoaded)
    {
        if (User == null)
        {
            Debug.LogError("로그인 필요");
            onLoaded?.Invoke(null);
            return;
        }

        var docRef = Firestore.Collection(SaveDataCollection).Document(SaveDataDocPrefix + User.UserId);
        docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("Firestore 로드 실패: " + task.Exception);
                onLoaded?.Invoke(null);
            }
            else
            {
                var snap = task.Result;
                if (snap.Exists)
                {
                    SaveData save = snap.ConvertTo<SaveData>();
                    Debug.Log("Firestore 로드 성공");
                    onLoaded?.Invoke(save);
                }
                else
                {
                    Debug.Log("세이브 데이터 없음 (최초 로그인)");
                    onLoaded?.Invoke(null);
                }
            }
        });
    }
    #endregion

    #region --------- 랭킹 저장 및 조회 ---------

    /// <summary>
    /// 랭킹용 데이터 저장 (스테이지/웨이브 클리어 갱신)
    /// </summary>
    public void SaveRanking(string nickname, int stage, int wave, Action<bool> callback = null)
    {
        Debug.Log($"[SaveRanking] 진입 - User is null? {User == null}");
        if (User == null)
        {
            Debug.LogError("로그인 필요");
            callback?.Invoke(false);
            return;
        }
        Debug.Log("[SaveRanking] User 있음, Firestore: " + (Firestore == null ? "null" : "ok"));

        var docRef = Firestore.Collection(RankingCollection).Document(User.UserId);
        var entry = new RankingEntry
        {
            nickname = nickname,
            maxClearedStage = stage,
            maxClearedWave = wave,
            userId = User.UserId,
     
        };

        Debug.Log("[SaveRanking] Firestore 저장 시도!");
        docRef.SetAsync(entry).ContinueWithOnMainThread(task =>
        {
            Debug.Log("[SaveRanking] Firestore 콜백 진입");
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.LogError("랭킹 저장 실패: " + task.Exception);
                callback?.Invoke(false);
            }
            else
            {
                Debug.Log("랭킹 저장 성공");
                callback?.Invoke(true);
            }
        });
    }


    /// <summary>
    /// 전체 랭킹을 스테이지 > 웨이브 순 내림차순으로 상위 n명만 조회
    /// </summary>
    public void LoadTopRankings(int count, Action<List<RankingEntry>> callback)
    {
        Firestore.Collection("StageRankings")
            .OrderByDescending("maxClearedStage")
            .OrderByDescending("maxClearedWave")
            .Limit(count)
            .GetSnapshotAsync()
            .ContinueWithOnMainThread(task =>
            {
                Debug.Log($"[LoadTopRankings] Firestore 콜백: Faulted={task.IsFaulted}, Canceled={task.IsCanceled}");

                if (task.Exception != null)
                    Debug.LogError("[LoadTopRankings] Exception: " + task.Exception);

                var list = new List<RankingEntry>();
                if (!task.IsFaulted && !task.IsCanceled)
                {
                    foreach (var doc in task.Result.Documents)
                    {
                        var entry = doc.ConvertTo<RankingEntry>();
                        list.Add(entry);
                    }
                }
                callback?.Invoke(list);
            });
    }



    #endregion
}
