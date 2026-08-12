using UnityEngine;
using UnityEngine.Tilemaps;

public class AllyManager : MonoBehaviour
{
    [Header("아군 생성 설정")]
    [SerializeField]
    private Transform allySpawnCenter;

    [SerializeField]
    private Transform alliesParent;

    [Header("아군 최대 수")]
    [SerializeField]
    private int maxAllyCount = 80;

    [Header("생성 위치 간격")]
    [SerializeField]
    private float spawnSpacing = 1f;

    [Header("아군 겹침 검사")]
    [SerializeField]
    private LayerMask allyLayer;

    [Header("아군 배치 가능 영역")]
    [SerializeField]
    private Tilemap allyFloorTilemap;

    [SerializeField]
    private float spawnCheckRadius = 0.6f;

    [SerializeField]
    private int maxSearchRing = 10;

    private int currentAllyCount;


    public bool CanSpawnAlly()
    {
        return currentAllyCount < maxAllyCount;
    }


    public GameObject SpawnAlly(AllyData allyData)
    {
        if (allyData == null)
        {
            Debug.LogError(
                $"{gameObject.name}: AllyData가 전달되지 않았습니다.",
                this
            );

            return null;
        }

        if (!CanSpawnAlly())
        {
            Debug.LogWarning(
                $"아군 최대 수 {maxAllyCount}마리에 도달했습니다."
            );

            return null;
        }

        if (allyData.allyPrefab == null)
        {
            Debug.LogError(
                $"{allyData.allyName}: Ally Prefab이 등록되지 않았습니다.",
                allyData
            );

            return null;
        }

        Vector3 spawnPosition =
            FindSpawnPosition();

        GameObject spawnedAlly =
            Instantiate(
                allyData.allyPrefab,
                spawnPosition,
                Quaternion.identity,
                alliesParent
            );

        spawnedAlly.name =
            allyData.allyName;

        AllyAnimator allyAnimator =
            spawnedAlly.GetComponent<AllyAnimator>();

        if (allyAnimator == null)
        {
            Debug.LogError(
                $"{spawnedAlly.name}: AllyAnimator가 없습니다.",
                spawnedAlly
            );

            Destroy(spawnedAlly);
            return null;
        }

        allyAnimator.SetAnimatorController(
            allyData.animatorOverrideController
        );
        AllyDrag allyDrag = spawnedAlly.GetComponent<AllyDrag>();

        if (allyDrag == null)
        {
            Debug.LogError(
                $"{spawnedAlly.name}: AllyDrag가 없습니다.",
                spawnedAlly
            );

            Destroy(spawnedAlly);
            return null;
        }

        allyDrag.SetAllyFloor(
            allyFloorTilemap
        );
        allyDrag.SetDirectionCenter(
            allySpawnCenter
        );

        currentAllyCount++;

        Debug.Log(
            $"아군 생성: {allyData.allyName} " +
            $"({currentAllyCount}/{maxAllyCount})"
        );

        return spawnedAlly;
    }


    private Vector3 FindSpawnPosition()
    {
        if (currentAllyCount == 0 &&
            IsPositionFree(allySpawnCenter.position))
        {
            return allySpawnCenter.position;
        }

        for (int ring = 1; ring <= maxSearchRing; ring++)
        {
            for (int x = -ring; x <= ring; x++)
            {
                for (int y = -ring; y <= ring; y++)
                {
                    if (
                        Mathf.Abs(x) != ring &&
                        Mathf.Abs(y) != ring
                    )
                    {
                        continue;
                    }

                    Vector3 candidatePosition =
                        allySpawnCenter.position
                        + new Vector3(
                            x * spawnSpacing,
                            y * spawnSpacing,
                            0f
                        );

                    if (IsPositionFree(candidatePosition))
                    {
                        return candidatePosition;
                    }
                }
            }
        }

        Debug.LogWarning(
            "빈 생성 위치를 찾지 못했습니다. SpawnCenter에 생성합니다."
        );

        return allySpawnCenter.position;
    }


    private bool IsPositionFree(Vector3 position)
    {
        Collider2D hit =
            Physics2D.OverlapCircle(
                position,
                spawnCheckRadius,
                allyLayer
            );

        return hit == null;
    }


    public void RemoveAlly(GameObject ally)
    {
        if (ally == null)
        {
            return;
        }

        Destroy(ally);

        currentAllyCount--;

        if (currentAllyCount < 0)
        {
            currentAllyCount = 0;
        }
    }
}