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
            yield break;
        }

        if (waveData.enemyData == null)
        {
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

        // 이동 경로 전달

        EnemyMovement enemyMovement =
            spawnedEnemy.GetComponent<EnemyMovement>();

        if (enemyMovement == null)
        {
            Destroy(spawnedEnemy);
            return;
        }

        enemyMovement.SetPath(pathPoints);

        // 애니메이션 교체
        EnemyAnimator enemyAnimator =
            spawnedEnemy.GetComponent<EnemyAnimator>();

        if (enemyAnimator == null)
        {
            Destroy(spawnedEnemy);
            return;
        }

        enemyAnimator.SetAnimatorController(
            enemyData.animatorOverrideController
        );
        
        enemyMovement.SetSpeed(
            enemyData.moveSpeed
        );

        EnemyHealth enemyHealth =
        spawnedEnemy.GetComponent<EnemyHealth>();

        if (enemyHealth == null)
        {
            Destroy(spawnedEnemy);
            return;
        }

        enemyHealth.Initialize(
            enemyData
        );

        if (GameResultManager.Instance != null)
        {
            GameResultManager.Instance.EnemySpawned();
        }
    }


    private bool ValidateSettings()
    {
        if (enemyPrefab == null)
        {
            return false;
        }

        if (pathPoints == null || pathPoints.Length < 2)
        {
            return false;
        }

        if (pathPoints[0] == null)
        {
            return false;
        }

        return true;
    }
}