using UnityEngine;

public class BossRewardManager : MonoBehaviour
{
    [Header("보상 시스템")]
    [SerializeField]
    private GoldOrbManager goldOrbManager;

    [SerializeField]
    private AllyGachaManager allyGachaManager;

    [SerializeField]
    private AllyManager allyManager;

    [Header("보상 로그")]
    [SerializeField]
    private GachaLogUI gachaLogUI;

    public void GiveBossReward(int waveNumber)
    {
        switch (waveNumber)
        {
            case 38:
                GiveReward(
                    30,
                    AllyGrade.Relic
                );
                break;

            case 48:
                GiveReward(
                    50,
                    AllyGrade.EpicStory
                );
                break;

            case 50:
                GiveReward(
                    100,
                    AllyGrade.EpicStory
                );
                break;
        }
    }


    private void GiveReward(
        int goldAmount,
        AllyGrade rewardGrade
    )
    {
        // 금구슬 지급
        if (goldOrbManager != null)
        {
            goldOrbManager.AddGoldOrb(
                goldAmount
            );
        }


        // 지정 등급에서 랜덤 유닛 선택
        if (allyGachaManager == null ||
            allyManager == null)
        {
            return;
        }

        AllyData rewardAlly =
            allyGachaManager.DrawAllyByGrade(
                rewardGrade
            );

        if (rewardAlly == null)
        {
            return;
        }


        // 실제 필드에 보상 유닛 생성
        GameObject spawnedAlly =
            allyManager.SpawnAlly(
                rewardAlly
            );

        if (spawnedAlly == null)
        {
            return;
        }

        if (gachaLogUI != null)
        {
            string gradeColor =
                GetGradeColor(rewardGrade);

            gachaLogUI.AddMessage(
                $"<color=#5FFFD2>[</color>" +
                $"보스 보상" +
                $"<color=#5FFFD2>]</color> " +

                $"<color=#FFD54A>금구슬 +{goldAmount}</color> / " +

                $"<color={gradeColor}>" +
                $"{GetGradeName(rewardGrade)}</color> " +

                $"{rewardAlly.allyName} 획득!"
            );
        }

        Debug.Log(
            $"보스 클리어 보상 / " +
            $"금구슬 +{goldAmount} / " +
            $"{rewardGrade} {rewardAlly.allyName} 획득"
        );
    }

    private string GetGradeName(AllyGrade grade)
    {
        switch (grade)
        {
            case AllyGrade.Common:
                return "일반";

            case AllyGrade.Rare:
                return "레어";

            case AllyGrade.Ancient:
                return "고대";

            case AllyGrade.Relic:
                return "유물";

            case AllyGrade.EpicStory:
                return "서사";

            case AllyGrade.Legendary:
                return "전설";

            case AllyGrade.Epic:
                return "에픽";

            case AllyGrade.Mythic:
                return "신화";

            case AllyGrade.Primordial:
                return "태초";

            default:
                return grade.ToString();
        }
    }

    private string GetGradeColor(AllyGrade grade)
    {
        switch (grade)
        {
            case AllyGrade.Common:
                return "#E5E5E5";

            case AllyGrade.Rare:
                return "#35C94A";

            case AllyGrade.Ancient:
                return "#E84BC5";

            case AllyGrade.Relic:
                return "#FF4242";

            case AllyGrade.EpicStory:
                return "#FF9A32";

            case AllyGrade.Legendary:
                return "#FFD83D";

            case AllyGrade.Epic:
                return "#26D9E8";

            case AllyGrade.Mythic:
                return "#FF3030";

            case AllyGrade.Primordial:
                return "#20D5C6";

            default:
                return "#FFFFFF";
        }
    }
}