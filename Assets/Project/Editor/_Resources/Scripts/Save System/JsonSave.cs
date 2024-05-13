using UnityEngine;
using System.IO;
using VariableInventorySystem.Sample;
using System.Collections.Generic;

public class JsonSave : MonoBehaviour
{
    public SelectableCharacter player;
    private DataStorage[] data;
    private string path = null;

    public void LoadInventory()
    {
        if (path == null)
            path = Path.Combine(Application.persistentDataPath, "data.json");
        if (File.Exists(path))
        {
            data = JsonHelper.FromJson<DataStorage>(File.ReadAllText(path));
            player.inventory_items = new List<ItemCellData>();
            for (int i = 0; i < data.Length; i++)
            {
                player.inventory_items.Add(new ItemCellData(data[i]));
            }
            for (int i = 0; i < player.inventory_items.Count; i++)
            {
                if (player.inventory_items[i].type == "weapon")
                {
                    player.inventory_items[i].currentAmmo = player.inventory_items[i].magSize;
                }
            }
            ItemCellData currentWeapon = null;
            for (int i = 0; i < player.inventory_items.Count; i++)
            {
                if (player.inventory_items[i].type == "weapon" && currentWeapon == null)
                {
                    currentWeapon = player.inventory_items[i];

                    var weaponHolder = player.playerController.weaponHolder;
                    if (weaponHolder.childCount > 0)
                        Destroy(weaponHolder.GetChild(0).gameObject);
                    var currentModel = Instantiate(currentWeapon.model, weaponHolder.transform);
                    currentModel.transform.parent = weaponHolder.transform;
                    var collectable = currentModel.GetComponent<Collectable>();
                    player.playerController.SetWeapon();
                    collectable.collider.enabled = false;
                    collectable.rigidbody.isKinematic = true;
                    collectable.outline.enabled = false;
                    collectable.inventoryItem.enabled = false;
                    collectable.enabled = false;
                    break;
                }
            }

            StartCoroutine(player.playerController.UIController.WeaponChanging(true, player, 0f));
        }
    }

    public void SaveInventory()
    {
        if (path == null)
            path = Path.Combine(Application.persistentDataPath, "data.json");
        data = new DataStorage[player.inventory_items.Count];
        for (int i = 0; i < player.inventory_items.Count; i++)
        {
            data[i] = new DataStorage(player.inventory_items[i]);
        }
        File.WriteAllText(path, JsonHelper.ToJson(data, true));
    }
}
