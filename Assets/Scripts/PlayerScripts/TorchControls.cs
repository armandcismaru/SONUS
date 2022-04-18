using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TorchControls : MonoBehaviour
{
    bool TorchOn;
    const int MAXTORCHLIFE = 30;
    const int MAXLIGHTINTENSITY = 7;
    private float LifeRemaining = MAXTORCHLIFE;
    private PhotonView view;


    private void Awake()
    {
        view = GetComponent<PhotonView>();
    }
    void Start()
    {
        TorchOn = false;
    }

    public void PowerTorch()
    {
        TorchOn = true;
        LifeRemaining = MAXTORCHLIFE;
    }

    public void DisableTorch()
    {
        TorchOn = false;
    }

    void Update()
    {    
        //To be deleted once torch is linked with voice recognition
        if (Input.GetKey(KeyCode.Q) && view.IsMine)
        {
            TorchOn = !TorchOn;
            LifeRemaining = MAXTORCHLIFE;
            view.RPC("RPC_StartTorch", RpcTarget.Others);
        }

        //Updates torch's "battery"
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
