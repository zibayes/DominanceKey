using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VariableInventorySystem;
using VariableInventorySystem.Sample;

[Serializable]
public class InventoryItem : MonoBehaviour
{
    public int? id = null;
    [SerializeField]
    public int Width;
    [SerializeField]
    public int Height;
    [SerializeField]
    public bool IsRotate;
    [SerializeField]
    public AudioClip[] shotSFX;
    [SerializeField]
    public AudioClip reloadSFX;
    [SerializeField]
    public Sprite image;
    [SerializeField]
    public GameObject model;
    [SerializeField]
    public float fireRate;
    [SerializeField]
    public float effectiveDistance;
    [SerializeField]
    public int magSize;
    [SerializeField]
    public int currentAmmo;
    [SerializeField]
    public float AimDecrease;
    [SerializeField]
    public float reloadTime;
    [SerializeField]
    public float damage;
    [SerializeField]
    public int burstSize;
    [SerializeField]
    public int burstSizeDelta;
    [SerializeField]
    public float nextBurstTime;
    [SerializeField]
    public float nextBurstTimeDelta;
    [SerializeField]
    public string name;
    [SerializeField]
    public float bulletSpeed;
    [SerializeField]
    public string type;
    [SerializeField]
    public string subType;
    [SerializeField]
    public string ammoType;
    [SerializeField]
    public bool IsStackable;
    [SerializeField]
    public int currentAmount;
    [SerializeField]
    public int maxAmount;
    [SerializeField]
    public float distanceKoef;
    [SerializeField]
    public Vector3 rightHandPos;
    [SerializeField]
    public Vector3 rightHandRot;

    void Awake()
    {
        model = gameObject;
    }

    public InventoryItem(ItemCellData item)
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

    public void SetData(ItemCellData item)
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

    public object Clone()
    {
        return this.MemberwiseClone();
    }
}
