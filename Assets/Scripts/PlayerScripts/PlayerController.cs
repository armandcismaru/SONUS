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
    private TMP_Text blueScore;
    private TMP_Text redScore;
    private TMP_Text timer;

    [SerializeField] private GameObject blueScorePrefab;
    [SerializeField] private GameObject redScorePrefab;
    [SerializeField] private GameObject timerPrefab;
    [SerializeField] private GameObject scorelinePrefab;

    [HideInInspector] public int team;
    private PlayerManager playerManager;

    private TMP_Text Team;
    [SerializeField] private GameObject TeamPrefab;

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
    public Text bulletsView;

    [SerializeField] private GameObject bulletsViewPrefab; 
    bool hasJumped = false;

    private Dictionary<string, List<IObserver>> observers = new Dictionary<string, List<IObserver>>();

    private UIScriptPlayer uiComponent;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        view = GetComponent<PhotonView>();
    }

    void Start()
    {
        //If the canvas exists, it asks form the uiComponent (if the UIScriptPlayer) acctually exists!
        uiComponent = this.gameObject.GetComponentInParent<UIScriptPlayer>();
        if (uiComponent == null) throw new MissingComponentException("UI Script missing from parent");

        GameObject uiComponentBlueScore = uiComponent.AttachUI(blueScorePrefab, blueScorePrefab.transform.localPosition,
                                                               blueScorePrefab.transform.rotation, blueScorePrefab.transform.localScale);
        blueScore = uiComponentBlueScore.GetComponent<TMP_Text>();

        GameObject uiComponentRedScore = uiComponent.AttachUI(redScorePrefab, redScorePrefab.transform.localPosition,
                                                              redScorePrefab.transform.rotation, redScorePrefab.transform.localScale);
        redScore = uiComponentRedScore.GetComponent<TMP_Text>();

        GameObject uiComponentTimer = uiComponent.AttachUI(timerPrefab, timerPrefab.transform.localPosition,
                                                           timerPrefab.transform.rotation, timerPrefab.transform.localScale);
        timer = uiComponentTimer.GetComponent<TMP_Text>();

        GameObject uiComponentLine = uiComponent.AttachUI(scorelinePrefab, scorelinePrefab.transform.localPosition,
                                                           scorelinePrefab.transform.rotation, scorelinePrefab.transform.localScale);

        GameObject uiComponentBullets = uiComponent.AttachUI(bulletsViewPrefab, bulletsViewPrefab.transform.localPosition,
                                                           bulletsViewPrefab.transform.rotation, bulletsViewPrefab.transform.localScale);
        bulletsView = uiComponentBullets.GetComponent<Text>();

        GameObject uiComponentTeam = uiComponent.AttachUI(TeamPrefab, TeamPrefab.transform.localPosition,
                                                           TeamPrefab.transform.rotation, TeamPrefab.transform.localScale);
        Team = uiComponentTeam.GetComponent<TMP_Text>();

        if (view.IsMine)
        {
            playerManager = PhotonView.Find((int)view.InstantiationData[0]).GetComponent<PlayerManager>();
            team = playerManager.team;
            if (team == 0)
            {
                Team.text = "Defenders";
                Team.color = Color.blue;
            }
            else
            {
                Team.text = "Attackers";
                Team.color = Color.red;
            }
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
            grounded = controller.isGrounded;
            if (grounded == true && hasJumped == true)
            {
                hasJumped = false;
                GetComponent<AudioManager>().Play("Jump");
                BroadcastSound("Jump");
            }
            if (grounded == false && hasJumped == false)
            {
                hasJumped = true;
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
            if (grounded && velocity.y < 0)
            {
                velocity.y = -1f;
            }
            velocity.y += gravity * Time.fixedDeltaTime;
            controller.Move(transform.TransformDirection(moveAmount) * Time.fixedDeltaTime);
            controller.Move(velocity * Time.fixedDeltaTime);
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

        if (isMoving && controller.isGrounded)
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
