using UnityEngine;

public class SlasherClaws : MonoBehaviour
{
    SlasherAttack slasherAttack;
    Quaternion relativeRotation;

    void Start()
    {
        slasherAttack = GetComponentInParent<SlasherAttack>();

        relativeRotation = transform.rotation;
    }

    void Update()
    {
        // this seems to fix weird rotation issues. idk.
        transform.rotation = relativeRotation;
    }

    void OnTriggerEnter(Collider other)
    {
        // handle trigger in SlasherAttack script, but pass information that the object collided with the claws
        slasherAttack.HandleTrigger(other, "Claws");
    }
}
