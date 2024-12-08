using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseHealth
{

    [SerializeField] protected float MaxHealth = 100f;
    [SerializeField] protected float Currenthealth = 0f;

    protected virtual void UpdateHealth(float health)
    {
        Currenthealth += health;
    }

}
