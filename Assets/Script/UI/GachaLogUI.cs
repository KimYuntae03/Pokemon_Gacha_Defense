using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GachaLogUI : MonoBehaviour
{
    [Header("로그 UI")]
    [SerializeField]
    private TMP_Text logText;

    [Header("최대 표시 개수")]
    [SerializeField]
    private int maxLogCount = 6;

    [Header("로그 유지 시간")]
    [SerializeField]
    private float logLifeTime = 5f;


    private class GachaLogEntry
    {
        public string message;
        public float createdTime;

        public GachaLogEntry(
            string newMessage,
            float newCreatedTime
        )
        {
            message = newMessage;
            createdTime = newCreatedTime;
        }
    }


    private readonly List<GachaLogEntry> logs =
        new List<GachaLogEntry>();


    private void Update()
    {
        bool removed = false;

        for (int i = logs.Count - 1; i >= 0; i--)
        {
            if (
                Time.time - logs[i].createdTime
                >= logLifeTime
            )
            {
                logs.RemoveAt(i);
                removed = true;
            }
        }

        if (removed)
        {
            RefreshUI();
        }
    }


    public void AddLog(
        string allyName,
        AllyGrade grade,
        float probability
    )
    {
        string gradeName =
            GetGradeDisplayName(grade);

        string gradeColor =
            GetGradeColor(grade);

        string coloredGrade =
            $"<color={gradeColor}>[{gradeName}]</color>";

        string newLog;


        if (grade >= AllyGrade.Legendary)
        {
            newLog =
                CreateSpecialLog(
                    allyName,
                    grade,
                    coloredGrade,
                    probability
                );
        }
        else
        {
            string endStar =
                grade == AllyGrade.EpicStory
                ? " ★"
                : "";

            newLog =
                $"{allyName} {coloredGrade} {probability:F2}%{endStar}";
        }


        logs.Add(
            new GachaLogEntry(
                newLog,
                Time.time
            )
        );


        if (logs.Count > maxLogCount)
        {
            logs.RemoveAt(0);
        }


        RefreshUI();
    }


    private string CreateSpecialLog(
        string allyName,
        AllyGrade grade,
        string coloredGrade,
        float probability
    )
    {
        string separator =
            "────────────────────────────";

        string stars =
            GetGradeStars(grade);

        string message =
            $"플레이어가 {probability:F2}%의 확률로 " +
            $"{allyName} {coloredGrade} 을 획득하였습니다";


        if (!string.IsNullOrEmpty(stars))
        {
            string starColor =
                GetGradeColor(grade);

            string coloredStars =
                $"<color={starColor}>{stars}</color>";

            message =
                $"{coloredStars} {message} {coloredStars}";
        }


        return
            $"{separator}\n" +
            $"{message}\n" +
            $"{separator}";
    }


    private string GetGradeDisplayName(
        AllyGrade grade
    )
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


    private string GetGradeColor(
        AllyGrade grade
    )
    {
        switch (grade)
        {
            case AllyGrade.Common:
                return "#F2F2F2";

            case AllyGrade.Rare:
                return "#32C8D0";

            case AllyGrade.Ancient:
                return "#A26AC7";

            case AllyGrade.Relic:
                return "#FF7045";

            case AllyGrade.EpicStory:
                return "#C5BED1";

            case AllyGrade.Legendary:
                return "#FFB52E";

            case AllyGrade.Epic:
                return "#35BADA";

            case AllyGrade.Mythic:
                return "#FF5A45";

            case AllyGrade.Primordial:
                return "#32D2E8";

            default:
                return "#FFFFFF";
        }
    }


    private string GetGradeStars(AllyGrade grade)
    {
        switch (grade)
        {
            case AllyGrade.Epic:
                return "★";

            case AllyGrade.Mythic:
                return "★★";

            case AllyGrade.Primordial:
                return "★★★";

            default:
                return "";
        }
    }

    private void RefreshUI()
    {
        if (logText == null)
        {
            return;
        }

        List<string> messages =
            new List<string>();

        foreach (GachaLogEntry log in logs)
        {
            messages.Add(
                log.message
            );
        }

        logText.text =
            string.Join(
                "\n",
                messages
            );
    }
}