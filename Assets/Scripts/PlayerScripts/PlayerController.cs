using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class PlayerController : MonoBehaviourPunCallbacks, IDamageable, IPlayerSubject
{
    public string FOOTSTEP_SOUND = "DirtWalk";
    public readonly string GUN_SOUND = "Gunshot";
    public readonly string JUMP_SOUND = "DirtJump";
    public readonly string KNIFE_SOUND = "Knife";
    public readonly string DRYFIRE_SOUND = "DryFire";
    public readonly string RELOAD_SOUND = "Reload";
    public readonly string[] INJURED_SOUNDS = {"InjuredOne", "InjuredTwo", "InjuredThree", "InjuredFour"};
    public readonly string[] DEATH_SOUNDS = {"DeathOne", "DeathTwo", "DeathThree", "DeathFour"};
    public readonly string EMITTER_SOUND = "Emitter";
    public readonly string PICKUP_SUPPLY_SOUND = "PickUpSupply";
    public readonly string KNIFE_KILLING_SOUND = "KnifeKilling";
    public readonly string SPELL_TRANSFORM_SOUND = "SpellTransform";
    public readonly string OPEN_TORCH_SOUND = "OpenTorch";
    public readonly string CLOSE_TORCH_SOUND = "CloseTorch";


    [SerializeField] private float walkSpeed, slowSpeed, jumpHeight, smoothTime, gravity;
    [SerializeField] private CharacterController controller;
    [SerializeField] private Material RedMat;
    [SerializeField] private Material BlueMat;
    private TMP_Text blueScore;
    private TMP_Text redScore;
    private TMP_Text timer;
    private TMP_Text Team;
    public TMP_Text timerSpell;
    private TMP_Text spellsText;

    [SerializeField] private GameObject blueScorePrefab;
    [SerializeField] private GameObject redScorePrefab;
    [SerializeField] private GameObject timerPrefab;
    [SerializeField] private GameObject TeamPrefab;
    [SerializeField] private GameObject TimerSpellPrefab;
    [SerializeField] private GameObject HorizontalLayout;
    [SerializeField] private GameObject SpellNamePrefab;
    [SerializeField] private GameObject BulletIcon;

    [HideInInspector] public int team;
    private PlayerManager playerManager;
    public PhotonView view;

    public bool grounded;
    private bool isMoving;
    bool hasJumped = false;

    private Vector3 smoothMoveVelocity;
    private Vector3 moveAmount;
    private Vector3 velocity;

    [SerializeField] public int bullets = 5;
    [SerializeField] public int max_bullets = 8;
    [SerializeField] Gun gun;
    [SerializeField] Knife knife;

    private Rigidbody rb;
    private float LastShootTime;

    [SerializeField]
    private float ShootDelay = 0.5f;
    [SerializeField]
    private GameObject bulletsViewPrefab;

    //Dictionary of observers
    private Dictionary<string, List<IObserver>> observers = new Dictionary<string, List<IObserver>>();

    private UIScriptPlayer uiComponent;

    public int index = -1;
    [SerializeField] private GameObject pauseObject;
    public float time;
    public float remainingTime;
    public bool invisibility;
    public float timeSpeed;
    public float remainingTimeSpeed;
    public bool fastSpeed;
    private float initialSpeed;
    public GameObject decoy;
    private bool isShiftPressed = false;

    public GameObject parent;

    public Camera mainCamera;
    [SerializeField] private GameObject playerIcon;
    [SerializeField] private Camera minimapCamera;

    [SerializeField] private GameObject AttackerModel;
    [SerializeField] private GameObject DefenderModel;

    public GameObject SpectateCanv;
    [SerializeField] private TMP_Text nickname;
    [SerializeField] private GameObject displayName;
    [SerializeField] private TMP_Text sceneNickname;
    private GameObject[] bulletsArray;
    private GameObject uiComponentBullets;
    private GameObject emptyGunIcon;
    [SerializeField] private GameObject crosshair;
    [SerializeField] private Animator animator;
    [SerializeField] private Animator animatorAtt;
    [SerializeField] private GameObject knifeModel;
    [SerializeField] private GameObject knifeAtt;

    private int runHash;
    private int walkHash;
    private int jumpTrigHash;
    private int landHash;
    private int meleeHash;
    private float meleeCd = 0;


    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        view = GetComponent<PhotonView>();
        index = (int)view.InstantiationData[1];

        runHash = Animator.StringToHash("isRunning");
        walkHash = Animator.StringToHash("isWalking");
        jumpTrigHash = Animator.StringToHash("jump");
        landHash = Animator.StringToHash("isTouchingGround");
        meleeHash = Animator.StringToHash("melee");
        initialSpeed = walkSpeed;
    }
    void Start()
    {
        /* Attaching UI elements (Prefabs) at run time */
        bulletsArray = new GameObject[max_bullets];

        //If the canvas exists, it asks form the uiComponent (if the UIScriptPlayer) acctually exists!
        uiComponent = this.gameObject.GetComponentInParent<UIScriptPlayer>();
        if (uiComponent == null) throw new MissingComponentException("UI Script missing from parent");

        GameObject uiComponentRedScore = uiComponent.AttachUI(redScorePrefab, HorizontalLayout, false);
        redScore = uiComponentRedScore.transform.GetChild(0).GetComponentInChildren<TMP_Text>();

        GameObject uiComponentTimer = uiComponent.AttachUI(timerPrefab, HorizontalLayout, false);
        timer = uiComponentTimer.transform.GetChild(0).GetComponentInChildren<TMP_Text>();

        GameObject uiComponentBlueScore = uiComponent.AttachUI(blueScorePrefab, HorizontalLayout, false);
        blueScore = uiComponentBlueScore.transform.GetChild(0).GetComponentInChildren<TMP_Text>();   

        uiComponentBullets = uiComponent.AttachUI(bulletsViewPrefab, parent, true);
        emptyGunIcon = GameObject.FindWithTag("EmptyGun");
        emptyGunIcon.SetActive(false);
        for (int i = 0; i < max_bullets; i++)
        {
            bulletsArray[i] = uiComponent.AttachUI(BulletIcon, uiComponentBullets.transform.GetChild(0).gameObject, false);
            if (i < max_bullets - bullets)
                bulletsArray[i].SetActive(false);
            else
                bulletsArray[i].SetActive(true);
        }

        GameObject uiComponentTeam = uiComponent.AttachUI(TeamPrefab, parent, true);
        Team = uiComponentTeam.GetComponent<TMP_Text>();

        GameObject uiComponentTimerSpell = uiComponent.AttachUI(TimerSpellPrefab, parent, true);
        timerSpell = uiComponentTimerSpell.GetComponent<TMP_Text>();

        GameObject uiComponentSpellNames = uiComponent.AttachUI(SpellNamePrefab, parent, true);
        spellsText = uiComponentSpellNames.GetComponent<TMP_Text>();

        invisibility = false;
        time = Time.time;

        if (view.IsMine)
        {
            var children = AttackerModel.GetComponentsInChildren<Transform>(includeInactive: true);
            foreach (var child in children)
            {
                child.gameObject.layer = 13;
            }
            children = DefenderModel.GetComponentsInChildren<Transform>(includeInactive: true);
            foreach (var child in children)
            {
                child.gameObject.layer = 13;
            }
            playerManager = PhotonView.Find((int)view.InstantiationData[0]).GetComponent<PlayerManager>();
            team = playerManager.team;
            if (team == 0)
            {
                Team.text = "Campers";
                Team.color = new Color(0.0745f, 0.1262f, 0.2941f, 1);
                spellsText.text = "Hold E to trigger spells\n'listen' - enemy sound feedback\n'torch' - activate torchlight";
            }
            else
            {
                Team.text = "Scavengers";
                Team.color = new Color(0.6431373f, 0.2039216f, 0.227451f, 1);
                spellsText.text = "Hold E to trigger spells\n'decoy' - launch decoy\n'torch' - activate torchlight";
            }
            displayName.SetActive(false);

            gameObject.layer = 2;

        }

        if (!view.IsMine)
        {
            //GetComponentInChildren(typeof(Canvas), true).gameObject.SetActive(false);
            mainCamera.gameObject.SetActive(false);
            GetComponentInChildren(typeof(Canvas), true).gameObject.SetActive(false);
            nickname.text = view.Owner.NickName;
            sceneNickname.text = view.Owner.NickName;

            //Destroy(GetComponentInChildren<Camera>().gameObject);
            //Destroy(mainCamera.gameObject);
            Destroy(minimapCamera.gameObject);
            Destroy(rb);
        }

        Cursor.lockState = CursorLockMode.Locked;
        isMoving = false;
        bullets = 5;

        // pauseObject = GameObject.FindGameObjectsWithTag("Pause");
    }
    public void SolveSpectateComponents()
    {
        SpectateCanv.SetActive(true);
    }
    void Update()
    {
        if (view.IsMine)
        {
            PauseMenu();

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
                animator.SetTrigger(landHash);

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
    // Logic for updating the HUD of the spell countdown
    public void UpdateTimerSpell(string newTimerSpell)
    {
         timerSpell.text = newTimerSpell;
    }

    // Logic for "Invisibility" Spell
    // Not curently in the game
    public void StartInvisibilitySpell()
    {
        time = Time.time;
        invisibility = true;
        GetComponent<Renderer>().enabled = false;
        gun.GetComponent<Renderer>().enabled = false;
        view.RPC("RPC_MakeInvisible", RpcTarget.All, index);
        Debug.Log("da");
    }

    // Logic for "Decoy" spell
    // It instatiate a decoy instance for all the players
    public void DeployDecoy()
    {
    decoy = PhotonNetwork.Instantiate("Decoy", transform.position + transform.forward, transform.rotation);
    decoy.GetComponent<Decoy>().direction = transform.forward;
    }
    // Netcode logic for "Decoy" spell
    [PunRPC]
    void RPC_DeployDecoy(Vector3 position, Quaternion rotation, int team) {
        Instantiate(decoy, position, rotation);
    }

    // Logic for updating the "Invisibility" spell
    // Not curently in the game
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
    // Logic for updating the "Fast Speed" spell
    // Not curently in the game
    void UpdateFastSpeed()
    {
        remainingTimeSpeed = timeSpeed + 5f - Time.time;

        if(remainingTimeSpeed <= 0)
        {
            FOOTSTEP_SOUND = "DirtWalk";
            fastSpeed = false;
            walkSpeed = initialSpeed;
        }
    }

    // Logic for "Fast Speed" spell
    // Not curently in the game
    public void StartFastSpeed()
    {
        if (!fastSpeed)
        {
            FOOTSTEP_SOUND = "DirtRun";
            timeSpeed = Time.time;
            fastSpeed = true;
            initialSpeed = walkSpeed;
            walkSpeed *= 1.5f;
        }
    }
    // Logic for "Listen" Spell
    // It looks for the closest player and it makes him trigger a sound
    public void EmittingSpell()
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
    // Sound effect triggered when a player uses "listen" spell
    public void EmitSound()
    {
        GetComponent<AudioManager>().Play(EMITTER_SOUND);
        BroadcastSound(EMITTER_SOUND);
    }

    // Sound effect for triggering a spell
    public void SpellTransformSound()
    {
        GetComponent<AudioManager>().Play(SPELL_TRANSFORM_SOUND);
        BroadcastSound(SPELL_TRANSFORM_SOUND);
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

            if(meleeCd >= 0)
            {
                meleeCd -= Time.deltaTime;
            }
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

    // Logic for the Pause Modal
    void PauseMenu()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (Pause.paused == false){
                crosshair.SetActive(false);
            }
            else
            {
                crosshair.SetActive(true);
            }
            pauseObject.GetComponent<Pause>().TogglePause();
        }
    }
    // Logic for Moving
    void Move()
    {
        Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"),
                                      0,
                                      Input.GetAxisRaw("Vertical")).normalized;

        if (moveDir.x != 0f || moveDir.z != 0f)
        {
            isMoving = true;
            animator.SetBool(runHash, true);
            animator.SetBool(walkHash, false);
        }
        else
        {
            animator.SetBool(runHash, false);
            isMoving = false;
        }

        if (isMoving && controller.isGrounded && !Pause.paused)
        {
            if (!GetComponent<AudioManager>().isPlaying(FOOTSTEP_SOUND) && grounded && !isShiftPressed)
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

        //shift walking
        if (Input.GetKeyDown(KeyCode.LeftShift) && !fastSpeed) {
            isShiftPressed = true;
            animator.SetBool(runHash, false);
            animator.SetBool(walkHash, true);
            walkSpeed = slowSpeed;
        }

        if (Input.GetKeyUp(KeyCode.LeftShift) && !fastSpeed) {
            isShiftPressed = false;
            animator.SetBool(walkHash, false);
            walkSpeed = initialSpeed;
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
        int index = Random.Range(0, INJURED_SOUNDS.Length);
        GetComponent<AudioManager>().Play(INJURED_SOUNDS[index]);
        GameObject bloodSplatter = GameObject.FindWithTag("Blood");
        var color = bloodSplatter.GetComponent<Image>().color;
        color.a = 0.8f;
        bloodSplatter.GetComponent<Image>().color = color;
    }

    // logic for Jumping
    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            velocity.y += Mathf.Sqrt(jumpHeight * -2f * gravity);
            animator.SetTrigger(jumpTrigHash);
        }
    }

    public void IncrementBullets(int amount)
    {
        if (bullets == 0) emptyGunIcon.SetActive(false);
        bulletsArray[max_bullets - bullets - 1].SetActive(true);
        bullets += amount;
    }

    // logic for shooting other players
    void Shoot()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && LastShootTime + ShootDelay < Time.time)
        {
            if (bullets > 0)
            {
                GetComponent<AudioManager>().Play(GUN_SOUND);
                BroadcastSound(GUN_SOUND);

                bulletsArray[max_bullets - bullets].SetActive(false);
                bullets -= 1;

                if (bullets == 0)
                    emptyGunIcon.SetActive(true);

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

    public void setKnife(bool state)
    {
        knifeModel.SetActive(state);
    }

    // logic for using Melee/Knife
    void UseKnife()
    {
        if (Input.GetKeyDown(KeyCode.F) && meleeCd <= 0)
        {
            knife.UseKnife();
            animator.SetTrigger(meleeHash);
            meleeCd = 1.5f;
            view.RPC("RPC_playKnifeAnimation", RpcTarget.Others);
        }
    }

    [PunRPC]
    void RPC_playKnifeAnimation()
    {
        animator.SetTrigger(meleeHash);
    }

    public void SetGroundedState(bool _grounded)
    {
        grounded = _grounded;
    }

    public void SetTeamAndUpdateMaterials(int tm)
    {
        team = tm;
        view.RPC("RPC_ChangeTextureAndMinimap", RpcTarget.AllBuffered, tm);
    }

    [PunRPC]
    void RPC_ChangeTextureAndMinimap(int tm)
    {
        team = tm;
        if (team == 0)
        {
            //Make sure correct HUD and text colour is used for team
            GetComponent<Renderer>().material = BlueMat;
            playerIcon.GetComponent<SpriteRenderer>().color = Color.blue;
            playerIcon.layer = 11;
            //Get correct player model to show depending on team
            AttackerModel.SetActive(false);
            DefenderModel.SetActive(true);
            if (view.IsMine)
            {
                minimapCamera.cullingMask |= (1 << 11); // adds layer 11 to the minimap
            }
        }
        else
        {
            //Make sure correct HUD and text colour is used for team
            GetComponent<Renderer>().material = RedMat;
            playerIcon.GetComponent<SpriteRenderer>().color = Color.red;
            playerIcon.layer = 10;
            //Get correct player model to show depending on team
            DefenderModel.SetActive(false);
            AttackerModel.SetActive(true);
            animator = animatorAtt;
            knifeModel = knifeAtt;
            if (view.IsMine)
            {
                minimapCamera.cullingMask |= (1 << 10); // adds layer 10 to the minimap
            }

        }
    }
    // Sonund effect when taking damage
    public void TakeDamage(int damage)
    {
        //GotHurt();
        view.RPC("RPC_TakeDamage", RpcTarget.All, damage);
    }
    // Sonund effect when picking up supplies
    public void PickUpSupplySound()
    {
        GetComponent<AudioManager>().Play(PICKUP_SUPPLY_SOUND);
        BroadcastSound(PICKUP_SUPPLY_SOUND);
    }

    // Sound effect when using the knife and hurting a player
    public void KnifeKillingSound()
    {
        GetComponent<AudioManager>().Play(KNIFE_KILLING_SOUND);
        BroadcastSound(KNIFE_KILLING_SOUND);
    }

    // Sound effect when using the knife
    public void KnifeSound()
    {
        GetComponent<AudioManager>().Play(KNIFE_SOUND);
        BroadcastSound(KNIFE_SOUND);
    }
    // Sound effect when opening the torch
    public void OpenTorchSound()
    {
        GetComponent<AudioManager>().Play(OPEN_TORCH_SOUND);
        BroadcastSound(OPEN_TORCH_SOUND);
    }

    // Sonund effect when closing the torch
    public void CloseTorchSound()
    {
        GetComponent<AudioManager>().Play(CLOSE_TORCH_SOUND);
        BroadcastSound(CLOSE_TORCH_SOUND);
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
        int index = Random.Range(0, DEATH_SOUNDS.Length);
        GetComponent<AudioManager>().Play(DEATH_SOUNDS[index]);
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
        GameObject bloodSplatter = GameObject.FindWithTag("Blood");
        var color = bloodSplatter.GetComponent<Image>().color;
        color.a = 0;
        bloodSplatter.GetComponent<Image>().color = color;
    }
    // Netcode logic for "Invisibility" spell - start the spell
    // Not currently in the game
    [PunRPC]
    void RPC_MakeInvisible(int ind)
    {
        if (index == ind)
        {
            GetComponent<Renderer>().enabled = false;
            // gun.GetComponent<Renderer>().enabled = false;
        }
    }

    // Netcode logic for "Invisibility" spell - stop the spell
    // Not currently in the game
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
    // Logic for "Reload" spell
    // Not currently in the game
    public void Reload()
    {
        if (bullets < 5)
        {
            for (int i = 0; i < max_bullets; i++)
            {
                if (i < max_bullets - bullets)
                    bulletsArray[i].SetActive(false);
                else
                    bulletsArray[i].SetActive(true);
            }
            bullets = 5;
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
