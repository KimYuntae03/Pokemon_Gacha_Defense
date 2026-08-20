using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [Header("게임 설명 UI")]
    [SerializeField]
    private GameObject guideOverlay;

    private void Start()
    {
        if (guideOverlay != null)
        {
            guideOverlay.SetActive(false);
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
}