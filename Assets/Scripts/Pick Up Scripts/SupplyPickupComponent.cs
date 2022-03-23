using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SupplyPickupComponent : PickUpComponent, IDieObserver
{
    public float current_food = 0;

    [Tooltip("Only one supply can be picked by an attacker. Leave value as 1 by default.")]
    [ReadOnly]
    [SerializeField] private float max_food = 1;
    [SerializeField] private float min_food;

    [SerializeField] private GameObject prefabType;

    public int supplyCharge;

    private PhotonView view;
    private void Awake()
    {
        view = GetComponent<PhotonView>();
    }

    private void Start()
    {
        var playerController = GetComponent<PlayerController>();
        playerController.addObserver<IDieObserver>(this);

        supplyCharge = prefabType.GetComponent<PickUpScript>().amount;
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
            for (int i = 0; i < current_food / supplyCharge; i++)
            {
                
                GameObject supply =  PhotonNetwork.Instantiate(prefabType.name, gameObject.transform.position, Quaternion.identity);
                RoomManager.collectables.Add(supply);
            }
        } else
        {
            throw new System.Exception("Incorrect prefab type");
        }
    }

    public override void updateUI()
    {
        base.setSlider(5, "Food", current_food / (max_food * supplyCharge));
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
        current_food = Mathf.Clamp(current_food + value, min_food, max_food * supplyCharge);
        if (GetComponent<PhotonView>().IsMine)
            updateUI(); 
    }
    
    public override void pickupTrigger(PickUpScript pickup)
    {
        if (gameObject.GetComponent<PlayerController>().team == 1)
        {
            if (pickup.pickupType == PickUpScript.PickUpType.Food && current_food < max_food * supplyCharge)
            {
                //if current_food <= pickup.amount --- because my peers want to be able to pick one supply at a time, take it to shelter,
                //then be able to pick one more
                incrementFood(pickup.amount);
                pickup.destroyThisObject();
                //RoomManager.Instance.AttackersWon();
            }
        }
    } 
}

