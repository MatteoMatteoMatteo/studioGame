using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    private bool _collected=false;
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player") && !_collected)
        {
            PlayerController.instance.activeGun.GetAmmo();
            Destroy(gameObject);
            _collected = true;
        }
    }
}
