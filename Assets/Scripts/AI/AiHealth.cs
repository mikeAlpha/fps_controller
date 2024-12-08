using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AiHealth : BaseHealth
{
    private AITree aiController;

    public AiHealth(AITree aiController)
    {
        Currenthealth = MaxHealth;
        this.aiController = aiController;

        EventHandler.RegisterEvent<float>(aiController.gameObject, GameEvents.OnAiHealthUpdate, UpdateHealth);
    }

    ~AiHealth()
    {
        EventHandler.UnregisterEvent<float>(aiController.gameObject, GameEvents.OnAiHealthUpdate, UpdateHealth);
    }

    protected override void UpdateHealth(float health)
    {
        base.UpdateHealth(health);
        if (Currenthealth <= 0)
        {

        }
    }
}
