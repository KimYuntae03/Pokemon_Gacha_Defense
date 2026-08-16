using UnityEngine;

public class GoldOrbManager : MonoBehaviour
{
    [Header("금구슬")]
    [SerializeField]
    private int currentGoldOrb = 10;

    [Header("금구슬 UI")]
    [SerializeField]
    private GoldOrbUI goldOrbUI;
    
    private void Start()
    {
        UpdateUI();
    }

    public int CurrentGoldOrb
    {
        get
        {
            return currentGoldOrb;
        }
    }

    public void AddGoldOrb(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        currentGoldOrb += amount;
        UpdateUI();
    }

    public bool CanSpend(int amount)
    {
        return currentGoldOrb >= amount;
    }


    public bool SpendGoldOrb(int amount)
    {
        if (!CanSpend(amount))
        {
            return false;
        }

        currentGoldOrb -= amount;
        UpdateUI();
        return true;
    }

    private void UpdateUI()
    {
        if (goldOrbUI == null)
        {
            return;
        }

        goldOrbUI.UpdateGoldOrb(
            currentGoldOrb
        );
    }
}