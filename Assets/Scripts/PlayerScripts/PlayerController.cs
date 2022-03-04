using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;

public class PlayerController : MonoBehaviourPunCallbacks, IDamageable, IPlayerSubject
{
    [SerializeField] private float mouseSensitivity, sprintSpeed, walkSpeed, jumpForce, smoothTime;
    [SerializeField] GameObject cameraHolder;
    [SerializeField] private Material RedMat;
    [SerializeField] private Material BlueMat;
    [SerializeField] private TMP_Text blueScore;
    [SerializeField] private TMP_Text redScore;
    [SerializeField] private TMP_Text timer;

    [HideInInspector] public int team;
    private PlayerManager playerManager;

    private float verticalLookRotation;
    private bool grounded;
    private bool isMoving;
    private Vector3 smoothMoveVelocity;
    private Vector3 moveAmount;
    private int bullets;
    private PhotonView view;
    [SerializeField] Gun gun;
    [SerializeField] Knife knife;
    private Rigidbody rb;
    [SerializeField] GameObject gunView;
    public Text healthView;
    public Text bulletsView;

    private Dictionary<string, List<IObserver>> observers = new Dictionary<string, List<IObserver>>();


    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        view = GetComponent<PhotonView>();

    }

    void Start()
    {
        if (view.IsMine)
        {
            playerManager = PhotonView.Find((int)view.InstantiationData[0]).GetComponent<PlayerManager>();
        }


        if (!view.IsMine)
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
            Destroy(rb);
        }
        Cursor.lockState = CursorLockMode.Locked;
        isMoving = false;
        //health = 100;
        bullets = 5;
    }

    void Update()
    {
        if (view.IsMine)
        {
            LockAndUnlockCursor();

            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Look();
            }

            Shoot();
            UseKnife();
            Move();
            Jump();
            float mins = Timer.Instance.GetTimerMinutes();
            float secs = Timer.Instance.GetTimerSeconds();
            if(secs < 10)
            {
                timer.text = mins.ToString() + ":0" + secs.ToString();
            } else
            {
                timer.text = mins.ToString() + ":" + secs.ToString();
            }
            redScore.text = RoomManager.Instance.scoreRed.ToString();
            blueScore.text = RoomManager.Instance.scoreBlue.ToString();
        }
         
    }

    private void FixedUpdate()
    {
        if (view.IsMine)
        {
            rb.MovePosition(rb.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
        }
    }

    void Look()
    {
        transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * mouseSensitivity);

        verticalLookRotation += Input.GetAxisRaw("Mouse Y") * mouseSensitivity;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90f);

        cameraHolder.transform.localEulerAngles = Vector3.left * verticalLookRotation;

    }
    void LockAndUnlockCursor()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }

    void Move()
    {
        Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

        if (moveDir.x != 0f || moveDir.z != 0f)
            isMoving = true;
        else
            isMoving = false;

        if (isMoving)
        {
            if (!FindObjectOfType<AudioManager>().isPlaying("ConcreteFootsteps") && grounded)
            {
                FindObjectOfType<AudioManager>().Play("ConcreteFootsteps");
                PlayStopSound("ConcreteFootsteps", "play");

            }
        }
        /*else
        {
            FindObjectOfType<AudioManager>().Stop("ConcreteFootsteps");
            //PlayStopSound("ConcreteFootsteps", "stop");
        }*/
            

        moveAmount = Vector3.SmoothDamp(moveAmount, moveDir * (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed), ref smoothMoveVelocity, smoothTime);
    }

    void Jump()
    {
        if (Input.GetKey(KeyCode.Space) && grounded)
        {
            rb.AddForce(transform.up * jumpForce);
        }
    }
    
    void Shoot()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (bullets > 0)
            {
                FindObjectOfType<AudioManager>().Play("Gunshot");
                PlayStopSound("Gunshot", "play");

                bullets -= 1;
                bulletsView.text = bullets.ToString();
                gun.Shoot();
            }
            else
            {
                FindObjectOfType<AudioManager>().Play("DryFire");
                PlayStopSound("DryFire", "play");     
            }
        }
    }

    void UseKnife()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            FindObjectOfType<AudioManager>().Play("stab");
            PlayStopSound("stab", "play");
            knife.UseKnife();

        }
    }

    public void SetGroundedState(bool _grounded)
    {
        grounded = _grounded;
    }

    public void SetTeamAndUpdateMaterials(int tm)
    {
        team = tm;
        view.RPC("RPC_ChangeTexture", RpcTarget.AllBuffered, tm);
    }

    [PunRPC]
    void RPC_ChangeTexture(int tm)
    {
        team = tm;
        if (team == 0)
        {
            GetComponent<Renderer>().material = BlueMat;
        }
        else
        {
            GetComponent<Renderer>().material = RedMat;
        }
    }

    public void TakeDamage(int damage)
    {
        /*if (!view.IsMine)
            return;*/

        Debug.Log("Take Damage RPC called");
        //view.RPC("RPC_TakeDamage", RpcTarget.MasterClient, damage);
        view.RPC("RPC_TakeDamage", RpcTarget.All, damage);
    }

    [PunRPC]
    void RPC_TakeDamage(int damage)
    {
        //Debug.Log("Take Damage RPC called");
        /*if (!view.IsMine)
            return;*/

        /*health -= damage;
        healthView.text = health.ToString();*/


       /* if (health <= 0)
        {
            Die();

        }*/

        foreach (string subscriberType in observers.Keys)
        {
            if (subscriberType.Equals("IDamageObserver"))
            {
                foreach (IObserver subscriber in observers[subscriberType])
                {
                    (subscriber as IDamageObserver).Notify(damage);
                }
            }
        }
    }
    

    /*public void increaseHealth()
    {
        healthPickUp.pickupTrigger(pickup);
    }*/

    public void PlayStopSound(string sound, string action)
    {
        if (action == "stop")
        {
            view.RPC("RPC_StopSound", RpcTarget.Others, sound);
        }    
        else
        if (action == "play")
        {
            view.RPC("RPC_PlaySound", RpcTarget.Others, sound);
        }    
    }

    [PunRPC]
    void RPC_PlaySound(string sound)
    {
        FindObjectOfType<AudioManager>().Play(sound);
    }

    [PunRPC]
    void RPC_StopSound(string sound)
    {
        FindObjectOfType<AudioManager>().Stop(sound);
    }

    public void Die()
    {
        playerManager.Die();
    }

    public void Reload()
    {
        if (bullets < 5)
        {
            bullets = 5;
            bulletsView.text = bullets.ToString();
            FindObjectOfType<AudioManager>().Play("Reload");
        }
    }

    public void addObserver<T>(IObserver observer)
    {
        //string check = typeof(T).Name;
        //observers.Add(typeof(T).Name, new List<IObserver>());
        //observers[typeof(T).Name].Add(observer);
        /*foreach (string subscriberType in observers.Keys)
        {
            
            *//*if (subscriberType == T as string)
            {

            }*//*
            if (typeof(T).Equals(subscriberType))
            {
                observers[subscriberType].Add(observer);
            }
        }*/

        

        //if the key element exists in observers.keys
        foreach (string observerType in observers.Keys)
        {
            if (typeof(T).Name == observerType)
            {
                observers[typeof(T).Name].Add(observer);
                return;
            }
        }

        observers.Add(typeof(T).Name, new List<IObserver>());
        observers[typeof(T).Name].Add(observer);
    }

    public void unsubscribeObserver<T>(IObserver observer)
    {
        foreach (string subscriberType in observers.Keys)
        {
            /*if (subscriberType == T as string)
            {

            }*/
            if (typeof(T).Equals(subscriberType))
            {
                observers[subscriberType].Remove(observer);
            }
        }
    }
}
