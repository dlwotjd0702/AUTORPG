using System;
using System.Collections.Generic;
using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using Firebase.Firestore;
using UnityEngine;

public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager Instance { get; private set; }

    public FirebaseAuth Auth { get; private set; }
    public FirebaseUser User { get; private set; }
    public FirebaseFirestore Firestore { get; private set; }

    private const string SaveDataCollection = "SaveData";
    private const string SaveDataDocPrefix = "User_";

    public bool IsReady { get; private set; } = false;

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
}
