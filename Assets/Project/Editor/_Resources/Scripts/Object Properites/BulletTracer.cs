using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BulletTracer : MonoBehaviour
{
    private SelectManager selectManager;
    private Transform canvas;

    public float tracerLife = 3f;
    public float damage;
    public float distanceKoef;
    public GameObject bulletHole;
    public GameObject bloodHole;
    public GameObject[] bloodSpot;
    public GameObject hitEffect;
    public GameObject humanHitEffect;
    public Vector3 startPoint;
    public Image hitmarker;
    public Image hitmarkerKill;
    public AudioClip[] humanHitSFX;
    public AudioClip[] hitSFX;
    public AudioSource audioSourceHit;
    public Rigidbody selfRigidbody;
    public LineRenderer lineRenderer;
    public Collider selfCollider;
    private bool isFirstEnter = true;
    private float hitForce = 1.4f;
    public int currentPlayer;
    public bool manualControl;
    // Start is called before the first frame update
    void Start()
    {
        selectManager = GameObject.Find("SelectingBox").GetComponent<SelectManager>();
        canvas = GameObject.Find("Canvas").transform;
        Destroy(gameObject, tracerLife);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isFirstEnter)
        {
            // Debug.Log(collision.gameObject.name);
            GameObject currentHitEffect;
            PlayerController playerController = collision.gameObject.GetComponentInParent<PlayerController>();
            if (playerController != null)
            {
                currentHitEffect = humanHitEffect;
                audioSourceHit.PlayOneShot(humanHitSFX[Random.Range(0, humanHitSFX.Length)]);
                Instantiate(bloodHole, collision.contacts[0].point + collision.contacts[0].normal * 0.02f, Quaternion.LookRotation(-collision.contacts[0].normal), collision.gameObject.transform);

                var distance = Vector3.Distance(collision.contacts[0].point, startPoint);
                var currentDamage = playerController.CalculateDamage(collision.gameObject, damage, distanceKoef, distance);
                playerController.ReceiveDamage(currentPlayer, collision.gameObject, collision, currentDamage, manualControl);
            }
            else if (collision.gameObject.GetComponent<Limb>() != null)
            {
                currentHitEffect = humanHitEffect;
                Instantiate(bloodHole, collision.contacts[0].point + collision.contacts[0].normal * 0.02f, Quaternion.LookRotation(-collision.contacts[0].normal), collision.gameObject.transform);
                var rigidbodyToForce = collision.gameObject.GetComponent<Rigidbody>();
                if (rigidbodyToForce == null)
                    rigidbodyToForce = collision.gameObject.GetComponentInParent<Rigidbody>();
                rigidbodyToForce.AddForceAtPosition(-collision.contacts[0].normal * hitForce * damage, collision.contacts[0].point, ForceMode.Impulse);
            }
            else
            {
                currentHitEffect = hitEffect;
                audioSourceHit.PlayOneShot(hitSFX[Random.Range(0, hitSFX.Length)]);
                var bulletMarkHole = Instantiate(bulletHole, collision.contacts[0].point + collision.contacts[0].normal * 0.02f, Quaternion.LookRotation(-collision.contacts[0].normal), collision.gameObject.transform);
                Destroy(bulletMarkHole, 20f);
            }

            GameObject bulletMark = Instantiate(currentHitEffect, collision.contacts[0].point, Quaternion.LookRotation(collision.contacts[0].normal));
            Destroy(bulletMark, 1f);

            lineRenderer.enabled = false;
            selfCollider.enabled = false;
            Destroy(gameObject, 0.7f);
            isFirstEnter = false;
        }
    }
}
