using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorMutex : MonoBehaviour
{
    [SerializeField] public bool isOpen = false;

    private void Start()
    {
        
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
