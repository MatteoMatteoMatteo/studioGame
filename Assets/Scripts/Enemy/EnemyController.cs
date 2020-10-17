using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    private bool _chasing = false;
    public float distanceToChase = 30, distanceToLose = 20, distanceToStartShooting = 10;
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
    
    private void RotateTowards (Transform target) {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5);
    }

    private void Update()
    {
        _targetPoint = PlayerController.instance.transform.position;

        if (!_chasing)
        {
            if (Vector3.Distance(transform.position, _targetPoint) < distanceToChase)
            {
                _chasing = true;
            }
        }
        else if(_chasing)
        {
            Vector3 targetDir = _targetPoint - transform.position;
            float angle = Vector3.SignedAngle(targetDir, transform.forward, Vector3.up);

            if (Vector3.Distance(transform.position, _targetPoint) < distanceToStartShooting)
            {
                //Shooting
                _fireRate -= Time.deltaTime;
                if (_fireRate <= 0)
                {
                    _fireRate = fireRate;
                    firepoint.LookAt(_targetPoint+new Vector3(0f,1.2f,0f));
                    if (Mathf.Abs(angle) < 30f )
                    {
                        agent.destination = transform.position;
                        Instantiate(bullet, firepoint.position, firepoint.rotation);
                        anim.SetBool("isMoving", false);
                        anim.SetTrigger("fireShot");
                    }
                    else
                    {
                        
                        agent.destination = _targetPoint;
                        anim.SetBool("isMoving", true);
                    }
                }
            }
            else
            {
                //Following
                _fireRate = 0.3f;
                agent.destination = _targetPoint; 
                anim.SetBool("isMoving", true);
            }
            
            //Cancel chasing
            if (Vector3.Distance(transform.position, _targetPoint) > distanceToLose)
            {
                agent.destination = _startPoint;
                if (Vector3.Distance(transform.position, _startPoint) < 1)
                {
                    _chasing = false;
                    anim.SetBool("isMoving", false);
                }
            }
        }
    }
}
