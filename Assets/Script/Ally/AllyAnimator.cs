using UnityEngine;

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
}