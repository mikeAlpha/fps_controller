using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerHealth : BaseHealth
{
    public PlayerHealth()
    {
        Currenthealth = MaxHealth;

        EventHandler.RegisterEvent<float>(GameEvents.OnPlayerHealthUpdate,UpdateHealth);
    }

    ~PlayerHealth()
    {
        EventHandler.UnregisterEvent<float>(GameEvents.OnPlayerHealthUpdate, UpdateHealth);
    }

    protected override void UpdateHealth(float health)
    {
        base.UpdateHealth(health);
        if (Currenthealth <= 0)
        {

        }
    }
}
