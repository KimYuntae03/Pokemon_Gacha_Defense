using System.Collections;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy 프리팹")]
    [SerializeField]
    private GameObject enemyPrefab;

    [Header("이동 경로")]
    [SerializeField]
    private Transform[] pathPoints;

    [Header("생성된 적 정리용 부모")]
    [SerializeField]
    private Transform enemiesParent;

    public IEnumerator SpawnWave(WaveData waveData)
    {
        if (!ValidateSettings())
        {
            yield break;
        }

        if (waveData == null)
        {
            Debug.LogError(
                $"{gameObject.name}: WaveData가 없습니다.",
                this
            );

            yield break;
        }

        if (waveData.enemyData == null)
        {
            Debug.LogError(
                $"{gameObject.name}: WaveData에 EnemyData가 없습니다.",
                this
            );

            yield break;
        }


        int spawnCount = waveData.spawnCount;
        float spawnInterval = waveData.spawnInterval;


        for (int i = 0; i < spawnCount; i++)
        {
            SpawnEnemy(waveData.enemyData);

            if (i < spawnCount - 1)
            {
                yield return new WaitForSeconds(spawnInterval);
            }
        }
    }
    private void SpawnEnemy(EnemyData enemyData)
    {
        // Point_00 위치에 적 생성
        Vector3 spawnPosition = pathPoints[0].position;

        GameObject spawnedEnemy = Instantiate(
            enemyPrefab,
            spawnPosition,
            Quaternion.identity,
            enemiesParent
        );

        // -------------------------
        // 이동 경로 전달
        // -------------------------

        EnemyMovement enemyMovement =
            spawnedEnemy.GetComponent<EnemyMovement>();

        if (enemyMovement == null)
        {
            Debug.LogError(
                $"{spawnedEnemy.name}: EnemyMovement가 없습니다.",
                spawnedEnemy
            );

            Destroy(spawnedEnemy);
            return;
        }

        enemyMovement.SetPath(pathPoints);


        // -------------------------
        // 애니메이션 교체
        // -------------------------

        EnemyAnimator enemyAnimator =
            spawnedEnemy.GetComponent<EnemyAnimator>();

        if (enemyAnimator == null)
        {
            Debug.LogError(
                $"{spawnedEnemy.name}: EnemyAnimator가 없습니다.",
                spawnedEnemy
            );

            Destroy(spawnedEnemy);
            return;
        }

        enemyAnimator.SetAnimatorController(
            enemyData.animatorOverrideController
        );
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

        return true;
    }
}