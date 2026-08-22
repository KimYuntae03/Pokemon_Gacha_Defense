using UnityEngine;
using TMPro;

using Firebase.Auth;
using Firebase.Firestore;
using Firebase.Extensions;

public class RankingManager : MonoBehaviour
{
    [Header("랭킹 UI")]
    [SerializeField]
    private TMP_Text rank1Text;

    [SerializeField]
    private TMP_Text rank2Text;

    [SerializeField]
    private TMP_Text rank3Text;

    [SerializeField]
    private TMP_Text rank4Text;

    [SerializeField]
    private TMP_Text rank5Text;

    [SerializeField]
    private TMP_Text myRankText;


    private FirebaseAuth auth;
    private FirebaseFirestore db;

    private TMP_Text[] rankTexts;


    private void Awake()
    {
        auth = FirebaseAuth.DefaultInstance;
        db = FirebaseFirestore.DefaultInstance;

        rankTexts = new TMP_Text[]
        {
            rank1Text,
            rank2Text,
            rank3Text,
            rank4Text,
            rank5Text
        };
    }


    public void LoadRanking()
    {
        /*
         * bestWave가 높은 순서대로 정렬한 뒤
         * 상위 5명만 가져온다.
         */
        Query rankingQuery =
            db.Collection("users")
              .OrderByDescending("bestWave")
              .Limit(5);


        rankingQuery.GetSnapshotAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled ||
                    task.IsFaulted)
                {
                    Debug.LogError(
                        "랭킹 불러오기 실패\n" +
                        task.Exception
                    );

                    return;
                }


                QuerySnapshot snapshot =
                    task.Result;


                ClearRankingTexts();


                int rank = 0;


                foreach (DocumentSnapshot document
                         in snapshot.Documents)
                {
                    if (rank >= rankTexts.Length)
                    {
                        break;
                    }


                    string nickname =
                        document.ContainsField("nickname")
                        ? document.GetValue<string>("nickname")
                        : "Unknown";


                    long bestWave =
                        document.ContainsField("bestWave")
                        ? document.GetValue<long>("bestWave")
                        : 0;


                    string rankColor;

                    switch (rank + 1)
                    {
                        case 1:
                            rankColor = "#FFD700"; // 금색
                            break;

                        case 2:
                            rankColor = "#C0C0C0"; // 은색
                            break;

                        case 3:
                            rankColor = "#CD7F32"; // 동색
                            break;

                        default:
                            rankColor = "#FFFFFF"; // 흰색
                            break;
                    }

                    rankTexts[rank].text =
                        $"<color={rankColor}>{rank + 1}위</color>" +
                        $"<pos=40%>{nickname}" +
                        $"<pos=78%>{bestWave}";

                    rank++;
                }


                LoadMyRecord();
            });
    }


    private void LoadMyRecord()
    {
        FirebaseUser currentUser =
            auth.CurrentUser;


        if (currentUser == null)
        {
            myRankText.text =
                "내 기록을 불러올 수 없습니다.";

            return;
        }


        db.Collection("users")
          .Document(currentUser.UserId)
          .GetSnapshotAsync()
          .ContinueWithOnMainThread(task =>
          {
              if (task.IsCanceled ||
                  task.IsFaulted)
              {
                  Debug.LogError(
                      "내 기록 불러오기 실패\n" +
                      task.Exception
                  );

                  return;
              }


              DocumentSnapshot document =
                  task.Result;


              if (!document.Exists)
              {
                  myRankText.text =
                      "내 기록 없음";

                  return;
              }


              string nickname =
                  document.GetValue<string>(
                      "nickname"
                  );


              long bestWave =
                  document.GetValue<long>(
                      "bestWave"
                  );


              myRankText.text =
                  $"내 기록     {nickname}     " +
                  $"WAVE {bestWave}";
          });
    }


    private void ClearRankingTexts()
    {
        foreach (TMP_Text text in rankTexts)
        {
            if (text != null)
            {
                text.text = "";
            }
        }
    }
}