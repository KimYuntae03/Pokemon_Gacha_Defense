using UnityEngine;
using UnityEngine.EventSystems;

public class AllyClickArea : MonoBehaviour,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler
{
    private AllyDrag allyDrag;


    private void Awake()
    {
        allyDrag =
            GetComponentInParent<AllyDrag>();
    }


    public void OnBeginDrag(
        PointerEventData eventData
    )
    {
        if (allyDrag == null)
        {
            return;
        }

        allyDrag.OnBeginDrag(
            eventData
        );
    }


    public void OnDrag(
        PointerEventData eventData
    )
    {
        if (allyDrag == null)
        {
            return;
        }

        allyDrag.OnDrag(
            eventData
        );
    }


    public void OnEndDrag(
        PointerEventData eventData
    )
    {
        if (allyDrag == null)
        {
            return;
        }

        allyDrag.OnEndDrag(
            eventData
        );
    }
}