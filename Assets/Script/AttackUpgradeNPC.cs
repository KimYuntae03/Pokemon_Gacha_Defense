using UnityEngine;
using UnityEngine.EventSystems;

public class AttackUpgradeNPC : MonoBehaviour,
    IPointerClickHandler
{
    [Header("공격력 강화 시스템")]
    [SerializeField]
    private AllyUpgradeManager allyUpgradeManager;


    public void OnPointerClick(
        PointerEventData eventData
    )
    {
        if (allyUpgradeManager == null)
        {
            Debug.LogError(
                $"{gameObject.name}: AllyUpgradeManager가 연결되지 않았습니다.",
                this
            );

            return;
        }

        allyUpgradeManager.UpgradeAttack();
    }
}