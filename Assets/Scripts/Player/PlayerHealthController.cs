using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthController : MonoBehaviour
{
    public int maxHealth, currentHealth;
    public static PlayerHealthController instance;
    public float invincibleLength = 1f;
    private float _invincibilityCounter;
    private void Awake()
    {
        instance = this;
        
    }
    private void Start()
    {
        currentHealth = maxHealth;
        UIController.instance.healthSlider.maxValue = maxHealth;
        UIController.instance.healthSlider.value = currentHealth;
        UIController.instance.healthText.text = "HEALTH: " + currentHealth + " / " + maxHealth;
    }

    private void Update()
    {
        if (_invincibilityCounter > 0)
        {
            _invincibilityCounter -= Time.deltaTime;
        }
    }

    public void DamagePlayer(int damageAmount)
    {

        if (_invincibilityCounter <= 0)
        {
            UIController.instance.ShowDamage();
            currentHealth -= damageAmount;
            if (currentHealth <= 0)
            {
                currentHealth = 0;
                GameManager.instance.PlayerDied();
            }
        }
        _invincibilityCounter = invincibleLength;
        
        UIController.instance.healthSlider.value = currentHealth;
        UIController.instance.healthText.text = "HEALTH: " + currentHealth + " / " + maxHealth;
    }

    public void HealPlayer(int healAmount)
    {
        currentHealth += healAmount;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        
        UIController.instance.healthSlider.value = currentHealth;
        UIController.instance.healthText.text = "HEALTH: " + currentHealth + " / " + maxHealth;
    }
}
