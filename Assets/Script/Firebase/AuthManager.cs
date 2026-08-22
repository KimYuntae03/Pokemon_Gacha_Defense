using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

using Firebase.Auth;
using Firebase.Firestore;
using Firebase.Extensions;

public class AuthManager : MonoBehaviour
{
    [Header("로그인 UI")]
    [SerializeField]
    private GameObject loginOverlay;

    [SerializeField]
    private TMP_InputField loginIdInput;

    [SerializeField]
    private TMP_InputField loginPasswordInput;

    [SerializeField]
    private TMP_Text loginMessageText;


    [Header("회원가입 UI")]
    [SerializeField]
    private GameObject signUpOverlay;

    [SerializeField]
    private TMP_InputField signUpIdInput;

    [SerializeField]
    private TMP_InputField signUpPasswordInput;

    [SerializeField]
    private TMP_InputField signUpNicknameInput;

    [SerializeField]
    private TMP_Text signUpMessageText;

    [SerializeField]
    private Toggle autoLoginToggle;


    private FirebaseAuth auth;
    private FirebaseFirestore db;

    private void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        db = FirebaseFirestore.DefaultInstance;

        signUpOverlay.SetActive(false);

        ClearMessages();

        CheckAutoLogin();
    }


    // 회원가입 화면 열기
    public void OpenSignUp()
    {
        loginOverlay.SetActive(false);
        signUpOverlay.SetActive(true);

        ClearMessages();
    }


    // 로그인 화면으로 돌아가기
    public void BackToLogin()
    {
        signUpOverlay.SetActive(false);
        loginOverlay.SetActive(true);

        ClearMessages();
    }


    // 회원가입
    public void SignUp()
    {
        string userId =
            signUpIdInput.text.Trim();

        string password =
            signUpPasswordInput.text;

        string nickname =
            signUpNicknameInput.text.Trim();


        if (string.IsNullOrEmpty(userId))
        {
            signUpMessageText.text =
                "아이디를 입력해주세요.";
            return;
        }

        if (string.IsNullOrEmpty(password))
        {
            signUpMessageText.text =
                "비밀번호를 입력해주세요.";
            return;
        }

        if (password.Length < 6)
        {
            signUpMessageText.text =
                "비밀번호는 6자 이상이어야 합니다.";
            return;
        }

        if (string.IsNullOrEmpty(nickname))
        {
            signUpMessageText.text =
                "닉네임을 입력해주세요.";
            return;
        }


        // Firebase Auth는 이메일 형식을 요구하므로
        // 게임 아이디를 내부 이메일 형태로 변환
        string firebaseEmail =
            ConvertIdToEmail(userId);


        signUpMessageText.text =
            "회원가입 중...";


        auth.CreateUserWithEmailAndPasswordAsync(
            firebaseEmail,
            password
        )
        .ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted ||
                task.IsCanceled)
            {
                Debug.LogError(task.Exception);

                signUpMessageText.text =
                    "회원가입에 실패했습니다.";

                return;
            }


            FirebaseUser user =
                task.Result.User;


            SaveUserData(
                user,
                userId,
                nickname
            );
        });
    }


    // Firestore에 사용자 정보 저장
    private void SaveUserData(
        FirebaseUser user,
        string userId,
        string nickname)
    {
        Dictionary<string, object> userData =
            new Dictionary<string, object>()
            {
                { "userId", userId },
                { "nickname", nickname },
                { "bestWave", 0 }
            };


        db.Collection("users")
            .Document(user.UserId)
            .SetAsync(userData)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted ||
                    task.IsCanceled)
                {
                    Debug.LogError(task.Exception);

                    signUpMessageText.text =
                        "사용자 정보 저장에 실패했습니다.";

                    return;
                }


                Debug.Log(
                    $"회원가입 완료: {userId}"
                );


                signUpMessageText.text =
                    "회원가입이 완료되었습니다!";

                signUpIdInput.text = "";
                signUpPasswordInput.text = "";
                signUpNicknameInput.text = "";
            });
    }


    // 로그인
    public void Login()
    {
        string userId =
            loginIdInput.text.Trim();

        string password =
            loginPasswordInput.text;


        if (string.IsNullOrEmpty(userId))
        {
            loginMessageText.text =
                "아이디를 입력해주세요.";
            return;
        }

        if (string.IsNullOrEmpty(password))
        {
            loginMessageText.text =
                "비밀번호를 입력해주세요.";
            return;
        }


        string firebaseEmail =
            ConvertIdToEmail(userId);


        loginMessageText.text =
            "로그인 중...";


        auth.SignInWithEmailAndPasswordAsync(
            firebaseEmail,
            password
        )
        .ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted ||
                task.IsCanceled)
            {
                Debug.LogError(task.Exception);

                loginMessageText.text =
                    "아이디 또는 비밀번호를 확인해주세요.";

                return;
            }


            FirebaseUser user =
                task.Result.User;


            Debug.Log(
                $"로그인 성공! UID: {user.UserId}"
            );


            loginMessageText.text = "";
            
            if (autoLoginToggle.isOn)
            {
                PlayerPrefs.SetInt("AutoLogin", 1);
            }
            else
            {
                PlayerPrefs.SetInt("AutoLogin", 0);
            }

            PlayerPrefs.Save();

            loginOverlay.SetActive(false);
        });
    }


    // 게임 아이디 → Firebase 내부 이메일
    private string ConvertIdToEmail(
        string userId)
    {
        return userId.ToLower()
            + "@pokemongachadefense.local";
    }


    private void ClearMessages()
    {
        if (loginMessageText != null)
        {
            loginMessageText.text = "";
        }

        if (signUpMessageText != null)
        {
            signUpMessageText.text = "";
        }
    }

    private void CheckAutoLogin()
    {
        bool autoLogin =
            PlayerPrefs.GetInt("AutoLogin", 0) == 1;

        FirebaseUser currentUser =
            auth.CurrentUser;

        if (autoLogin && currentUser != null)
        {
            loginOverlay.SetActive(false);
        }
        else
        {
            loginOverlay.SetActive(true);
        }
    }
}