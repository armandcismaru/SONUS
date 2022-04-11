using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class PlayerController : MonoBehaviourPunCallbacks, IDamageable, IPlayerSubject
{
    public readonly string FOOTSTEP_SOUND = "ConcreteFootsteps";
    public readonly string GUN_SOUND = "Gunshot";
    public readonly string JUMP_SOUND = "Jump";
    public readonly string STAB_SOUND = "stab";
    public readonly string DRYFIRE_SOUND = "DryFire";
    public readonly string RELOAD_SOUND = "Reload";
    public readonly string GETSHOT_SOUND = "GetShot";

    [SerializeField] private float walkSpeed, jumpHeight, smoothTime, gravity;
    [SerializeField] private CharacterController controller;
    [SerializeField] private Material RedMat;
    [SerializeField] private Material BlueMat;
    private TMP_Text blueScore;
    private TMP_Text redScore;
    private TMP_Text timer;
    private TMP_Text Team;

    [SerializeField] private GameObject blueScorePrefab;
    [SerializeField] private GameObject redScorePrefab;
    [SerializeField] private GameObject timerPrefab;
    [SerializeField] private GameObject scorelinePrefab;
    [SerializeField] private GameObject TeamPrefab;

    [HideInInspector] public int team;
    private PlayerManager playerManager;
    public PhotonView view;

    public bool grounded;
    private bool isMoving;
    bool hasJumped = false;

    private Vector3 smoothMoveVelocity;
    private Vector3 moveAmount;
    private Vector3 velocity;

    [SerializeField] private int bullets = 5;
    [SerializeField] Gun gun;
    [SerializeField] Knife knife;

    private Rigidbody rb;
    public Text bulletsView;
    private float LastShootTime;

    [SerializeField]
    private float ShootDelay = 0.5f;
    [SerializeField]
    private GameObject bulletsViewPrefab;

    private Dictionary<string, List<IObserver>> observers = new Dictionary<string, List<IObserver>>();

    private UIScriptPlayer uiComponent;

    public int index = -1;
    [SerializeField] private GameObject pauseObject;
    float time;
    float remainingTime;
    public bool invisibility;
    float timeSpeed;
    float remainingTimeSpeed;
    public bool fastSpeed;
    private float initialSpeed;
    public GameObject decoy;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        view = GetComponent<PhotonView>();
        index = (int)view.InstantiationData[1];
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

        invisibility = false;
        time = Time.time;

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

        // pauseObject = GameObject.FindGameObjectsWithTag("Pause");
    }

    void Update()
    {
        if (view.IsMine)
        {
            PauseMenu();

            if (Input.GetKeyDown(KeyCode.M))
            {
                EmittingSpell();
            }
            if (fastSpeed)
            {
                UpdateFastSpeed();
            }

            if (invisibility)
            {
                UpdateInvisibilitySpell();
            }
            if (grounded == false && hasJumped == false)
            {
                hasJumped = true;
            }
            grounded = controller.isGrounded;

            if (grounded == true && hasJumped == true)
            {
                GetComponent<AudioManager>().Play(JUMP_SOUND);
                hasJumped = false;
                BroadcastSound(JUMP_SOUND);
            }

            if (!Pause.paused) {
                Shoot();
                UseKnife();
                Move();
                Jump();

                FadeBloodDamage();
                SelfHit();
            }

            float mins = Timer.Instance.GetTimerMinutes();
            float secs = Timer.Instance.GetTimerSeconds();

            if(secs < 10)
            {
                timer.text = mins.ToString() + ":0" + secs.ToString();
            }
            else
            {
                timer.text = mins.ToString() + ":" + secs.ToString();
            }

            redScore.text = RoomManager.Instance.scoreRed.ToString();
            blueScore.text = RoomManager.Instance.scoreBlue.ToString();
        }
    }

    public void StartInvisibilitySpell()
    {
        time = Time.time;
        invisibility = true;
        GetComponent<Renderer>().enabled = false;
        gun.GetComponent<Renderer>().enabled = false;
        view.RPC("RPC_MakeInvisible", RpcTarget.All, index);
        Debug.Log("da");
    }

    public void DeployDecoy()
    {
        // Instantiate(decoy, transform.position, transform.rotation);
        // Vector3 decoy_position = GetComponentInChildren<Camera>().gameObject.transform.position;
        // Quaternion decoy_rotation = GetComponentInChildren<Camera>().gameObject.transform.rotation;
        // view.RPC("RPC_DeployDecoy", RpcTarget.All, camera_position + transform.forward, camera_rotation, playerManager.team);
        // view.RPC("RPC_DeployDecoy", RpcTarget.All, transform.position + transform.forward, Quaternion.identity, playerManager.team);
        decoy = PhotonNetwork.Instantiate("Decoy", transform.position + transform.forward, transform.rotation);
        decoy.GetComponent<Decoy>().direction = transform.forward;
    }

    [PunRPC]
    void RPC_DeployDecoy(Vector3 position, Quaternion rotation, int team) {
        Instantiate(decoy, position, rotation);
    }

    void UpdateInvisibilitySpell()
    {
        remainingTime = time + 5f - Time.time;

        if(remainingTime <= 0)
        {
            this.gameObject.GetComponent<Renderer>().enabled = true;
            gun.GetComponent<Renderer>().enabled = true;
            invisibility = false;
            view.RPC("RPC_StopInvisible", RpcTarget.All, index);
        }
    }

    void UpdateFastSpeed()
    {
        remainingTimeSpeed = timeSpeed + 5f - Time.time;

        if(remainingTimeSpeed <= 0)
        {
            fastSpeed = false;
            walkSpeed = initialSpeed;
        }
    }

    void StartFastSpeed()
    {
        timeSpeed = Time.time;
        fastSpeed = true;
        initialSpeed = walkSpeed;
        walkSpeed *= 2;
    }

    void EmittingSpell()
    {
        float minDistance = float.MaxValue;
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        GameObject closestPlayer = null;
        for(int i = 0; i < players.Length; i++)
        {
            int playerTeam = players[i].GetComponent<PlayerController>().team;
            if (Mathf.Abs(team - playerTeam) == 1)
            {
                float distance = (players[i].transform.position - transform.position).sqrMagnitude;
                if (distance < minDistance)
                {
                    closestPlayer = players[i];
                    minDistance = distance;
                }
            }
        }
        int closestIndex = closestPlayer.GetComponent<PlayerController>().index;
        closestPlayer.GetComponent<PlayerController>().EmitSound();
    }

    public void EmitSound()
    {
        GetComponent<AudioManager>().Play(GETSHOT_SOUND);
        BroadcastSound(GETSHOT_SOUND);
    }
    private void FixedUpdate()
    {
        if (view.IsMine)
        {
            if (grounded && Pause.paused) {
                return;
            }
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

    void PauseMenu()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
           pauseObject.GetComponent<Pause>().TogglePause();
        }
    }

    void Move()
    {
        Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"),
                                      0,
                                      Input.GetAxisRaw("Vertical")).normalized;

        if (moveDir.x != 0f || moveDir.z != 0f)
            isMoving = true;
        else
            isMoving = false;

        if (isMoving && controller.isGrounded && !Pause.paused)
        {
            if (!GetComponent<AudioManager>().isPlaying(FOOTSTEP_SOUND) && grounded)
            {
                GetComponent<AudioManager>().Play(FOOTSTEP_SOUND);
                BroadcastSound(FOOTSTEP_SOUND);
            }
        }
        else
        {
            GetComponent<AudioManager>().Stop(FOOTSTEP_SOUND);
            BroadcastSoundS(FOOTSTEP_SOUND);
        }

        moveAmount = Vector3.SmoothDamp(moveAmount,
                                        moveDir * walkSpeed,
                                        ref smoothMoveVelocity,
                                        smoothTime); //TODO
    }

    private void FadeBloodDamage()
    {
        GameObject bloodSplatter = GameObject.FindWithTag("Blood");

        if (bloodSplatter != null)
        {
            if (bloodSplatter.GetComponent<Image>().color.a > 0)
            {
                var color = bloodSplatter.GetComponent<Image>().color;
                color.a -= 0.01f;

                bloodSplatter.GetComponent<Image>().color = color;
            }
        }
    }

    // ---- TESTING PURPOSES ----
    void SelfHit()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            TakeDamage(50);
        }
    }

    public void GotHurt()
    {
        GetComponent<AudioManager>().Play(GETSHOT_SOUND);
        GameObject bloodSplatter = GameObject.FindWithTag("Blood");
        var color = bloodSplatter.GetComponent<Image>().color;
        color.a = 0.8f;
        bloodSplatter.GetComponent<Image>().color = color;
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
        if (Input.GetKeyDown(KeyCode.Mouse0) && LastShootTime + ShootDelay < Time.time)
        {
            if (bullets > 0)
            {
                GetComponent<AudioManager>().Play(GUN_SOUND);
                BroadcastSound(GUN_SOUND);

                bullets -= 1;
                bulletsView.text = bullets.ToString();
                gun.Shoot();
                LastShootTime = Time.time;
            }
            else
            {
                GetComponent<AudioManager>().Play(DRYFIRE_SOUND);
                BroadcastSound(DRYFIRE_SOUND);
            }
        }
    }

    void UseKnife()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            GetComponent<AudioManager>().Play(STAB_SOUND);
            BroadcastSound(STAB_SOUND);
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
        //GotHurt();
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

    public void Die()
    {
        view.RPC("RPC_Die", RpcTarget.All);
    }

    [PunRPC]
    void RPC_Die()
    {
        foreach (string subscriberType in observers.Keys)
        {
            if (subscriberType.Equals("IDieObserver"))
            {
                foreach (IObserver subscriber in observers[subscriberType])
                {
                    (subscriber as IDieObserver).Notify();
                }
            }
        }
    }

    [PunRPC]
    void RPC_MakeInvisible(int ind)
    {
        if (index == ind)
        {
            GetComponent<Renderer>().enabled = false;
            // gun.GetComponent<Renderer>().enabled = false;
        }
    }

    [PunRPC]
    void RPC_StopInvisible(int ind)
    {
        if (index == ind)
        {
            GetComponent<Renderer>().enabled = true;
            // gun.GetComponent<Renderer>().enabled = true;
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
            FindObjectOfType<AudioManager>().Play(RELOAD_SOUND);
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
