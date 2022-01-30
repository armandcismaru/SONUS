using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Camera cam;

    private Rigidbody rb;
    private float movementSpeed = 10f;
    private bool _jump;
    private bool onGround;
    private float horizontal;
    private float vertical;

    void Start()
    {
        cam = Camera.main;
        rb = GetComponent<Rigidbody>();
        _jump = false;
        onGround = false;
        horizontal = 0;
        vertical = 0;
}

    void Update()
    {
        Debug.Log(onGround);
        if (onGround)
        {

            horizontal = Input.GetAxis("Horizontal") * movementSpeed;
            vertical = Input.GetAxis("Vertical") * movementSpeed;
            if (Input.GetKey(KeyCode.Space))
            {
                _jump = true;
                onGround = false;
            }

        }
    }
    private void FixedUpdate()
    {

        if (_jump)
        {
            rb.AddForce(Vector3.up * 5, ForceMode.VelocityChange);
            _jump = false;
        }

        Vector3 cameraOrientation = cam.transform.right * horizontal + cam.transform.forward * vertical;
        rb.velocity = new Vector3(cameraOrientation.x, rb.velocity.y, cameraOrientation.z);
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            onGround = true;
        }

    }
}
