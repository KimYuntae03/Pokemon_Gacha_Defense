using UnityEngine;

public class AttackEffect : MonoBehaviour
{
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogError(
                $"{gameObject.name}: Animator가 없습니다.",
                this
            );

            return;
        }

        AnimationClip[] clips =
            animator.runtimeAnimatorController.animationClips;

        if (clips.Length == 0)
        {
            Debug.LogError(
                $"{gameObject.name}: Animation Clip이 없습니다.",
                this
            );

            return;
        }

        float animationLength =
            clips[0].length;

        Destroy(
            gameObject,
            animationLength
        );
    }
}