using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class SelectManager : MonoBehaviour
{
    [Tooltip("The camera used for highlighting")]
    public Camera selectCam;
    [Tooltip("The rectangle to modify for selection")]
    public RectTransform SelectingBoxRect;

    private Rect SelectingRect;
    private Vector3 SelectingStart;

    [Tooltip("Changes the minimum square before selecting characters. Needed for single click select")]
    public float minBoxSizeBeforeSelect = 10f;
    public float selectUnderMouseTimer = 0.1f;
    private float selectTimer = 0f;

    public List<Image> squadIcons = new List<Image>();
    public List<Image> crewRoles = new List<Image>();
    public List<SelectableCharacter> selectableChars = new List<SelectableCharacter>();
    public List<SelectableCharacter> selectedArmy = new List<SelectableCharacter>();

    public Sprite driver;
    public Sprite gunner;
    public Sprite charger;
    public Sprite commander;

    public Sprite infantryIcon;
    public Sprite tankIcon;

    public Image portrait;
    public TextMeshProUGUI personName;
    public TextMeshProUGUI specialization;
    public Dropdown weaponSelect;
    public Text ammoCount;
    public Dropdown grenadeSelect;
    public Text grenadesCount;

    public GameObject unitInfoGUI;
    public GameObject panelCentral;
    public GameObject panelAction;
    public GameObject toolTip;
    public GameObject Inventory;

    public SampleScene inventoryManager;
    public UIController uiController;

    public Sprite emptyImage;

    public int player = 0;

    private void Awake() {
        if (!SelectingBoxRect) {
            SelectingBoxRect = GetComponent<RectTransform>();
        }

        SelectableCharacter[] chars = FindObjectsOfType<SelectableCharacter>();
        for (int i = 0; i <= (chars.Length - 1); i++) {
            selectableChars.Add(chars[i]);
        }

        for (int i = 0; i < squadIcons.Count; i++)
        {
            var postfix = i == 0 ? "" : " (" + i + ")";
            squadIcons[i] = GameObject.Find("SquadUnitIcon" + postfix).GetComponent<Image>();
            if (i < crewRoles.Count)
            {
                crewRoles[i] = squadIcons[i].gameObject.transform.Find("CrewRole").GetComponent<Image>();
            }
        }

        unitInfoGUI.SetActive(false);
        panelCentral.SetActive(false);
        panelAction.SetActive(false);
    }

    void Update() {
        if (SelectingBoxRect == null) {
            Debug.LogError("There is no Rect Transform to use for selection!");
            return;
        }

        // Selecting
        if (Input.GetMouseButtonDown(0) && !IsMouseOverUI() && !Input.GetKey(KeyCode.LeftControl) && !uiController.isActiveManualControl && 
            !inventoryManager.standardStashView.isDragging && !uiController.isActiveAttack && !uiController.isActiveRotate) {
            ReSelect();

            // Sets up the screen box
            SelectingStart = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
            SelectingBoxRect.anchoredPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        } else if (Input.GetMouseButtonUp(0)) {
            selectTimer = 0f;
        }

        if (Input.GetMouseButton(0) && !IsMouseOverUI() && !Input.GetKey(KeyCode.LeftControl) && !uiController.isActiveManualControl && 
            !inventoryManager.standardStashView.isDragging && !uiController.isActiveAttack && !uiController.isActiveRotate) {
            SelectingArmy();
            selectTimer += Time.deltaTime;

            //Only check if there is a character under the mouse for a fixed time
            if (selectTimer <= selectUnderMouseTimer) {
                CheckIfUnderMouse();
            }
            if (!selectedArmy.Any())
            {
                UnitInfoGUIOff();
            }
            else
            {
                UnitInfoGUIOn();
            }
        }
        else
        {
            SelectingBoxRect.sizeDelta = new Vector2(0, 0);
        }

        if (selectedArmy.Any())
        {
            if (selectedArmy.Count == 1 && selectedArmy[0].tankController != null)
            {
                var tankController = selectedArmy[0].tankController;
                int index;
                for (int i = 0; i < squadIcons.Count; i++)
                {
                    if (i < tankController.crewAmount)
                    {
                        if (i < tankController.crewAmount && tankController.crew[i] != null)
                        {
                            index = tankController.crew[i].placeInTank;
                            squadIcons[index].gameObject.SetActive(true);
                            squadIcons[index].sprite = infantryIcon;
                            crewRoles[index].gameObject.SetActive(true);
                            crewRoles[index].sprite = getRoleIcon(i);
                        }
                    }
                    if (i >= tankController.countCrew())
                    {
                        squadIcons[i].gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                for (int i = 0; i <= squadIcons.Count - 1; i++)
                {
                    if (i < selectedArmy.Count)
                    {
                        squadIcons[i].gameObject.SetActive(true);
                        if (selectedArmy[i].playerController != null)
                            squadIcons[i].sprite = infantryIcon;
                        else if (selectedArmy[i].tankController != null)
                            squadIcons[i].sprite = tankIcon;
                        if (i < crewRoles.Count)
                        {
                            crewRoles[i].gameObject.SetActive(false);
                        }
                    }
                    else
                    {
                        squadIcons[i].gameObject.SetActive(false);
                    }
                }
            }
        }
    }

    //Resets what is currently being selected
    void ReSelect() {
        for (int i = 0; i <= (selectedArmy.Count - 1); i++) {
            selectedArmy[i].TurnOffSelector();
            selectedArmy.Remove(selectedArmy[i]);
        }
    }

    //Does the calculation for mouse dragging on screen
    //Moves the UI pivot based on the direction the mouse is going relative to where it started
    //Update: Made this a bit more legible
    void SelectingArmy() {
        Vector2 _pivot = Vector2.zero;
        Vector3 _sizeDelta = Vector3.zero;
        Rect _rect = Rect.zero;

        //Controls x's of the pivot, sizeDelta, and rect
        if (-(SelectingStart.x - Input.mousePosition.x) > 0) 
        {
            _sizeDelta.x = -(SelectingStart.x - Input.mousePosition.x);
            _rect.x = SelectingStart.x;
        } 
        else 
        {
            _pivot.x = 1;
            _sizeDelta.x = (SelectingStart.x - Input.mousePosition.x);
            _rect.x = SelectingStart.x - SelectingBoxRect.sizeDelta.x;
        }

        //Controls y's of the pivot, sizeDelta, and rect
        if (SelectingStart.y - Input.mousePosition.y > 0) 
        {
            _pivot.y = 1;
            _sizeDelta.y = SelectingStart.y - Input.mousePosition.y;
            _rect.y = SelectingStart.y - SelectingBoxRect.sizeDelta.y;
        } 
        else 
        {
            _sizeDelta.y = -(SelectingStart.y - Input.mousePosition.y);
            _rect.y = SelectingStart.y;
        }

        //Sets pivot if of UI element
        if (SelectingBoxRect.pivot != _pivot)
            SelectingBoxRect.pivot = _pivot;

        //Sets the size
        SelectingBoxRect.sizeDelta = _sizeDelta;

        //Finished the Rect set up then set rect
        _rect.height = SelectingBoxRect.sizeDelta.x;
        _rect.width = SelectingBoxRect.sizeDelta.y;
        SelectingRect = _rect;

        //Only does a select check if the box is bigger than the minimum size.
        //While checking it messes with single click
        if (_rect.height > minBoxSizeBeforeSelect && _rect.width > minBoxSizeBeforeSelect) {
            CheckForSelectedCharacters();
        }
    }

    //Checks if the correct characters can be selected and then "selects" them
    void CheckForSelectedCharacters() {
        foreach (SelectableCharacter soldier in selectableChars) {
            if (soldier.isActiveAndEnabled)
            {
                Vector2 screenPos = selectCam.WorldToScreenPoint(soldier.transform.position);
                if (SelectingRect.Contains(screenPos))
                {
                    if (soldier.player == player)
                    {
                        if (!selectedArmy.Contains(soldier))
                            selectedArmy.Add(soldier);
                        soldier.TurnOnSelector();
                    }
                    if (selectedArmy.Any())
                        selectedArmy[0].TurnOnCurrent();
                }
                else if (!SelectingRect.Contains(screenPos) && soldier.player == player)
                {
                    soldier.TurnOffSelector();

                    if (selectedArmy.Contains(soldier))
                        selectedArmy.Remove(soldier);
                }
            }
        }
    }

    //Checks if there is a character under the mouse that is on the Selectable list
    void CheckIfUnderMouse() {
        RaycastHit hit;
        Ray ray = selectCam.ScreenPointToRay(Input.mousePosition);
        ReSelect();
        //Raycast from mouse and select character if its hit!
        if (Physics.Raycast(ray, out hit, 100f)) {
            if(hit.transform != null)
            {
                PlayerController soldier = hit.transform.gameObject.GetComponentInParent<PlayerController>();
                if (soldier != null)
                {
                    var selection = soldier.selection;
                    if (selectableChars.Contains(selection) && !selectedArmy.Contains(selection) && selection.player == player)
                    {
                        selectedArmy.Add(selection);
                        selection.TurnOnSelector();
                    }
                }
                else
                {
                    TankController tankController = hit.transform.gameObject.GetComponentInParent<TankController>();
                    if (tankController != null)
                    {
                        var selection = tankController.selection;
                        if (selectableChars.Contains(selection) && !selectedArmy.Contains(selection) && selection.player == player)
                        {
                            selectedArmy.Add(selection);
                            selection.TurnOnSelector();
                        }
                    }
                }                
                if (selectedArmy.Any())
                    selectedArmy[0].TurnOnCurrent();
            }
        }
    }

    public bool IsMouseOverUI()
    {
         return EventSystem.current.IsPointerOverGameObject();
    }

    public void UnitInfoGUIOn(bool usedGrenade = false)
    {
        uiController.isChoseAnotherUnit = true;
        unitInfoGUI.SetActive(true);
        panelAction.SetActive(true);
        panelCentral.SetActive(true);
        uiController.SetCurrentValues();
        if (uiController.isActiveManualControl && !usedGrenade)
            uiController.ToggleManualControl();
        SelectableCharacter currentChar = selectedArmy[0];
        currentChar.TurnOnCurrent();
        portrait.sprite = currentChar.portrait;
        personName.text = currentChar.personName;
        specialization.text = currentChar.specialization;

        weaponSelect.options.Clear();

        if (currentChar.playerController != null)
        {
            weaponSelect.options.Add(new Dropdown.OptionData("", emptyImage));
            if (currentChar.currentWeapon != null)
            {
                weaponSelect.captionImage.sprite = currentChar.currentWeapon.image;
                weaponSelect.captionText.text = currentChar.currentWeapon.selectId + ". " + currentChar.currentWeapon.name;
                ammoCount.text = currentChar.currentWeapon.currentAmmo + "/" + currentChar.currentWeapon.magSize;
            }
            else
            {
                weaponSelect.captionImage.sprite = emptyImage;
                weaponSelect.captionText.text = "";
                ammoCount.text = "";
            }
            for (int i = 0; i < currentChar.inventory_items.Count; i++)
            {
                if (currentChar.inventory_items[i].type == "weapon")
                {
                    currentChar.inventory_items[i].selectId = weaponSelect.options.Count;
                    weaponSelect.options.Add(new Dropdown.OptionData(currentChar.inventory_items[i].selectId + ". " + currentChar.inventory_items[i].name, currentChar.inventory_items[i].image));
                    if (currentChar.inventory_items[i] == currentChar.currentWeapon)
                        weaponSelect.value = currentChar.inventory_items[i].selectId;
                }
            }
            if (currentChar.currentWeapon == null)
                weaponSelect.value = 0;

            grenadeSelect.options.Clear();
            if (currentChar.currentGrenade != null)
            {
                grenadeSelect.captionImage.sprite = currentChar.currentGrenade.image;
                grenadeSelect.captionText.text = currentChar.currentGrenade.selectId + ". " + currentChar.currentGrenade.name;
                grenadesCount.text = currentChar.currentGrenade.currentAmmo + "/" + currentChar.currentGrenade.magSize;
            }
            else
            {
                grenadeSelect.captionImage.sprite = emptyImage;
                grenadeSelect.captionText.text = "";
                grenadesCount.text = "";
            }
            grenadeSelect.options.Add(new Dropdown.OptionData("", emptyImage));
            for (int i = 0; i < currentChar.inventory_items.Count; i++)
            {
                if (currentChar.inventory_items[i].type == "grenade")
                {
                    currentChar.inventory_items[i].selectId = grenadeSelect.options.Count;
                    grenadeSelect.options.Add(new Dropdown.OptionData(currentChar.inventory_items[i].selectId + ". " + currentChar.inventory_items[i].name, currentChar.inventory_items[i].image));
                    if (currentChar.inventory_items[i] == currentChar.currentGrenade)
                        grenadeSelect.value = currentChar.inventory_items[i].selectId;
                }
            }
            if (currentChar.currentGrenade == null)
                grenadeSelect.value = 0;
        } 
        else if (currentChar.tankController != null)
        {
            weaponSelect.options.Add(new Dropdown.OptionData("0. " + currentChar.tankController.mainGun.name, currentChar.tankController.mainGun.image));
            if (ReferenceEquals(currentChar.tankController.currentWeapon, currentChar.tankController.mainGun))
            {
                weaponSelect.captionImage.sprite = currentChar.tankController.currentWeapon.image;
                weaponSelect.captionText.text = "0. " + currentChar.tankController.currentWeapon.name;
                ammoCount.text = currentChar.tankController.currentWeapon.currentAmmo + "/" + currentChar.tankController.currentWeapon.magSize;
                
                weaponSelect.value = 1;
            }
            else
            {
                weaponSelect.captionImage.sprite = emptyImage;
                weaponSelect.captionText.text = "";
                ammoCount.text = "";
                weaponSelect.value = 0;
            }

            grenadeSelect.options.Clear();
            grenadeSelect.options.Add(new Dropdown.OptionData("", emptyImage));
            if (currentChar.tankController.pairedMgun != null)
                grenadeSelect.options.Add(new Dropdown.OptionData("0. Спаренный пулемёт", currentChar.tankController.pairedMgun.image)); //  + currentChar.tankController.pairedMgun.name
            if (currentChar.tankController.courseMgun != null)
                grenadeSelect.options.Add(new Dropdown.OptionData("1. Курсовой пулемёт", currentChar.tankController.courseMgun.image)); //  + currentChar.tankController.courseMgun.name
            if ((ReferenceEquals(currentChar.tankController.currentWeapon, currentChar.tankController.pairedMgun) || 
                ReferenceEquals(currentChar.tankController.currentWeapon, currentChar.tankController.courseMgun)) && currentChar.tankController.currentWeapon != null)
            {
                int index = 0;
                if (ReferenceEquals(currentChar.tankController.currentWeapon, currentChar.tankController.courseMgun))
                    index = 1;
                grenadeSelect.captionImage.sprite = currentChar.tankController.currentWeapon.image;
                grenadeSelect.captionText.text = index + ". " + currentChar.currentGrenade.name;
                grenadesCount.text = currentChar.tankController.currentWeapon.currentAmmo + "/" + currentChar.tankController.currentWeapon.magSize;
                grenadeSelect.value = index + 1;
            }
            else
            {
                grenadeSelect.captionImage.sprite = emptyImage;
                grenadeSelect.captionText.text = "";
                grenadesCount.text = "";
                grenadeSelect.value = 0;
            }
        }
        
        
        

        if (Inventory.activeSelf)
        {
            inventoryManager.StartCoroutine(inventoryManager.InsertCoroutine(currentChar.inventory_items));
        }
        uiController.isChoseAnotherUnit = false;
    }

    public void UnitInfoGUIOff()
    {
        unitInfoGUI.SetActive(false);
        panelAction.SetActive(false);
        panelCentral.SetActive(false);
        if (Inventory.activeSelf)
        {
            inventoryManager.StartCoroutine(inventoryManager.InsertCoroutine());
        }
        if (uiController.isActiveManualControl)
            uiController.ToggleManualControl();
    }

    

    public void ChoseSquadSoldier(int index)
    {
        // TODO: if (!(index == 2 && tankController.mainGunOnReload))
        if (selectedArmy.Count == 1 && selectedArmy[0].tankController != null)
        {
            var tankController = selectedArmy[0].tankController;
            for (int i = tankController.crewAmount - 1; i >= 0; i--)
            {
                PlayerController soldier = tankController.crew[i];
                if (soldier != null)
                {
                    if (index == soldier.placeInTank)
                    {
                        soldier.gameObject.SetActive(true);
                        soldier.enabled = true;
                        soldier.selection.enabled = true;
                        soldier.transform.position = tankController.boardingPlace.position;
                        tankController.crew[i] = null;
                    }
                    else if (soldier.placeInTank > index)
                    {
                        soldier.placeInTank--;
                    }
                }
            }
        }
        else
        {
            for (int i = selectedArmy.Count - 1; i >= 0; i--)
            {
                if (i != index)
                {
                    selectedArmy[i].TurnOffSelector();
                    selectedArmy.Remove(selectedArmy[i]);
                }
                else
                {
                    selectedArmy[i].TurnOnCurrent();
                }
            }
            UnitInfoGUIOn();
        }
        toolTip.SetActive(false);
    }

    public Sprite getRoleIcon(int index)
    {
        if (index == 0)
            return driver;
        if (index == 1)
            return gunner;
        if (index == 2)
            return charger;
        if (index == 3)
            return commander;
        return null;
    }
}
