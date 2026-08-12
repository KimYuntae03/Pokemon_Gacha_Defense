using UnityEngine;

public enum AllyGrade
{
    Common,
    Rare,
    Ancient,
    Relic,
    EpicStory,
    Legendary,
    Epic,
    Mythic,
    Primordial
}

[CreateAssetMenu(
    fileName = "NewAllyData",
    menuName = "Pokemon Gacha Defense/Ally Data"
)]
public class AllyData : ScriptableObject
{
    [Header("기본 정보")]
    public string allyName;

    public AllyGrade grade;

    [Header("생성 정보")]
    public GameObject allyPrefab;

    [Header("애니메이션")]
    public AnimatorOverrideController animatorOverrideController;
}