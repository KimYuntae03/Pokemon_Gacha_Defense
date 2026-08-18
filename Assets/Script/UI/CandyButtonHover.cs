using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CandyButtonHover : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler
{
    [SerializeField]
    private Outline candyOutline;


    private void Awake()
    {
        if (candyOutline != null)
        {
            candyOutline.enabled = false;
        }
    }


    public void OnPointerEnter(
        PointerEventData eventData
    )
    {
        if (candyOutline != null)
        {
            candyOutline.enabled = true;
        }
    }


    public void OnPointerExit(
        PointerEventData eventData
    )
    {
        if (candyOutline != null)
        {
            candyOutline.enabled = false;
        }
    }
}