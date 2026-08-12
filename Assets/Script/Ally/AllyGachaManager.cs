using System.Collections.Generic;
using UnityEngine;

public class AllyGachaManager : MonoBehaviour
{
    [Header("뽑기에 등장할 아군 데이터")]
    [SerializeField]
    private AllyData[] allyDataList;


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
            Debug.LogWarning(
                $"{selectedGrade} 등급에 등록된 아군이 없습니다."
            );

            return null;
        }


        int randomIndex =
            Random.Range(
                0,
                candidates.Count
            );


        AllyData selectedAlly =
            candidates[randomIndex];


        Debug.Log(
            $"뽑기 결과: [{selectedGrade}] {selectedAlly.allyName}"
        );


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
}