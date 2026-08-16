using UnityEngine;
using UnityEngine.EventSystems;

public class GachaBall : MonoBehaviour,
    IPointerClickHandler
{
    [Header("뽑기 시스템")]
    [SerializeField]
    private AllyGachaManager allyGachaManager;

    [Header("아군 관리")]
    [SerializeField]
    private AllyManager allyManager;

    [Header("가챠 비용")]
    [SerializeField]
    private int gachaCost = 10;

    [SerializeField]
    private GoldOrbManager goldOrbManager;

    public void OnPointerClick(PointerEventData eventData)
    {
        // 최대 아군 수에 도달했으면 뽑기 금지
        if (!allyManager.CanSpawnAlly())
        {
            return;
        }
        if (goldOrbManager == null)
        {
            return;
        }
        if (!goldOrbManager.CanSpend(gachaCost))
        {
            return;
        }

        // 정해둔 확률에 따라 아군 가챠
        AllyData selectedAlly =
            allyGachaManager.DrawAlly();

        if (selectedAlly == null)
        {
            return;
        }
        GameObject spawnedAlly =
            allyManager.SpawnAlly(
                selectedAlly
            );

        if (spawnedAlly == null)
        {
            return;
        }
        // 모든 과정이 성공했을 때 금구슬 10개 차감
        goldOrbManager.SpendGoldOrb(
            gachaCost
        );
    }
}