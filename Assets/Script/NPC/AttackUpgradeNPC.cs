using UnityEngine;
using UnityEngine.EventSystems;

public class AttackUpgradeNPC : MonoBehaviour,
    IPointerClickHandler
{
    [SerializeField]
    private AttackUpgradeUIController upgradeUIController;

    public void OnPointerClick(
        PointerEventData eventData
    )
    {
        if (upgradeUIController == null)
        {
            return;
        }

        upgradeUIController.OpenUpgradeUI();
    }
}