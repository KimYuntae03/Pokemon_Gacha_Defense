using TMPro;
using UnityEngine;

public class WaveUI : MonoBehaviour
{
    [SerializeField]
    private TMP_Text waveText;

    public void UpdateWave(int waveNumber)
    {
        if (waveText == null)
        {
            return;
        }

        waveText.text = $"WAVE {waveNumber}";
    }
}