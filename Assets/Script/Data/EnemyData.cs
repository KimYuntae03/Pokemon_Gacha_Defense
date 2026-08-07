using UnityEngine;

[CreateAssetMenu(
    fileName = "NewEnemyData",
    menuName = "Pokemon Gacha Defense/Enemy Data"
)]
public class EnemyData : ScriptableObject
{
    [Header("기본 정보")]
    public string enemyName; //포켓몬 이름

    [Header("능력치")]
    public float maxHealth = 100f; //최대 Hp

    public float moveSpeed = 2f; 

    [Header("애니메이션")]
    public AnimatorOverrideController animatorOverrideController;
}