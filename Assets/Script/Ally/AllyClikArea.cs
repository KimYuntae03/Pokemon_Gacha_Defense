using UnityEngine;
using UnityEngine.EventSystems;

public class AllyClickArea : MonoBehaviour,
    IPointerClickHandler,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler
    
{
    private AllyDrag allyDrag;
    private CelebiSkill celebiSkill;

    private void Awake()
    {
        allyDrag =
            GetComponentInParent<AllyDrag>();
        celebiSkill =
            GetComponentInParent<CelebiSkill>();
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

    public void OnPointerClick(
        PointerEventData eventData
    )
    {
        CelebiSkill celebiSkill =
            GetComponentInParent<CelebiSkill>();

        if (celebiSkill == null)
        {
            return;
        }

        celebiSkill.SelectSkill();
    }
}