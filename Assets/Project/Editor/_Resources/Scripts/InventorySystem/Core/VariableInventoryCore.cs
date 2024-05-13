using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace VariableInventorySystem
{
    public abstract class VariableInventoryCore : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        protected List<IVariableInventoryView> InventoryViews { get; set; } = new List<IVariableInventoryView>();

        protected virtual GameObject CellPrefab { get; set; }
        protected virtual RectTransform EffectCellParent { get; set; }

        protected IVariableInventoryCell stareCell;
        protected IVariableInventoryCell effectCell;

        bool? originEffectCellRotate;
        Vector2 cursorPosition;

        public virtual void Initialize()
        {
            effectCell = Instantiate(CellPrefab, EffectCellParent).GetComponent<IVariableInventoryCell>();
            effectCell.RectTransform.gameObject.SetActive(false);
            effectCell.SetSelectable(false);
        }

        public virtual void AddInventoryView(IVariableInventoryView variableInventoryView)
        {
            InventoryViews.Add(variableInventoryView);
            variableInventoryView.SetCellCallback(OnCellClick, OnCellOptionClick, OnCellEnter, OnCellExit);
        }

        public virtual void RemoveInventoryView(IVariableInventoryView variableInventoryView)
        {
            InventoryViews.Remove(variableInventoryView);
        }

        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }

            foreach (var inventoryViews in InventoryViews)
            {
                inventoryViews.OnPrePick(stareCell);
            }

            var stareData = stareCell?.CellData;
            var isHold = InventoryViews.Any(x => x.OnPick(stareCell));

            if (!isHold)
            {
                return;
            }

            effectCell.RectTransform.gameObject.SetActive(true);
            effectCell.Apply(stareData);
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
            if (effectCell?.CellData == null)
            {
                return;
            }

            foreach (var inventoryViews in InventoryViews)
            {
                inventoryViews.OnDrag(stareCell, effectCell, eventData);
            }

            RectTransformUtility.ScreenPointToLocalPointInRectangle(EffectCellParent, eventData.position, eventData.enterEventCamera, out cursorPosition);

            var (width, height) = GetRotateSize(effectCell.CellData);
            effectCell.RectTransform.localPosition = cursorPosition + new Vector2(
                 -(width - 1) * effectCell.DefaultCellSize.x * 0.5f,
                (height - 1) * effectCell.DefaultCellSize.y * 0.5f);
        }

        (int, int) GetRotateSize(IVariableInventoryCellData cell)
        {
            return (cell.IsRotate ? cell.Height : cell.Width, cell.IsRotate ? cell.Width : cell.Height);
        }

        public virtual void OnEndDrag(PointerEventData eventData)
        {
            if (effectCell.CellData == null)
            {
                return;
            }

            var selectedChar = GameObject.Find("SelectingBox").GetComponent<SelectManager>().selectedArmy[0];
            if (!IsMouseOverUI())
            {
                var weaponOptions = GameObject.Find("WeaponSelect").GetComponent<Dropdown>().options;
                var grenadeOptions = GameObject.Find("GrenadeSelect").GetComponent<Dropdown>().options;
                for (int i = 0; i < selectedChar.inventory_items.Count; i++)
                {
                    var inventoryItem = selectedChar.inventory_items[i];
                    if (inventoryItem.Id == effectCell.CellData.Id)
                    {
                        var playerController = selectedChar.GetComponentInParent<PlayerController>();
                        var weaponHolder = playerController.RecursiveFindChild(playerController.transform, "WeaponHolder");
                        var currentObj = Instantiate(inventoryItem.model, weaponHolder.transform.position + playerController.transform.forward * 0.4f, weaponHolder.transform.rotation);
                        currentObj.GetComponent<InventoryItem>().SetData(inventoryItem);
                        currentObj.GetComponent<Rigidbody>().AddForce(-playerController.transform.up + playerController.transform.forward, ForceMode.Impulse);
                        if (inventoryItem.type == "weapon")
                        {
                            for (int j = 0; j < weaponOptions.Count; j++)
                            {
                                if (weaponOptions[j].text == inventoryItem.selectId + ". " + inventoryItem.name)
                                {
                                    if (selectedChar.currentWeapon != null)
                                    {
                                        if (selectedChar.currentWeapon.selectId == inventoryItem.selectId)
                                        {
                                            selectedChar.currentWeapon = null;
                                            GameObject.Find("AmmoCount").GetComponent<Text>().text = "";
                                            GameObject.Find("WeaponSelect").GetComponent<Dropdown>().value = 0;
                                            Destroy(weaponHolder.GetChild(0).gameObject);
                                        }
                                    }
                                    selectedChar.inventory_items.RemoveAt(i);
                                    weaponOptions.RemoveAt(j);
                                    break;
                                }
                            }
                        }
                        else if (inventoryItem.type == "grenade")
                        {
                            for (int j = 0; j < grenadeOptions.Count; j++)
                            {
                                if (grenadeOptions[j].text == inventoryItem.selectId + ". " + inventoryItem.name)
                                {
                                    if (selectedChar.currentGrenade != null)
                                    {
                                        if (selectedChar.currentGrenade.selectId == inventoryItem.selectId)
                                        {
                                            selectedChar.currentGrenade = null;
                                            GameObject.Find("GrenadesCount").GetComponent<Text>().text = "";
                                            GameObject.Find("GrenadeSelect").GetComponent<Dropdown>().value = 0;
                                        }
                                    }
                                    selectedChar.inventory_items.RemoveAt(i);
                                    grenadeOptions.RemoveAt(j);
                                    break;
                                }
                            }
                        }    
                        else
                        {
                            selectedChar.inventory_items.RemoveAt(i);
                            break;
                        }
                    }
                }

                int optionIndex = 1;
                for (int i = 0; i < selectedChar.inventory_items.Count; i++)
                {
                    if (selectedChar.inventory_items[i].type == "weapon")
                    {
                        selectedChar.inventory_items[i].selectId = optionIndex;
                        weaponOptions[optionIndex].text = selectedChar.inventory_items[i].selectId + ". " + selectedChar.inventory_items[i].name;
                        weaponOptions[optionIndex].image = selectedChar.inventory_items[i].image;
                        if (selectedChar.inventory_items[i] == selectedChar.currentWeapon)
                            GameObject.Find("WeaponSelect").GetComponent<Dropdown>().value = optionIndex;
                        optionIndex++;
                    }
                }

                optionIndex = 1;
                for (int i = 0; i < selectedChar.inventory_items.Count; i++)
                {
                    if (selectedChar.inventory_items[i].type == "grenade")
                    {
                        selectedChar.inventory_items[i].selectId = optionIndex;
                        grenadeOptions[optionIndex].text = selectedChar.inventory_items[i].selectId + ". " + selectedChar.inventory_items[i].name;
                        grenadeOptions[optionIndex].image = selectedChar.inventory_items[i].image;
                        if (selectedChar.inventory_items[i] == selectedChar.currentGrenade)
                            GameObject.Find("GrenadeSelect").GetComponent<Dropdown>().value = optionIndex;
                        optionIndex++;
                    }
                }
            }

            var isRelease = InventoryViews.Any(x => x.OnDrop(stareCell, effectCell));


            if (!isRelease && originEffectCellRotate.HasValue)
            {
                effectCell.CellData.IsRotate = originEffectCellRotate.Value;
                effectCell.Apply(effectCell.CellData);
                originEffectCellRotate = null;
            }
            foreach (var inventoryViews in InventoryViews)
            {
                inventoryViews.OnDroped(isRelease);
            }

            effectCell.RectTransform.gameObject.SetActive(false);
            effectCell.Apply(null);

            if (GameObject.Find("Inventory") != null)
            {
                var inventoryManager = GameObject.Find("CanvasParent").GetComponent<SampleScene>();
                inventoryManager.StartCoroutine(inventoryManager.InsertCoroutine(selectedChar.inventory_items));
            }
        }

        public virtual void SwitchRotate()
        {
            if (effectCell.CellData == null)
            {
                return;
            }

            if (!originEffectCellRotate.HasValue)
            {
                originEffectCellRotate = effectCell.CellData.IsRotate;
            }

            effectCell.CellData.IsRotate = !effectCell.CellData.IsRotate;
            effectCell.Apply(effectCell.CellData);

            var (width, height) = GetRotateSize(effectCell.CellData);
            effectCell.RectTransform.localPosition = cursorPosition + new Vector2(
                 -(width - 1) * effectCell.DefaultCellSize.x * 0.5f,
                (height - 1) * effectCell.DefaultCellSize.y * 0.5f);

            foreach (var inventoryViews in InventoryViews)
            {
                inventoryViews.OnSwitchRotate(stareCell, effectCell);
            }
        }

        protected virtual void OnCellClick(IVariableInventoryCell cell)
        {
        }

        protected virtual void OnCellOptionClick(IVariableInventoryCell cell)
        {
        }

        protected virtual void OnCellEnter(IVariableInventoryCell cell)
        {
            stareCell = cell;

            foreach (var inventoryView in InventoryViews)
            {
                inventoryView.OnCellEnter(stareCell, effectCell);
            }
        }

        protected virtual void OnCellExit(IVariableInventoryCell cell)
        {
            foreach (var inventoryView in InventoryViews)
            {
                inventoryView.OnCellExit(stareCell);
            }

            stareCell = null;
        }

        private bool IsMouseOverUI()
        {
            return EventSystem.current.IsPointerOverGameObject();
        }
    }
}
