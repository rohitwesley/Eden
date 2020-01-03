using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodInfo : AgentInfo
{
    float _amountRemaining = 1;

    public float Consume (float amount) {
        float amountConsumed = Mathf.Max (0, Mathf.Min (_amountRemaining, amount));
        _amountRemaining -= amount;

        transform.localScale = Vector3.one * _amountRemaining;

        if (_amountRemaining <= 0) {
            // EcoManagmentSystem.RegisterPlantDeath(this);
        }

        return amountConsumed;
    }

}
