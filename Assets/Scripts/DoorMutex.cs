using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorMutex : MonoBehaviour
{
    [SerializeField] public bool isOpen = false;
    [SerializeField] public bool lock_access_pivot = true;

  
    public void Evt_mutex()
    {
        lock_access_pivot = true;
        GetComponent<PhotonView>().RPC("AquireAccessPivot", RpcTarget.Others, true);
    }

    [PunRPC]
    public void AquireAccessPivot(bool state)
    {
        lock_access_pivot = state;
    }

    [PunRPC]
    public void CallDisableCollider()
    {
        var colliders = gameObject.transform.parent.GetComponentsInChildren<BoxCollider>();

        isOpen = !isOpen;

        foreach (BoxCollider collider in colliders)
        {
            collider.isTrigger = isOpen;
        }
    }
}
