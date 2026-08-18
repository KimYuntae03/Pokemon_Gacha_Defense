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

    private float elapsedTime;

    private bool gameStarted;


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
        elapsedTime = 0f;

        StartCoroutine(
            StartCountdown()
        );
    }


    private void Update()
    {
        if (!gameStarted)
        {
            return;
        }

        elapsedTime += Time.deltaTime;

        UpdateTimeText();
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

        elapsedTime = 0f;

        if (gameSpeedController != null)
        {
            gameSpeedController.ResetSpeed();
        }
        else
        {
            Time.timeScale = 1f;
        }

        UpdateTimeText();

    }


    private void UpdateTimeText()
    {
        int totalSeconds =
            Mathf.FloorToInt(
                elapsedTime
            );

        int minutes =
            totalSeconds / 60;

        int seconds =
            totalSeconds % 60;

        gameTimeText.text =
            $"{minutes:00}:{seconds:00}";
    }
}