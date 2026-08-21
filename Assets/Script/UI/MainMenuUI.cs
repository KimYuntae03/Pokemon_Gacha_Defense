using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [Header("게임 설명 UI")]
    [SerializeField]
    private GameObject guideOverlay;

    [Header("순위 UI")]
    [SerializeField]
    private GameObject rankingOverlay;

    private void Start()
    {
        if (guideOverlay != null)
        {
            guideOverlay.SetActive(false);
        }
        if (rankingOverlay != null)
        {
            rankingOverlay.SetActive(false);
        }
    }

    public void OpenGuide()
    {
        if (guideOverlay != null)
        {
            guideOverlay.SetActive(true);
        }
    }

    public void CloseGuide()
    {
        if (guideOverlay != null)
        {
            guideOverlay.SetActive(false);
        }
    }

    public void ExitGame()
    {
        Debug.Log("게임 종료");

    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
    }

    public void StartGame()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene("InGameScene");
    }

    public void OpenRanking()
    {
        if (rankingOverlay != null)
        {
            rankingOverlay.SetActive(true);
        }
    }

    public void CloseRanking()
    {
        if (rankingOverlay != null)
        {
            rankingOverlay.SetActive(false);
        }
    }
}