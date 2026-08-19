using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class GameResultManager : MonoBehaviour
{
    public static GameResultManager Instance
    {
        get;
        private set;
    }


    [Header("적 수 조건")]
    [SerializeField]
    private int warningEnemyCount = 70;

    [SerializeField]
    private int gameOverEnemyCount = 100;

    [Header("웨이브")]
    [SerializeField]
    private WaveManager waveManager;

    [Header("결과 UI")]
    [SerializeField]
    private GameObject gameResultOverlay;

    [SerializeField]
    private TMP_Text resultText;

    [SerializeField]
    private TMP_Text recordText;

    private int currentEnemyCount;

    private bool warningShown;
    private bool gameEnded;


    public bool GameEnded
    {
        get { return gameEnded; }
    }


    private void Awake()
    {
        Instance = this;

        //게임 시작 시 게임 클리어&종료 패널 OFF
        if (gameResultOverlay != null)
        {
            gameResultOverlay.SetActive(false);
        }
    }

private void Update() //UI테스트용 코드
{
    if (Keyboard.current.cKey.wasPressedThisFrame)
    {
        GameClear();
    }

    if (Keyboard.current.oKey.wasPressedThisFrame)
    {
        GameOver();
    }
}


    public void EnemySpawned()
    {
        if (gameEnded)
        {
            return;
        }

        currentEnemyCount++;

        CheckEnemyCount();
    }


    public void EnemyDied()
    {
        if (gameEnded)
        {
            return;
        }

        currentEnemyCount--;

        if (currentEnemyCount < 0)
        {
            currentEnemyCount = 0;
        }


        // 70마리 아래로 다시 내려갔다면
        // 다음에 다시 70마리가 되었을 때 경고 가능
        if (currentEnemyCount < warningEnemyCount)
        {
            warningShown = false;
        }

        CheckClearCondition();
    }


    private void CheckEnemyCount()
    {
        if (currentEnemyCount >= gameOverEnemyCount)
        {
            GameOver();

            return;
        }


        if (currentEnemyCount >= warningEnemyCount &&
            !warningShown)
        {
            warningShown = true;

            ShowEnemyWarning();
        }
    }


    private void ShowEnemyWarning()
    {
        Debug.Log(
            $"경고! 현재 적 {currentEnemyCount}마리"
        );

        // 다음 단계에서 경고 UI 연결
    }


    private void GameOver()
    {
        if (gameEnded)
        {
            return;
        }

        gameEnded = true;

        if (resultText != null)
        {
            resultText.text =
                "GAME <color=#FF4D4D>OVER</color>";
        }

        if (recordText != null)
        {
            recordText.text =
                $"FINAL WAVE {waveManager.CurrentWave}";
        }

        if (gameResultOverlay != null)
        {
            gameResultOverlay.SetActive(true);
        }

        Time.timeScale = 0f;
    }

    public void GameClear()
    {
        if (gameEnded)
        {
            return;
        }

        gameEnded = true;

        if (resultText != null)
        {
            resultText.text =
                "GAME <color=#5FFFD2>CLEAR!</color>";
        }

        if (recordText != null)
        {
            recordText.text =
                $"FINAL WAVE {waveManager.CurrentWave}";
        }

        if (gameResultOverlay != null)//클리어시 클리어UI ON
        {
            gameResultOverlay.SetActive(true);
        }
        Time.timeScale = 0f;
    }

    public void CheckClearCondition()
    {
        if (waveManager == null)
        {
            return;
        }

        if (waveManager.AllWavesStarted &&
            currentEnemyCount == 0)
        {
            GameClear();
        }
    }
}