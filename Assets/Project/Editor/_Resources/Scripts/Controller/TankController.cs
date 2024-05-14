using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using VariableCode.Cursor;

public class TankController : MonoBehaviour
{
    public SelectManager selectManager;
    public SampleScene inventoryManager;
    public GameObject inventory;
    public UIController UIController;
    public CursorSwitcher cursorSwitcher;

    public Transform canvas;
    public Transform cameraRig;

    public Animator animator;
    public Rigidbody rigidbody;
    public NavMeshAgent agent;
    public SelectableCharacter selection;
    public float roatationSpeed = 0.7f;
    public float turretRoatationSpeed = 0.1f;
    public float rotationProgress = 0f;
    public float speed = 6f;
    public Vector3 directionVector;
    public float nextShoot = 0f;
    public LayerMask whatCanBeClickedOn;
    public bool afteburner = false;
    public float spreadSize;
    public float fireRate = 1f;
    public float effectiveDistance = 60f;
    public float firingAimDecrease = 0f;
    public float firingAimDecreaseMax = 10f;
    public float aimDecrease;
    public float maxFuel = 500f;
    public float currentFuel = 500f;

    public ParticleSystem muzzleFlash;
    public BulletTracer bulletTracer;

    public AudioClip shootSFX;
    public AudioClip reloadSFX;
    public AudioSource audioSourceShoot;

    public AudioClip[] mgunShootSFX;
    public AudioClip mgunReloadSFX;
    public AudioSource audioSourceMgun;

    public AudioClip moveSFX;
    public AudioSource audioSourceMove;
    public AudioClip engineSFX;
    public AudioClip engineUpSFX;
    public AudioClip engineEndSFX;
    public AudioSource audioSourceEngine;
    public AudioClip turretSFX;
    public AudioSource audioSourceTurret;


    public float standardSpeed = 6f;
    public float afteburnerSpeed = 12f;

    public GameObject mainGun;
    public GameObject pairedMgun;
    public GameObject courseMgun;
    public GameObject currentWeapon;

    public GameObject rig;
    public GameObject aimTargetCursor;

    public int rightMouseButtonTaps = 0;

    private LineDrawer lineDrawer;
    private LineDrawer lineDrawerNoContact;
    public SpriteRenderer aimCircle;
    public TextMeshProUGUI rangefinder;

