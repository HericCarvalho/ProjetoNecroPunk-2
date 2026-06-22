using UnityEngine;

public class UnitAnimatorController : MonoBehaviour
{
    [Header("Animator")]
    [SerializeField] private Animator animator;

    [Header("Parameters")]
    [SerializeField] private string moveBool = "IsMoving";
    [SerializeField] private string attackTrigger = "Attack";
    [SerializeField] private string speedFloat = "Speed";

    private bool isMoving;

    void Awake()
    {
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    public void SetMoving(bool value)
    {
        isMoving = value;

        if (animator == null) return;

        animator.SetBool(moveBool, value);
    }

    public void SetSpeed(float speed)
    {
        if (animator == null) return;

        animator.SetFloat(speedFloat, speed);
    }

    public void PlayAttack()
    {
        if (animator == null)
        {
            return;
        }

        animator.SetTrigger("Attack");
    }
    public void PlayDeath()
    {
        if (animator == null)
            return;

        animator.SetTrigger("Die");
    }

    public void AnimationEvent_AttackHit()
    {
        SendMessage("OnAnimationAttackHit", SendMessageOptions.DontRequireReceiver);
    }

    public void AnimationEvent_AttackEnd()
    {
        SendMessage("OnAnimationAttackEnd", SendMessageOptions.DontRequireReceiver);
    }
    public void AnimationEvent_DeathFinished()
    {
        SendMessage(
            "OnDeathAnimationFinished",
            SendMessageOptions.DontRequireReceiver
        );
    }
}