using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class PlayerController : MonoBehaviourPunCallbacks, IDamageable
{
    [SerializeField] private float mouseSensitivity, sprintSpeed, walkSpeed, jumpForce, smoothTime;
    [SerializeField] GameObject cameraHolder;
    private float verticalLookRotation;
    private bool grounded;
    private bool isMoving;
    private Vector3 smoothMoveVelocity;
    private Vector3 moveAmount;
    private int health;
    private int bullets;
    private PhotonView view;
    SpawnPlayers spawnPlayers;
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
    }

    void Start()
    {
        //spawnPlayers = PhotonView.Find((int)view.InstantiationData[0]).GetComponent<SpawnPlayers>();
        if (!view.IsMine)
        {
            Destroy(GetComponentInChildren<Camera>().gameObject);
            //gunView.SetActive(true);
            Destroy(rb);
        }
        Cursor.lockState = CursorLockMode.Locked;
        isMoving = false;
        health = 100;
        bullets = 5;
    }

    // Update is called once per frame
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
            if (!FindObjectOfType<AudioManager>().isPlaying("ConcreteFootsteps"))
                FindObjectOfType<AudioManager>().Play("ConcreteFootsteps");
        }
        else
            FindObjectOfType<AudioManager>().Stop("ConcreteFootsteps");

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
        if (Input.GetKeyDown(KeyCode.Mouse0) && bullets > 0)
        {
            FindObjectOfType<AudioManager>().Play("Gunshot");
            Debug.Log("da");
            bullets -= 1;
            bulletsView.text = bullets.ToString();
            gun.Shoot();
            Debug.Log("nu");

        }
        else if (Input.GetKeyDown(KeyCode.Mouse0) &&  bullets <= 0)
        {
            Debug.Log("DASDA");
            FindObjectOfType<AudioManager>().Play("DryFire");
        }
    }

    void UseKnife()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            FindObjectOfType<AudioManager>().Play("stab");
            //Debug.Log("da");
            knife.UseKnife();
            //Debug.Log("nu");

        }
    }

    public void SetGroundedState(bool _grounded)
    {
        grounded = _grounded;
    }

    //private void Die()
    //{
    //    Destroy(gameObject);
    //}

    public void TakeDamage(int damage)
    {
        view.RPC("RPC_TakeDamage", RpcTarget.All, damage);
    }

    [PunRPC]
    void RPC_TakeDamage(int damage)
    {
        //if (!view.IsMine)
        //    return;
        health -= damage;
        healthView.text = health.ToString();
        Debug.Log(health);
        //healthbarImage.fillAmount = currentHealth / maxHealth;

        if (health <= 0)
        {
            Debug.Log("AM MURIT");
            Die();
            
        }
    }

    //void Die()
    //{
    //    spawnPlayers.Die();
    //}

    public void Die()
    {
        Debug.Log("5");
        PhotonNetwork.Destroy(gameObject);
        //CreateController();
    }
}
