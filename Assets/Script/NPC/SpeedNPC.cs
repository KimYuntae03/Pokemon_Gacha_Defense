using UnityEngine;
using UnityEngine.EventSystems;

public class SpeedNPC : MonoBehaviour,
    IPointerClickHandler
{
    [SerializeField]
    private GameSpeedController gameSpeedController;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (gameSpeedController == null)
        {
            Debug.LogError(
                $"{gameObject.name}: GameSpeedController가 등록되지 않았습니다.",
                this
            );

            return;
        }

        gameSpeedController.CycleSpeed();
    }
}