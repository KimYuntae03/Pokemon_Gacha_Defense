using UnityEngine;

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
}