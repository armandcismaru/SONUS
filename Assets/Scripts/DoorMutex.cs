using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* The door mutex is attached to the door pivot
 * On the door pivot are the animations too
 * Once a player has interacted with the door the mutex is aquired
 * and is not released until the end of the animation
 */
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
