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

    public List<GameObject> squadIcons = new List<GameObject>();
    public List<SelectableCharacter> selectableChars = new List<SelectableCharacter>();
    public List<SelectableCharacter> selectedArmy = new List<SelectableCharacter>();

    public GameObject unitInfoGUI;
    public GameObject panelAction;
    public GameObject toolTip;
    public GameObject Inventory;

    public SampleScene inventoryManager;
    public UIController uiController;

    public Sprite emptyImage;

    public int player = 0;

    public bool formationDone = false;

    private void Awake() {
        //This assumes that the manager is placed on the image used to select
        if (!SelectingBoxRect) {
            SelectingBoxRect = GetComponent<RectTransform>();
        }

        //Searches for all of the objects with the selectable character script
        //Then converts to list
        SelectableCharacter[] chars = FindObjectsOfType<SelectableCharacter>();
        for (int i = 0; i <= (chars.Length - 1); i++) {
            selectableChars.Add(chars[i]);
        }

        unitInfoGUI.SetActive(false);
        panelAction.SetActive(false);

        GameObject squadPanel = GameObject.Find("SquadPanel");
        for (int i = 0; i < squadPanel.transform.childCount; i++)
        {
            squadIcons.Add(squadPanel.transform.GetChild(i).gameObject);
            squadIcons[i].SetActive(false);
        }
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

        for (int i = 0; i <= squadIcons.Count - 1; i++)
        {
            if (i < selectedArmy.Count)
            {
                squadIcons[i].SetActive(true);
            }
            else
            {
                squadIcons[i].SetActive(false);
            }
        }

        // Formation
        /*
        if (selectedArmy.Count > 1)
        {
            var startPos = selectedArmy[0].transform.position;
            var unitWidth = 1.5f;
            var unitDepth = 1.5f;
            for (int i = 0; i < selectedArmy.Count; i++)
            {
                // selectedArmy[0].GetComponentInParent<PlayerController>().agent.ResetPath();
                // selectedArmy[0].GetComponentInParent<PlayerController>().agent.SetDestination(center - new Vector3((i - selectedArmy.Count/2) * unitWidth, 0, 0));
                selectedArmy[0].GetComponentInParent<PlayerController>().gameObject.transform.position = startPos + new Vector3(i * unitWidth, 0, 0);
            }
            formationDone = true;
        }
        */
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
            Vector2 screenPos = selectCam.WorldToScreenPoint(soldier.transform.position);
            if (SelectingRect.Contains(screenPos)) {
                if (soldier.player == player)
                {
                    if (!selectedArmy.Contains(soldier))
                        selectedArmy.Add(soldier);
                    soldier.TurnOnSelector();
                }
                if (selectedArmy.Any())
                    selectedArmy[0].TurnOnCurrent();
            } else if (!SelectingRect.Contains(screenPos) && soldier.player == player) {
                soldier.TurnOffSelector();

                if (selectedArmy.Contains(soldier))
                    selectedArmy.Remove(soldier);
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
        uiController.SetCurrentValues();
        if (uiController.isActiveManualControl && !usedGrenade)
            uiController.ToggleManualControl();
        SelectableCharacter currentChar = selectedArmy[0];
        currentChar.TurnOnCurrent();
        GameObject.Find("Portrait").GetComponent<RawImage>().texture = currentChar.portrait;
        GameObject.Find("PersonName").GetComponent<TextMeshProUGUI>().text = currentChar.personName;
        GameObject.Find("Specialization").GetComponent<TextMeshProUGUI>().text = currentChar.specialization;

        GameObject.Find("WeaponSelect").GetComponent<Dropdown>().options.Clear();
        if (currentChar.currentWeapon != null)
        {
            GameObject.Find("WeaponSelect").GetComponent<Dropdown>().captionImage.sprite = currentChar.currentWeapon.image;
            GameObject.Find("WeaponSelect").GetComponent<Dropdown>().captionText.text = currentChar.currentWeapon.selectId + ". " + currentChar.currentWeapon.name;
            GameObject.Find("AmmoCount").GetComponent<Text>().text = currentChar.currentWeapon.currentAmmo + "/" + currentChar.currentWeapon.magSize;
        }
        else
        {
            GameObject.Find("WeaponSelect").GetComponent<Dropdown>().captionImage.sprite = emptyImage;
            GameObject.Find("WeaponSelect").GetComponent<Dropdown>().captionText.text = "";
            GameObject.Find("AmmoCount").GetComponent<Text>().text = "";
        }
        GameObject.Find("WeaponSelect").GetComponent<Dropdown>().options.Add(new Dropdown.OptionData("", emptyImage));
        for (int i = 0; i < currentChar.inventory_items.Count; i++)
        {
            if (currentChar.inventory_items[i].type == "weapon")
            {
                currentChar.inventory_items[i].selectId = GameObject.Find("WeaponSelect").GetComponent<Dropdown>().options.Count;
                GameObject.Find("WeaponSelect").GetComponent<Dropdown>().options.Add(new Dropdown.OptionData(currentChar.inventory_items[i].selectId + ". " + currentChar.inventory_items[i].name, currentChar.inventory_items[i].image));
                if (currentChar.inventory_items[i] == currentChar.currentWeapon)
                    GameObject.Find("WeaponSelect").GetComponent<Dropdown>().value = currentChar.inventory_items[i].selectId;
            }
        }
        if (currentChar.currentWeapon == null)
            GameObject.Find("WeaponSelect").GetComponent<Dropdown>().value = 0;

        GameObject.Find("GrenadeSelect").GetComponent<Dropdown>().options.Clear();
        if (currentChar.currentGrenade != null)
        {
            GameObject.Find("GrenadeSelect").GetComponent<Dropdown>().captionImage.sprite = currentChar.currentGrenade.image;
            GameObject.Find("GrenadeSelect").GetComponent<Dropdown>().captionText.text = currentChar.currentGrenade.selectId + ". " + currentChar.currentGrenade.name;
            GameObject.Find("GrenadesCount").GetComponent<Text>().text = currentChar.currentGrenade.currentAmmo + "/" + currentChar.currentGrenade.magSize;
        }
        else
        {
            GameObject.Find("GrenadeSelect").GetComponent<Dropdown>().captionImage.sprite = emptyImage;
            GameObject.Find("GrenadeSelect").GetComponent<Dropdown>().captionText.text = "";
            GameObject.Find("GrenadesCount").GetComponent<Text>().text = "";
        }
        GameObject.Find("GrenadeSelect").GetComponent<Dropdown>().options.Add(new Dropdown.OptionData("", emptyImage));
        for (int i = 0; i < currentChar.inventory_items.Count; i++)
        {
            if (currentChar.inventory_items[i].type == "grenade")
            {
                currentChar.inventory_items[i].selectId = GameObject.Find("GrenadeSelect").GetComponent<Dropdown>().options.Count;
                GameObject.Find("GrenadeSelect").GetComponent<Dropdown>().options.Add(new Dropdown.OptionData(currentChar.inventory_items[i].selectId + ". " + currentChar.inventory_items[i].name, currentChar.inventory_items[i].image));
                if (currentChar.inventory_items[i] == currentChar.currentGrenade)
                    GameObject.Find("GrenadeSelect").GetComponent<Dropdown>().value = currentChar.inventory_items[i].selectId;
            }
        }
        if (currentChar.currentGrenade == null)
            GameObject.Find("GrenadeSelect").GetComponent<Dropdown>().value = 0;

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
        if (Inventory.activeSelf)
        {
            inventoryManager.StartCoroutine(inventoryManager.InsertCoroutine());
        }
        if (uiController.isActiveManualControl)
            uiController.ToggleManualControl();
    }

    

public void ChoseSquadSoldier(int index)
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
        toolTip.SetActive(false);
    }
}
