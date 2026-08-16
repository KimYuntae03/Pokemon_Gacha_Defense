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

    private Transform directionCenter;
    private AllyAnimator allyAnimator;

    private bool isDragging; //아군 유닛을 드래그중인지 확인

    public bool IsDragging
    {
        get { return isDragging; }
    }

    public void SetAllyFloor(Tilemap tilemap)
    {
        allyFloorTilemap = tilemap;
    }

    private void Awake()
    {
        mainCamera = Camera.main;

        allyAnimator =
            GetComponent<AllyAnimator>();

        allyCollider =
            GetComponent<Collider2D>();
    }

    public void SetDirectionCenter(Transform center)
    {
        directionCenter = center;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 드래그 시작 전 위치 저장
        isDragging = true;

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
        isDragging = false;
        
        if (!IsInsideAllyFloor())
        {
            transform.position = originalPosition;

            return;
        }

        if (IsOverlappingOtherAlly())
        {
            // 겹쳤으면 원래 위치로 복귀
            transform.position = originalPosition;

            return;
        }
        
        UpdateDirection();

        Debug.Log(
            $"{gameObject.name} 배치 완료: {transform.position}"
        );
    }

    private bool IsInsideAllyFloor()
    {
        if (allyFloorTilemap == null)
        {
            return false;
        }

        if (allyCollider == null)
        {
            return false;
        }

        Bounds bounds =
            allyCollider.bounds;

        Vector3[] checkPoints =
        {
            bounds.center,

            new Vector3(
                bounds.min.x,
                bounds.min.y,
                transform.position.z
            ),

            new Vector3(
                bounds.min.x,
                bounds.max.y,
                transform.position.z
            ),

            new Vector3(
                bounds.max.x,
                bounds.min.y,
                transform.position.z
            ),

            new Vector3(
                bounds.max.x,
                bounds.max.y,
                transform.position.z
            )
        };


        foreach (Vector3 point in checkPoints)
        {
            Vector3Int cellPosition =
                allyFloorTilemap.WorldToCell(
                    point
                );

            if (!allyFloorTilemap.HasTile(cellPosition))
            {
                return false;
            }
        }


        return true;
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

    private void UpdateDirection()
    {
        if (directionCenter == null ||
            allyAnimator == null)
        {
            return;
        }

        Vector2 direction =
            transform.position -
            directionCenter.position;

        float angle =
            Mathf.Atan2(direction.y, direction.x)
            * Mathf.Rad2Deg;

        // 11시 ~ 1시
        if (angle >= 60f && angle < 120f)
        {
            allyAnimator.SetDirection(
                AllyDirection.Up
            );
        }
        // 7시 ~ 11시
        else if (angle >= 120f ||
                angle < -120f)
        {
            allyAnimator.SetDirection(
                AllyDirection.Left
            );
        }
        // 5시 ~ 7시
        else if (angle >= -120f &&
                angle < -60f)
        {
            allyAnimator.SetDirection(
                AllyDirection.Down
            );
        }
        // 1시 ~ 5시
        else
        {
            allyAnimator.SetDirection(
                AllyDirection.Right
            );
        }
    }
}