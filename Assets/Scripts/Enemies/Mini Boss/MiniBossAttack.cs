using UnityEngine;
using System.Collections;

public class MinibossAttack : MonoBehaviour
{
    public float attackCooldown = 3.0f;
    public float attackRange = 10f;
    public GameObject armHitbox;
    public Transform player;

    private Animator animator;
    private float lastAttackTime = 0f;
    MiniBossMovement miniBossMovement;

    private void Start()
    {
        animator = GetComponent<Animator>();
        armHitbox.SetActive(false);
        miniBossMovement = GetComponent<MiniBossMovement>();
    }

    private void Update()
    {
        if (player != null && Time.time >= lastAttackTime + attackCooldown)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            if (distanceToPlayer <= attackRange)
            {
                StartCoroutine(Attack());
            }
        }
    }

    IEnumerator Attack()
    {
        lastAttackTime = Time.time;
        animator.SetTrigger("Attack");
        miniBossMovement.SetIsAttacking(true);

        yield return new WaitForSeconds(0.5f);

        armHitbox.SetActive(true);
        yield return new WaitForSeconds(1.75f);
        armHitbox.SetActive(false);
        miniBossMovement.SetIsAttacking(false);
    }
}
