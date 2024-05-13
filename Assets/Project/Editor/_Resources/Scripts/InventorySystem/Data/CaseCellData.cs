using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using VariableInventorySystem;

namespace VariableInventorySystem.Sample
{
    public class CaseCellData : IStandardCaseCellData
    {
        int? id = null;
        public int? Id
        {
            set { id = value; }
            get { return id; }
        }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public bool IsRotate { get; set; }
        public IVariableInventoryAsset ImageAsset { get; }
        public AudioClip[] shotSFX { get; set; }
        public AudioClip reloadSFX { get; set; }
        public Sprite image { get; set; }
        public float fireRate { get; set; }
        public float effectiveDistance { get; set; }
        public int magSize { get; set; }
        public int currentAmmo { get; set; }
        public float AimDecrease { get; set; }
        public float reloadTime { get; set; }
        public float damage { get; set; }
        public string name { get; set; }
        public float bulletSpeed { get; set; }
        public string type { get; set; }
        public string ammoType { get; set; }
        public int selectId { get; set; }
        public bool IsStackable { get; set; }
        public int currentAmount { get; set; }
        public int maxAmount { get; set; }

        public StandardCaseViewData CaseData { get; }

        public CaseCellData(int sampleSeed)
        {
            Width = 4;
            Height = 3;
            ImageAsset = new VariableInventorySystem.StandardAsset("Image/chest");
            CaseData = new VariableInventorySystem.StandardCaseViewData(8, 6);
        }
    }
}