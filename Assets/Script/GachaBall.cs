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


    public void OnPointerClick(PointerEventData eventData)
    {
        // 최대 아군 수에 도달했으면 뽑기 금지
        if (!allyManager.CanSpawnAlly())
        {
            Debug.LogWarning(
                "아군 최대 수에 도달해서 더 이상 뽑을 수 없습니다."
            );

            return;
        }


        // 정해둔 확률에 따라 아군 가챠
        AllyData selectedAlly =
            allyGachaManager.DrawAlly();


        // 해당 등급의 후보가 없는 등의 이유로
        // 뽑기에 실패했다면 생성하지 않는다.
        if (selectedAlly == null)
        {
            return;
        }


        // 뽑힌 AllyData를 AllyManager에게 넘겨서 실제 생성
        allyManager.SpawnAlly(
            selectedAlly
        );
    }
}