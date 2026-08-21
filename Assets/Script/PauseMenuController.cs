using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
    [Header("일시정지 버튼")]
    [SerializeField]
    private Image pauseButtonImage;

    [SerializeField]
    private Sprite pauseSprite;

    [SerializeField]
    private Sprite playSprite;


    [Header("홈 확인 UI")]
    [SerializeField]
    private GameObject homeConfirmOverlay;


    private bool isPaused;
    private bool wasPausedBeforeHomePopup;


    private void Start()
    {
        isPaused = false;

        if (homeConfirmOverlay != null)
        {
            homeConfirmOverlay.SetActive(false);
        }

        UpdatePauseIcon();
    }


    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }

        UpdatePauseIcon();
    }


    private void UpdatePauseIcon()
    {
        if (pauseButtonImage == null)
        {
            return;
        }

        if (isPaused)
        {
            pauseButtonImage.sprite = playSprite;
        }
        else
        {
            pauseButtonImage.sprite = pauseSprite;
        }
    }


    public void OpenHomeConfirm()
    {
        /*
         * 홈 버튼을 누르기 전 게임이
         * 이미 일시정지 상태였는지 기억한다.
         */
        wasPausedBeforeHomePopup = isPaused;

        Time.timeScale = 0f;

        if (homeConfirmOverlay != null)
        {
            homeConfirmOverlay.SetActive(true);
        }
    }


    public void ConfirmHome()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            "MainMenuScene"
        );
    }


    public void CancelHome()
    {
        if (homeConfirmOverlay != null)
        {
            homeConfirmOverlay.SetActive(false);
        }

        /*
         * 홈 버튼을 누르기 전에
         * 게임이 진행 중이었다면 다시 재생한다.
         *
         * 이미 일시정지 상태였다면
         * 그대로 멈춰둔다.
         */
        if (wasPausedBeforeHomePopup)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }
}