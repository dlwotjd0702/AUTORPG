using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginUI : MonoBehaviour
{
    public FirebaseManager firebaseManager;

    string email = "";
    string password = "";
    string infoMsg = "";
    bool isSignUpMode = false;
    Rect panelRect;

    void Start()
    {
        float w = 400, h = 320, margin = 40f;
        float x = Screen.width - w - margin;
        float y = Screen.height - h - margin;
        panelRect = new Rect(x, y, w, h);
    }

    void OnGUI()
    {
        GUIStyle panelStyle = new GUIStyle(GUI.skin.box) { fontSize = 18 };
        GUIStyle labelStyle = new GUIStyle(GUI.skin.label) { fontSize = 18 };
        GUIStyle fieldStyle = new GUIStyle(GUI.skin.textField) { fontSize = 18 };
        GUIStyle btnStyle = new GUIStyle(GUI.skin.button) { fontSize = 18, fixedHeight = 36 };

        GUI.BeginGroup(panelRect, GUIContent.none);

        GUILayout.BeginVertical(panelStyle, GUILayout.Width(panelRect.width), GUILayout.Height(panelRect.height));
        GUILayout.Space(15);
        GUILayout.Label(isSignUpMode ? "회원가입" : "로그인", labelStyle);

        GUILayout.Space(10);
        GUILayout.Label("이메일", labelStyle);
        email = GUILayout.TextField(email, fieldStyle, GUILayout.Width(350));
        GUILayout.Space(6);
        GUILayout.Label("비밀번호", labelStyle);
        password = GUILayout.PasswordField(password, '*', 32, fieldStyle, GUILayout.Width(350));

        GUILayout.Space(12);

        if (isSignUpMode)
        {
            if (GUILayout.Button("회원가입", btnStyle, GUILayout.Width(340)))
            {
                infoMsg = "가입 시도 중...";
                firebaseManager.SignUpWithEmail(email, password, (success, msg) =>
                {
                    infoMsg = success ? "회원가입 성공! 로그인하세요." : "회원가입 실패: " + msg;
                    isSignUpMode = !success;
                });
            }
            if (GUILayout.Button("로그인 화면으로", btnStyle, GUILayout.Width(340)))
            {
                isSignUpMode = false;
                infoMsg = "";
            }
        }
        else
        {
            if (GUILayout.Button("로그인", btnStyle, GUILayout.Width(340)))
            {
                infoMsg = "로그인 시도 중...";
                firebaseManager.SignInWithEmail(email, password, (success, msg) =>
                {
                    infoMsg = success ? "로그인 성공!" : "로그인 실패: " + msg;
                    if (success)
                    {
                        infoMsg = "세이브데이터 로딩 중...";
                        firebaseManager.LoadGame((save) =>
                        {
                            // <<==== 세이브데이터를 SaveManager에 임시 저장
                            SaveManager.LoadOnNextScene(save);
                            // Ingame 씬 전환
                            SceneManager.LoadScene("Ingame");
                        });
                    }
                });
            }
            if (GUILayout.Button("회원가입", btnStyle, GUILayout.Width(340)))
            {
                isSignUpMode = true;
                infoMsg = "";
            }
            if (GUILayout.Button("익명 로그인 (게스트)", btnStyle, GUILayout.Width(340)))
            {
                infoMsg = "게스트 로그인...";
                firebaseManager.SignInAnonymously((success, msg) =>
                {
                    infoMsg = success ? "로그인 성공!" : "로그인 실패: " + msg;
                    if (success)
                    {
                        infoMsg = "세이브데이터 로딩 중...";
                        firebaseManager.LoadGame((save) =>
                        {
                            SaveManager.LoadOnNextScene(save);
                            SceneManager.LoadScene("Ingame");
                        });
                    }
                });
            }
        }
        GUILayout.Space(12);
        GUILayout.Label(infoMsg, labelStyle);

        GUILayout.EndVertical();
        GUI.EndGroup();
    }
}
