using UnityEngine;
using UnityEngine.EventSystems;

public class SellNPC : MonoBehaviour,
    IPointerClickHandler
{
    [SerializeField]
    private SellUIController sellUIController;


     public void OnPointerClick(
        PointerEventData eventData
    )
    {
        if (sellUIController == null)
        {
            return;
        }

        sellUIController.OpenSellUI();
    }
}