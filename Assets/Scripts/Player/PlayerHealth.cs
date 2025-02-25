using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerHealth : BaseHealth
{
    private GameObject player;

    public PlayerHealth(GameObject gO)
    {
        player = gO;
        Currenthealth = MaxHealth;
        
        if(player != null)
        EventHandler.RegisterEvent<float>(player,GameEvents.OnPlayerHealthUpdate,UpdateHealth);
    }

    ~PlayerHealth()
    {
        if(player != null) 
        EventHandler.UnregisterEvent<float>(player,GameEvents.OnPlayerHealthUpdate, UpdateHealth);
    }

    protected override void UpdateHealth(float health)
    {
        base.UpdateHealth(health);
        if (Currenthealth <= 0)
        {
            EventHandler.ExecuteEvent(player,GameEvents.OnPlayerDied);
        }
    }
}