    void Start()
    {
        selectManager = GameObject.Find("SelectingBox").GetComponent<SelectManager>();
        inventoryManager = GameObject.Find("CanvasParent").GetComponent<SampleScene>();
        inventory = GameObject.Find("Inventory");
        UIController = GameObject.Find("UIController").GetComponent<UIController>();
        canvas = GameObject.Find("Canvas").transform;
        cameraRig = GameObject.Find("CameraRig").transform;
        cursorSwitcher = GameObject.Find("CursorManager").GetComponent<CursorSwitcher>();
        currentWeapon = mainGun;
        lineDrawer = new LineDrawer();
        lineDrawerNoContact = new LineDrawer();
    }
    void Update()
    {
        // Aim decrease decrement
        if (firingAimDecrease > 0)
            firingAimDecrease -= 0.08f;
        else
            firingAimDecrease = 0f;

        // Moving handlers
        if (afteburner)
        {
            agent.speed = afteburnerSpeed;
        }
        else
        {
            agent.speed = standardSpeed;
        }
        if (agent.remainingDistance > 0)
        {
            /*
            animator.SetFloat("v", 1f);
            animator.SetFloat("h", 0f);
            if (afteburner)
            {
                animator.SetFloat("Speed", Mathf.Clamp(agent.remainingDistance, -1f, 1f));
            }
            else
            {
                animator.SetFloat("Speed", Mathf.Clamp(agent.remainingDistance, -0.3f, 0.3f));
            }
            animator.SetBool("Move", true);
            */
            firingAimDecrease += 0.1f;
            if (!audioSourceMove.isPlaying)
            {
                audioSourceMove.PlayOneShot(moveSFX);
                audioSourceEngine.PlayOneShot(engineSFX);
            }
        }
        else
        {
            // Stop moving
            if (audioSourceMove.isPlaying && !Input.GetKey(KeyCode.LeftControl) && !UIController.isActiveManualControl)
            {
                // animator.SetBool("Move", false);
                agent.ResetPath();
                rigidbody.velocity = Vector3.zero;
                audioSourceMove.Stop();
                audioSourceEngine.Stop();
                audioSourceEngine.PlayOneShot(engineEndSFX);
                afteburner = false;
            }
        }

        // Handle max aim decrease
        if (firingAimDecrease > firingAimDecreaseMax)
            firingAimDecrease = firingAimDecreaseMax;

        // Point-and-click Controls
        if (Input.GetMouseButtonDown(1))
        {
            afteburner = false;
            rightMouseButtonTaps++;
            StartCoroutine(resetRMBTaps(0.5f));
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100, whatCanBeClickedOn))
            {
                audioSourceEngine.Stop();
                audioSourceEngine.PlayOneShot(engineUpSFX);
                MoveToPoint(hit.point);
            }
        }
        if (rightMouseButtonTaps >= 2)
        {
            afteburner = true;
        }

        // WASD Controls
        if ((Input.GetKey(KeyCode.LeftControl) || UIController.isActiveManualControl))
        {
            if (Time.time > nextShoot)
            {
                cursorSwitcher.ChangeType("aim");
            }

            agent.ResetPath();

            // Calculate direction vector
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            if (h != 0 && v < 0)
            {
                h *= -1; // Imitate tank rotation on moving back
            }
            if (h != 0 || v != 0)
            {
                if (!audioSourceMove.isPlaying)
                {
                    audioSourceMove.PlayOneShot(moveSFX);
                    audioSourceEngine.PlayOneShot(engineSFX);
                }
            }
            else
            {
                audioSourceMove.Stop();
            }
            directionVector = transform.forward * v;
            directionVector.y = 0;
            rigidbody.velocity = new Vector3(rigidbody.velocity.x, 0, rigidbody.velocity.z);
            transform.Rotate(0, h * roatationSpeed, 0, Space.Self);
            // animator.SetFloat("Speed", Vector3.ClampMagnitude(directionVector, 1).magnitude);
            if (Vector3.ClampMagnitude(directionVector, 1).magnitude > 0)
            {
                if (rigidbody.velocity.magnitude == 0)
                {
                    audioSourceEngine.Stop();
                    audioSourceEngine.PlayOneShot(engineUpSFX);
                }
                rigidbody.velocity = Vector3.ClampMagnitude(directionVector, 1) * speed * 1.5f;
                // animator.SetBool("Move", true);
                firingAimDecrease += 0.15f;
            }
            else
            {
                if (rigidbody.velocity.magnitude > 0)
                {
                    audioSourceEngine.Stop();
                    audioSourceEngine.PlayOneShot(engineEndSFX);
                }
                rigidbody.velocity = Vector3.zero;
            }

            var localCoords = transform.InverseTransformDirection(directionVector);
            // animator.SetFloat("h", localCoords.z);
            // animator.SetFloat("v", localCoords.x);

            spreadSize = effectiveDistance - firingAimDecrease;

            // if (selection.currentSprite.enabled && currentWeapon != null)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 1000))
                {
                    Rotate(hit.point);
                    Aiming(hit.point, spreadSize, true);
                }
                else
                {
                    HideWhiteLine();
                    HideRedLine();
                }

                // Shoot
                if (Input.GetMouseButton(0) && !selectManager.IsMouseOverUI())
                {
                    Shoot(spreadSize, true);
                }
            }
        }
        else
        {
            audioSourceTurret.Stop();
            HideWhiteLine();
            HideRedLine();
            rigidbody.velocity = Vector3.zero;
        }
    }

    public void MoveToPoint(Vector3 directionVector)
    {
        if (selectManager.selectedArmy.Count > 1 && selection.player == selectManager.player)
        {
            int squadHalf = selectManager.selectedArmy.Count / 2;
            var unitsWidth = 2.3f;
            var unitsDepth = 1.7f;
            var maxSoldiersInLine = 4;
            var index = selectManager.selectedArmy.IndexOf(selection);

            int secondLine = 0;
            if (index > maxSoldiersInLine - 1)
                secondLine = 1;

            var oneLineCond = index == squadHalf;
            var secondLineCond = secondLine == 1 && index == squadHalf / 2;
            if ((oneLineCond && !secondLineCond) || (!oneLineCond && secondLineCond))
            {
                index = 0;
            }
            else if (index == 0)
            {
                index = squadHalf;
                if (secondLine == 1)
                    index /= 2;
            }

            // var direction = directionVector - transform.position;
            var navigator = Instantiate(new GameObject(), transform);
            navigator.transform.parent = null;
            navigator.transform.position = transform.position;
            navigator.transform.rotation = Quaternion.LookRotation(directionVector - transform.position);

            directionVector += (index % maxSoldiersInLine - squadHalf / (secondLine + 1)) * navigator.transform.right * unitsWidth - navigator.transform.forward * (index / maxSoldiersInLine) * unitsDepth;
            Destroy(navigator.gameObject);
        }

        agent.SetDestination(directionVector);
        // animator.SetFloat("Speed", Vector3.ClampMagnitude(directionVector, 1).magnitude);
        // animator.SetBool("Move", true);
    }

    public void Rotate(Vector3 point)
    {
        aimTargetCursor.transform.position = point;
        var result = Vector3.Lerp(rig.transform.position, aimTargetCursor.transform.position, turretRoatationSpeed);
        if ((result - rig.transform.position).magnitude > 0.12f)
        {
            if (!audioSourceTurret.isPlaying)
                audioSourceTurret.PlayOneShot(turretSFX);
        }
        else
        {
            audioSourceTurret.Stop();
        }
        rig.transform.position = result;
        /*
        var currentY = transform.eulerAngles.y;
        var currentTargetY = Quaternion.LookRotation(point - transform.position).eulerAngles.y;
        while (currentY < 0)
            currentY += 360;
        while (currentTargetY < 0)
            currentTargetY += 360;
        if (Mathf.Abs(currentY - currentTargetY) > 40)
            onRotate = true;
        if (onRotate)
        {
            if (Mathf.Abs(currentY - currentTargetY) < 3)
                onRotate = false;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(point - transform.position), Time.deltaTime * roatationSpeed);
        }
        */
    }

    public void Aiming(Vector3 pointToRotate, float spreadSize, bool manualControl)
    {
        // animator.SetBool("Aim", true);

        if (manualControl)
        {
            RaycastHit hitBarrier;
            Ray aimRay = new Ray(currentWeapon.transform.position, currentWeapon.transform.forward);
            Vector3 finalPoint;
            if (Physics.Raycast(aimRay, out hitBarrier, 1000))
            {
                DrawWhiteLine(hitBarrier.point);
                var noContactHit = hitBarrier.point + currentWeapon.transform.forward * (pointToRotate - hitBarrier.point).magnitude;
                DrawRedLine(hitBarrier, noContactHit);
                finalPoint = hitBarrier.point;
            }
            else
            {
                DrawWhiteLine(aimRay.GetPoint(100));
                finalPoint = aimRay.GetPoint(100);
                HideRedLine();
            }
            var tmpSize = (this.transform.position - finalPoint).magnitude / spreadSize;
            aimCircle.size = new Vector2(tmpSize, tmpSize);
            var aimCircleTmp = Instantiate(aimCircle, finalPoint, Quaternion.LookRotation(aimRay.direction));
            Destroy(aimCircleTmp, 0.02f);
            rangefinder.text = System.Math.Round((this.transform.position - finalPoint).magnitude, 1) + "m";
            rangefinder.color = new Color32(255, 194, 230, 255);
            rangefinder.outlineWidth = 0.4f;
            rangefinder.outlineColor = new Color32(0, 0, 0, 255);
            var rangefinderTmp = Instantiate(rangefinder, Camera.main.WorldToScreenPoint(finalPoint), Quaternion.LookRotation(Vector3.forward));
            rangefinderTmp.transform.SetParent(canvas);
            Destroy(rangefinderTmp, 0.02f);
        }
    }

    public bool Shoot(float spreadSize, bool manualControl)
    {
        // makingNoise = true;
        // makingNoiseTime = Time.time + makingNoiseCooldown;

        bool thereWasShot = false;
        if (Time.time > nextShoot) //  && selection.currentWeapon.currentAmmo > 0
        {
            thereWasShot = true;
            /*
            animator.SetBool("Fire", true);
            selection.currentWeapon.currentAmmo--;
            
            if (IsThisCurrentChar())
            {
                UIController.ammoCount.text = selection.currentWeapon.currentAmmo + "/" + selection.currentWeapon.magSize;
                if (inventory.activeSelf)
                    inventoryManager.StartCoroutine(inventoryManager.InsertCoroutine(selection.inventory_items));
            }
            */
            if (manualControl)
            {
                cursorSwitcher.ChangeType("fire");
            }

            nextShoot = Time.time + 1f / fireRate;
            // firingAimDecrease += selection.currentWeapon.AimDecrease;
            audioSourceShoot.PlayOneShot(shootSFX);
            muzzleFlash.Play();
            ParticleSystem flash = Instantiate(muzzleFlash, currentWeapon.transform.position, currentWeapon.transform.rotation);
            Destroy(flash, 1f);

            var spread = 60f / spreadSize;
            Ray shootRay = new Ray(currentWeapon.transform.position, Quaternion.Euler(0, Random.Range(-spread, spread), Random.Range(-spread, spread)) * currentWeapon.transform.forward);
            BulletTracer currentBulletTracer = Instantiate(bulletTracer, currentWeapon.transform.position, Quaternion.LookRotation(shootRay.direction));
            currentBulletTracer.selfRigidbody.AddForce(shootRay.direction.normalized * 100f, ForceMode.Impulse);
            /*
            currentBulletTracer.damage = selection.currentWeapon.damage;
            currentBulletTracer.distanceKoef = selection.currentWeapon.distanceKoef;
            currentBulletTracer.startPoint = currentWeapon.transform.position;
            currentBulletTracer.currentPlayer = selection.player;
            currentBulletTracer.manualControl = manualControl;
            */
        }
        return thereWasShot;
    }

    public IEnumerator resetRMBTaps(float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);
        rightMouseButtonTaps = 0;
    }

    public void DrawWhiteLine(Vector3 hitBarrier)
    {
        if (currentWeapon != null)
        {
            lineDrawer.lineSize = 0.05f;
            lineDrawer.DrawLineInGameView(currentWeapon.transform.position, hitBarrier, Color.white);
        }

    }

    public void HideWhiteLine()
    {
        if (currentWeapon != null)
        {
            lineDrawer.lineSize = 0f;
            lineDrawer.DrawLineInGameView(currentWeapon.transform.position, Vector3.forward, Color.white);
        }

    }

    public void DrawRedLine(RaycastHit hitBarrier, Vector3 hit)
    {
        lineDrawerNoContact.lineSize = 0.05f;
        lineDrawerNoContact.DrawLineInGameView(hitBarrier.point, hit, Color.red);
    }
    public void HideRedLine()
    {
        if (currentWeapon != null)
        {
            lineDrawerNoContact.lineSize = 0f;
            lineDrawerNoContact.DrawLineInGameView(currentWeapon.transform.position, Vector3.forward, Color.white);
        }
    }
}
