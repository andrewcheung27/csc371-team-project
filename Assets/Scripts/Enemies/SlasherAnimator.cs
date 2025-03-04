using UnityEngine;

public class SlasherAnimator : MonoBehaviour
{
    private Animator animator;
    public GameObject slasherClaw;
    private Collider slasherClawCollider;

    [Header ("Animation Names")]
    public string idleAnimation = "Idle";
    public string attackAnimation = "Attack";
    public string walkAnimation = "Walk";

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
        return animator.GetCurrentAnimatorStateInfo(0).IsName(walkAnimation);
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

    public void Walk()
    {
        if (!InAttackAnimation() && !InWalkAnimation()) {
            animator.Play(walkAnimation);
        }
    }
}
