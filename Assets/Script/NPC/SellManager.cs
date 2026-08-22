using UnityEngine;

public class SellManager : MonoBehaviour
{
    [Header("참조")]
    [SerializeField]
    private Transform alliesParent;

    [SerializeField]
    private AllyManager allyManager;

    [SerializeField]
    private GoldOrbManager goldOrbManager;

    [SerializeField]
    private GachaLogUI gachaLogUI;

    public void SellCommon()
    {
        SellOneByGrade(
            AllyGrade.Common,
            3
        );
    }

    public void SellRare()
    {
        SellOneByGrade(
            AllyGrade.Rare,
            6
        );
    }

    public void SellAncient()
    {
        SellOneByGrade(
            AllyGrade.Ancient,
            9
        );
    }

    public void SellRelic()
    {
        SellOneByGrade(
            AllyGrade.Relic,
            16
        );
    }


    private void SellOneByGrade(
        AllyGrade grade,
        int reward
    )
    {
        foreach (Transform child in alliesParent)
        {
            AllyUnit allyUnit =
                child.GetComponent<AllyUnit>();

            if (allyUnit == null ||
                allyUnit.Data == null)
            {
                continue;
            }

            if (allyUnit.Data.grade != grade)
            {
                continue;
            }

            goldOrbManager.AddGoldOrb(
                reward
            );

            allyManager.RemoveAlly(
                child.gameObject
            );
            if (gachaLogUI != null)
            {
                gachaLogUI.AddMessage(
                    $"+{reward}"
                );
            }

            Debug.Log(
                $"{allyUnit.Data.allyName} 판매 / 금구슬 +{reward}"
            );

            return;
        }
    }
}