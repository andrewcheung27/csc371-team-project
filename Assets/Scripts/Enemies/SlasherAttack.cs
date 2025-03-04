using System.Collections;
using UnityEngine;

public class SlasherAttack : MonoBehaviour
{
    private SlasherAnimator slasherAnimator;
    public GameObject slasherClaw;
    private Collider slasherClawCollider;
    private GameObject player;
    public int damage = 3;
    public float attackRange = 3f;
    public float attackDamageDelay = 1f;

    void Awake()
    {
        slasherAnimator = GetComponent<SlasherAnimator>();

        player = GameObject.FindGameObjectWithTag("Player");

        slasherClawCollider = slasherClaw.GetComponent<Collider>();
    }

    void Update()
    {
        if ((player.transform.position - transform.position).magnitude < attackRange) {
            if (slasherAnimator.Attack()) {
                StartCoroutine(EnableAttackCollider());
            }
        }
    }

    public void HandleTrigger(Collider other, string triggerSource)
    {
        // damage player if they collided with the Claws
        if (slasherAnimator.InAttackAnimation() && triggerSource == "Claws") {
            if (other.gameObject.CompareTag("Player")) {
                GameManager.instance.AddToHealth(-damage);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        HandleTrigger(other, "");
    }

    IEnumerator EnableAttackCollider()
    {
        yield return new WaitForSeconds(attackDamageDelay);

        slasherClawCollider.gameObject.SetActive(true);
    }
}
