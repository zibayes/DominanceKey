using UnityEngine;

namespace VariableInventorySystem
{
    public interface IVariableInventoryCellData
    {
        int? Id { get; set; }
        int Width { get; }
        int Height { get; }
        bool IsRotate { get; set; }
        AudioClip[] shotSFX { get; set; }
        AudioClip reloadSFX { get; set; }
        Sprite image { get; set; }
        float fireRate { get; set; }
        float effectiveDistance { get; set; }
        int magSize { get; set; }
        int currentAmmo { get; set; }
        float AimDecrease { get; set; }
        float reloadTime { get; set; }
        float damage { get; set; }
        string name { get; set; }
        float bulletSpeed { get; set; }
        string type { get; set; }
        string ammoType { get; set; }
        int selectId { get; set; }
        bool IsStackable { get; set; }
        int currentAmount { get; set; }
        int maxAmount { get; set; }
    }
}
