using UnityEngine;

public enum AllyDirection
{
    Up,
    Left,
    Down,
    Right
}

[RequireComponent(typeof(Animator))]

public class AllyAnimator : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void SetAnimatorController(
        AnimatorOverrideController overrideController
    )
    {
        if (overrideController == null)
        {
            Debug.LogError(
                $"{gameObject.name}: Animator Override Controller가 없습니다.",
                this
            );

            return;
        }

        animator.runtimeAnimatorController =
            overrideController;
    }
    public void SetDirection(AllyDirection direction)
    {
        if (animator == null)
        {
            return;
        }

        switch (direction)
        {
            case AllyDirection.Up:
                animator.Play("MoveUp");
                break;

            case AllyDirection.Left:
                animator.Play("MoveLeft");
                break;

            case AllyDirection.Down:
                animator.Play("MoveDown");
                break;

            case AllyDirection.Right:
                animator.Play("MoveRight");
                break;
        }
    }
}