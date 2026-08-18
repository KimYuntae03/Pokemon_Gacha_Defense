using UnityEngine;
using UnityEngine.EventSystems;

public class AttackUpgradeNPC : MonoBehaviour,
    IPointerClickHandler
{
    [SerializeField]
    private GameObject attackUpgradePanel;


    public void OnPointerClick(
        PointerEventData eventData
    )
    {
        if (attackUpgradePanel == null)
        {
            return;
        }

        attackUpgradePanel.SetActive(
            !attackUpgradePanel.activeSelf
        );
    }
}