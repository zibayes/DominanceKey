using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using VariableCode.Cursor;
using VariableInventorySystem.Sample;

public class Collectable : MonoBehaviour
{
    public SampleScene inventoryManager;
    public SelectManager selectManager;
    public GameObject toolTip;
    public CursorSwitcher cursorSwitcher;
    private Transform canvas;

    public Outline outline;
    public InventoryItem inventoryItem;
    public Explosive explosive;
    public Rigidbody rigidbody;
    public Collider collider;

    public int itemId;
    public string itemName;
    public string type;
    private bool isActiveHighlight = false;
    private bool wantToPickUp = false;
    private float collectDistance = 1.5f;

    public Image highlightIcon;
    public Sprite highlightIconInactive;
    public Sprite highlightIconActive;

    void Awake()
    {
        inventoryManager = GameObject.Find("CanvasParent").GetComponent<SampleScene>();
        selectManager = GameObject.Find("SelectingBox").GetComponent<SelectManager>();
        cursorSwitcher = GameObject.Find("CursorManager").GetComponent<CursorSwitcher>();
        canvas = GameObject.Find("Canvas").transform;

        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();

        toolTip = Instantiate(toolTip);
        toolTip.SetActive(false);
    }

    private void Start()
    {
        toolTip.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
        toolTip.transform.SetParent(canvas);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isActiveHighlight)
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                if (outline.OutlineColor != Color.blue)
                    outline.OutlineColor = Color.red;
                outline.OutlineWidth = 5;
            }
            if (Input.GetKeyUp(KeyCode.C))
            {
                if (outline.OutlineColor != Color.blue)
                    outline.OutlineWidth = 0;
            }
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100))
        {
            var collectable = hit.collider.GetComponent<Collectable>();
            if (collectable != null && !IsMouseOverUI())
            {
                if (collectable == this)
                {
                    outline.OutlineWidth = 5;
                    outline.OutlineColor = Color.blue;
                    toolTip.SetActive(true);
                    toolTip.transform.position = Camera.main.WorldToScreenPoint(Input.mousePosition);
                    toolTip.transform.rotation = Quaternion.LookRotation(Vector3.forward);
                    var toolTipText = toolTip.transform.Find("TooltipStroke").Find("TooltipBackground");
                    toolTipText.transform.Find("TooltipName").GetComponent<TextMeshProUGUI>().text = itemName;
                    toolTipText.transform.Find("TooltipSpecialization").GetComponent<TextMeshProUGUI>().text = type;
                    float tooltipSize;
                    if (itemName.Length > type.Length)
                        tooltipSize = itemName.Length;
                    else
                        tooltipSize = type.Length;

                    toolTip.transform.Find("TooltipStroke").GetComponent<RectTransform>().sizeDelta = new Vector2(tooltipSize * 9 + 10, toolTip.transform.Find("TooltipStroke").GetComponent<RectTransform>().sizeDelta.y);
                    toolTipText.GetComponent<RectTransform>().sizeDelta = new Vector2(tooltipSize * 9 + 10, toolTipText.GetComponent<RectTransform>().sizeDelta.y);

                    if (selectManager.selectedArmy.Any() && !Input.GetKey(KeyCode.LeftControl) && !selectManager.uiController.isActiveManualControl && !selectManager.uiController.isActiveAttack && !selectManager.uiController.isActiveRotate)
                        cursorSwitcher.ChangeType("collect");

                    if (Input.GetMouseButton(1) && selectManager.selectedArmy.Any())
                    {
                        wantToPickUp = true;
                    }
                }
            }
            else
            {
                if (!Input.GetKey(KeyCode.C) && !isActiveHighlight)
                {
                    outline.OutlineWidth = 0;
                }
                outline.OutlineColor = Color.red;
                if (!IsMouseOverUI())
                    toolTip.SetActive(false);
                if (!Input.GetKey(KeyCode.LeftControl) && !selectManager.uiController.isActiveManualControl && !selectManager.uiController.isActiveAttack && !selectManager.uiController.isActiveRotate && cursorSwitcher.current.objectIndex != 6)
                    cursorSwitcher.ChangeType("default");
                if (Input.GetMouseButton(1))
                    wantToPickUp = false;
            }
        }

        if (wantToPickUp && (gameObject.transform.position - selectManager.selectedArmy[0].transform.position).magnitude <= collectDistance)
        {
            toolTip.SetActive(false);
            selectManager.selectedArmy[0].inventory_items.Add(new ItemCellData(inventoryItem));
            if (GameObject.Find("Inventory") != null)
                inventoryManager.StartCoroutine(inventoryManager.InsertCoroutine(selectManager.selectedArmy[0].inventory_items));
            this.gameObject.SetActive(false);
            cursorSwitcher.ChangeType("default");
            var item = selectManager.selectedArmy[0].inventory_items.Last();
            if (item.type == "weapon")
            {
                selectManager.selectedArmy[0].inventory_items.Last().selectId = GameObject.Find("WeaponSelect").GetComponent<Dropdown>().options.Count;
                GameObject.Find("WeaponSelect").GetComponent<Dropdown>().options.Add(new Dropdown.OptionData(selectManager.selectedArmy[0].inventory_items.Last().selectId + ". " + item.name, item.image));
            }
            else if (item.type == "grenade")
            {
                selectManager.selectedArmy[0].inventory_items.Last().selectId = GameObject.Find("GrenadeSelect").GetComponent<Dropdown>().options.Count;
                GameObject.Find("GrenadeSelect").GetComponent<Dropdown>().options.Add(new Dropdown.OptionData(selectManager.selectedArmy[0].inventory_items.Last().selectId + ". " + item.name, item.image));
            }
        }
    }

    public void ToggleHighlight()
    {
        Collectable[] collecatables = FindObjectsOfType<Collectable>().ToArray();
        isActiveHighlight ^= true;
        if (isActiveHighlight)
        {
            highlightIcon.sprite = highlightIconActive;
            for (int i = 0; i < collecatables.Length; i++)
            {
                if (collecatables[i].outline.OutlineColor != Color.blue)
                    collecatables[i].outline.OutlineColor = Color.red;
                collecatables[i].outline.OutlineWidth = 5;
                collecatables[i].isActiveHighlight = isActiveHighlight;
            }
        }
        else
        {
            highlightIcon.sprite = highlightIconInactive;
            for (int i = 0; i < collecatables.Length; i++)
            {
                if (collecatables[i].outline.OutlineColor != Color.blue)
                    collecatables[i].outline.OutlineWidth = 0;
                collecatables[i].isActiveHighlight = isActiveHighlight;
            }
        }
    }

    private bool IsMouseOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }
}
