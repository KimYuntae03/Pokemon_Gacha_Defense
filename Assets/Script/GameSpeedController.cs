using UnityEngine;

public class GameSpeedController : MonoBehaviour
{
    [SerializeField]
    private float currentSpeed = 1f;

    public void CycleSpeed()
    {
        Debug.Log("CycleSpeed 호출됨");

        if (currentSpeed == 1f)
        {
            SetSpeed(2f);
        }
        else if (currentSpeed == 2f)
        {
            SetSpeed(4f);
        }
        else
        {
            SetSpeed(1f);
        }
    }

    public void ResetSpeed()
    {
        SetSpeed(1f);
    }

    private void SetSpeed(float speed)
    {
        currentSpeed = speed;
        Time.timeScale = speed;

        Debug.Log($"현재 게임 배속: x{currentSpeed}");
    }
}