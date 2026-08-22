using UnityEngine;

using Firebase.Auth;
using Firebase.Firestore;
using Firebase.Extensions;

public class PlayerRecordManager : MonoBehaviour
{
    private FirebaseAuth auth;
    private FirebaseFirestore db;


    private void Awake()
    {
        auth = FirebaseAuth.DefaultInstance;
        db = FirebaseFirestore.DefaultInstance;
    }


    public void SaveBestWave(int reachedWave)
    {
        FirebaseUser currentUser =
            auth.CurrentUser;

        if (currentUser == null)
        {
            Debug.LogWarning(
                "로그인된 사용자가 없어 Wave 기록을 저장할 수 없습니다."
            );

            return;
        }


        DocumentReference userDocument =
            db.Collection("users")
              .Document(currentUser.UserId);


        /*
         * 서버에 저장되어 있는 기존 최고 Wave를 먼저 읽는다.
         */
        userDocument.GetSnapshotAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled ||
                    task.IsFaulted)
                {
                    Debug.LogError(
                        "기존 Wave 기록 불러오기 실패\n" +
                        task.Exception
                    );

                    return;
                }


                DocumentSnapshot snapshot =
                    task.Result;


                if (!snapshot.Exists)
                {
                    Debug.LogWarning(
                        "Firestore에 사용자 데이터가 없습니다."
                    );

                    return;
                }


                long savedBestWave = 0;


                if (snapshot.ContainsField("bestWave"))
                {
                    savedBestWave =
                        snapshot.GetValue<long>(
                            "bestWave"
                        );
                }


                /*
                 * 이번 기록이 기존 최고 기록보다
                 * 낮거나 같으면 서버를 수정하지 않는다.
                 */
                if (reachedWave <= savedBestWave)
                {
                    Debug.Log(
                        $"최고 Wave 갱신 없음 / " +
                        $"기존: {savedBestWave} / " +
                        $"이번: {reachedWave}"
                    );

                    return;
                }


                /*
                 * 신기록일 경우에만 bestWave 갱신
                 */
                userDocument.UpdateAsync(
                    "bestWave",
                    reachedWave
                )
                .ContinueWithOnMainThread(
                    updateTask =>
                    {
                        if (updateTask.IsCanceled ||
                            updateTask.IsFaulted)
                        {
                            Debug.LogError(
                                "최고 Wave 저장 실패\n" +
                                updateTask.Exception
                            );

                            return;
                        }


                        Debug.Log(
                            $"최고 Wave 갱신 완료! " +
                            $"{savedBestWave} → {reachedWave}"
                        );
                    }
                );
            });
    }
}