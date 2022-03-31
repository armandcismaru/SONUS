using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SupplyPickupComponent : PickUpComponent, IDieObserver
{
    private float current_food;
    [SerializeField] private float max_food;
    [SerializeField] private float min_food;

    private bool picked;

    [SerializeField] private GameObject prefabType;

    private PhotonView view;
    private void Awake()
    {
        view = GetComponent<PhotonView>();
    }

    private void Start()
    {
        var playerController = GetComponent<PlayerController>();
        playerController.addObserver<IDieObserver>(this);
    }

    public void Notify()
    {
        if (view.IsMine)
             DropSupplies();
    }

    public void DropSupplies()
    {
        PickUpScript pickup = prefabType.GetComponent<PickUpScript>();
        if (pickup != null && pickup.pickupType == PickUpScript.PickUpType.Food)
        {
            for (int i = 0; i < current_food / pickup.amount; i++)
            {
                
                GameObject supply =  PhotonNetwork.Instantiate(prefabType.name, gameObject.transform.position, Quaternion.identity);
                RoomManager.collectables.Add(supply);
                //PhotonNetwork.Instantiate(prefabType.name, gameObject.transform.position, Quaternion.identity);
                /*PhotonNetwork.Instantiate(prefabType.name, new Vector3(5, 30, 5), Quaternion.identity);
                Debug.Log("ATTACKER HAS DIED AND FOOD WAS SPAWNED");   /// DOESN T GET CALLED*/
            }
        } else
        {
            throw new System.Exception("Incorrect prefab type");
        }
    }

    public override void updateUI()
    {
        base.setSlider(5, "Food", current_food / max_food);
    }

    private void incrementFood(float value)
    {
        if (!GetComponent<PhotonView>().IsMine)
            return;

        if (PhotonNetwork.IsMasterClient)
        {
            rpcIncrementFood(value);
        }
        else
        {
            GetComponent<PhotonView>().RPC("rpcIncrementFood", RpcTarget.MasterClient, value);
        }
    }

    [PunRPC]
    private void rpcIncrementFood(float value)
    {
        GetComponent<PhotonView>().RPC("replicateIncrementFood", RpcTarget.AllViaServer, value);

    }

    [PunRPC]
    private void replicateIncrementFood(float value)
    {
        current_food = Mathf.Clamp(current_food + value, min_food, max_food);
        if (GetComponent<PhotonView>().IsMine)
            updateUI(); 
    }
    
    public override void pickupTrigger(PickUpScript pickup)
    {
        if (gameObject.GetComponent<PlayerController>().team == 1)
        {
            if (pickup.pickupType == PickUpScript.PickUpType.Food)
            {
                incrementFood(pickup.amount);
                pickup.destroyThisObject();
                //RoomManager.Instance.AttackersWon();
            }
        }
    } 
}

