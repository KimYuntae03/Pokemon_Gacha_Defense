using UnityEngine;

[RequireComponent(typeof(Animator))]
public class EnemyMovement : MonoBehaviour
{
    [Header("이동 설정")]
    [SerializeField]
    private float moveSpeed = 4f;

    [SerializeField]
    private float arrivalDistance = 0.05f;

    [SerializeField]
    private float directionTolerance = 0.05f;

    [Header("이동 경로")]
    [SerializeField]
    private Transform[] pathPoints; //이동방향을 바꿀 포인트들 등록

    private Animator animator;
    private int currentPointIndex;

    private int currentAnimationHash;

    private static readonly int MoveLeftHash =
        Animator.StringToHash("Base Layer.MoveLeft");

    private static readonly int MoveDownHash =
        Animator.StringToHash("Base Layer.MoveDown");

    private static readonly int MoveRightHash =
        Animator.StringToHash("Base Layer.MoveRight");

    private static readonly int MoveUpHash =
        Animator.StringToHash("Base Layer.MoveUp");

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void SetPath(Transform[] newPath)
    {
        pathPoints = newPath;
    }

    private void Start()
    {
        if (pathPoints == null || pathPoints.Length < 2)
        {
            Debug.LogError(
                $"{gameObject.name}: 이동 경로가 등록되지 않았습니다.",
                this
            );

            enabled = false;
            return;
        }

        currentPointIndex = 1;

        PlayAnimation(MoveLeftHash);
    }

    private void Update()
    {
        MoveToCurrentPoint();
    }

    public void SetSpeed(float speed)
    {
        moveSpeed = speed;
    }

    private void MoveToCurrentPoint()
    {
        Transform targetPoint = pathPoints[currentPointIndex];

        if (targetPoint == null)
        {
            Debug.LogError(
                $"{gameObject.name}: Path Points의 {currentPointIndex}번이 비어 있습니다.",
                this
            );

            enabled = false;
            return;
        }

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPoint.position,
            moveSpeed * Time.deltaTime
        );

        float distanceToTarget = Vector3.Distance(
            transform.position,
            targetPoint.position
        );

        if (distanceToTarget <= arrivalDistance)
        {
            transform.position = targetPoint.position;

            currentPointIndex++;

            if (currentPointIndex >= pathPoints.Length)
            {
                currentPointIndex = 1;
            }

            UpdateMoveAnimation();
        }
    }

    private void UpdateMoveAnimation()
{
    Vector3 targetPosition = pathPoints[currentPointIndex].position;
    Vector3 moveDirection = targetPosition - transform.position;

    float horizontalDistance = Mathf.Abs(moveDirection.x);
    float verticalDistance = Mathf.Abs(moveDirection.y);

    /*
     * 세로 이동량이 거의 없으면 완전한 좌우 이동으로 판단한다.
     */
    if (verticalDistance <= directionTolerance &&
        horizontalDistance > directionTolerance)
    {
        if (moveDirection.x < 0f)
        {
            PlayAnimation(MoveLeftHash);
        }
        else
        {
            PlayAnimation(MoveRightHash);
        }

        return;
    }

    /*
     * 가로 이동량이 거의 없으면 완전한 상하 이동으로 판단한다.
     */
    if (horizontalDistance <= directionTolerance &&
        verticalDistance > directionTolerance)
    {
        if (moveDirection.y < 0f)
        {
            PlayAnimation(MoveDownHash);
        }
        else
        {
            PlayAnimation(MoveUpHash);
        }

        return;
    }

    /*
     * X축과 Y축이 모두 변하는 대각선 이동이면
     * 새로운 애니메이션을 재생하지 않는다.
     *
     * 따라서 직전에 사용하던 방향 애니메이션이 그대로 유지된다.
     */
}

    private void PlayAnimation(int animationHash)
    {
        if (currentAnimationHash == animationHash)
        {
            return;
        }

        currentAnimationHash = animationHash;

        animator.Play(animationHash, 0, 0f);
    }
}