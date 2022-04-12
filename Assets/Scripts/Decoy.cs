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
        // transform.position = new Vector3(0,0,1) * Time.deltaTime;
        remainingTime = time + 5f - Time.time;

        if(remainingTime <= 0)
        {
           destroyThisObject();
        }
        else
        {
            MoveDecoy();
            // GetComponent<AudioManager>().Play(FOOTSTEP_SOUND);
        }
    }

    void MoveDecoy()
    {
        float step = Time.deltaTime;
        // transform.position = Vector3.MoveTowards(transform.position, target.position, step);
        transform.position = transform.position + direction * step;
    }
    public async void destroyThisObject()
    {
        if (PhotonNetwork.IsMasterClient && GetComponent<PhotonView>().IsMine && !isDestroyed)
        {
            PhotonNetwork.Destroy(gameObject);
            isDestroyed = true;
        }
        else
        {
            GetComponent<PhotonView>().RPC("RPC_DestroyObject", RpcTarget.All);
            //Destroy(gameObject);
        }
    }

    [PunRPC]
    void RPC_DestroyObject()
    {
        Destroy(gameObject);
    }
}
