using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public SampleScene inventoryManager;
    public SelectManager selectManager;
    public GameObject Minimap;
    public GameObject Inventory;
    public Text ammoCount;
    
    public bool isChoseAnotherUnit = false;

    public bool isActiveMinimap = false;
    public Image minimapIcon;
    public Sprite minimapIconInactive;
    public Sprite minimapIconActive;

    public bool isActiveInventory = false;
    public Image inventoryIcon;
    public Sprite inventoryIconInactive;
    public Sprite inventoryIconActive;

    public bool isActiveManualControl = false;
    public Image manualControlIcon;
    public Sprite manualControlIconInactive;
    public Sprite manualControlIconActive;

    public bool isActiveCollectableHighlight = false;
    public Image collectableHighlightIcon;
    public Sprite collectableHighlightIconInactive;
    public Sprite collectableHighlightIconActive;

    public bool isActiveCorpseHighlight = false;
    public Image corpseHighlightIcon;
    public Sprite corpseHighlightIconInactive;
    public Sprite corpseHighlightIconActive;

    public Image fireAIIcon;
    public Sprite fireAIIconInactive;
    public Sprite fireAIIconActive;

    public Image moveAIIcon;
    public Sprite moveAIIconInactive;
    public Sprite moveAIIconActive;

    public Image poseIcon;
    public Sprite standPose;
    public Sprite sitPose;
    public Sprite liePose;

    PlayerController playerWithCommand;

    public bool isActiveAttack = false;
    public bool attackUse = false;
    public bool attackWasCalled = false;
    public Image attackIcon;
    public Sprite attackIconInactive;
    public Sprite attackIconActive;

    public bool isActiveRotate = false;
    public bool rotateUse = false;
    public Image rotateIcon;
    public Sprite rotateIconInactive;
    public Sprite rotateIconActive;

    void Start()
    {
        Minimap.SetActive(isActiveMinimap);
        Inventory.SetActive(isActiveInventory);
    }

    public void ToggleMinimap()
    {
        isActiveMinimap ^= true;
        if (isActiveMinimap)
        {
            minimapIcon.sprite = minimapIconActive;
        }
        else
        {
            minimapIcon.sprite = minimapIconInactive;
        }
        Minimap.SetActive(isActiveMinimap);
    }

    public void ToggleInventory()
    {
        isActiveInventory ^= true;
        Inventory.SetActive(isActiveInventory);
        if (isActiveInventory)
        {
            inventoryIcon.sprite = inventoryIconActive;
            if (selectManager.selectedArmy.Count > 0)
            {
                inventoryManager.StartCoroutine(inventoryManager.InsertCoroutine(selectManager.selectedArmy[0].inventory_items));
            }
            else
            {
                inventoryManager.StartCoroutine(inventoryManager.InsertCoroutine());
            }
        }
        else
        {
            inventoryIcon.sprite = inventoryIconInactive;
        }
    }

    public void ToggleManualControl()
    {
        isActiveManualControl ^= true;
        if (isActiveManualControl)
        {
            manualControlIcon.sprite = manualControlIconActive;
        }
        else
        {
            manualControlIcon.sprite = manualControlIconInactive;
        }
    }

    public void ToggleHighlight()
    {
        isActiveCollectableHighlight ^= true;
        if (isActiveCollectableHighlight)
        {
            collectableHighlightIcon.sprite = collectableHighlightIconActive;
        }
        else
        {
            collectableHighlightIcon.sprite = collectableHighlightIconActive;
        }
    }

    public void ToggleCorpseHighlight()
    {
        isActiveCorpseHighlight ^= true;
        if (isActiveCorpseHighlight)
        {
            corpseHighlightIcon.sprite = corpseHighlightIconActive;
        }
        else
        {
            corpseHighlightIcon.sprite = corpseHighlightIconInactive;
        }
    }

    public void ToggleFireAI()
    {
        if (selectManager.selectedArmy.Any())
        {
            if (selectManager.selectedArmy[0].playerController.attackType == "open")
            {
                fireAIIcon.sprite = fireAIIconInactive;
                selectManager.selectedArmy[0].playerController.attackType = "hold";
            }
            else
            {
                fireAIIcon.sprite = fireAIIconActive;
                selectManager.selectedArmy[0].playerController.attackType = "open";
            }
        }
    }

    public void ToggleMoveAI()
    {
        if (selectManager.selectedArmy.Any())
        {
            if (selectManager.selectedArmy[0].playerController.movementType == "free")
            {
                moveAIIcon.sprite = moveAIIconInactive;
                selectManager.selectedArmy[0].playerController.movementType = "hold";
            }
            else
            {
                moveAIIcon.sprite = moveAIIconActive;
                selectManager.selectedArmy[0].playerController.movementType = "free";
            }
        }
    }

    public void SetAttack()
    {
        if (selectManager.selectedArmy.Any())
        {
            isActiveAttack = true;
            attackIcon.sprite = attackIconActive;
            selectManager.selectedArmy[0].playerController.cursorSwitcher.ChangeType("aim");
        }
    }

    public void SetRotate()
    {
        if (selectManager.selectedArmy.Any())
        {
            isActiveRotate = true;
            rotateIcon.sprite = rotateIconActive;
            selectManager.selectedArmy[0].playerController.cursorSwitcher.ChangeType("rotate");
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1) || !selectManager.selectedArmy.Any())
        {
            isActiveAttack = false;
            isActiveRotate = false;
            attackIcon.sprite = attackIconInactive;
            rotateIcon.sprite = rotateIconInactive;
        }
        else if (Input.GetMouseButtonDown(0))
        {
            if (selectManager.selectedArmy.Any())
            {
                if (isActiveAttack)
                {
                    attackUse = true;
                    attackIcon.sprite = attackIconInactive;
                    playerWithCommand = selectManager.selectedArmy[0].playerController;

                    // isActiveAttack = false;
                } 
                else if (isActiveRotate)
                {
                    rotateUse = true;
                    rotateIcon.sprite = rotateIconInactive;
                    playerWithCommand = selectManager.selectedArmy[0].playerController;

                    // isActiveRotate = false;
                }
            }
        }

        if (attackUse)
        {
            var spreadSize = (playerWithCommand.effectiveDistance / playerWithCommand.currentSpread - playerWithCommand.firingAimDecrease * playerWithCommand.currentSpread);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1000))
            {
                playerWithCommand.Aiming(hit.point, spreadSize, false);
            }
            if (!attackWasCalled)
                StartCoroutine(makeShot(spreadSize, 0.3f));
            StartCoroutine(activeAttackOff(0.1f));
            attackWasCalled = true;
        }

        if (rotateUse)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1000))
            {
                playerWithCommand.Rotate(hit.point);
            }
            StartCoroutine(activeRotateOff(0.2f));
        }
    }

    public IEnumerator activeAttackOff(float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);
        isActiveAttack = false;
        attackUse = false;
        attackWasCalled = false;
    }

    public IEnumerator activeRotateOff(float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);
        isActiveRotate = false;
        rotateUse = false;
    }

    public IEnumerator makeShot(float spreadSize, float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);
        selectManager.selectedArmy[0].playerController.Shoot(spreadSize, false);
    }

    public void SetCurrentValues()
    {
        if (selectManager.selectedArmy.Any())
        {
            if (selectManager.selectedArmy[0].playerController != null)
            {
                if (selectManager.selectedArmy[0].playerController.attackType == "open")
                {
                    fireAIIcon.sprite = fireAIIconActive;
                }
                else
                {
                    fireAIIcon.sprite = fireAIIconInactive;
                }

                if (selectManager.selectedArmy[0].playerController.movementType == "free")
                {
                    moveAIIcon.sprite = moveAIIconActive;
                }
                else
                {
                    moveAIIcon.sprite = moveAIIconInactive;
                }

                var animator = selectManager.selectedArmy[0].playerController.animator;
                if (!animator.GetBool("Sit") && !animator.GetBool("Lie"))
                {
                    poseIcon.sprite = standPose;
                }
                else if (animator.GetBool("Sit") && !animator.GetBool("Lie"))
                {
                    poseIcon.sprite = sitPose;
                }
                else if (!animator.GetBool("Sit") && animator.GetBool("Lie"))
                {
                    poseIcon.sprite = liePose;
                }
            }
        }
    }

    public void HealSelf()
    {
        if (selectManager.selectedArmy.Any())
        {
            selectManager.selectedArmy[0].playerController.HealSelf();
        }
    }

    public void Reload()
    {
        if (selectManager.selectedArmy.Any())
        {
            selectManager.selectedArmy[0].playerController.Reload(true);
        }
    }

    public void Cancel()
    {
        if (selectManager.selectedArmy.Any())
        {
            var playerController = selectManager.selectedArmy[0].playerController;
            playerController.agent.ResetPath();
            playerController.TurnOffAiming();
            playerController.TurnOffFiring();
        }
    }

    public void ChangePose()
    {
        if (selectManager.selectedArmy.Any())
        {
            selectManager.selectedArmy[0].playerController.ChangeBodyPosition();
            var animator = selectManager.selectedArmy[0].playerController.animator;
            if (!animator.GetBool("Sit") && !animator.GetBool("Lie"))
            {
                 poseIcon.sprite = standPose;
            } 
            else if (animator.GetBool("Sit") && !animator.GetBool("Lie"))
            {
                poseIcon.sprite = sitPose;
            }
            else if (!animator.GetBool("Sit") && animator.GetBool("Lie"))
            {
                poseIcon.sprite = liePose;
            }
        }
    }

    public void ChangeWeapon()
    {
        if (isChoseAnotherUnit)
            return;
        if ("" == GameObject.Find("WeaponSelect").GetComponent<Dropdown>().captionText.text)
        {
            selectManager.selectedArmy[0].currentWeapon = null;
            GameObject.Find("AmmoCount").GetComponent<Text>().text = "";
            GameObject.Find("WeaponSelect").GetComponent<Dropdown>().captionImage.sprite = selectManager.emptyImage;
            Destroy(RecursiveFindChild(selectManager.selectedArmy[0].transform.parent.transform, "WeaponHolder").GetChild(0).gameObject);
        }
        else
        {
            for (int i = 0; i < selectManager.selectedArmy[0].inventory_items.Count; i++)
            {
                if (selectManager.selectedArmy[0].inventory_items[i].selectId + ". " + selectManager.selectedArmy[0].inventory_items[i].name == GameObject.Find("WeaponSelect").GetComponent<Dropdown>().captionText.text)
                {
                    selectManager.selectedArmy[0].currentWeapon = selectManager.selectedArmy[0].inventory_items[i];
                    selectManager.selectedArmy[0].playerController.animator.SetTrigger("TakeWeapon");
                    selectManager.selectedArmy[0].playerController.animator.SetBool("TakeWeaponOver", false);
                    StartCoroutine(WeaponChanging(soldier: selectManager.selectedArmy[0]));
                    break;
                }
            }
        }     
    }

    public void ChangeGrenade()
    {
        if ("" == GameObject.Find("GrenadeSelect").GetComponent<Dropdown>().captionText.text)
        {
            selectManager.selectedArmy[0].currentGrenade = null;
            GameObject.Find("GrenadesCount").GetComponent<Text>().text = "";
            GameObject.Find("GrenadeSelect").GetComponent<Dropdown>().captionImage.sprite = selectManager.emptyImage;
        }
        else
        {
            for (int i = 0; i < selectManager.selectedArmy[0].inventory_items.Count; i++)
            {
                if (selectManager.selectedArmy[0].inventory_items[i].selectId + ". " + selectManager.selectedArmy[0].inventory_items[i].name == GameObject.Find("GrenadeSelect").GetComponent<Dropdown>().captionText.text)
                {
                    selectManager.selectedArmy[0].currentGrenade = selectManager.selectedArmy[0].inventory_items[i];
                    break;
                }
            }
        }
    }

    public void CenterCameraOnUnit()
    {
        Vector3 Ray_start_position = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        Ray ray = Camera.main.ScreenPointToRay(Ray_start_position);
        RaycastHit hit;
        int layerMask = 1 << 8;
        if (Physics.Raycast(ray, out hit, 100, layerMask))
        {
            var camRig = GameObject.Find("CameraRig").GetComponentInParent<Transform>();
            var distance = (ray.origin - hit.point).magnitude;
            ray.origin = selectManager.selectedArmy[0].transform.position;
            var newPos = ray.GetPoint(-distance);
            if ((newPos - camRig.position).magnitude > 4)
                camRig.position = new Vector3(newPos.x, camRig.position.y, newPos.z);
        }
    }

    Transform RecursiveFindChild(Transform parent, string childName)
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

    public IEnumerator WeaponChanging(bool onAwake = false, SelectableCharacter soldier = null, float timeToWait = 1.33f)
    {
        yield return new WaitForSeconds(timeToWait);
        if (soldier == null && selectManager.selectedArmy.Any())
            soldier = selectManager.selectedArmy[0];
        var weaponHolder = RecursiveFindChild(soldier.gameObject.transform.parent, "WeaponHolder");
        if (weaponHolder.childCount > 0)
            Destroy(weaponHolder.GetChild(0).gameObject);
        var currentModel = Instantiate(soldier.currentWeapon.model, weaponHolder.transform);
        currentModel.SetActive(true);
        currentModel.transform.parent = weaponHolder.transform;
        soldier.gameObject.transform.parent.GetComponent<PlayerController>().SetWeapon();

        SetRigs(weaponHolder, currentModel, "RightHandAim_target");
        // SetRigs(weaponHolder, "RightHandAim_hint");
        SetRigs(weaponHolder, currentModel, "LeftHandAim_target");
        // SetRigs(weaponHolder, "LeftHandAim_hint");
        SetRigs(weaponHolder, currentModel, "LeftHandStand_target");
        // SetRigs(weaponHolder, "LeftHandStand_hint");

        
        var objectTranciver = currentModel.transform.Find("WeaponPosition");
        currentModel.transform.localPosition = objectTranciver.transform.localPosition;
        // currentModel.transform.localEulerAngles = objectTranciver.transform.localEulerAngles;
        

        // selectManager.selectedArmy[index].gameObject.transform.parent.GetComponent<CharacterIK>().SelectCurrentWeapon();
        var colliders = new List<Collider>(currentModel.GetComponentsInChildren<Collider>());
        foreach (Collider collider in colliders)
            collider.enabled = false;
        var rigidbodies = new List<Rigidbody>(currentModel.GetComponentsInChildren<Rigidbody>());
        foreach (Rigidbody rigidbody in rigidbodies)
            rigidbody.isKinematic = true;
        currentModel.GetComponent<Collectable>().enabled = false;
        currentModel.GetComponent<Outline>().enabled = false;
        currentModel.GetComponent<InventoryItem>().enabled = false;

        // soldier.gameObject.GetComponentInParent<PlayerController>().SetWeapon();

        if (!onAwake)
        {
            GameObject.Find("AmmoCount").GetComponent<Text>().text = soldier.currentWeapon.currentAmmo + "/" + selectManager.selectedArmy[0].currentWeapon.magSize;
            StartCoroutine(TakeWeaponOver(soldier, 1.33f));
        }
    }

    IEnumerator TakeWeaponOver(SelectableCharacter soldier, float timeToWait = 1.33f)
    {
        yield return new WaitForSeconds(timeToWait);
        if (soldier == null && selectManager.selectedArmy.Any())
            soldier = selectManager.selectedArmy[0];
        soldier.playerController.animator.SetBool("TakeWeaponOver", true);
    }

    void SetRigs(Transform weaponHolder, GameObject currentModel, string rigObj)
    {
        var objectReciver = weaponHolder.transform.parent.transform.Find(rigObj);
        var objectTranciver = RecursiveFindChild(currentModel.transform, rigObj);
        objectReciver.transform.localPosition = objectTranciver.transform.localPosition;
        objectReciver.transform.localEulerAngles = objectTranciver.transform.localEulerAngles;
    }
}
