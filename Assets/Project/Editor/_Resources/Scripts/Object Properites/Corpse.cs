using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using VariableCode.Cursor;

public class Corpse : MonoBehaviour
{
    public SelectableCharacter soldier;
    public SampleScene inventoryManager;
    public SelectManager selectManager;
    public UIController UIController;
    public Outline outline;
    public GameObject toolTip;
    public GameObject currentToolTip;
    public CursorSwitcher cursorSwitcher;
    public int itemId;
    public string itemName;
    public string type;
    private bool wantToExamine = false;
    private float examineDistance = 1.5f;

    void Awake()
    {
        inventoryManager = GameObject.Find("CanvasParent").GetComponent<SampleScene>();
        selectManager = GameObject.Find("SelectingBox").GetComponent<SelectManager>();
        cursorSwitcher = GameObject.Find("CursorManager").GetComponent<CursorSwitcher>();
        UIController = GameObject.Find("UIController").GetComponent<UIController>();
        outline = GetComponent<Outline>();
    } 

    // Update is called once per frame
    void Update()
    {
        if (UIController.isActiveCorpseHighlight || Input.GetKey(KeyCode.V))
        {
            if (soldier.player == selectManager.player)
                outline.OutlineColor = Color.green;
            else
                outline.OutlineColor = Color.red;
            outline.OutlineWidth = 5;
        }
        else
        {
            if (outline.OutlineColor != Color.blue)
                outline.OutlineWidth = 0;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100))
        {
            var corpse = hit.collider.GetComponentInParent<Corpse>();
            if (corpse != null && !IsMouseOverUI())
            {
                if (corpse == this && (UIController.isActiveCorpseHighlight || Input.GetKey(KeyCode.V)))
                {
                    outline.OutlineWidth = 5;
                    outline.OutlineColor = Color.blue;
                    if (currentToolTip == null)
                    {
                        currentToolTip = Instantiate(toolTip, Camera.main.WorldToScreenPoint(Input.mousePosition), Quaternion.LookRotation(Vector3.forward));
                        currentToolTip.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
                        currentToolTip.transform.SetParent(GameObject.Find("Canvas").transform);
                    }
                    var toolTipText = currentToolTip.transform.Find("TooltipStroke").Find("TooltipBackground");
                    toolTipText.transform.Find("TooltipName").GetComponent<TextMeshProUGUI>().text = soldier.personName;
                    toolTipText.transform.Find("TooltipSpecialization").GetComponent<TextMeshProUGUI>().text = soldier.specialization;
                    float tooltipSize;
                    if (soldier.personName.Length > soldier.specialization.Length)
                        tooltipSize = soldier.personName.Length;
                    else
                        tooltipSize = soldier.specialization.Length;

                    currentToolTip.transform.Find("TooltipStroke").GetComponent<RectTransform>().sizeDelta = new Vector2(tooltipSize * 9 + 10, currentToolTip.transform.Find("TooltipStroke").GetComponent<RectTransform>().sizeDelta.y);
                    toolTipText.GetComponent<RectTransform>().sizeDelta = new Vector2(tooltipSize * 9 + 10, toolTipText.GetComponent<RectTransform>().sizeDelta.y);

                    /*
                    if (selectManager.selectedArmy.Any() && !Input.GetKey(KeyCode.LeftControl))
                        cursorSwitcher.ChangeType(1);
                    */

                    if (Input.GetMouseButton(1) && selectManager.selectedArmy.Any())
                    {
                        wantToExamine = true;
                    }
                }
            }
            else
            {
                /*
                if (!Input.GetKey(KeyCode.V) && !UIController.isActiveCollectableHighlight)
                {
                    outline.OutlineWidth = 0;
                }
                if (soldier.player == selectManager.player)
                    outline.OutlineColor = Color.green;
                else
                    outline.OutlineColor = Color.red;
                */
                if (!IsMouseOverUI())
                    Destroy(currentToolTip);
                /*
                if (!Input.GetKey(KeyCode.LeftControl))
                    cursorSwitcher.ChangeType(0);
                */
                if (Input.GetMouseButton(1))
                    wantToExamine = false;
            }
        }

        if (wantToExamine && (gameObject.transform.position - selectManager.selectedArmy[0].transform.position).magnitude <= examineDistance)
        {
            // Examine
        }
    }

    private bool IsMouseOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }
}
