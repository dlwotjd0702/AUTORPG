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
        float scale = 1.8f;

        float w = 400 * scale;
        float h = 320 * scale;
        float margin = 40f * scale;

        float x = Screen.width - w - margin;
        float y = Screen.height - h - margin;

        panelRect = new Rect(x, y, w, h);
    }

    void OnGUI()
    {
        float scale = 1.8f;

        GUIStyle panelStyle = new GUIStyle(GUI.skin.box) { fontSize = (int)(18 * scale) };
        GUIStyle labelStyle = new GUIStyle(GUI.skin.label) { fontSize = (int)(18 * scale) };
        GUIStyle fieldStyle = new GUIStyle(GUI.skin.textField)
        {
            fontSize = (int)(18 * scale),
            alignment = TextAnchor.MiddleLeft,
            padding = new RectOffset(10, 10, 8, 8), // 패딩 축소
        };
        GUIStyle btnStyle = new GUIStyle(GUI.skin.button)
        {
            fontSize = (int)(18 * scale),
            fixedHeight = 36 * scale
        };

        GUI.BeginGroup(panelRect, GUIContent.none);

        GUILayout.BeginVertical(panelStyle, GUILayout.Width(panelRect.width), GUILayout.Height(panelRect.height));

        // 가운데 정렬을 위한 FlexibleSpace
        GUILayout.FlexibleSpace();

        GUILayout.Label(isSignUpMode ? "회원가입" : "로그인", labelStyle);

        GUILayout.Space(10 * scale);
        GUILayout.Label("이메일", labelStyle);
        email = GUILayout.TextField(email, fieldStyle, GUILayout.Width(350 * scale), GUILayout.Height(45)); // 34로 축소

        GUILayout.Space(6 * scale);
        GUILayout.Label("비밀번호", labelStyle);
        password = GUILayout.PasswordField(password, '*', 32, fieldStyle, GUILayout.Width(350 * scale), GUILayout.Height(45)); // 34로 축소

        GUILayout.Space(12 * scale);

        if (isSignUpMode)
        {
            if (GUILayout.Button("회원가입", btnStyle, GUILayout.Width(340 * scale)))
            {
                infoMsg = "가입 시도 중...";
                firebaseManager.SignUpWithEmail(email, password, (success, msg) =>
                {
                    infoMsg = success ? "회원가입 성공! 로그인하세요." : "회원가입 실패: " + msg;
                    isSignUpMode = !success;
                });
            }

            if (GUILayout.Button("로그인 화면으로", btnStyle, GUILayout.Width(340 * scale)))
            {
                isSignUpMode = false;
                infoMsg = "";
            }
        }
        else
        {
            if (GUILayout.Button("로그인", btnStyle, GUILayout.Width(340 * scale)))
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
                            SaveManager.LoadOnNextScene(save);
                            SceneManager.LoadScene("Ingame");
                        });
                    }
                });
            }

            if (GUILayout.Button("회원가입", btnStyle, GUILayout.Width(340 * scale)))
            {
                isSignUpMode = true;
                infoMsg = "";
            }

            if (GUILayout.Button("익명 로그인 (게스트)", btnStyle, GUILayout.Width(340 * scale)))
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

        GUILayout.Space(12 * scale);
        GUILayout.Label(infoMsg, labelStyle);

        GUILayout.FlexibleSpace(); // 아래도 FlexibleSpace로 완전 중앙정렬

        GUILayout.EndVertical();
        GUI.EndGroup();
    }
}
