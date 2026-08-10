using UnityEngine;

[RequireComponent(typeof(Animator))]
public class EnemyAnimator : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // 현재 Wave의 Enemy가 사용할 애니메이션 컨트롤러를 교체
    public void SetAnimatorController(AnimatorOverrideController overrideController)
    {
        if (overrideController == null)
        {
            Debug.LogError(
                $"{gameObject.name}: Animator Override Controller가 없습니다.",
                this
            );

            return;
        }

        animator.runtimeAnimatorController = overrideController;
    }
}