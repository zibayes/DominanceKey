using UnityEngine;

public class MissaleTracer : MonoBehaviour
{
    public float tracerLife = 3f;
    public GameObject missaleHole;
    public Rigidbody selfRigidbody;
    public LineRenderer lineRenderer;
    public Collider selfCollider;
    private bool isFirstEnter = true;
    public int currentPlayer;

    public InventoryItem inventoryItem;
    public GameObject explosionEffect;
    public AudioSource audioSource;

    public float radius = 4f;
    public float force = 5000f;
    public bool manualControl;

    void Start()
    {
        Destroy(gameObject, tracerLife);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isFirstEnter)
        {
            Collider[] overlappedColliders = Physics.OverlapSphere(transform.position, radius);

            for (int i = 0; i < overlappedColliders.Length; i++)
            {
                PlayerController playerController = overlappedColliders[i].gameObject.GetComponentInParent<PlayerController>();
                if (playerController != null)
                {
                    var distanceKoef = 1 / Mathf.Clamp((playerController.transform.position - transform.position).magnitude, 0.2f, 100f) - 1;
                    var currentDamage = playerController.CalculateDamage(overlappedColliders[i].gameObject, inventoryItem.damage, distanceKoef, 1f);
                    playerController.ReceiveDamage(currentPlayer, overlappedColliders[i].gameObject, null, currentDamage, manualControl);
                }

                TankController tankController = overlappedColliders[i].gameObject.GetComponentInParent<TankController>();
                if (tankController != null)
                {
                    var distanceKoef = 1 / Mathf.Clamp((tankController.transform.position - transform.position).magnitude, 0.2f, 100f) - 1;
                    var currentDamage = tankController.CalculateDamage(overlappedColliders[i].gameObject, inventoryItem.damage, distanceKoef, 1f);
                    tankController.ReceiveDamage(currentPlayer, overlappedColliders[i].gameObject, null, currentDamage, manualControl);
                }

                Rigidbody rigidbodyOverlaped = overlappedColliders[i].attachedRigidbody;
                if (rigidbodyOverlaped != null)
                {
                    rigidbodyOverlaped.AddExplosionForce(force, transform.position, radius);
                }
            }
            audioSource.PlayOneShot(inventoryItem.shotSFX[Random.Range(0, inventoryItem.shotSFX.Length)]);
            var instantiatePos = transform.position;
            instantiatePos.y += 1.3f;
            Instantiate(explosionEffect, instantiatePos, Quaternion.identity);

            lineRenderer.enabled = false;
            selfCollider.enabled = false;
            Destroy(gameObject, 2f);
            isFirstEnter = false;
        }
    }
}
