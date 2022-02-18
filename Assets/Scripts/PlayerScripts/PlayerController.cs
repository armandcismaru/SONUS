using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using TMPro;

public class PlayerController : MonoBehaviourPunCallbacks, IDamageable
{
    [SerializeField] private float mouseSensitivity, walkSpeed, jumpHeight, smoothTime, gravity;
    [SerializeField] private CharacterController controller;
    [SerializeField] GameObject cameraHolder;
    [SerializeField] private Material RedMat;
    [SerializeField] private Material BlueMat;
    [SerializeField] private TMP_Text blueScore;
    [SerializeField] private TMP_Text redScore;
    [SerializeField] private TMP_Text timer;

    [HideInInspector] public int team;
    private PlayerManager playerManager;

    private float verticalLookRotation;
    public bool grounded;
    private bool isMoving;
    private Vector3 smoothMoveVelocity;
    private Vector3 moveAmount;
    private Vector3 velocity;
    private int health;
    private int bullets;
    private PhotonView view;
    [SerializeField] Gun gun;
    [SerializeField] Knife knife;
    private Rigidbody rb;
    [SerializeField] GameObject gunView;
    public Text healthView;
    public Text bulletsView;

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        view = GetComponent<PhotonView>();

        if (view.IsMine)
        {
            playerManager = PhotonView.Find((int)view.InstantiationData[0]).GetComponent<PlayerManager>();
        }
    }

    void Start()
    {
        if (!view.IsMine)
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
            Destroy(rb);
        }
        Cursor.lockState = CursorLockMode.Locked;
        isMoving = false;
        health = 100;
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
    void Look()
    {
        var x = Input.GetAxis("Mouse X") * Time.deltaTime;
        var y = Input.GetAxis("Mouse Y") * Time.deltaTime;
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            x = DampenedMovement(x);
            y = DampenedMovement(y);
        }
        x *= mouseSensitivity;
        y *= mouseSensitivity;
        transform.Rotate(Vector3.up * x * mouseSensitivity);

        verticalLookRotation += y * mouseSensitivity;
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
        view.RPC("RPC_TakeDamage", RpcTarget.All, damage);
    }

    [PunRPC]
    void RPC_TakeDamage(int damage)
    {
        if (!view.IsMine)
            return;

        health -= damage;
        healthView.text = health.ToString();

        if (health <= 0)
        {
            Die();
            
        }
    }

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

    private void Die()
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
}
