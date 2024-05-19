using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class TurretIK : MonoBehaviour
{
    public RigBuilder turretRig;
    public Rigidbody turretRigidbody;
    public Collider turretCollider;
    public Collider mantletCollider;
    public Collider gunCollider;
    public bool died = false;
    public float blowForceXZ = 4f;
    public float blowForceY = 5f;
    public float blowSpread = 1.5f;

    void Update()
    {
        if (transform.parent == null && !died)
        {
            TurretBlow();
        }
    }

    public void TurretBlow()
    {
        turretRig.enabled = false;
        turretCollider.enabled = true;
        mantletCollider.enabled = true;
        gunCollider.enabled = true;
        turretRigidbody.isKinematic = false;
        
        turretRigidbody.velocity += turretRigidbody.transform.up * Random.Range(3f, 6f) + 
            turretRigidbody.transform.right * Random.Range(-3f, 3f) + turretRigidbody.transform.forward * Random.Range(-3f, 3f);
        /*
        turretRigidbody.AddForce(turretRigidbody.transform.up * Random.Range(3f, 6f) +
            turretRigidbody.transform.right * Random.Range(-3f, 3f) + turretRigidbody.transform.forward * Random.Range(-3f, 3f), ForceMode.Impulse);
        */
        died = true;
    }
}
