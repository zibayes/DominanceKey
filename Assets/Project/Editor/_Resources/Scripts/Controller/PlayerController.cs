using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using VariableCode.Cursor;
using Random = UnityEngine.Random;
using Time = UnityEngine.Time;
using Transform = UnityEngine.Transform;

public class PlayerController : MonoBehaviour
{
    public SelectManager selectManager;
    public SampleScene inventoryManager;
    public GameObject inventory;
    public UIController UIController;
    public CursorSwitcher cursorSwitcher;

    public Animator animator;
    public Rigidbody rigidbody;
    public NavMeshAgent agent;
    public SelectableCharacter selection;
    public float roatationSpeed = 10f;
    public float speed = 3f;
    public Vector3 directionVector;
    public float nextShoot = 0f;
    public LayerMask whatCanBeClickedOn;

    public Transform canvas;
    public Transform cameraRig;

    public UnitVision unitVision;
    public Transform weaponHolder;
    public Transform magHolder;
    public Corpse corpse;
    public List<Rigidbody> rigidbodies;
    public List<Collider> colliders;

    public float spreadSize;
    public float fireRate = 1f;
    public float effectiveDistance = 30f;
    public float firingAimDecrease = 0f;
    public float firingAimDecreaseMax = 8f;
    public float aimDecrease;
    public GameObject rig;
    public GameObject foresight3;
    public GameObject shutter;
    public Rigidbody sleeve;
    public GameObject magazine;
    public ParticleSystem muzzleFlash;
    public BulletTracer bulletTracer;
    public AudioSource audioSourceShoot;
    public bool isHealing = false;
    public bool onRotate = false;
    public bool isRunning = false;
    public float reloadOver = 0f;
    public float healedHealth = 0f;
    public float healingSpeed = 0.001f;

    public int ammoNeed;
    public int sum;
    private Animator shutterAnimator;

    public bool poseToLie = true;
    public float runSpeed = 6f;
    public float standSpeed = 3f;
    public float sitSpeed = 1.5f;
    public float lieSpeed = 0.7f;
    public float standSpread = 1f;
    public float sitSpread = 0.88f;
    public float lieSpread = 0.7f;
    public float currentSpread;

    public string attackType = "open"; // open - return - hold
    public string movementType = "free"; // free - zone - hold

    public bool makingNoise = false;
    public float makingNoiseCooldown = 1f;
    public float makingNoiseTime = 0;
    public float noiseDetectionRaius= 30f;
    public float noiseDetectionCooldown = 15f;
    public float noiseDetectionTime = 0;
    public PlayerController noiseDetectionTarget = null;

    public AudioClip moveSFX;
    public AudioSource audioSourceMove;
    private LineDrawer lineDrawer;
    private LineDrawer lineDrawerNoContact;
    public SpriteRenderer aimCircle;
    public TextMeshProUGUI rangefinder;

    public GameObject[] bloodSpot;
    public Image hitmarker;
    public Image hitmarkerKill;

    public float throwForce = 10f;
    public float throwUpForce = 5f;
    public LineRenderer lineRenderer;
    public GameObject grenadeTrajectorySphere; 

    public int rightMouseButtonTaps = 0;

    public TankController vehicleWantToBoardOn = null;
    private float boardingDistance = 5f;

    public Ray ray;
    public RaycastHit hit;

    // Start is called before the first frame update
    void Awake()
    {
        lineDrawer = new LineDrawer();
        lineDrawerNoContact = new LineDrawer();
        lineRenderer = Instantiate(lineRenderer);
        lineRenderer.enabled = false;
        grenadeTrajectorySphere = Instantiate(grenadeTrajectorySphere);
        grenadeTrajectorySphere.SetActive(false);

        selectManager = GameObject.Find("SelectingBox").GetComponent<SelectManager>();
        inventoryManager = GameObject.Find("CanvasParent").GetComponent<SampleScene>();
        inventory = GameObject.Find("Inventory");
        UIController = GameObject.Find("UIController").GetComponent<UIController>();
        canvas = GameObject.Find("Canvas").transform;
        cameraRig = GameObject.Find("CameraRig").transform;
        cursorSwitcher = GameObject.Find("CursorManager").GetComponent<CursorSwitcher>();

        rigidbodies = new List<Rigidbody>(GetComponentsInChildren<Rigidbody>());
        colliders = new List<Collider>(GetComponentsInChildren<Collider>());
    }

