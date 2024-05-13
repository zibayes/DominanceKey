using System;
using Unity.VisualScripting;
using UnityEngine;
using VariableInventorySystem;

namespace VariableInventorySystem.Sample
{
    public class ItemCellData : IVariableInventoryCellData
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
        public GameObject model { get; set; }
        public float fireRate { get; set; }
        public float effectiveDistance { get; set; }
        public int magSize { get; set; }
        public int currentAmmo { get; set; }
        public float AimDecrease { get; set; }
        public float reloadTime { get; set; }
        public float damage { get; set; }
        public int burstSize { get; set; }
        public int burstSizeDelta { get; set; }
        public float nextBurstTime { get; set; }
        public float nextBurstTimeDelta { get; set; }
        public string name { get; set; }
        public float bulletSpeed { get; set; }
        public string type { get; set; }
        public string subType { get; set; }
        public string ammoType { get; set; }
        public int selectId { get; set; }
        public bool IsStackable { get; set; }
        public int currentAmount { get; set; }
        public int maxAmount { get; set; }
        public float distanceKoef { get; set; }
        public Vector3 rightHandPos { get; set; }
        public Vector3 rightHandRot { get; set; }

        public ItemCellData(InventoryItem item)
        {
            Width = item.Width;
            Height = item.Height;
            shotSFX = item.shotSFX;
            reloadSFX = item.reloadSFX;
            image = item.image;
            model = item.model;
            fireRate = item.fireRate;
            effectiveDistance = item.effectiveDistance;
            magSize = item.magSize;
            currentAmmo = item.currentAmmo;
            AimDecrease = item.AimDecrease;
            reloadTime = item.reloadTime;
            damage = item.damage;
            burstSize = item.burstSize;
            burstSizeDelta = item.burstSizeDelta;
            nextBurstTime = item.nextBurstTime;
            nextBurstTimeDelta = item.nextBurstTimeDelta;
            bulletSpeed = item.bulletSpeed;
            type = item.type;
            subType = item.subType;
            ammoType = item.ammoType;
            name = item.name;
            IsStackable = item.IsStackable;
            currentAmount = item.currentAmount;
            maxAmount = item.maxAmount;
            distanceKoef = item.distanceKoef;
            rightHandPos = item.rightHandPos;
            rightHandRot = item.rightHandRot;
        }

        public void SetData(InventoryItem item)
        {
            Width = item.Width;
            Height = item.Height;
            shotSFX = item.shotSFX;
            reloadSFX = item.reloadSFX;
            image = item.image;
            model = item.model;
            fireRate = item.fireRate;
            effectiveDistance = item.effectiveDistance;
            magSize = item.magSize;
            currentAmmo = item.currentAmmo;
            AimDecrease = item.AimDecrease;
            reloadTime = item.reloadTime;
            damage = item.damage;
            burstSize = item.burstSize;
            burstSizeDelta = item.burstSizeDelta;
            nextBurstTime = item.nextBurstTime;
            nextBurstTimeDelta = item.nextBurstTimeDelta;
            bulletSpeed = item.bulletSpeed;
            type = item.type;
            subType = item.subType;
            ammoType = item.ammoType;
            name = item.name;
            IsStackable = item.IsStackable;
            currentAmount = item.currentAmount;
            maxAmount = item.maxAmount;
            distanceKoef = item.distanceKoef;
            rightHandPos = item.rightHandPos;
            rightHandRot = item.rightHandRot;
        }

        public ItemCellData(DataStorage item)
        {
            Width = item.Width;
            Height = item.Height;
            shotSFX = item.shotSFX;
            reloadSFX = item.reloadSFX;
            image = item.image;
            model = item.model;
            fireRate = item.fireRate;
            effectiveDistance = item.effectiveDistance;
            magSize = item.magSize;
            currentAmmo = item.currentAmmo;
            AimDecrease = item.AimDecrease;
            reloadTime = item.reloadTime;
            damage = item.damage;
            burstSize = item.burstSize;
            burstSizeDelta = item.burstSizeDelta;
            nextBurstTime = item.nextBurstTime;
            nextBurstTimeDelta = item.nextBurstTimeDelta;
            bulletSpeed = item.bulletSpeed;
            type = item.type;
            subType = item.subType;
            ammoType = item.ammoType;
            name = item.name;
            IsStackable = item.IsStackable;
            currentAmount = item.currentAmount;
            maxAmount = item.maxAmount;
            distanceKoef = item.distanceKoef;
            rightHandPos = item.rightHandPos;
            rightHandRot = item.rightHandRot;
        }

        public void SetData(DataStorage item)
        {
            Width = item.Width;
            Height = item.Height;
            shotSFX = item.shotSFX;
            reloadSFX = item.reloadSFX;
            image = item.image;
            model = item.model;
            fireRate = item.fireRate;
            effectiveDistance = item.effectiveDistance;
            magSize = item.magSize;
            currentAmmo = item.currentAmmo;
            AimDecrease = item.AimDecrease;
            reloadTime = item.reloadTime;
            damage = item.damage;
            burstSize = item.burstSize;
            burstSizeDelta = item.burstSizeDelta;
            nextBurstTime = item.nextBurstTime;
            nextBurstTimeDelta = item.nextBurstTimeDelta;
            bulletSpeed = item.bulletSpeed;
            type = item.type;
            subType = item.subType;
            ammoType = item.ammoType;
            name = item.name;
            IsStackable = item.IsStackable;
            currentAmount = item.currentAmount;
            maxAmount = item.maxAmount;
            distanceKoef = item.distanceKoef;
            rightHandPos = item.rightHandPos;
            rightHandRot = item.rightHandRot;
        }
    }
}