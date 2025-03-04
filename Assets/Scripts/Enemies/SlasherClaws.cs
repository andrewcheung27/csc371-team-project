using UnityEngine;

public class SlasherClaws : MonoBehaviour
{
    SlasherAttack slasherAttack;

    void Start()
    {
        slasherAttack = GetComponentInParent<SlasherAttack>();
    }

    void OnTriggerEnter(Collider other)
    {
        // handle trigger in SlasherAttack script, but pass information that the object collided with the claws
        slasherAttack.HandleTrigger(other, "Claws");
    }
}
