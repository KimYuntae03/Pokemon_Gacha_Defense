using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class AllyDrag : MonoBehaviour,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler
{
    [Header("겹침 검사")]
    [SerializeField]
    private LayerMask allyLayer;

    private Tilemap allyFloorTilemap;

    private Camera mainCamera;

    private Vector3 originalPosition;

    private Collider2D allyCollider;

    public void SetAllyFloor(Tilemap tilemap)
    {
        allyFloorTilemap = tilemap;
    }

    private void Awake()
    {
        mainCamera = Camera.main;

        allyCollider =
            GetComponent<Collider2D>();
    }


    public void OnBeginDrag(
        PointerEventData eventData
    )
    {
        // 드래그 시작 전 위치 저장
        originalPosition =
            transform.position;
    }


    public void OnDrag(
        PointerEventData eventData
    )
    {
        Vector3 mousePosition =
            mainCamera.ScreenToWorldPoint(
                eventData.position
            );

        mousePosition.z =
            transform.position.z;

        transform.position =
            mousePosition;
    }


    public void OnEndDrag(PointerEventData eventData)
    {
        if (!IsInsideAllyFloor())
        {
            transform.position = originalPosition;

            Debug.Log(
                $"{gameObject.name}: 배치 가능 영역 밖입니다."
            );

            return;
        }

        if (IsOverlappingOtherAlly())
        {
            // 겹쳤으면 원래 위치로 복귀
            transform.position =
                originalPosition;

            Debug.Log(
                $"{gameObject.name}: 다른 아군과 겹쳐서 배치할 수 없습니다."
            );

            return;
        }

        Debug.Log(
            $"{gameObject.name} 배치 완료: {transform.position}"
        );
    }

    private bool IsInsideAllyFloor()
    {
        if (allyFloorTilemap == null)
        {
            Debug.LogError(
                $"{gameObject.name}: AllyFloor Tilemap이 등록되지 않았습니다.",
                this
            );

            return false;
        }

        Vector3Int cellPosition =
            allyFloorTilemap.WorldToCell(
                transform.position
            );

        return allyFloorTilemap.HasTile(cellPosition);
    }


    private bool IsOverlappingOtherAlly()
    {
        if (allyCollider == null)
        {
            return false;
        }


        Collider2D[] hits =
            Physics2D.OverlapBoxAll(
                allyCollider.bounds.center,
                allyCollider.bounds.size * 0.9f,
                0f,
                allyLayer
            );


        foreach (Collider2D hit in hits)
        {
            /*
             * 자기 자신의 Collider는 무시한다.
             */
            if (hit == allyCollider)
            {
                continue;
            }

            return true;
        }


        return false;
    }
}