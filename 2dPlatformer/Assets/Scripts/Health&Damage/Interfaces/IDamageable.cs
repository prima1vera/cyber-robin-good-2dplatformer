using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    void TakeDamage(int damageAmount, Transform hitSource = null, UnityEngine.Vector2? hitPoint = null);
    void ReceiveHealing(int amount);
}

