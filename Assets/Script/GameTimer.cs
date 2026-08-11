using TMPro;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    [Header("시간 표시")]
    [SerializeField]
    private TMP_Text gameTimeText;

    private float elapsedTime;


    private void Start()
    {
        elapsedTime = 0f;

        UpdateTimeText();
    }


    private void Update()
    {
        elapsedTime += Time.deltaTime;

        UpdateTimeText();
    }


    private void UpdateTimeText()
    {
        int totalSeconds =
            Mathf.FloorToInt(elapsedTime);

        int minutes =
            totalSeconds / 60;

        int seconds =
            totalSeconds % 60;


        gameTimeText.text =
            $"{minutes:00}:{seconds:00}";
    }
}