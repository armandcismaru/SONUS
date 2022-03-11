using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class PlayerController : MonoBehaviourPunCallbacks, IDamageable, IPlayerSubject
{
    [SerializeField] private float walkSpeed, jumpHeight, smoothTime, gravity;
    [SerializeField] private CharacterController controller;
    [SerializeField] private Material RedMat;
    [SerializeField] private Material BlueMat;
    [SerializeField] private TMP_Text blueScore;
    [SerializeField] private TMP_Text redScore;
    [SerializeField] private TMP_Text timer;

    [HideInInspector] public int team;
    private PlayerManager playerManager;

    public bool grounded;
    private bool isMoving;

    private Vector3 smoothMoveVelocity;
    private Vector3 moveAmount;
    private Vector3 velocity;
    private int bullets;
    private PhotonView view;
    [SerializeField] Gun gun;
    [SerializeField] Knife knife;
    private Rigidbody rb;
    public Text healthView;
    public Text bulletsView;

    public bool canvasOn = true;

    private Dictionary<string, List<IObserver>> observers = new Dictionary<string, List<IObserver>>();

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
            team = playerManager.team;
        }

        if (!view.IsMine)
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
            Destroy(rb);
        }

        Cursor.lockState = CursorLockMode.Locked;
        isMoving = false;
        bullets = 5;
    }

    void Update()
    {
        if (view.IsMine)
        {
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
            if (grounded && velocity.y < 0)
            {
                velocity.y = -1f;
            }
            velocity.y += gravity * Time.fixedDeltaTime;
            controller.Move(transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
            controller.Move(velocity * Time.fixedDeltaTime);
            //rb.AddForce(velocity * Time.fixedDeltaTime);
        }
    }

    private float DampenedMovement(float value)
    {
        if (Mathf.Abs(value) > 1f)
        {
            return Mathf.Lerp(value, Mathf.Sign(value), 0.25f);
        }
        return value;
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
            if (!GetComponent<AudioManager>().isPlaying("ConcreteFootsteps") && grounded)
            {
                GetComponent<AudioManager>().Play("ConcreteFootsteps");
                BroadcastSound("ConcreteFootsteps");
            }
        }
        else
        {
            GetComponent<AudioManager>().Stop("ConcreteFootsteps");
            BroadcastSoundS("ConcreteFootsteps");
        }

        moveAmount = Vector3.SmoothDamp(moveAmount, moveDir *  walkSpeed, ref smoothMoveVelocity, smoothTime);
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            Debug.Log("miosugi");
            velocity.y += Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    void Shoot()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (bullets > 0)
            {
                GetComponent<AudioManager>().Play("Gunshot");
                BroadcastSound("Gunshot");

                bullets -= 1;
                bulletsView.text = bullets.ToString();
                gun.Shoot();
            }
            else
            {
                GetComponent<AudioManager>().Play("DryFire");
                BroadcastSound("DryFire");
            }
        }
    }

    void UseKnife()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            GetComponent<AudioManager>().Play("stab");
            BroadcastSound("stab");
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
        view.RPC("RPC_TakeDamage", RpcTarget.All, damage);
    }

    [PunRPC]
    void RPC_TakeDamage(int damage)
    {
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

    public void BroadcastSound(string sound)
    {
        view.RPC("RPC_BroadcastSound", RpcTarget.Others, sound);
    }

    public void BroadcastSoundS(string sound)
    {
        view.RPC("RPC_BroadcastSoundS", RpcTarget.Others, sound);
    }

    public void PlayRemote(string sound)
    {
        GetComponent<AudioManager>().Play(sound);
    }

    public void StopRemote(string sound)
    {
        GetComponent<AudioManager>().Stop(sound);
    }

    [PunRPC]
    void RPC_BroadcastSound(string sound)
    {
        PlayRemote(sound);
    }

    [PunRPC]
    void RPC_BroadcastSoundS(string sound)
    {
        StopRemote(sound);
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
        // If the key element exists in observers.keys
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
            if (typeof(T).Equals(subscriberType))
            {
                observers[subscriberType].Remove(observer);
            }
        }
    }
}
