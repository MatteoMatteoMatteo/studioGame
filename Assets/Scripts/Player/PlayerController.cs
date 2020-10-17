using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;
    public Transform grabHolder;
    public CharacterController controller;
    public Transform groundCheck;
    public LayerMask groundMask;
    public Gun activeGun;
    public Transform firePoint;
    public Transform playerBody;
    private float _groundDistance = 0.4f;
    private bool _isGrounded;
    public float walkingSpeed = 12f;
    public float sprint = 20f;
    private bool _isSprinting = false;
    private float _dashingPower=1;
    public float dashingPower;
    private bool _isDashing = false;
    public float gravity = -30f;
    public float jumpHeight = 10;
    private Vector3 _velocity;
    private GameObject _grabbedObj;
    public Transform grabPosition;
    private bool _isGrabbing;
    private Vector3 hookShotPosition;
    public float hookShotSpeedMax;
    public GameObject speedParticles;
    public GameObject dashParticles;
    private float _savedGravity;
    private State state;
    public List<Gun> allGuns = new List<Gun>();
    public List<Gun> unlockableGun = new List<Gun>();
    public int currentGun;
    public Transform adsPoint, gunHolder;
    private Vector3 _gunStartPosition;
    public float adsSpeed = 2f;
    public GameObject muzzleFlash;
    public Animator anim;
    public AudioSource rifleShot;
    private static readonly int AimReloading = Animator.StringToHash("aimReloading");
    private static readonly int IsMoving = Animator.StringToHash("isMoving");
    private static readonly int Jumping = Animator.StringToHash("jumping");
    private static readonly int IsAiming = Animator.StringToHash("isAiming");
    private static readonly int IsAidleShooting = Animator.StringToHash("isAidleShooting");
    private static readonly int IsAimShooting = Animator.StringToHash("isAimShooting");

    private enum  State
    {
        Normal,
        HookshotFlyingPlayer
    }

    private void Awake()
    {
        instance = this;
        _savedGravity = gravity;
        state = State.Normal;
    }

    private void Start()
    {
        currentGun--;
        SwitchGun();
        _gunStartPosition = gunHolder.localPosition;
    }

    void Update()
    {
        switch (state)
        {
            default:
            case State.Normal:
                HandleHookShotStart();
                break;
            case State.HookshotFlyingPlayer:
                HandleHookshotMovement();
                break;
        }
        Movement();
        HandleShot();
        Grab();
        HandleHookShotStart();
    }

    private void Movement()
    {
        //Basic Movement
        _isGrounded = Physics.CheckSphere(groundCheck.position, _groundDistance, groundMask);
        if (_isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f;
        }
        var x = Input.GetAxisRaw("Horizontal");
        var z = Input.GetAxisRaw("Vertical");
        var transform1 = transform;
        var move = transform1.right * x + transform1.forward * z;
        
        //Dashing
        if (_isDashing)
        {
            _dashingPower -= _dashingPower * 1.5f * Time.deltaTime;
            dashParticles.SetActive(false);
            if (_dashingPower < 1f)
            {
                _isDashing = false;
                _dashingPower = 1f;
                dashParticles.SetActive(false);
            }
        }
        if (Input.GetKeyDown(KeyCode.Tab) && state == State.Normal)
        {
            _dashingPower = dashingPower;
            dashParticles.SetActive(true);
            _isDashing = true;
        }

        //Walking & Sprinting
        if (Input.GetKey(KeyCode.LeftShift) && state == State.Normal)
        {
            controller.Move(move * (sprint * Time.deltaTime));
            _isSprinting = true;
            if (!anim.GetBool(AimReloading))
            {
                anim.SetBool(IsMoving, true);
            }

        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            
            _isSprinting = false;
            anim.SetBool(IsMoving, false);
        }
        else
        {
            controller.Move(move * (walkingSpeed * _dashingPower * Time.deltaTime)); 

        }
        
        //Jumping & Gravity
        if (Input.GetButtonDown("Jump") && _isGrounded)
        {
            _velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            anim.SetTrigger(Jumping);
        }
        _velocity.y += gravity * Time.deltaTime;
        controller.Move(_velocity * Time.deltaTime);
    }
    
    private void HandleShot()
    {
        // muzzleFlash.SetActive(false);
        
        if (Input.GetMouseButtonDown(0) && activeGun.fireCounter<=0)
        {
            if (Physics.Raycast(playerBody.position, playerBody.forward, out var hit))
            {
                if (Vector3.Distance(playerBody.position, hit.point) > 2f)
                {
                    firePoint.LookAt(hit.point);
                }
            }
            else
            {
                firePoint.LookAt(playerBody.position + (playerBody.forward * 30f));
            }

            if (hit.collider)
            {
                var rigid = hit.collider.transform.gameObject.GetComponent<Rigidbody>();

                if (rigid)
                {
                    FireShot(true, rigid, transform.forward);
                }
                else
                {
                    FireShot(false);
                }
            }
            else
            {
                FireShot(false);
            }
        }
        
        //Start Reload
        if (Input.GetKeyDown(KeyCode.R))
        {
            anim.SetBool(AimReloading, true);
            anim.SetBool(IsMoving, false);
            StartCoroutine(Reloaded(2));
        }
        
        //Switch Gun
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            SwitchGun();
        }

        //Aiming
        if (Input.GetMouseButtonDown(1))
        {
            MouseScript.instance.ZoomIn(activeGun.zoomAmount);
            anim.SetBool(IsAiming, true);
        }

        // if (Input.GetMouseButton(1))
        // {
        //     gunHolder.position = Vector3.MoveTowards(gunHolder.position, adsPoint.position, adsSpeed * Time.deltaTime);
        // }
        // else
        // {
        //     gunHolder.localPosition = Vector3.MoveTowards(gunHolder.localPosition, _gunStartPosition, adsSpeed * Time.deltaTime);
        // }
        
        if (Input.GetMouseButtonUp(1))
        {
            MouseScript.instance.ZoomOut();
            anim.SetBool(IsAiming, false);
        }
    }
    
    private void FireShot(bool rb, Rigidbody rigid=default,Vector3 rbp=default)
    {

        if (activeGun.currentAmo > 0 && !anim.GetBool(AimReloading))
        {
            if(rb){                    
                _grabbedObj = null;
                if (!(rigid is null))
                {
                    rigid.AddForce(rbp * 2000);
                    rigid.AddForce(0, 500, 0);
                    rigid.AddTorque(rbp * 200);
                }
            }
            // Instantiate(activeGun.bullet, firePoint.position, firePoint.rotation);

            if (anim.GetBool(IsAiming))
            {
                anim.SetBool(IsAimShooting, true);
            }
            else
            {
                anim.SetBool(IsAidleShooting, true);  
            }

            StartCoroutine(ShotFinished(0.2f));  

            // muzzleFlash.SetActive(true);
            
            rifleShot.Play();
            
            activeGun.fireCounter = activeGun.fireRate;
            activeGun.currentAmo--;
            UIController.instance.ammoText.text = activeGun.currentAmo + " Bullets";
        }
    }
    
    IEnumerator Reloaded(float time)
    {
        yield return new WaitForSeconds(time);
        
        activeGun.currentAmo += 5;
        UIController.instance.ammoText.text = activeGun.currentAmo + " Bullets";
        anim.SetBool(AimReloading, false);
        
    }
    
    IEnumerator ShotFinished(float time)
    {
        yield return new WaitForSeconds(time);
        anim.SetBool(IsAidleShooting, false); 
        anim.SetBool(IsAimShooting, false);
    }

    private void SwitchGun()
    {
        activeGun.gameObject.SetActive(false);

        currentGun++;
        if (currentGun >= allGuns.Count)
        {
            currentGun = 0;
        }
        activeGun = allGuns[currentGun];
        activeGun.gameObject.SetActive(true);
        firePoint.position = activeGun.firepoint.position;
        UIController.instance.ammoText.text = activeGun.currentAmo + " Bullets";
    }

    public void AddGun(string gunToAdd)
    {
        bool gunUnlocked = false;
        if (unlockableGun.Count > 0)
        {
            for (int i = 0; i < unlockableGun.Count; i++) 
            {
                if (unlockableGun[i].gunName == gunToAdd)
                {
                    gunUnlocked = true;
                    allGuns.Add(unlockableGun[i]);
                    unlockableGun.RemoveAt(i);

                    i = unlockableGun.Count;
                }
            }
        }

        if (gunUnlocked)
        {
            currentGun = allGuns.Count - 2;
            SwitchGun();
        }
    }
    private void Grab()
    {
        if (Input.GetKeyDown(KeyCode.F) && Physics.Raycast(playerBody.position, playerBody.forward , out var hit) && hit.transform.GetComponent<Rigidbody>() && !_isGrabbing)
        {
            _grabbedObj = hit.transform.gameObject;
            _isGrabbing = true;
        }
        else if (Input.GetKeyDown(KeyCode.F) && _isGrabbing)
        {
            _isGrabbing = false;
            _grabbedObj = null;
        }

        if (!_grabbedObj) return;
        if (!(_grabbedObj is null))
            _grabbedObj.GetComponent<Rigidbody>().velocity =
                10 * (grabPosition.position - _grabbedObj.transform.position);
    }

    private void HandleHookShotStart()
    {
        if (Input.GetKeyDown(KeyCode.E) && !_isSprinting)
        {
            DisableGravity();

            if (Physics.Raycast(playerBody.transform.position, playerBody.transform.forward, out RaycastHit raycastHit))
            {
                grabHolder.position = raycastHit.point - new Vector3(0,2,0);
                hookShotPosition = raycastHit.point;
                state = State.HookshotFlyingPlayer;
            }
        }
    }

    private void HandleHookshotMovement()
    {
        var position = transform.position;
        float hookShotSpeedMin = 20f;
        float hookShotSpeed = Mathf.Clamp(Vector3.Distance(position,hookShotPosition), hookShotSpeedMin, hookShotSpeedMax);
        float hookshotSpeedMultiplier = 2f;
        
        Vector3 hookShotDir = (hookShotPosition - position).normalized;
        controller.Move(hookShotDir * (hookShotSpeed * hookshotSpeedMultiplier * Time.deltaTime));
        
        if (Vector3.Distance(position, hookShotPosition) > 3f)
        {
            speedParticles.SetActive(true);
        }
        else
        {
            speedParticles.SetActive(false);  
        }

        
        if(Input.GetKeyUp(KeyCode.E))
        {
            state = State.Normal;
            EnableGravity();
            speedParticles.SetActive(false); 
        }
    }

    private void DisableGravity()
    {
        gravity = 0;
        _velocity.y = 0;
    }
    private void EnableGravity()
    {
        gravity = _savedGravity;
    }
}
