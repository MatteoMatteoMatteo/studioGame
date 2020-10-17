using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyControllerFancy : MonoBehaviour
{
    private bool _chasing = false;
    public float distanceToChase = 20, distanceToLose = 25, distanceToStop = 2;
    private Vector3 _targetPoint, _startPoint;
    public NavMeshAgent agent;
    public GameObject bullet;
    public Transform firepoint;

    public float fireRate, waitBetweenShots, timeToShoot = 1f;
    private float _fireRate, _shotWaitCounter, _shootTimeCounter;

    public Animator anim;
    
    private void Start()
    {
        _startPoint = transform.position;
        _shootTimeCounter = timeToShoot;
        _shotWaitCounter = waitBetweenShots;
    }

    private void Update()
    {
        _targetPoint = PlayerController.instance.transform.position;

        if (!_chasing)
        {
            //Start Chasing
            if (Vector3.Distance(transform.position, _targetPoint) < distanceToChase)
            {
                _chasing = true;
                _shootTimeCounter = timeToShoot;
                
                //How long enemy waits with shooting at first contact with player
                _shotWaitCounter = 1f;
            }
        }
        else
        {
            //Check how far enemy is from player
            if (Vector3.Distance(transform.position, _targetPoint) > distanceToStop)
            {
                //Follow player
                agent.destination = _targetPoint;
            }else 
            {
                Vector3 targetDir = _targetPoint - transform.position;
                float angle = Vector3.SignedAngle(targetDir, transform.forward, Vector3.up);
               
                //If Enemy is looking at player stop
                if (Mathf.Abs(angle) < 30f)
                {
                    agent.destination = transform.position;
                }
                //Follow as long as enemy faces player
                else
                {
                    agent.destination = _targetPoint;
                }
            }
            
            //Check if distance to lose is reached
            if (Vector3.Distance(transform.position, _targetPoint) > distanceToLose)
            {
                _chasing = false;
                agent.destination = _startPoint;
            }

            //--------Manage Shooting--------
            
            //Shooting brake time
            if (_shotWaitCounter > 0)
            {
                _shotWaitCounter -= Time.deltaTime;
            }
            else
            {
                //Shooting time interval
                _shootTimeCounter -= Time.deltaTime;
                if (_shootTimeCounter > 0)
                {
                    //How fast is enemy shooting
                    _fireRate -= Time.deltaTime;
                    if (_fireRate <= 0)
                    {
                        _fireRate = fireRate;
                        
                        //Dont shoot at the feet of player but higher
                        firepoint.LookAt(_targetPoint+new Vector3(0f,1.2f,0f));
                        
                        //Check angle of the player
                        Vector3 targetDir = _targetPoint - transform.position;
                        float angle = Vector3.SignedAngle(targetDir, transform.forward, Vector3.up);

                        if (Mathf.Abs(angle) < 30f )
                        {
                            //Fire shot
                            Instantiate(bullet, firepoint.position, firepoint.rotation);  
                            
                            //Fire Animation
                            anim.SetTrigger("fireShot");
                            
                        }
                        else
                        {
                            _shotWaitCounter = waitBetweenShots;
                        }
                    } 
                }
                //New shooting interval begins counting 
                else
                {
                    _shotWaitCounter = waitBetweenShots;
                    _shootTimeCounter = timeToShoot;
                }
            }
        }
    }
}
