using UnityEngine;

public class SlasherAnimator : MonoBehaviour
{
    private Animator animator;
    public GameObject slasherClaw;
    private Collider slasherClawCollider;

    [Header ("Animation Names")]
    public string idleAnimation = "Idle";
    public string attackAnimation = "Attack";
    public string walkLeftAnimation = "Walk Left";
    public string walkRightAnimation = "Walk Right";

    void Start()
    {
        animator = GetComponent<Animator>();

        slasherClawCollider = slasherClaw.GetComponent<Collider>();
    }

    public bool InAttackAnimation()
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName(attackAnimation);
    }

    public bool InWalkAnimation()
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName(walkLeftAnimation) 
            || animator.GetCurrentAnimatorStateInfo(0).IsName(walkRightAnimation);
    }

    // returns true if it attacked, false otherwise
    public bool Attack()
    {
        if (!InAttackAnimation()) {
            // disable attack collider to start, so it only does damage after a delay in SlasherAttack.EnableAttackCollider()
            slasherClawCollider.gameObject.SetActive(false);
            // play animation
            animator.Play(attackAnimation);
            return true;
        }

        return false;
    }

    public void WalkLeft()
    {
        if (!InAttackAnimation() && !InWalkAnimation()) {
            animator.Play(walkLeftAnimation);
        }
    }

    public void WalkRight()
    {
        if (!InAttackAnimation() && !InWalkAnimation()) {
            animator.Play(walkRightAnimation);
        }
    }
}
