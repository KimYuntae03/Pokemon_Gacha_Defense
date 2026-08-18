using UnityEngine;

public class SellUIController : MonoBehaviour
{
    [SerializeField]
    private GameObject sellOverlay;


    public void OpenSellUI()
{

    if (sellOverlay == null)
    {
        return;
    }
    sellOverlay.SetActive(true);

}


    public void CloseSellUI()
{
    if (sellOverlay == null)
    {
        return;
    }

    sellOverlay.SetActive(false);
}
}