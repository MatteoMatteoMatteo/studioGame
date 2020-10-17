using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public float moveSpeed, lifeTime;
    public Rigidbody theRB;
    public int power;
    public GameObject gravityGunImpactEffect;
    public GameObject enemyImpactEffect;
    public GameObject enemyHeadshotImpactEffect;
    public int damage=1;

    // Update is called once per frame
    void Update()
    {
        theRB.velocity = transform.forward * moveSpeed;

        lifeTime -= Time.deltaTime;

        if (lifeTime <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        var rigid = other.transform.gameObject.GetComponent<Rigidbody>();
        if (other.gameObject.CompareTag("Enemy"))
        {
            other.gameObject.GetComponent<EnemyHealthController>().DamageEnemy(damage);
            Instantiate(enemyImpactEffect, transform.position + (transform.forward * (-moveSpeed * Time.deltaTime)), transform.rotation);
        }
        if (other.gameObject.CompareTag("Headshot"))
        {
            other.transform.parent.GetComponent<EnemyHealthController>().DamageEnemy(damage*2);
            Instantiate(enemyHeadshotImpactEffect, transform.position + (transform.forward * (-moveSpeed * Time.deltaTime)), transform.rotation);
        }

        if (other.gameObject.CompareTag("Player"))
        {
            PlayerHealthController.instance.DamagePlayer(damage);
        }
        else if (rigid)
        {
            rigid.AddForce(transform.forward * power);
            Instantiate(gravityGunImpactEffect, transform.position + (transform.forward * (-moveSpeed * Time.deltaTime)), transform.rotation);
        }
        
        Destroy(gameObject);




    }
}
