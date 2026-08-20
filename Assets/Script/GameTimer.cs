using System.Collections;
using TMPro;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    [Header("시간 표시")]
    [SerializeField]
    private TMP_Text gameTimeText;

    [Header("게임 시작 대기시간")]
    [SerializeField]
    private int startCountdown = 5;

    [Header("배속 시스템")]
    [SerializeField]
    private GameSpeedController gameSpeedController;

    private float remainingTime;
    private bool gameStarted;
    private bool waveTimerStopRequested;

    public bool GameStarted
    {
        get { return gameStarted; }
    }


    private void Awake()
    {
        // 다른 시스템보다 먼저 게임을 정지시킨다.
        Time.timeScale = 0f;

        gameStarted = false;
    }


    private void Start()
    {
        StartCoroutine(
            StartCountdown()
        );
    }

    private IEnumerator StartCountdown()
    {
        for (int countdown = startCountdown;
             countdown >= 1;
             countdown--)
        {
            gameTimeText.text =
                countdown.ToString();

            // Time.timeScale = 0이어도
            // 실제 시간 기준 1초 기다림
            yield return new WaitForSecondsRealtime(
                1f
            );
        }

        StartGame();
    }


    private void StartGame()
    {
        gameStarted = true;

        if (gameSpeedController != null)
        {
            gameSpeedController.ResetSpeed();
        }
        else
        {
            Time.timeScale = 1f;
        }
    }

    public IEnumerator RunWaveTimer(float duration)
    {
        waveTimerStopRequested = false;
        remainingTime = duration;

        UpdateTimeText();

        while (remainingTime > 0f && !waveTimerStopRequested)
        {
            remainingTime -= Time.deltaTime;

            if (remainingTime < 0f)
            {
                remainingTime = 0f;
            }

            UpdateTimeText();

            yield return null;
        }
    }

    public void StopWaveTimer()
    {
        waveTimerStopRequested = true;
    }


    private void UpdateTimeText()
    {
        int totalSeconds =
            Mathf.FloorToInt(
                remainingTime
            );

        int minutes =
            totalSeconds / 60;

        int seconds =
            totalSeconds % 60;

        gameTimeText.text =
            $"{minutes:00}:{seconds:00}";
    }
}