using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VariableInventorySystem;
using VariableInventorySystem.Sample;

public class SampleScene : MonoBehaviour
{
    [SerializeField] public StandardCore standardCore;
    [SerializeField] public StandardStashView standardStashView; 
    [SerializeField] public UnityEngine.UI.Button rotateButton;

    void Awake()
    {
        standardCore.Initialize();
        standardCore.AddInventoryView(standardStashView);

        // rotateButton.onClick.AddListener(standardCore.SwitchRotate);

        // StartCoroutine(InsertCoroutine());
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            standardCore.SwitchRotate();
        }
    }

    public IEnumerator InsertCoroutine(List<ItemCellData> inventory_items = null)
    {
        var stashData = new StandardStashViewData(6, 10);

        if(inventory_items == null || inventory_items.Count == 0)
        {
            standardStashView.Apply(stashData);
            yield return null;
        }
        else
        {
            for (var i = 0; i < inventory_items.Count; i++)
            {
                if (inventory_items[i].Id == null)
                {
                    inventory_items[i].Id = stashData.GetInsertableId(inventory_items[i]).Value;
                }
                stashData.InsertInventoryItem((int) inventory_items[i].Id, inventory_items[i]);
                standardStashView.Apply(stashData);

                yield return null;
            }
        }
    }
}
