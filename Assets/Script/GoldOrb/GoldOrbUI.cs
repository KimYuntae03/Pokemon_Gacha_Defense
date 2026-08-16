using TMPro;
using UnityEngine;

public class GoldOrbUI : MonoBehaviour
{
    [SerializeField]
    private TMP_Text goldOrbText;


    public void UpdateGoldOrb(int amount)
    {
        if (goldOrbText == null)
        {
            return;
        }

        goldOrbText.text =
            amount.ToString();
    }
}