    // Update is called once per frame
    void Update()
    {
        if (selection.currentWeapon != null && !isUsingWeapon() && selection.health > 0)
        {
            SetWeapon();
        }

        // Choose spread for current body position
        currentSpread = standSpread;
        if (animator.GetBool("Sit"))
            currentSpread = sitSpread;
        else if (animator.GetBool("Lie"))
            currentSpread = lieSpread;

        // Aim decrease decrement
        if (firingAimDecrease > 0)
            firingAimDecrease -= 0.08f;
        else
            firingAimDecrease = 0f;

        // Making noise off
        if (makingNoise && Time.time > makingNoiseTime)
        {
            makingNoise = false;
        }

        // Noise detection
        foreach (SelectableCharacter character in selectManager.selectableChars)
        {
            if (character.player != selection.player)
            {
                if (Vector3.Distance(character.transform.position, transform.position) <= noiseDetectionRaius)
                {
                    if (character.playerController != null)
                    {
                        if (character.playerController.makingNoise)
                        {
                            if (noiseDetectionTarget != null)
                            {
                                if (Vector3.Distance(noiseDetectionTarget.transform.position, transform.position) < Vector3.Distance(character.transform.position, transform.position))
                                {
                                    noiseDetectionTarget = character.playerController;
                                    noiseDetectionTime = noiseDetectionCooldown + Time.time;
                                }
                            }
                            else
                            {
                                noiseDetectionTarget = character.playerController;
                                noiseDetectionTime = noiseDetectionCooldown + Time.time;
                            }
                        }
                    }
                }
            }
        }
        if (noiseDetectionTarget != null)
        {
            if (Time.time > noiseDetectionTime || noiseDetectionTarget.selection.health <= 0)
            {
                noiseDetectionTarget = null;
            }
        }

        // Reload over
        if (!animator.GetBool("ReloadOver") && Time.time > reloadOver)
        {
            var magObjs = magazine.GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer obj in magObjs)
                obj.enabled = true;
            animator.ResetTrigger("Reload");
            animator.SetBool("ReloadOver", true);
            selection.currentWeapon.currentAmmo = sum;
            if (IsThisCurrentChar())
            {
                UIController.ammoCount.text = selection.currentWeapon.currentAmmo + "/" + selection.currentWeapon.magSize;
                if (inventory.activeSelf)
                    inventoryManager.StartCoroutine(inventoryManager.InsertCoroutine(selection.inventory_items));
            }
        }

        // Healing
        if (isHealing)
        {
            selection.health = Mathf.Lerp(selection.health, healedHealth, healingSpeed);
            if (selection.health == healedHealth)
                isHealing = false;
        }

        // Moving handlers
        if (!isRunning)
        {
            if (!animator.GetBool("Sit") && !animator.GetBool("Lie"))
                agent.speed = standSpeed;
            if (selection.power < selection.maxPower)
                selection.power += 0.3f;
            else
                selection.power = selection.maxPower;
        }
        else
        {
            agent.speed = runSpeed;
        }
        if (agent.remainingDistance > 0)
        {
            animator.SetFloat("v", 1f);
            animator.SetFloat("h", 0.2f);
            if (isRunning)
            {
                animator.SetFloat("Speed", Mathf.Clamp(agent.remainingDistance, -1f, 1f));
                selection.power -= 0.5f;
                if (selection.power <= 0)
                {
                    selection.power = 0;
                    isRunning = false;
                }
            }
            else
            {
                animator.SetFloat("Speed", Mathf.Clamp(agent.remainingDistance, -0.3f, 0.3f));
            }
            animator.SetBool("Move", true);
            firingAimDecrease += 0.1f;
            if (!audioSourceMove.isPlaying && !animator.GetBool("Lie"))
                audioSourceMove.PlayOneShot(moveSFX);
        }
        else
        {
            if (audioSourceMove.isPlaying && (!Input.GetKey(KeyCode.LeftControl) && !UIController.isActiveManualControl))
            {
                animator.SetBool("Move", false);
                agent.ResetPath();
                rigidbody.velocity = Vector3.zero;
                audioSourceMove.Stop();
                isRunning = false;
            }
        }

        // Handle max aim decrease
        if (firingAimDecrease > firingAimDecreaseMax)
            firingAimDecrease = firingAimDecreaseMax;

