using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;

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

    [Header("아군 공격")]
    [SerializeField]
    private LayerMask enemyLayer;

    [Header("전역 공격 위치")]
    [SerializeField]
    private Transform[] globalAttackPoints;

    private int currentAllyCount;

[Header("임시 테스트")]
[SerializeField]
private AllyData testArceusData;

    public bool CanSpawnAlly()
    {
        return currentAllyCount < maxAllyCount;
    }
    private void Update()
{
    if (Keyboard.current == null)
    {
        return;
    }

    if (Keyboard.current.digit1Key.wasPressedThisFrame)
    {
        SpawnAlly(testArceusData);
    }
}

    public GameObject SpawnAlly(AllyData allyData)
    {
        if (allyData == null)
        {
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
            return null;
        }

        Vector3 spawnPosition = FindSpawnPosition();

        GameObject spawnedAlly = Instantiate(
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
            Destroy(spawnedAlly);
            return null;
        }

        allyAnimator.SetAnimatorController(
            allyData.animatorOverrideController
        );
        AllyDrag allyDrag = spawnedAlly.GetComponent<AllyDrag>();

        if (allyDrag == null)
        {
            Destroy(spawnedAlly);
            return null;
        }

        allyDrag.SetAllyFloor(
            allyFloorTilemap
        );
        allyDrag.SetDirectionCenter(
            allySpawnCenter
        );

        AllyAttack allyAttack =
            spawnedAlly.GetComponent<AllyAttack>();

        if (allyAttack == null)
        {
            Destroy(spawnedAlly);
            return null;
        }

        allyAttack.Initialize(
            allyData,
            enemyLayer,
            globalAttackPoints
        );

        AllyUnit allyUnit = spawnedAlly.GetComponent<AllyUnit>();

        if (allyUnit == null)
        {
            Destroy(spawnedAlly);
            return null;
        }

        allyUnit.Initialize(
            allyData
        );

        currentAllyCount++;

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