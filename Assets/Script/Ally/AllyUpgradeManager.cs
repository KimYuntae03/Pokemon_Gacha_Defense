using UnityEngine;

public class AllyUpgradeManager : MonoBehaviour
{
    [Header("공격력 강화")]
    [SerializeField]
    private int attackUpgradeLevel = 0;

    [SerializeField]
    private float attackIncreasePerLevel = 0.05f;

    public int AttackUpgradeLevel
    {
        get { return attackUpgradeLevel; }
    }


    public float GetAttackMultiplier()
    {
        return 1f +
            attackUpgradeLevel *
            attackIncreasePerLevel;
    }


    public float GetUpgradedDamage(
        float baseDamage
    )
    {
        return baseDamage *
            GetAttackMultiplier();
    }

    public void UpgradeAttack()
    {
        attackUpgradeLevel++;
        Debug.Log(
            $"공격력 강화 Lv.{attackUpgradeLevel} / " +
            $"공격력 +{attackUpgradeLevel * attackIncreasePerLevel * 100f:0}%"
        );
    }
}