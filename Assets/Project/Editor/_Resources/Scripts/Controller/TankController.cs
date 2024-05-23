using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
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

    public int player;
    public Rigidbody rigidbody;
    public NavMeshAgent agent;
    public SelectableCharacter selection;
    public float roatationSpeed = 0.7f;
    public float turretRoatationSpeed = 0.1f;
    public float turretRoatationThreshold = 0.15f;
    public float rotationProgress = 0f;
    public float speed = 6f;
    public Vector3 directionVector;
    public float rotationVector;
    public float reloadOver = 0f;
    public LayerMask whatCanBeClickedOn;

    public float spreadSize;
    public float firingAimDecrease = 0f;
    public float firingAimDecreaseMax = 10f;

    public bool mainGunLoaded = true;
    public bool mainGunOnReload = false;
    public float mainGunAimDecrease = 10f;

    public float nextShoot = 0f;

    public int crewAmount = 4;
    public PlayerController[] crew;

    public ParticleSystem blackSmoke;
    public ParticleSystem muzzleFlash;
    public MissaleTracer missaleTracer;
    public BulletTracer bulletTracer;

    public AudioSource audioSourceShoot;
    public Animator gunAnimator;

    public AudioSource audioSourceMgun;

    public AudioClip moveSFX;
    public AudioSource audioSourceMove;
    public AudioClip engineSFX;
    public AudioClip engineUpSFX;
    public AudioClip engineEndSFX;
    public AudioSource audioSourceEngine;
    public AudioClip turretSFX;
    public AudioSource audioSourceTurret;
    public Animator animatorChasisLeft;
    public Animator animatorChasisRight;

    public float standardSpeed = 6f;
    public float afteburnerSpeed = 12f;
    public bool afteburner = false;

    public GameObject mainGunObj;
    public InventoryItem mainGun;
    public GameObject pairedMgunObj;
    public InventoryItem pairedMgun;
    public GameObject courseMgunObj;
    public InventoryItem courseMgun;
    public InventoryItem currentWeapon;
    public GameObject turret;
    public Transform boardingPlace;

    public int ammoNeed;
    public int sum;
    public bool mgunOnReload = false;

    public GameObject rig;
    public GameObject aimTargetCursor;
    public GameObject rigCourse;
    public GameObject aimTargetCursorCourse;

    public int rightMouseButtonTaps = 0;

    private LineDrawer lineDrawer;
    private LineDrawer lineDrawerNoContact;
    public SpriteRenderer aimCircle;
    public TextMeshProUGUI rangefinder;

    public Image hitmarker;
    public Image hitmarkerKill;

    public List<Rigidbody> rigidbodies;
    public List<Collider> colliders;
    public Collider collider;

    void Start()
    {
        selectManager = GameObject.Find("SelectingBox").GetComponent<SelectManager>();
        inventoryManager = GameObject.Find("CanvasParent").GetComponent<SampleScene>();
        inventory = GameObject.Find("Inventory");
        UIController = GameObject.Find("UIController").GetComponent<UIController>();
        canvas = GameObject.Find("Canvas").transform;
        cameraRig = GameObject.Find("CameraRig").transform;
        cursorSwitcher = GameObject.Find("CursorManager").GetComponent<CursorSwitcher>();

        lineDrawer = new LineDrawer();
        lineDrawerNoContact = new LineDrawer();

        rigidbodies = new List<Rigidbody>(GetComponentsInChildren<Rigidbody>());
        colliders = new List<Collider>(GetComponentsInChildren<Collider>());

        if (mainGun != null)
        {
            mainGun = (InventoryItem)mainGun.Clone();
            mainGun.model = mainGunObj;
            mainGun.currentAmmo = mainGun.magSize;
        }
        if (pairedMgun != null)
        {
            pairedMgun = (InventoryItem)pairedMgun.Clone();
            pairedMgun.model = pairedMgunObj;
            pairedMgun.currentAmmo = pairedMgun.magSize;
        }
        if (courseMgun != null)
        {
            courseMgun = (InventoryItem)courseMgun.Clone();
            courseMgun.model = courseMgunObj;
            courseMgun.currentAmmo = courseMgun.magSize;
        }

        currentWeapon = mainGun;

        audioSourceMove.Stop();

        foreach (PlayerController crewmate in crew)
        {
            if (crewmate != null)
            {
                crewmate.enabled = false;
                crewmate.selection.enabled = false;
                crewmate.gameObject.SetActive(false);
            }
        }
    }
    void Update()
    {
        // Die();
        // Aim decrease decrement
        if (firingAimDecrease > 0)
            firingAimDecrease -= 0.08f;
        else
            firingAimDecrease = 0f;

        if (Time.time > reloadOver && mainGunOnReload)
        {
            mainGunLoaded = true;
            mainGunOnReload = false;
        }

        if (mgunOnReload && Time.time > reloadOver)
        {
            mgunOnReload = false;
            currentWeapon.currentAmmo = sum;
            if (IsThisCurrentChar())
            {
                UIController.grenadeCount.text = currentWeapon.currentAmmo + "/" + currentWeapon.magSize;
                if (inventory.activeSelf)
                    inventoryManager.StartCoroutine(inventoryManager.InsertCoroutine(selection.inventory_items));
            }
        }

        if (selection.power <= 0f)
        {
            agent.ResetPath();
            selection.power = 0f;
        }

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
            firingAimDecrease += 0.1f;
            if (!audioSourceMove.isPlaying)
            {
                audioSourceMove.PlayOneShot(moveSFX);
                audioSourceEngine.PlayOneShot(engineSFX);
            }

            if (afteburner)
                selection.power -= 0.08f;
            else
                selection.power -= 0.04f;
        }
        else
        {
            // Stop moving
            if (audioSourceMove.isPlaying && !Input.GetKey(KeyCode.LeftControl) && !UIController.isActiveManualControl)
            {
                agent.ResetPath();
                rigidbody.velocity = Vector3.zero;
                audioSourceMove.Stop();
                animatorChasisLeft.SetBool("isMoving", false);
                animatorChasisRight.SetBool("isMoving", false);
                audioSourceEngine.Stop();
                audioSourceEngine.PlayOneShot(engineEndSFX);
                afteburner = false;
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
                if (currentWeapon == mainGun)
                {
                    ReloadMainGun(true);
                }
                else
                {
                    if (!mgunOnReload)
                    {
                        ReloadMgun(true);
                    }
                }
            }

            // Point-and-click Controls
            if (Input.GetMouseButtonDown(1) && selection.power > 0f && IsThisCurrentChar())
            {
                if (getDriver() == null)
                    setDriver(getCrewmateWithLowestPriority());
                if (getDriver() != null)
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
                        animatorChasisLeft.SetBool("isMoving", true);
                        animatorChasisRight.SetBool("isMoving", true);
                        MoveToPoint(hit.point);
                    }
                }
            }
            if (rightMouseButtonTaps >= 2)
            {
                afteburner = true;
            }

            // WASD Controls
            if ((Input.GetKey(KeyCode.LeftControl) || UIController.isActiveManualControl) && IsThisCurrentChar())
            {
                if (Time.time > reloadOver)
                {
                    cursorSwitcher.ChangeType("aim");
                }

                agent.ResetPath();

                if (getDriver() == null)
                    setDriver(getCrewmateWithLowestPriority());
                if (getDriver() != null)
                {
                    // Calculate direction vector
                    float h = Input.GetAxis("Horizontal");
                    float v = Input.GetAxis("Vertical");
                    rotationVector = h;
                    if (h != 0 && v < 0)
                    {
                        h *= -1; // Imitate tank rotation on moving back
                    }
                    if (h != 0 || v == 0)
                    {
                        animatorChasisLeft.SetFloat("Speed", h);
                        animatorChasisRight.SetFloat("Speed", -h);
                    }
                    else
                    {
                        animatorChasisLeft.SetFloat("Speed", v);
                        animatorChasisRight.SetFloat("Speed", v);
                    }
                    if (h != 0 || v != 0)
                    {
                        if (!audioSourceMove.isPlaying)
                        {
                            animatorChasisLeft.SetBool("isMoving", true);
                            animatorChasisRight.SetBool("isMoving", true);
                            audioSourceMove.PlayOneShot(moveSFX);
                            audioSourceEngine.PlayOneShot(engineSFX);
                        }
                        selection.power -= 0.04f;
                    }
                    else
                    {
                        audioSourceMove.Stop();
                        animatorChasisLeft.SetBool("isMoving", false);
                        animatorChasisRight.SetBool("isMoving", false);
                    }

                    if (selection.power > 0f)
                    {
                        directionVector = transform.forward * v;
                        directionVector.y = 0;
                        transform.Rotate(0, h * roatationSpeed, 0, Space.Self);
                        if (Vector3.ClampMagnitude(directionVector, 1).magnitude > 0)
                        {
                            if (rigidbody.velocity.magnitude == 0)
                            {
                                audioSourceEngine.Stop();
                                audioSourceEngine.PlayOneShot(engineUpSFX);
                                animatorChasisLeft.SetBool("isMoving", true);
                                animatorChasisRight.SetBool("isMoving", true);
                            }
                            rigidbody.velocity = Vector3.ClampMagnitude(directionVector, 1) * speed * 1.5f;
                            firingAimDecrease += 0.15f;
                        }
                        else
                        {
                            if (rigidbody.velocity.magnitude > 0)
                            {
                                animatorChasisLeft.SetBool("isMoving", false);
                                animatorChasisRight.SetBool("isMoving", false);
                                audioSourceEngine.Stop();
                                audioSourceEngine.PlayOneShot(engineEndSFX);
                            }
                            rigidbody.velocity = Vector3.zero;
                        }
                    }
                }

                if (getGunner() == null)
                    setGunner(getCrewmateWithLowestPriority());
                if (getGunner() != null)
                {
                    spreadSize = currentWeapon.effectiveDistance - firingAimDecrease;

                    if (currentWeapon != null)
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
                            if (currentWeapon == mainGun)
                            {
                                ShootMainGun(spreadSize, true);
                            }
                            else
                            {
                                ShootMgun(spreadSize, true);
                            }
                        }
                    }
                }
                else
                {
                    HideWhiteLine();
                    HideRedLine();
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
    }

    public void Rotate(Vector3 point)
    {
        if (ReferenceEquals(currentWeapon, mainGun) || ReferenceEquals(currentWeapon, pairedMgun))
        {
            aimTargetCursor.transform.position = point;
            var result = Vector3.Lerp(rig.transform.position, aimTargetCursor.transform.position, turretRoatationSpeed);
            if ((result - rig.transform.position).magnitude > turretRoatationThreshold)
            {
                if (!audioSourceTurret.isPlaying && getGunner() != null)
                    audioSourceTurret.PlayOneShot(turretSFX);
            }
            else 
            {
                audioSourceTurret.Stop();
            }
            rig.transform.position = result;
        }
        else if(ReferenceEquals(currentWeapon, courseMgun))
        {
            aimTargetCursorCourse.transform.position = point;
            rigCourse.transform.position = Vector3.Lerp(rigCourse.transform.position, aimTargetCursorCourse.transform.position, turretRoatationSpeed);
        }
    }

    public void Aiming(Vector3 pointToRotate, float spreadSize, bool manualControl)
    {
        if (manualControl)
        {
            RaycastHit hitBarrier;
            Ray aimRay = new Ray(currentWeapon.model.transform.position, currentWeapon.model.transform.forward);
            Vector3 finalPoint;
            if (Physics.Raycast(aimRay, out hitBarrier, 1000))
            {
                DrawWhiteLine(hitBarrier.point);
                var noContactHit = hitBarrier.point + currentWeapon.model.transform.forward * (pointToRotate - hitBarrier.point).magnitude;
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

    public bool ShootMainGun(float spreadSize, bool manualControl)
    {
        // makingNoise = true;
        // makingNoiseTime = Time.time + makingNoiseCooldown;

        bool thereWasShot = false;
        if (mainGunLoaded)
        {
            thereWasShot = true;

            if (manualControl)
            {
                cursorSwitcher.ChangeType("fire");
            }
            if (gunAnimator != null)
                gunAnimator.SetTrigger("Fire");

            firingAimDecrease += mainGunAimDecrease;
            audioSourceShoot.PlayOneShot(currentWeapon.shotSFX[Random.Range(0, currentWeapon.shotSFX.Length)]); 
            muzzleFlash.Play();
            ParticleSystem flash = Instantiate(muzzleFlash, currentWeapon.model.transform.position, currentWeapon.model.transform.rotation);
            Destroy(flash, 1f);

            var spread = 60f / spreadSize;
            Ray shootRay = new Ray(currentWeapon.model.transform.position, Quaternion.Euler(0, Random.Range(-spread, spread), Random.Range(-spread, spread)) * currentWeapon.model.transform.forward);
            MissaleTracer currentBulletTracer = Instantiate(missaleTracer, currentWeapon.model.transform.position, Quaternion.LookRotation(shootRay.direction));
            currentBulletTracer.selfRigidbody.AddForce(shootRay.direction.normalized * 100f, ForceMode.Impulse);
            currentBulletTracer.currentPlayer = selection.player;
            currentBulletTracer.manualControl = manualControl;
            mainGunLoaded = false;
            ReloadMainGun(manualControl);
        }
        return thereWasShot;
    }

    public void ReloadMainGun(bool manualControl)
    {
        if (getCharger() == null)
            setCharger(getCrewmateWithLowestPriority());
        if (getCharger() != null)
        {
            bool haveAmmo = false;
            // Take ammo from inventory and load it to the gun
            for (int i = 0; i < selection.inventory_items.Count; i++)
            {
                if (selection.inventory_items[i].type == currentWeapon.ammoType)
                {
                    if (selection.inventory_items[i].IsStackable)
                    {
                        if (selection.inventory_items[i].currentAmount == 1)
                        {
                            selection.inventory_items.RemoveAt(i);
                            haveAmmo = true;
                            break;
                        }
                        else
                        {
                            selection.inventory_items[i].currentAmount--;
                            haveAmmo = true;
                            break;
                        }
                    }
                    else
                    {
                        selection.inventory_items.RemoveAt(i);
                        haveAmmo = true;
                        break;
                    }
                }
            }

            if (haveAmmo)
            {
                if (IsThisCurrentChar())
                {
                    if (inventory.activeSelf)
                        inventoryManager.StartCoroutine(inventoryManager.InsertCoroutine(selection.inventory_items));
                }
                if (manualControl)
                {
                    cursorSwitcher.ChangeType("reload");
                }
                mainGunOnReload = true;
                reloadOver = Time.time + currentWeapon.reloadTime;
                audioSourceShoot.PlayOneShot(currentWeapon.reloadSFX);
            }
        }
    }

    public bool ShootMgun(float spreadSize, bool manualControl)
    {
        // makingNoise = true;
        // makingNoiseTime = Time.time + makingNoiseCooldown;

        bool thereWasShot = false;
        if (Time.time > nextShoot && currentWeapon.currentAmmo > 0)
        {
            thereWasShot = true;
            currentWeapon.currentAmmo--;

            if (IsThisCurrentChar())
            {
                UIController.grenadeCount.text = currentWeapon.currentAmmo + "/" + currentWeapon.magSize;
                if (inventory.activeSelf)
                    inventoryManager.StartCoroutine(inventoryManager.InsertCoroutine(selection.inventory_items));
            }
            if (manualControl)
            {
                cursorSwitcher.ChangeType("fire");
            }

            nextShoot = Time.time + 1f / currentWeapon.fireRate;
            firingAimDecrease += currentWeapon.AimDecrease;
            audioSourceShoot.PlayOneShot(currentWeapon.shotSFX[Random.Range(0, currentWeapon.shotSFX.Length)]);
            muzzleFlash.Play();
            ParticleSystem flash = Instantiate(muzzleFlash, currentWeapon.model.transform.position, currentWeapon.model.transform.rotation);
            Destroy(flash, 1f);

            var spread = 60f / spreadSize;
            Ray shootRay = new Ray(currentWeapon.model.transform.position, Quaternion.Euler(0, Random.Range(-spread, spread), Random.Range(-spread, spread)) * currentWeapon.model.transform.forward);
            BulletTracer currentBulletTracer = Instantiate(bulletTracer, currentWeapon.model.transform.position, Quaternion.LookRotation(shootRay.direction));
            currentBulletTracer.selfRigidbody.AddForce(shootRay.direction.normalized * currentWeapon.bulletSpeed, ForceMode.Impulse);
            currentBulletTracer.damage = currentWeapon.damage;
            currentBulletTracer.distanceKoef = currentWeapon.distanceKoef;
            currentBulletTracer.startPoint = currentWeapon.model.transform.position;
            currentBulletTracer.currentPlayer = selection.player;
            currentBulletTracer.manualControl = manualControl;
        }
        return thereWasShot;
    }

    public void ReloadMgun(bool manualControl)
    {
        if (getGunner() == null)
            setGunner(getCrewmateWithLowestPriority());
        if (getGunner() != null)
        {
            if (currentWeapon.currentAmmo < currentWeapon.magSize)
            {
                bool haveAmmo = false;
                ammoNeed = 0;
                List<int> indexes = new List<int>();
                sum = currentWeapon.currentAmmo;
                // Check ammo count
                for (int i = 0; i < selection.inventory_items.Count; i++)
                {
                    if (selection.inventory_items[i].type == currentWeapon.ammoType)
                    {
                        indexes.Add(i);
                        if (selection.inventory_items[i].IsStackable)
                        {
                            if (sum + selection.inventory_items[i].currentAmount >= currentWeapon.magSize)
                            {
                                ammoNeed += currentWeapon.magSize - sum;
                                sum = currentWeapon.magSize;
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

                    mgunOnReload = true;
                    reloadOver = Time.time + currentWeapon.reloadTime;
                    audioSourceShoot.PlayOneShot(currentWeapon.reloadSFX);
                }
            }
        }
    }

    public float CalculateDamage(GameObject collision, float damage, float distanceKoef, float distance)
    {
        float damageKoef = 1f;
        if (new[] { "Chasis", "Wheel", "Track" }.Any(c => collision.gameObject.name.Contains(c)))
        {
            damageKoef = 0.5f;
        }
        else if (new[] { "Turret", "Mantlet", "Gun" }.Any(c => collision.gameObject.name.Contains(c)))
        {
            damageKoef = 0.9f;
        }
        else if (new[] { "Panzer_VI_E" }.Any(c => collision.gameObject.name.Contains(c)))
        {
            damageKoef = 1f;
        }

        var currentDamage = damage * (1 + Mathf.Pow(distanceKoef, distance)) * damageKoef;
        return currentDamage;
    }

    public void ReceiveDamage(int currentPlayer, GameObject collision, Collision contactPoint, float currentDamage, bool manualControl)
    {
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
                }

                if (manualControl)
                {
                    var hitmarkerTmp = Instantiate(hitmarkerToShow, Input.mousePosition, Quaternion.LookRotation(Vector3.forward));
                    hitmarkerTmp.transform.SetParent(canvas);
                    hitmarkerTmp.gameObject.layer = 0;
                    Destroy(hitmarkerTmp, 0.3f);
                }
            }
        }
    }

    public void Die()
    {
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

        selection.TurnOffAll();
        agent.ResetPath();

        foreach (Collider collider in colliders)
        {
            if (collider != null)
            {
                if (collider.gameObject != gameObject)
                    collider.enabled = true;
                else
                    collider.enabled = false;
            }
        }

        setDriver(null);
        setGunner(null);
        setCharger(null);
        setCommander(null);

        blackSmoke.Play();
        ParticleSystem smoke = Instantiate(blackSmoke, transform.position + transform.up, Quaternion.LookRotation(transform.up), transform);
        Destroy(smoke, 60f);
        turret.transform.parent = null;

        selection.enabled = false;
        enabled = false;
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
            lineDrawer.DrawLineInGameView(currentWeapon.model.transform.position, hitBarrier, Color.white);
        }

    }

    public void HideWhiteLine()
    {
        if (currentWeapon != null)
        {
            lineDrawer.lineSize = 0f;
            lineDrawer.DrawLineInGameView(currentWeapon.model.transform.position, Vector3.forward, Color.white);
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
            lineDrawerNoContact.DrawLineInGameView(currentWeapon.model.transform.position, Vector3.forward, Color.white);
        }
    }

    public bool IsThisCurrentChar()
    {
        SelectableCharacter currentChar = null;
        if (selectManager.selectedArmy.Any())
            currentChar = selectManager.selectedArmy[0];
        return selection == currentChar;
    }

    public PlayerController getDriver()
    {
        return crew[0];
    }

    public PlayerController getGunner()
    {
        if (crewAmount > 1)
            return crew[1];
        return null;
    }
    public PlayerController getCharger()
    {
        if (crewAmount > 2)
            return crew[2];
        return null;
    }
    public PlayerController getCommander()
    {
        if (crewAmount > 3)
            return crew[3];
        return null;
    }

    public void setDriver(PlayerController soldier)
    {
        crew[0] = soldier;
    }

    public void setGunner(PlayerController soldier)
    {
        if (crewAmount > 1)
            crew[1] = soldier;
    }
    public void setCharger(PlayerController soldier)
    {
        if (crewAmount > 2)
            crew[2] = soldier;
    }
    public void setCommander(PlayerController soldier)
    {
        if (crewAmount > 3)
            crew[3] = soldier;
    }

    public PlayerController getCrewmateWithLowestPriority()
    {
        PlayerController soldier = getCommander();
        setCommander(null);
        if (soldier != null)
            return soldier;

        soldier = getGunner();
        setGunner(null);
        if (soldier != null)
            return soldier;

        if (!mainGunOnReload)
        {
            soldier = getCharger();
            setCharger(null);
            if (soldier != null)
                return soldier;
        }

        if (directionVector.magnitude == 0 && rotationVector == 0 && agent.remainingDistance == 0)
        {
            soldier = getDriver();
            setDriver(null);
            if (soldier != null)
                return soldier;
        }
        return null;
    }

    public int getFreeCrewRole()
    {
        for (int i = 0; i < crewAmount; i++)
        {
            if (crew[i] == null)
                return i;
        }
        return -1;
    }

    public PlayerController getAnyCrewmate()
    {
        for (int i = 0; i < crewAmount; i++)
        {
            if (crew[i] != null)
                return crew[i];
        }
        return null;
    }

    public int countCrew()
    {
        int sum = 0;
        for (int i = 0; i < crewAmount; i++)
        {
            if (crew[i] != null)
                sum++;
        }
        return sum;
    }
}