        // Control commands
        if (selection.selectedSprite.enabled || selection.currentSprite.enabled)
        {
            // Start reload
            if (Input.GetKey(KeyCode.R))
            {
                Reload(true);
            }

            // Change body position
            if (Input.GetKeyDown(KeyCode.LeftAlt))
            {
                ChangeBodyPosition();
                UIController.SetCurrentValues();
            }

            // Start healing
            if (Input.GetKeyUp(KeyCode.F8))
            {
                HealSelf();
            }

            // Point-and-click Controls
            if (Input.GetMouseButtonDown(1) && !UIController.isActiveAttack && !UIController.isActiveRotate)
            {
                vehicleWantToBoardOn = null;
                isRunning = false;
                rightMouseButtonTaps++;
                StartCoroutine(resetRMBTaps(0.5f));
                ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out hit, 100, whatCanBeClickedOn))
                {
                    MoveToPoint(hit.point);
                }
            }
            if (rightMouseButtonTaps >= 2 && !animator.GetBool("Sit") && !animator.GetBool("Lie"))
            {
                isRunning = true;
            }

            // WASD Controls
            if ((Input.GetKey(KeyCode.LeftControl) || UIController.isActiveManualControl) && IsThisCurrentChar() && !UIController.isActiveAttack && !UIController.isActiveRotate)
            {
                vehicleWantToBoardOn = null;
                if (Time.time > nextShoot && animator.GetBool("ReloadOver"))
                {
                    cursorSwitcher.ChangeType("aim");
                }

                // Direct control move
                if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
                {
                    if (!audioSourceMove.isPlaying && !animator.GetBool("Lie"))
                        audioSourceMove.PlayOneShot(moveSFX);
                    agent.ResetPath();
                }
                else
                {
                    animator.SetBool("Move", false);
                    isRunning = false;
                }

                // Calculate direction vector
                if (agent.remainingDistance == 0 && animator.GetBool("ChangePositionOver"))
                {
                    float h = Input.GetAxis("Horizontal");
                    float v = Input.GetAxis("Vertical");
                    // directionVector = new Vector3(h, 0, v);
                    GameObject direction = new GameObject();
                    direction.transform.position = cameraRig.position;
                    direction.transform.localEulerAngles = new Vector3(0, cameraRig.localEulerAngles.y, cameraRig.localEulerAngles.z);
                    directionVector = direction.transform.right * h + direction.transform.forward * v;
                    DestroyImmediate(direction);
                    directionVector.y = 0;
                    //if (directionVector.magnitude > Mathf.Abs(0.01f))
                    //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(directionVector), Time.deltaTime * roatationSpeed);
                    animator.SetFloat("Speed", Vector3.ClampMagnitude(directionVector, 1).magnitude);
                    if (Vector3.ClampMagnitude(directionVector, 1).magnitude > 0)
                    {
                        animator.SetBool("Move", true);
                        firingAimDecrease += 0.15f;
                    }
                        
                    rigidbody.velocity = Vector3.ClampMagnitude(directionVector, 1) * speed * 1.5f;

                    var localCoords = transform.InverseTransformDirection(directionVector);
                    animator.SetFloat("h", localCoords.z);
                    animator.SetFloat("v", localCoords.x);
                }

                // Stop moving
                if (audioSourceMove.isPlaying && rigidbody.velocity == Vector3.zero)
                {
                    audioSourceMove.Stop();
                    animator.SetBool("Move", false);
                    isRunning = false;
                }

                // Use grenade
                if (selection.currentGrenade != null)
                {
                    ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    // Grenade throw aiming
                    if (Physics.Raycast(ray, out hit, 1000))
                    {
                        Rotate(hit.point);
                    }

                    // Draw grenade throw trajectory
                    if ((weaponHolder.position - hit.point).magnitude <= 18.5f)
                    {
                        HideWhiteLine();
                        HideRedLine();
                        lineRenderer.enabled = true;
                        grenadeTrajectorySphere.SetActive(true);
                        var flightTime = (weaponHolder.position - hit.point).magnitude * 0.2f;
                        var throwForceGrenade = -(weaponHolder.position + Physics.gravity * Mathf.Pow(flightTime, 2f) / 2f - hit.point) / flightTime;
                        ShowGrenadeTrajectory(weaponHolder.position, throwForceGrenade, hit.point);
                        grenadeTrajectorySphere.transform.position = lineRenderer.GetPosition(lineRenderer.positionCount - 1);

                        rangefinder.text = System.Math.Round((this.transform.position - hit.point).magnitude, 1) + "m";
                        rangefinder.outlineWidth = 0.4f;
                        rangefinder.color = new Color32(255, 194, 230, 255);
                        rangefinder.outlineColor = new Color32(0, 0, 0, 255);
                        var rangefinderTmp = Instantiate(rangefinder, Camera.main.WorldToScreenPoint(hit.point), Quaternion.LookRotation(Vector3.forward));
                        rangefinderTmp.transform.SetParent(canvas);
                        Destroy(rangefinderTmp, 0.02f);

                        // Throw grenade
                        if (Input.GetMouseButtonUp(0) && animator.GetBool("ThrowGrenadeOver") && !selectManager.IsMouseOverUI())
                        {
                            TurnOffAiming();
                            ThrowGrenade(throwForceGrenade, true);
                        }
                    }
                    else
                    {
                        lineRenderer.enabled = false;
                        grenadeTrajectorySphere.SetActive(false);
                    }
                }
                else
                { 
                    // Aiming
                    lineRenderer.enabled = false;
                    grenadeTrajectorySphere.SetActive(false);

                    spreadSize = effectiveDistance / currentSpread - firingAimDecrease * currentSpread;

                    if (selection.currentSprite.enabled && selection.currentWeapon != null) // Disable it and have FUN!!!
                    {
                        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                        if (Physics.Raycast(ray, out hit, 1000))
                        {
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
            }
            else
            {
                if (selection.currentWeapon != null)
                {
                    HideWhiteLine();
                    HideRedLine();
                }
                lineRenderer.enabled = false;
                grenadeTrajectorySphere.SetActive(false);
                if (!animator.GetBool("ChaseAI") && !animator.GetBool("AttackAI"))
                {
                    TurnOffAiming();
                }
            }
            if (Time.time > nextShoot)
            {
                TurnOffFiring();
            }

            ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, 100))
            {
                TankController tank = hit.collider.GetComponent<TankController>();
                if (tank != null)
                {
                    var crewmate = tank.getAnyCrewmate();
                    if (tank.getFreeCrewRole() >= 0)
                    {
                        if (crewmate == null)
                        {
                            wantToBoard(tank);
                        }
                        else if (crewmate != null)
                        {
                            if (crewmate.selection.player == selection.player)
                            {
                                wantToBoard(tank);
                            }
                        }
                    }
                } 
            }

            if (vehicleWantToBoardOn != null)
            {
                if (Vector3.Distance(transform.position, vehicleWantToBoardOn.transform.position) <= boardingDistance)
                    boardOnVehicle();
            }
        }
    }

    public void wantToBoard(TankController tank)
    {
        cursorSwitcher.ChangeType("board");

        if (Input.GetMouseButton(1))
        {
            vehicleWantToBoardOn = tank;
            MoveToPoint(vehicleWantToBoardOn.boardingPlace.position);
        }
    }

    public void boardOnVehicle()
    {
        int freeRole = vehicleWantToBoardOn.getFreeCrewRole();
        vehicleWantToBoardOn.crew[freeRole] = this;
        vehicleWantToBoardOn.selection.player = selection.player;
        vehicleWantToBoardOn = null;
        gameObject.SetActive(false);
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

            /*
            if ((selectManager.selectedArmy[selectManager.selectedArmy.Count - 1 - index].transform.position -
                directionVector - (selectManager.selectedArmy.Count / 2 - 1 - index) * navigator.transform.right * unitsWidth).magnitude > 
                (selectManager.selectedArmy[index].transform.position - directionVector - (index - selectManager.selectedArmy.Count / 2) * 
                navigator.transform.right * unitsWidth).magnitude)
            {
                index = selectManager.selectedArmy.Count - 1 - index;
            }
            */

            directionVector += (index % maxSoldiersInLine - squadHalf / (secondLine + 1)) * navigator.transform.right * unitsWidth - navigator.transform.forward * (index / maxSoldiersInLine) * unitsDepth;
            Destroy(navigator.gameObject);
        }

        agent.SetDestination(directionVector);
        animator.SetFloat("Speed", Vector3.ClampMagnitude(directionVector, 1).magnitude);
        animator.SetBool("Move", true);
    }

    public void Rotate(Vector3 point)
    {
        rig.transform.position = point;
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
    }

    public void Aiming(Vector3 pointToRotate, float spreadSize, bool manualControl)
    {
        Rotate(pointToRotate);
        animator.SetBool("Aim", true);
        
        if (manualControl)
        {
            RaycastHit hitBarrier;
            // Ray aimRay = new Ray(foresight3.transform.position, hit.point - foresight3.transform.position);
            Ray aimRay = new Ray(foresight3.transform.position, foresight3.transform.forward);
            Vector3 finalPoint;
            if (Physics.Raycast(aimRay, out hitBarrier, 1000))
            {
                DrawWhiteLine(hitBarrier.point);
                var noContactHit = hitBarrier.point + foresight3.transform.forward * (pointToRotate - hitBarrier.point).magnitude;
                DrawRedLine(hitBarrier, noContactHit);
                finalPoint = hitBarrier.point;
            }
            else
            {
                DrawWhiteLine(aimRay.GetPoint(100));
                finalPoint = aimRay.GetPoint(100);
                // DrawWhiteLine(hit.point);
                HideRedLine();
                // finalPoint = hit;
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

    public void TurnOffAiming()
    {
        // if (Time.time > nextShoot)
        // cursorSwitcher.ChangeType(0);

        animator.SetBool("Aim", false);
    }

    public void TurnOffFiring()
    {
        if (shutterAnimator != null)
            shutterAnimator.SetBool("Fire", false);
        animator.SetBool("Fire", false);
    }

    public bool Shoot(float spreadSize, bool manualControl)
    {
        makingNoise = true;
        makingNoiseTime = Time.time + makingNoiseCooldown;

        bool thereWasShot = false;
        if (Time.time > nextShoot && selection.currentWeapon.currentAmmo > 0 && !isUsingWeapon())
        {
            thereWasShot = true;
            if (shutterAnimator != null)
                shutterAnimator.SetBool("Fire", true);
            animator.SetBool("Fire", true);
            selection.currentWeapon.currentAmmo--;
            if (IsThisCurrentChar())
            {
                UIController.ammoCount.text = selection.currentWeapon.currentAmmo + "/" + selection.currentWeapon.magSize;
                if (inventory.activeSelf)
                    inventoryManager.StartCoroutine(inventoryManager.InsertCoroutine(selection.inventory_items));
            }
            if (manualControl)
            {
                cursorSwitcher.ChangeType("fire");
            }

            nextShoot = Time.time + 1f / fireRate;
            firingAimDecrease += selection.currentWeapon.AimDecrease;
            audioSourceShoot.PlayOneShot(selection.currentWeapon.shotSFX[Random.Range(0, selection.currentWeapon.shotSFX.Length)]);
            muzzleFlash.Play();
            ParticleSystem flash = Instantiate(muzzleFlash, foresight3.transform.position, foresight3.transform.rotation);
            Destroy(flash, 1f);
            Rigidbody currentSleeve = Instantiate(sleeve, shutter.transform.position, shutter.transform.rotation);
            currentSleeve.AddForce(Random.Range(2.3f, 3.7f) * (shutter.transform.right + shutter.transform.up), ForceMode.Impulse);
            Destroy(currentSleeve.gameObject, 20f);

            var spread = 60f / spreadSize;
            Ray shootRay = new Ray(foresight3.transform.position, Quaternion.Euler(0, Random.Range(-spread, spread), Random.Range(-spread, spread)) * foresight3.transform.forward);
            BulletTracer currentBulletTracer = Instantiate(bulletTracer, foresight3.transform.position, Quaternion.LookRotation(shootRay.direction));
            currentBulletTracer.selfRigidbody.AddForce(shootRay.direction.normalized * selection.currentWeapon.bulletSpeed, ForceMode.Impulse);
            currentBulletTracer.damage = selection.currentWeapon.damage;
            currentBulletTracer.distanceKoef = selection.currentWeapon.distanceKoef;
            currentBulletTracer.startPoint = foresight3.transform.position;
            currentBulletTracer.currentPlayer = selection.player;
            currentBulletTracer.manualControl = manualControl;
        }
        return thereWasShot;
    }

    public void Reload(bool manualControl)
    {
        if (selection.currentWeapon.currentAmmo < selection.currentWeapon.magSize && !isUsingWeapon())
        {
            bool haveAmmo = false;
            ammoNeed = 0;
            List<int> indexes = new List<int>();
            sum = selection.currentWeapon.currentAmmo;
            // Check ammo count
            for (int i = 0; i < selection.inventory_items.Count; i++)
            {
                if (selection.inventory_items[i].type == selection.currentWeapon.ammoType)
                {
                    indexes.Add(i);
                    if (selection.inventory_items[i].IsStackable)
                    {
                        if (sum + selection.inventory_items[i].currentAmount >= selection.currentWeapon.magSize)
                        {
                            ammoNeed += selection.currentWeapon.magSize - sum;
                            sum = selection.currentWeapon.magSize;
                            haveAmmo = true;
                            break;
                        }
                        else
                        {
                            sum += selection.inventory_items[i].currentAmount;
                            ammoNeed += selection.inventory_items[i].currentAmount;
                        }
                    }
                    else
                    {
                        haveAmmo = true;
                        break;
                    }
                }
            }
            if (ammoNeed > 0)
                haveAmmo = true;
            // Take ammo from inventory and load it to the gun
            if (haveAmmo && indexes.Any())
            {
                var ammoNeedTmp = ammoNeed;
                for (int i = indexes.Count - 1; i >= 0; i--)
                {
                    if (i != 0)
                    {
                        ammoNeedTmp -= selection.inventory_items[indexes[i]].currentAmount;
                        selection.inventory_items.RemoveAt(indexes[i]);
                    }
                    else
                    {
                        selection.inventory_items[indexes[i]].currentAmount -= ammoNeedTmp;
                        if (selection.inventory_items[indexes[i]].currentAmount <= 0)
                            selection.inventory_items.RemoveAt(indexes[i]);
                    }

                }

                if (IsThisCurrentChar())
                {
                    if (inventory.activeSelf)
                        inventoryManager.StartCoroutine(inventoryManager.InsertCoroutine(selection.inventory_items));
                }
                if (manualControl)
                {
                    cursorSwitcher.ChangeType("reload");
                }

                animator.SetBool("ReloadOver", false);
                animator.SetTrigger("Reload");
                reloadOver = Time.time + selection.currentWeapon.reloadTime;
                audioSourceShoot.PlayOneShot(selection.currentWeapon.reloadSFX);
                var oldMagazine = Instantiate(magazine, magazine.transform);
                oldMagazine.GetComponent<Rigidbody>().isKinematic = false;
                oldMagazine.GetComponent<Collider>().enabled = true;
                oldMagazine.transform.parent = null;
                Invoke("TakeNewMagazine", 1f);
                var magObjs = magazine.GetComponentsInChildren<MeshRenderer>();
                foreach (MeshRenderer obj in magObjs)
                    obj.enabled = false;
                Destroy(oldMagazine, 20f);
            }
        }
    }

    public void HealSelf()
    {
        if (selection.maxHealth > selection.health && !isHealing)
        {
            bool haveMedkit = false;
            int index = -1;
            for (int i = 0; i < selection.inventory_items.Count; i++)
            {
                if (selection.inventory_items[i].type == "medkit")
                {
                    if (selection.inventory_items[i].currentAmount >= 1)
                    {
                        selection.inventory_items[i].currentAmount -= 1;
                    }
                    else
                    {
                        index = i;
                    }
                    haveMedkit = true;
                    break;
                }
            }
            if (haveMedkit)
            {
                if (index >= 0)
                    selection.inventory_items.RemoveAt(index);
                isHealing = true;
                healedHealth = selection.health + 40f;
                if (healedHealth > selection.maxHealth)
                    healedHealth = selection.maxHealth;

                if (IsThisCurrentChar())
                {
                    if (inventory.activeSelf)
                        inventoryManager.StartCoroutine(inventoryManager.InsertCoroutine(selection.inventory_items));
                }
            }
        }
    }

    public void ChangeBodyPosition()
    {
        if (animator.GetBool("ChangePositionOver") && animator.GetBool("ThrowGrenadeOver"))
        {
            animator.SetBool("ChangePositionOver", false);
            if (!animator.GetBool("Sit"))
            {
                if (poseToLie)
                {
                    animator.SetBool("Sit", true);
                    StartCoroutine(ChangeBodyPositionOver(0.45f));
                }
                else
                {
                    animator.SetBool("Lie", false);
                    animator.SetBool("Sit", true);
                    StartCoroutine(ChangeBodyPositionOver(1.5f));
                }
                speed = sitSpeed;
                agent.speed = sitSpeed;
            }
            else
            {
                if (poseToLie)
                {
                    animator.SetBool("Sit", false);
                    animator.SetBool("Lie", true);
                    poseToLie = false;
                    speed = lieSpeed;
                    agent.speed = lieSpeed;
                    StartCoroutine(ChangeBodyPositionOver(1.5f));
                }
                else
                {
                    animator.SetBool("Sit", false);
                    poseToLie = true;
                    speed = standSpeed;
                    agent.speed = standSpeed;
                    StartCoroutine(ChangeBodyPositionOver(1.1f));
                }
            }
        }
    }

    public IEnumerator ChangeBodyPositionOver(float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);
        animator.SetBool("ChangePositionOver", true);
    }

    public void DrawWhiteLine(Vector3 hitBarrier)
    {
        if (foresight3 != null)
        {
            lineDrawer.lineSize = 0.05f;
            lineDrawer.DrawLineInGameView(foresight3.transform.position, hitBarrier, Color.white);
        }
        
    }

    public void HideWhiteLine()
    {    
        if (foresight3 != null)
        {
            lineDrawer.lineSize = 0f;
            lineDrawer.DrawLineInGameView(foresight3.transform.position, Vector3.forward, Color.white);
        }
        
    }

    public void DrawRedLine(RaycastHit hitBarrier, Vector3 hit)
    {
        lineDrawerNoContact.lineSize = 0.05f;
        lineDrawerNoContact.DrawLineInGameView(hitBarrier.point, hit, Color.red);
    }
    public void HideRedLine()
    {
        if (foresight3 != null)
        {
            lineDrawerNoContact.lineSize = 0f;
            lineDrawerNoContact.DrawLineInGameView(foresight3.transform.position, Vector3.forward, Color.white);
        }
    }

    public void ShowGrenadeTrajectory(Vector3 origin, Vector3 speed, Vector3 finalPoint)
    {
        Vector3[] points = new Vector3[100];
        lineRenderer.positionCount = points.Length;
        var max = origin;

        for (int i = 0; i < points.Length; i++)
        {
            float time = i * 0.1f;

            points[i] = origin + speed * time + Physics.gravity * Mathf.Pow(time, 2) / 2f;

            if (points[i].y < finalPoint.y && max.y > points[i].y)
            {
                points[i] = finalPoint;
                lineRenderer.positionCount = i+1;
                break;
            }
            max = points[i];
        }

        lineRenderer.SetPositions(points);
    }

    public Transform RecursiveFindChild(Transform parent, string childName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == childName)
            {
                return child;
            }
            else
            {
                Transform found = RecursiveFindChild(child, childName);
                if (found != null)
                {
                    return found;
                }
            }
        }
        return null;
    }

    public void SetWeapon()
    {
        foresight3 = RecursiveFindChild(transform, "Foresight3").gameObject;
        shutter = RecursiveFindChild(transform, "Shutter").gameObject;
        rig = RecursiveFindChild(transform, "AimTarget").gameObject;
        magazine = RecursiveFindChild(transform, "Magazine").gameObject;
        shutterAnimator = weaponHolder.GetChild(0).GetComponentInChildren<Animator>();
        fireRate = selection.currentWeapon.fireRate;
        effectiveDistance = selection.currentWeapon.effectiveDistance;
        aimDecrease = selection.currentWeapon.AimDecrease;
    }

    public void TakeNewMagazine()
    {
        var newMagazine = Instantiate(magazine, magazine.transform);
        newMagazine.transform.parent = RecursiveFindChild(transform, "MagazineHolder").transform;
        newMagazine.transform.localPosition = new Vector3();
        newMagazine.transform.localEulerAngles = new Vector3();
        var magObjs = newMagazine.GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer obj in magObjs)
            obj.enabled = true;
        Destroy(newMagazine, 1f);
    }

    public void ThrowGrenade(Vector3 throwForceGrenade, bool manualControl)
    {
        animator.SetTrigger("ThrowGrenade");
        animator.SetBool("ThrowGrenadeOver", false);

        for (int i = 0; i < selection.inventory_items.Count; i++)
        {
            if (selection.inventory_items[i].type == selection.currentGrenade.type && selection.inventory_items[i].Id == selection.currentGrenade.Id)
            {
                selection.inventory_items.RemoveAt(i);
                break;
            }
        }
        if (IsThisCurrentChar())
        {
            if (inventory.activeSelf)
                inventoryManager.StartCoroutine(inventoryManager.InsertCoroutine(selection.inventory_items));
            UIController.ChangeGrenade();
        }

        if (selection.currentWeapon != null)
        {
            weaponHolder.GetChild(0).transform.parent = magHolder;
        }

        StartCoroutine(TakeGrenadeOnThrow(throwForceGrenade, manualControl, 0.33f));
    }

    public IEnumerator TakeGrenadeOnThrow(Vector3 throwForceGrenade, bool manualControl, float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);
        var grenadeModel = selection.currentGrenade.model.GetComponent<Collectable>();
        var grenade = Instantiate(grenadeModel);
        grenade.transform.parent = weaponHolder;
        var objectTranciver = grenade.transform.Find("WeaponPosition");
        grenade.transform.localPosition = objectTranciver.transform.localPosition;
        grenade.transform.localEulerAngles = objectTranciver.transform.localEulerAngles;

        grenade.rigidbody.isKinematic = true;
        grenade.explosive.currentPlayer = selection.player;
        grenade.explosive.manualControl = manualControl;
        grenade.explosive.enabled = false;
        grenade.outline.enabled = false;
        grenade.enabled = false;
        StartCoroutine(ThrowGrenade(grenade, 1.66f, throwForceGrenade));
    }

    public IEnumerator ThrowGrenade(Collectable grenade, float timeToWait, Vector3 throwForceGrenade)
    {
        yield return new WaitForSeconds(timeToWait);
        grenade.explosive.enabled = true;
        grenade.transform.parent = null;
        grenade.rigidbody.isKinematic = false;
        grenade.collider.enabled = true;
        grenade.rigidbody.AddForce(throwForceGrenade, ForceMode.Impulse);
        // grenade.GetComponent<Rigidbody>().velocity *= 1.5f;
        StartCoroutine(ReturnWeapon(0.36f));
    }

    public IEnumerator ReturnWeapon(float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);
        selection.currentGrenade = null;
        if (selectManager.selectedArmy.Any())
        {
            selectManager.UnitInfoGUIOn(true);
        }
        if (selection.currentWeapon != null)
        {
            var currentModel = magHolder.GetChild(0);
            currentModel.transform.parent = weaponHolder.transform;
            var objectTranciver = RecursiveFindChild(weaponHolder.transform, "WeaponPosition");
            currentModel.transform.localPosition = objectTranciver.transform.localPosition;
            currentModel.transform.localEulerAngles = objectTranciver.transform.localEulerAngles;
        }
        animator.SetBool("ThrowGrenadeOver", true);
    }

    public IEnumerator resetRMBTaps(float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);
        rightMouseButtonTaps = 0;
    }

    public float CalculateDamage(GameObject collision, float damage, float distanceKoef, float distance)
    {
        float damageKoef = 1f;
        if (new[] { "Foot", "Leg" }.Any(c => collision.gameObject.name.Contains(c)))
        {
            damageKoef = 0.85f;
        }
        else if (new[] { "Arm", "Hand" }.Any(c => collision.gameObject.name.Contains(c)))
        {
            damageKoef = 0.75f;
        }
        else if (new[] { "Head", "Neck" }.Any(c => collision.gameObject.name.Contains(c)))
        {
            damageKoef = 2f;
        }
        else if (new[] { "Spine1" }.Any(c => collision.gameObject.name.Contains(c)))
        {
            damageKoef = 1.3f;
        }

        var currentDamage = damage * (1 + Mathf.Pow(distanceKoef, distance)) * damageKoef;
        return currentDamage;
    }

    public void ReceiveDamage(int currentPlayer, GameObject collision, Collision contactPoint, float currentDamage, bool manualControl)
    {
        float hitForce = 1.4f;
        SelectableCharacter target = selection;
        Image hitmarkerToShow;

        if (target.player != currentPlayer)
        {
            bool isAlreadyDead = target.health <= 0;
            
            target.health -= currentDamage;
            //Debug.Log("Damage: " + damage * (1 + Mathf.Pow(distanceKoef, (collision.contacts[0].point - startPoint).magnitude)) * damageKoef + "; Koef: " + damageKoef);
            // Debug.Log("Target health: " + target.health);

            if (!isAlreadyDead)
            {
                
                if (target.health <= 0)
                {
                    hitmarkerToShow = hitmarkerKill;
                    Die();
                }
                else
                {
                    hitmarkerToShow = hitmarker;
                    animator.SetTrigger("Hit");
                }

                if (manualControl)
                {
                    var hitmarkerTmp = Instantiate(hitmarkerToShow, Input.mousePosition, Quaternion.LookRotation(Vector3.forward));
                    hitmarkerTmp.transform.SetParent(canvas);
                    hitmarkerTmp.gameObject.layer = 0;
                    Destroy(hitmarkerTmp, 0.3f);
                }
            }

            // Limbs dismemberment
            var limb = collision.gameObject.GetComponent<Limb>();
            if (limb == null)
                limb = collision.gameObject.GetComponentInParent<Limb>();
            if (limb != null)
            {
                limb.limbHealth -= currentDamage;
                if (limb.limbHealth <= 0 && target.health <= 0)
                    limb.GetHit();
            }
            if (contactPoint != null)
            {
                // Rigidbody impulse
                var rigidbodyToForce = collision.gameObject.GetComponent<Rigidbody>();
                if (rigidbodyToForce == null)
                    rigidbodyToForce = collision.gameObject.GetComponentInParent<Rigidbody>();
                rigidbodyToForce.AddForceAtPosition(-contactPoint.contacts[0].normal * hitForce * currentDamage, contactPoint.contacts[0].point, ForceMode.Impulse);

                // Blood spots on the ground
                if (Random.Range(0, 10) <= 6)
                {
                    var bloodPos = contactPoint.contacts[0].point;
                    bloodPos.y = 0;
                    float bloodSpread = 0.4f;
                    var bloodSpotDecal = Instantiate(bloodSpot[Random.Range(0, bloodSpot.Length)], bloodPos + new Vector3(Random.Range(-bloodSpread, bloodSpread), 0.05f, Random.Range(-bloodSpread, bloodSpread)), Quaternion.LookRotation(-target.transform.up));
                    bloodSpotDecal.transform.rotation = Quaternion.AngleAxis(Random.Range(0, 360), bloodSpotDecal.transform.forward) * bloodSpotDecal.transform.rotation;
                }
            }
        }
    }

    public void Die()
    {
        lineRenderer.enabled = false;
        grenadeTrajectorySphere.SetActive(false);
        HideWhiteLine();
        HideRedLine();

        var armyCount = selectManager.selectedArmy.Count;
        var isThisCurrentChar = IsThisCurrentChar();
        selectManager.selectableChars.Remove(selection);
        if (selection.player == selectManager.player)
            selectManager.selectedArmy.Remove(selection);

        if (selection.selectedSprite.enabled || selection.currentSprite.enabled)
        {
            if (isThisCurrentChar)
            {
                if (armyCount > 1)
                {
                    selectManager.UnitInfoGUIOn();
                }
                else
                {
                    selectManager.UnitInfoGUIOff();
                }
            }
        }

        unitVision.enabled = false;
        corpse.enabled = true;
        selection.TurnOffAll();
        agent.ResetPath();
        animator.SetTrigger("Death");
        foreach (Rigidbody rigidbody in rigidbodies)
        {
            if (rigidbody != null)
            {
                rigidbody.isKinematic = false;
            }
        }
        foreach (Collider collider in colliders)
        {
            if (collider != null)
            {
                if (collider.gameObject != animator.gameObject)
                    collider.enabled = true;
                else
                    collider.enabled = false;
            }
        }
        if (selection.currentWeapon != null)
        {
            var item = weaponHolder.gameObject.GetComponentInChildren<Collectable>();
            if (item != null)
            {
                item.enabled = true;
                item.rigidbody.isKinematic = false;
                item.collider.enabled = true;
                item.inventoryItem.enabled = true;
                item.inventoryItem.SetData(selection.currentWeapon);
                item.outline.enabled = true;
                item.gameObject.transform.parent = null;
            }
        }
        

        animator.enabled = false;
        enabled = false;
    }

    public bool IsThisCurrentChar()
    {
        SelectableCharacter currentChar = null;
        if (selectManager.selectedArmy.Any())
            currentChar = selectManager.selectedArmy[0];
        return selection == currentChar;
    }

    public bool isUsingWeapon()
    {
        return !animator.GetBool("ThrowGrenadeOver") || !animator.GetBool("TakeWeaponOver") || !animator.GetBool("ReloadOver"); ;
    }
}
