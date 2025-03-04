using UnityEngine;

public class SlasherAttack : MonoBehaviour
{
    Animator animator;
    public int damage = 3;

    [Header ("Animation Names")]
    string attackAnimationName = "Attack";

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
            animator.Play(attackAnimationName);
        }
    }

    bool IsAttacking()
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName(attackAnimationName);
    }

    public void HandleTrigger(Collider other, string triggerSource)
    {
        if (IsAttacking() && triggerSource == "Claws") {
            Debug.Log("Claws collided with: " + other.gameObject.name);
            if (other.gameObject.CompareTag("Player")) {
                GameManager.instance.AddToHealth(-damage);
            }
        }
        else {
            Debug.Log("Body collided with: " + other.gameObject.name);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        HandleTrigger(other, "");
    }
}
