using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Explosive : MonoBehaviour
{
    public InventoryItem inventoryItem;
    public MeshRenderer meshRenderer;
    public AudioSource audioSource;
    public Collider collider;
    public int currentPlayer;

    public GameObject explosionEffect;
    public TextMeshProUGUI timer;
    public Image hitmarker;
    public Image hitmarkerKill;

    public float radius = 2f;
    public float force = 500f;
    public float timeDelay = 3.4f;
    public float timeLeft;
    public bool manualControl;   

    void Start()
    {
        timeLeft = Time.time + timeDelay;
    }

    void Update()
    {
        if (meshRenderer.enabled)
        {
            var left = timeLeft - Time.time;

            var position = Camera.main.WorldToScreenPoint(transform.position);
            position.x -= 40f;
            position.y += 20f;
            var timerTmp = Instantiate(timer, position, Quaternion.LookRotation(Vector3.forward));
            timerTmp.text = (left + "").Substring(0, 3);
            timerTmp.outlineWidth = 0.4f;
            timerTmp.color = new Color32(255, (byte)(left / timeDelay * 255f), 0, 255);
            timerTmp.fontSize = 16f;
            timerTmp.transform.SetParent(GameObject.Find("Canvas").transform);
            Destroy(timerTmp, 0.02f);

            if (timeLeft < Time.time)
            {
                Collider[] overlappedColliders = Physics.OverlapSphere(transform.position, radius);

                for (int i = 0; i < overlappedColliders.Length; i++)
                {
                    PlayerController playerController = overlappedColliders[i].gameObject.GetComponentInParent<PlayerController>();
                    if (playerController != null)
                    {
                        var distanceKoef = 1 / Mathf.Clamp((playerController.transform.position - transform.position).magnitude, 0.2f, 100f) - 1;
                        var currentDamage = playerController.CalculateDamage(overlappedColliders[i].gameObject, inventoryItem.damage, distanceKoef, 1f);
                        // var currentDamage = inventoryItem.damage * damageKoef * distanceKoef;
                        playerController.ReceiveDamage(currentPlayer, overlappedColliders[i].gameObject, null, currentDamage, manualControl);

                    }
                    Rigidbody rigidbodyOverlaped = overlappedColliders[i].attachedRigidbody;
                    if (rigidbodyOverlaped != null)
                    {
                        rigidbodyOverlaped.AddExplosionForce(force, transform.position, radius);
                    }
                }
                meshRenderer.enabled = false;
                collider.enabled = false;
                audioSource.PlayOneShot(inventoryItem.shotSFX[Random.Range(0, inventoryItem.shotSFX.Length)]);
                var instantiatePos = transform.position;
                instantiatePos.y += 1.3f;
                Instantiate(explosionEffect, instantiatePos, Quaternion.identity);
                Destroy(gameObject, 1f);
            }
        }
    }
}
