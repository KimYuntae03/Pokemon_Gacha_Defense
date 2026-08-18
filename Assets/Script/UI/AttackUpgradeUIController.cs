using UnityEngine;

public class AttackUpgradeUIController : MonoBehaviour
{
    [SerializeField]
    private GameObject attackUpgradeOverlay;


    public void OpenUpgradeUI()
    {
        if (attackUpgradeOverlay == null)
        {
            return;
        }

        attackUpgradeOverlay.SetActive(true);
    }


    public void CloseUpgradeUI()
    {
        if (attackUpgradeOverlay == null)
        {
            return;
        }

        attackUpgradeOverlay.SetActive(false);
    }
}