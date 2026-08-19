using TMPro;
using UnityEngine;

public class AttackUpgradeUI : MonoBehaviour
{
    [Header("강화 시스템")]
    [SerializeField]
    private AllyUpgradeManager allyUpgradeManager;

    [Header("UI 텍스트")]
    [SerializeField]
    private TMP_Text levelText;

    [SerializeField]
    private TMP_Text costText;


    private void OnEnable()
    {
        RefreshUI();
    }


    public void UpgradeAttack()
    {
        if (allyUpgradeManager == null)
        {
            return;
        }

        bool success =
            allyUpgradeManager.UpgradeAttack();

        if (success)
        {
            RefreshUI();
        }
    }


    private void RefreshUI()
    {
        if (allyUpgradeManager == null)
        {
            return;
        }

        int currentLevel =
            allyUpgradeManager.AttackUpgradeLevel;

        int nextLevel =
            currentLevel + 1;

        int cost =
            allyUpgradeManager.GetAttackUpgradeCost();


        levelText.text =
            $"Lv.{currentLevel} → Lv.{nextLevel}";

        costText.text =
            $"강화비용 : {cost}";
    }
}