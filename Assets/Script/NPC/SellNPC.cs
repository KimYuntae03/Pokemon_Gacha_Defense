using UnityEngine;
using UnityEngine.EventSystems;

public class SellNPC : MonoBehaviour,
    IPointerClickHandler
{
    [SerializeField]
    private GameObject sellPanel;


    public void OnPointerClick(
        PointerEventData eventData
    )
    {
        if (sellPanel == null)
        {
            return;
        }

        sellPanel.SetActive(
            !sellPanel.activeSelf
        );
    }
}