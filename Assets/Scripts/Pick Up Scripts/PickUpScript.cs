using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Different objects in the game can have different pick up types. These types of derivates need to be attached to each object in Unity.*/
public class PickUpScript : MonoBehaviour {
    public enum PickUpType {
        Food,
        //Fuel,
        Health,
        Armor
    };

    [SerializeField] private Vector3 _rotation;
    [SerializeField] private float _speed;
    [SerializeField] private PickUpType _pickupType;

    public PickUpType pickupType { get => _pickupType;}

    private bool isDestroyed = false;

    [SerializeField] public int amount = 0;

    public async void destroyThisObject()
    {
        if (PhotonNetwork.IsMasterClient && GetComponent<PhotonView>().IsMine && !isDestroyed)
        {
            PhotonNetwork.Destroy(gameObject);
            isDestroyed = true;
        }
        else
        {
            GetComponent<PhotonView>().RPC("rpcDestroyObject", RpcTarget.All);
            //Destroy(gameObject);
        }
    }

    [PunRPC]
    void rpcDestroyObject()
    {
        Destroy(gameObject);

        if (PhotonNetwork.IsMasterClient)
        {
            RoomManager.Instance.RemoveCollectable(gameObject);
        }
    }

    void Update()
    {
        transform.Rotate(_rotation * _speed * Time.deltaTime);   
    }

    private void Start()
    {
        gameObject.tag = "PickUp";
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player") 
        {
        }
    }
}
