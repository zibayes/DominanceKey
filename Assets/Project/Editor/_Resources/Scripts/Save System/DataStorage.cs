using System;
using UnityEngine;
using VariableInventorySystem;
using VariableInventorySystem.Sample;

[Serializable]
public class DataStorage
{
    int? id = null;
    public int Width;
    public int Height;
    public bool IsRotate;
    public IVariableInventoryAsset ImageAsset;
    public AudioClip[] shotSFX;
    public AudioClip reloadSFX;
    public Sprite image;
    public GameObject model;
    public float fireRate;
    public float effectiveDistance;
    public int magSize;
    public int currentAmmo;
    public float AimDecrease;
    public float reloadTime;
    public float damage;
    public int burstSize;
    public int burstSizeDelta;
    public float nextBurstTime;
    public float nextBurstTimeDelta;
    public string name;
    public float bulletSpeed;
    public string type;
    public string subType;
    public string ammoType;
    public bool IsStackable;
    public int currentAmount;
    public int maxAmount;
    public float distanceKoef;
    public Vector3 rightHandPos;
    public Vector3 rightHandRot;

    public DataStorage(ItemCellData item)
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
