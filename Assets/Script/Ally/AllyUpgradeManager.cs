using UnityEngine;

public class AllyUpgradeManager : MonoBehaviour
{
    [Header("공격력 강화")]
    [SerializeField]
    private int attackUpgradeLevel = 0;

    [SerializeField]
    private float attackIncreasePerLevel = 0.05f;

    [Header("공격력 강화 비용")]
    [SerializeField]
    private int baseAttackUpgradeCost = 10;

    [SerializeField]
    private int attackUpgradeCostIncrease = 5;

    [SerializeField]
    private GoldOrbManager goldOrbManager;

    [Header("로그 UI")]
    [SerializeField]
    private GachaLogUI gachaLogUI;

    private float temporaryAttackMultiplier = 1f;

    public int GetAttackUpgradeCost()
    {
        return baseAttackUpgradeCost
            + attackUpgradeLevel * attackUpgradeCostIncrease;
    }

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
         return baseDamage
            * GetAttackMultiplier()
            * temporaryAttackMultiplier;
    }

    public bool UpgradeAttack()
    {
        if (goldOrbManager == null)
        {
            return false;
        }
         int cost = GetAttackUpgradeCost();

            if (!goldOrbManager.SpendGoldOrb(cost))
            {
                return false;
            }

        int previousLevel = attackUpgradeLevel;

        attackUpgradeLevel++;

        if (gachaLogUI != null)
        {
            int bonusPercent =
                Mathf.RoundToInt(
                    attackUpgradeLevel *
                    attackIncreasePerLevel *
                    100f
                );

            gachaLogUI.AddMessage(
                $"공격력 강화 Lv.{previousLevel} → Lv.{attackUpgradeLevel}"
            );
        }

        return true;
    }

    public void ApplyTemporaryAttackBuff(
        float buffAmount
    )
    {
        temporaryAttackMultiplier =
            1f + buffAmount;
    }


    public void RemoveTemporaryAttackBuff()
    {
        temporaryAttackMultiplier = 1f;
    }
    
}