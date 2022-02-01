using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundCheck : MonoBehaviour
{
    PlayerController playerController;
    void Awake()
    {
        playerController = GetComponentInParent<PlayerController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != playerController.gameObject)
        {
            playerController.SetGroundedState(true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject != playerController.gameObject)
        {
            playerController.SetGroundedState(false);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject != playerController.gameObject)
        {
            playerController.SetGroundedState(true);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject != playerController.gameObject)
        {
            playerController.SetGroundedState(true);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject != playerController.gameObject)
        {
            playerController.SetGroundedState(false);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject != playerController.gameObject)
        {
            playerController.SetGroundedState(true);
        }
    }
}
