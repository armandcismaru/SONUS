using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TorchControls : MonoBehaviour
{
    bool TorchOn;
    const int MAXTORCHLIFE = 20;
    const int MAXLIGHTINTENSITY = 7;
    private float LifeRemaining = MAXTORCHLIFE;
    private PhotonView view;
    private float lastClosed;


    private void Awake()
    {
        view = GetComponent<PhotonView>();
    }
    void Start()
    {
        //Make sure game starts with torch off and torch intensity countdown at 0
        TorchOn = false;
        lastClosed = 0;
    }

    //what to do when torch is powered on
    public void PowerTorch()
    {
        if (TorchOn == false)
        {
            //When torch is powered on, torch turns on and the life remaining is set to maximum
            TorchOn = true;
            LifeRemaining = MAXTORCHLIFE;
        }
    }

    //turn torch off
    public void DisableTorch()
    {
        TorchOn = false;
    }

    //main on/off trigger for torch
    public void TriggerTorch()
    {
        //rejuvinates torchlight without an 'on' sound if torch already on
        if (TorchOn == true && LifeRemaining > MAXTORCHLIFE - 0.5)
        {
            return;
        }

        if (TorchOn == false && lastClosed != 0 && Time.time < lastClosed + 0.5)
        {
            return;
        }
        //turns torch on with an 'on' sound if not already on
        if (TorchOn == false)
        {
            GetComponentInParent<PlayerController>().OpenTorchSound();
            lastClosed = Time.time;
        }
        //turns torch off with an 'off' sound
        else
        {
            GetComponentInParent<PlayerController>().CloseTorchSound();
        }
        //reverse state of torch and maximise torch life when turned on/off
        TorchOn = !TorchOn;
        LifeRemaining = MAXTORCHLIFE;
        view.RPC("RPC_StartTorch", RpcTarget.Others);
    }

    void Update()
    {
        //Extra method for triggering torch for testing purposes
        if (Input.GetKeyDown(KeyCode.Q))
        {
            TriggerTorch();
        }

        //Updates torch's "battery" every second, so torchlight slowly fades
        //turns torch off if there's no 'life' left
        if (LifeRemaining == 0)
        {
            TorchOn = false;
        }
        if (TorchOn)
        {
            //decreases life remaining if possible by a set amount
            if (LifeRemaining > 0)
            {
                LifeRemaining -= Time.deltaTime;
            }
            else
            {
                LifeRemaining = 0;
            }
            //calculates overall torchlight intensity based on percentage of torchlife left
            this.GetComponent<Light>().intensity = MAXLIGHTINTENSITY * (LifeRemaining / MAXTORCHLIFE);
        }
        else
        {
            //turns torch off
            this.GetComponent<Light>().intensity = 0;
        }
    }

    //sends torch information over web so other players can see it
    [PunRPC]
    public void RPC_StartTorch()
    {
        TorchOn = !TorchOn;
        LifeRemaining = MAXTORCHLIFE;
    }

    //draw gizmos for testing purposes
    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Vector3 direction = transform.TransformDirection(Vector3.forward) * 5;
        Gizmos.DrawRay(this.GetComponent<Transform>().position, direction);
    }
}
