using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : MonoBehaviour
{
    public string gun;
    private bool _collected = false;
    
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player") && !_collected)
        {
            PlayerController.instance.AddGun(gun);
            Destroy(gameObject);
            _collected = true;
        }
    }
}
