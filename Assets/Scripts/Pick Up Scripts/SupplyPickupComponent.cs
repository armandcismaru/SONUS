using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SupplyPickupComponent : PickUpComponent
{
    private float current_food;
    [SerializeField] private float max_food;
    [SerializeField] private float min_food;

    public override List<GameObject> GetUIElements()
    {
        var elements = base.GetUIElements();
        base.setSlider(5, "Food", current_food / max_food);

        return elements;
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
            base.setSlider(5, "Food", current_food / max_food); 
    }
    
    public override void pickupTrigger(PickUpScript pickup)
    {
        if (pickup != null)
        {
            if (pickup.pickupType == PickUpScript.PickUpType.Food)
            {
                incrementFood(5f);                
                pickup.destroyThisObject();
            }
        }
    } 
}

