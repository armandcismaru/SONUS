using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Decoy : MonoBehaviourPunCallbacks
{
    float remainingTime;
    float time;
    private PhotonView view;
    bool isDestroyed;
    public Vector3 direction;
    // Start is called before the first frame update
    void Start()
    {
        time = Time.time;
        isDestroyed = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        remainingTime = time + 5f - Time.time;

        if(remainingTime <= 0)
        {
           destroyThisObject();
        }
        else
        {
            MoveDecoy();
        }
    }

    // The decoy moves itself
    void MoveDecoy()
    {
        float step = Time.deltaTime;
        transform.position = transform.position + direction * step;
    }

    // After 5 seconds the decoy auto-destroy itself
    public void destroyThisObject()
    {
        if (PhotonNetwork.IsMasterClient && GetComponent<PhotonView>().IsMine && !isDestroyed)
        {
            PhotonNetwork.Destroy(gameObject);
            isDestroyed = true;
        }
        else
        {
            GetComponent<PhotonView>().RPC("RPC_DestroyObject", RpcTarget.All);
        }
    }

    // Netcode logic for destroying the decoy 
    [PunRPC]
    void RPC_DestroyObject()
    {
        Destroy(gameObject);
    }
}
