using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeVoiceRecognition : MonoBehaviour
{
    [SerializeField] Gun gun;
    PlayerController playerController;
    // [SerializeField] GameObject playerController;
    // Start is called before the first frame update
    void Start()
    {
        playerController = this.gameObject.transform.parent.gameObject.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void TriggerSpell(string hypseg) {
        Debug.Log("From Unity:" + hypseg);
        playerController.GetComponent<AudioManager>().Play("Gunshot");
        // if (true)
        // {
        //     if (playerController.bullets > 0)
        //     {
        //         playerController.GetComponent<AudioManager>().Play("Gunshot");
        //         playerController.BroadcastSound("Gunshot");

        //         playerController.bullets -= 1;
        //         playerController.bulletsView.text = playerController.bullets.ToString();
        //         gun.Shoot();
        //     }
        //     else
        //     {
        //         GetComponent<AudioManager>().Play("DryFire");
        //         playerController.BroadcastSound("DryFire");
        //     }
        // }
        // gun.Shoot();
    }
}
