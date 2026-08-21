using System.Collections.Generic;
using UnityEngine;

public class AllyGachaManager : MonoBehaviour
{
    [Header("뽑기에 등장할 아군 데이터")]
    [SerializeField]
    private AllyData[] allyDataList;

    [Header("가챠 로그 UI")]
    [SerializeField]
    private GachaLogUI gachaLogUI;

    public AllyData DrawAlly()
    {
        AllyGrade selectedGrade =
            DrawGrade();

        List<AllyData> candidates =
            new List<AllyData>();

        foreach (AllyData allyData in allyDataList)
        {
            if (allyData == null)
            {
                continue;
            }

            if (allyData.grade == selectedGrade)
            {
                candidates.Add(allyData);
            }
        }

        if (candidates.Count == 0)
        {
            return null;
        }


        int randomIndex =
            Random.Range(
                0,
                candidates.Count
            );


        AllyData selectedAlly =
            candidates[randomIndex];

        if (gachaLogUI != null)
        {
            gachaLogUI.AddLog(
                selectedAlly.allyName,
                selectedGrade,
                GetGradeProbability(selectedGrade)
            );
        }

        return selectedAlly;
    }


    private AllyGrade DrawGrade()
    {
        float roll =
            Random.Range(0f, 100f);


        if (roll < 50f)
        {
            return AllyGrade.Common;
        }

        if (roll < 80f)
        {
            return AllyGrade.Rare;
        }

        if (roll < 90f)
        {
            return AllyGrade.Ancient;
        }

        if (roll < 97f)
        {
            return AllyGrade.Relic;
        }

        if (roll < 99f)
        {
            return AllyGrade.EpicStory;
        }

        if (roll < 99.55f)
        {
            return AllyGrade.Legendary;
        }

        if (roll < 99.80f)
        {
            return AllyGrade.Epic;
        }

        if (roll < 99.95f)
        {
            return AllyGrade.Mythic;
        }

        return AllyGrade.Primordial;
    }

    private float GetGradeProbability(AllyGrade grade)
    {
        switch (grade)
        {
            case AllyGrade.Common:
                return 50f;

            case AllyGrade.Rare:
                return 30f;

            case AllyGrade.Ancient:
                return 10f;

            case AllyGrade.Relic:
                return 7f;

            case AllyGrade.EpicStory:
                return 2f;

            case AllyGrade.Legendary:
                return 0.55f;

            case AllyGrade.Epic:
                return 0.25f;

            case AllyGrade.Mythic:
                return 0.15f;

            case AllyGrade.Primordial:
                return 0.05f;

            default:
                return 0f;
        }
    }

    public AllyData DrawAllyByGrade(AllyGrade grade)
    {
        List<AllyData> candidates =
            new List<AllyData>();

        foreach (AllyData allyData in allyDataList)
        {
            if (allyData == null)
            {
                continue;
            }

            if (allyData.grade == grade)
            {
                candidates.Add(allyData);
            }
        }

        if (candidates.Count == 0)
        {
            return null;
        }

        int randomIndex =
            Random.Range(
                0,
                candidates.Count
            );

        return candidates[randomIndex];
    }
}