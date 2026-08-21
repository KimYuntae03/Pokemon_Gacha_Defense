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

    [Header("웨이브 UI")]
    [SerializeField]
    private WaveUI waveUI;

    [Header("보스 보상")]
    [SerializeField]
    private BossRewardManager bossRewardManager;

    [SerializeField]
    private float firstWaveDelay = 1f;

    private int currentWaveIndex = 0;

    public int CurrentWave
    {
        get
        {
            if (waves == null || waves.Length == 0)
            {
                return 0;
            }

            if (currentWaveIndex >= waves.Length)
            {
                return waves[waves.Length - 1].waveNumber;
            }

            return waves[currentWaveIndex].waveNumber;
        }
    }

    public bool AllWavesStarted
    {
        get
        {
            return currentWaveIndex >= waves.Length;
        }
    }

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

            if (currentWave.isBossWave &&
                GameResultManager.Instance != null)
            {
                GameResultManager.Instance.ResetBossEnemyCount();
            }

            Debug.Log($"Wave {currentWave.waveNumber} 시작");


            if (waveUI != null)
            {
                waveUI.UpdateWave(
                    currentWave.waveNumber
                );
            }

            StartCoroutine(
                enemySpawner.SpawnWave(currentWave)
            );

            yield return StartCoroutine(
                gameTimer.RunWaveTimer(
                    currentWave.waveDuration
                )
            );
            
            if (currentWave.isBossWave &&
                GameResultManager.Instance != null &&
                GameResultManager.Instance.CurrentBossEnemyCount > 0)
            {
                GameResultManager.Instance.TriggerGameOver();

                yield break;
            }
            if (currentWave.isBossWave &&
                bossRewardManager != null)
            {
                bossRewardManager.GiveBossReward(
                    currentWave.waveNumber
                );
            }

            currentWaveIndex++;
        }
        if (GameResultManager.Instance != null)
        {
            GameResultManager.Instance.CheckClearCondition();
        }
    }

    private bool ValidateSettings()
    {
        if (enemySpawner == null)
        {
            return false;
        }

        if (gameTimer == null)
        {
            return false;
        }

        if (waves == null || waves.Length == 0)
        {
            return false;
        }

        for (int i = 0; i < waves.Length; i++)
        {
            if (waves[i] == null)
            {
                return false;
            }
        }

        return true;
    }

    public void BossWaveCleared()
    {
        if (gameTimer != null)
        {
            gameTimer.StopWaveTimer();
        }
    }
}