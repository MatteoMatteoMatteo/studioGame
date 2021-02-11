using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public GameObject bullet;
    
    public bool canAutoFire;
    public float fireRate;
    public int currentAmo, pickupAmount;
    public Transform firepoint;
    public float zoomAmount;
    public string gunName;
    [HideInInspector]
    public float fireCounter;

    private void Start()
    {
        UIController.instance.ammoText.text = currentAmo + " Bullets";
    }

    private void Update()
    {
        if (fireCounter > 0)
        {
            fireCounter -= Time.deltaTime;
        }
    }

    public void GetAmmo()
    {
        currentAmo += pickupAmount;
        UIController.instance.ammoText.text = currentAmo + " Bullets";
    }

    public void ShootBullet()
    {
        Instantiate(bullet, firepoint.position, firepoint.rotation);
    }
}
