using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy 프리팹")]
    [SerializeField]
    private GameObject enemyPrefab;

    [Header("스폰 설정")]
    [SerializeField]
    private int spawnCount = 20;

    [SerializeField]
    private float spawnDuration = 30f;

    [Header("이동 경로")]
    [SerializeField]
    private Transform[] pathPoints;

    [Header("생성된 적 정리용 부모")]
    [SerializeField]
    private Transform enemiesParent;

    private void Start()
    {
        if (!ValidateSettings())
        {
            enabled = false;
            return;
        }

        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        float spawnInterval = spawnDuration / (spawnCount-1);

        for (int i = 0; i < spawnCount; i++)
        {
            SpawnEnemy();

            if (i < spawnCount - 1)
            {
                yield return new WaitForSeconds(spawnInterval);
            }
        }
    }

    private void SpawnEnemy()
    {
        Vector3 spawnPosition = pathPoints[0].position;
        //Point_00은 적 생성 위치로만 사용

        GameObject spawnedEnemy = Instantiate(
            enemyPrefab,
            spawnPosition,
            Quaternion.identity,
            enemiesParent
        );

        //방금 생성된 적의 EnemyMovement를 가져옴
        EnemyMovement enemyMovement =
            spawnedEnemy.GetComponent<EnemyMovement>();

        if (enemyMovement == null)
        {
            Debug.LogError(
                $"{spawnedEnemy.name}에 EnemyMovement가 없습니다.",
                spawnedEnemy
            );

            Destroy(spawnedEnemy);
            return;
        }

        //새로 생성된 Enemy에 이동 포인트들을 전달
        enemyMovement.SetPath(pathPoints);
    }

    private bool ValidateSettings()
    {
        if (enemyPrefab == null)
        {
            Debug.LogError(
                $"{gameObject.name}: Enemy Prefab이 등록되지 않았습니다.",
                this
            );

            return false;
        }

        if (pathPoints == null || pathPoints.Length < 2)
        {
            Debug.LogError(
                $"{gameObject.name}: Path Points를 2개 이상 등록해야 합니다.",
                this
            );

            return false;
        }

        if (pathPoints[0] == null)
        {
            Debug.LogError(
                $"{gameObject.name}: Point_00이 등록되지 않았습니다.",
                this
            );

            return false;
        }

        if (spawnCount <= 0)
        {
            Debug.LogError(
                $"{gameObject.name}: Spawn Count는 1 이상이어야 합니다.",
                this
            );

            return false;
        }

        if (spawnDuration < 0f)
        {
            Debug.LogError(
                $"{gameObject.name}: Spawn Duration은 0 이상이어야 합니다.",
                this
            );

            return false;
        }

        return true;
    }
}