using System;
using UnityEngine;

[Serializable]
public struct ArmPos
{
    public Vector3 pos; 
    public Vector3 rot;
}

[CreateAssetMenu(fileName = "New Weapon", menuName = "MikeyTools/Create Weapon")]
public class BaseWeapon : EquippableItem
{
    public float FireRate = 0.5f;
    public int ammo = 30;
    public float damage = 10f;
    public AudioClip FireClip;
    public float FireSpreadAngle = 0.05f;

    public ArmPos rightArmPos, leftArmPos;

    public override void Use()
    {
        Debug.Log(ItemName + "is equipped");
    }
}
