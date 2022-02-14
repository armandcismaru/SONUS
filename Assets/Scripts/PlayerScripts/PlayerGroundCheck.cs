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

    // TO FIX: GUN TOO LONG IF YOU POINT IT INTO GROUND YOU LIFT ABOVE GROUND
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject != playerController.gameObject)
        {
            playerController.SetGroundedState(true);
            if (other.gameObject.CompareTag("Ground") &&
                other.GetContact(0).thisCollider.transform.gameObject.name != "Gun")
            {
                FindObjectOfType<AudioManager>().Play("Jump");
                playerController.PlayStopSound("Jump", "play");
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
