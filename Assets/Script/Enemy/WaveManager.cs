using System.Collections;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [Header("웨이브 목록")]
    [SerializeField]
    private WaveData[] waves;

    [Header("적 스포너")]
    [SerializeField]
    private EnemySpawner enemySpawner;

    [Header("게임 시작")]
    [SerializeField]
    private GameTimer gameTimer;

    [SerializeField]
    private float firstWaveDelay = 1f;

    private int currentWaveIndex = 0;

    private void Start()
    {
        if (!ValidateSettings())
        {
            enabled = false;
            return;
        }

        StartCoroutine(WaitForGameStart());
    }

    private IEnumerator WaitForGameStart()
    {
        // GameTimer가 실제 게임 시작 상태가 될 때까지 기다린다.
        yield return new WaitUntil(
            () => gameTimer != null &&
                gameTimer.GameStarted
        );

        // 00:00에서 1초 대기 후 첫 웨이브 시작
        yield return new WaitForSeconds(
            firstWaveDelay
        );

        StartCoroutine(
            RunWaves()
        );
    }

    private IEnumerator RunWaves()
    {
        while (currentWaveIndex < waves.Length)
        {
            WaveData currentWave = waves[currentWaveIndex];

            Debug.Log($"Wave {currentWave.waveNumber} 시작");

            /*
             * EnemySpawner에게 현재 웨이브 데이터를 전달하고
             * 해당 웨이브의 적들을 생성한다.
             */
            StartCoroutine(
                enemySpawner.SpawnWave(currentWave)
            );

            /*
             * 이 웨이브가 진행되는 시간만큼 기다린다.
             */
            yield return new WaitForSeconds(
                currentWave.waveDuration
            );

            /*
             * 다음 웨이브로 이동
             */
            currentWaveIndex++;
        }

        Debug.Log("모든 웨이브 종료");
    }

    private bool ValidateSettings()
    {
        if (enemySpawner == null)
        {
            Debug.LogError(
                $"{gameObject.name}: EnemySpawner가 등록되지 않았습니다.",
                this
            );

            return false;
        }

        if (gameTimer == null)
        {
            Debug.LogError(
                $"{gameObject.name}: GameTimer가 등록되지 않았습니다.",
                this
            );

            return false;
        }

        if (waves == null || waves.Length == 0)
        {
            Debug.LogError(
                $"{gameObject.name}: WaveData가 등록되지 않았습니다.",
                this
            );

            return false;
        }

        for (int i = 0; i < waves.Length; i++)
        {
            if (waves[i] == null)
            {
                Debug.LogError(
                    $"{gameObject.name}: Waves의 {i}번째 데이터가 비어 있습니다.",
                    this
                );

                return false;
            }
        }

        return true;
    }
}