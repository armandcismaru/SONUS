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
        TorchOn = false;
        lastClosed = 0;
    }

    public void PowerTorch()
    {
        if (TorchOn == false)
        {
            TorchOn = true;
            LifeRemaining = MAXTORCHLIFE;
        }
    }

    public void DisableTorch()
    {
        TorchOn = false;
    }

    public void TriggerTorch()
    {
        if (TorchOn == true && LifeRemaining > MAXTORCHLIFE - 0.5)
        {
            return;
        }

        if (TorchOn == false && lastClosed != 0 && Time.time < lastClosed + 0.5)
        {
            return;
        }

        if (TorchOn == false)
        {
            GetComponentInParent<PlayerController>().OpenTorchSound();
            lastClosed = Time.time;
        }
        else
        {
            GetComponentInParent<PlayerController>().CloseTorchSound();
        }
        TorchOn = !TorchOn;
        LifeRemaining = MAXTORCHLIFE;
        view.RPC("RPC_StartTorch", RpcTarget.Others);
    }

    void Update()
    {
        //TODO
        /*if (Input.GetKeyDown(KeyCode.Q))
        {
            TriggerTorch();
        }*/

        //Updates torch's "battery"
        if (LifeRemaining == 0)
        {
            TorchOn = false;
        }
        if (TorchOn)
        {
            if (LifeRemaining > 0)
            {
                LifeRemaining -= Time.deltaTime;
            }
            else
            {
                LifeRemaining = 0;
            }
            this.GetComponent<Light>().intensity = MAXLIGHTINTENSITY * (LifeRemaining / MAXTORCHLIFE);
        }
        else
        {
            this.GetComponent<Light>().intensity = 0;
        }
    }

    [PunRPC]
    public void RPC_StartTorch()
    {
        TorchOn = !TorchOn;
        LifeRemaining = MAXTORCHLIFE;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Vector3 direction = transform.TransformDirection(Vector3.forward) * 5;
        Gizmos.DrawRay(this.GetComponent<Transform>().position, direction);
    }
}
