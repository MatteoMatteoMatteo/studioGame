using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealthController : MonoBehaviour
{

    public int currentHealth = 5;
    
    public void DamageEnemy(int damageAmount)
    {
        currentHealth-=damageAmount;
        
        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
}
