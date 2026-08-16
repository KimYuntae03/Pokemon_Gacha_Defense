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

    [Header("전투 능력치")]
    public float attackDamage = 10f; //공격력

    public float attackInterval = 1f; //쿨타임

    public float attackRange = 4f; //사거리
    
    [Header("공격 프리팹")]
    public GameObject attackPrefab;

    [Header("공격 방식")]
    public AllyAttackType attackType;   

    [Header("전역 공격")]
    public bool useGlobalAttack = false;

    public GameObject globalAttackPrefab;

    public float globalAttackInterval = 3f;

    public float globalAttackDamage = 10f;
}