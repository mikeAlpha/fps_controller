using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : BaseWeapon
{

    public bool CurrentlySelected = false;

    protected override void OnEnable()
    {
        EventHandler.RegisterEvent(GameEvents.OnPlayerFire, Shoot);
    }

    protected override void OnDisable()
    {
        EventHandler.UnregisterEvent(GameEvents.OnPlayerFire, Shoot);
    }

    protected override void Shoot()
    {
        if (CurrentlySelected)
        {
            base.Shoot();
        }
    }
}