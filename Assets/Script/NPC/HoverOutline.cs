using UnityEngine;
using UnityEngine.EventSystems;

public class HoverOutline : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler
{
    [SerializeField]
    private GameObject outlineObject;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (outlineObject != null)
        {
            outlineObject.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (outlineObject != null)
        {
            outlineObject.SetActive(false);
        }
    }
}