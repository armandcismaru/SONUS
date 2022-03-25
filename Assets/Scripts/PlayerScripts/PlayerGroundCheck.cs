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

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject != playerController.gameObject)
        {
            playerController.SetGroundedState(true);
            if (other.collider.gameObject.tag == "Ground" &&
                other.GetContact(0).thisCollider.transform.gameObject.name != "Gun")
            {
                playerController.GetComponent<AudioManager>().Play("Jump");
                playerController.BroadcastSound("Jump");
            }
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject != playerController.gameObject)
        {
            playerController.SetGroundedState(false);
        }
    }

    private void OnCollisionStay(Collision other)
    {
        if (other.gameObject != playerController.gameObject)
        {
            playerController.SetGroundedState(true);
        }
    }
}
