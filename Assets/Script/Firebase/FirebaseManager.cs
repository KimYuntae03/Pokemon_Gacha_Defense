using UnityEngine;
using Firebase;
using Firebase.Extensions;

public class FirebaseManager : MonoBehaviour
{
    private void Start()
    {
        CheckFirebaseDependencies();
    }

    private void CheckFirebaseDependencies()
    {
        FirebaseApp.CheckAndFixDependenciesAsync()
            .ContinueWithOnMainThread(task =>
            {
                DependencyStatus dependencyStatus =
                    task.Result;

                if (dependencyStatus ==
                    DependencyStatus.Available)
                {
                    Debug.Log(
                        "Firebase 연결 준비 완료!"
                    );
                }
                else
                {
                    Debug.LogError(
                        "Firebase 초기화 실패: " +
                        dependencyStatus
                    );
                }
            });
    }
}