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

    private bool picked;

    public override List<GameObject> GetUIElements()
    {
        var elements = base.GetUIElements();
        base.setSlider(5, "Food", current_food / max_food);

        return elements;
    }

    private void incrementFood(float value)
    {
        current_food = Mathf.Clamp(current_food + value, min_food, max_food);
        base.setSlider(5, "Food", current_food / max_food);
    }

    /*public override void pickupTrigger(PickUpScript pickup)
    {
            if (pickup != null)
            {
                // Check if it s a food component 
                if (pickup.pickupType == PickUpScript.PickUpType.Food)
                {
                    incrementFood(5f);
                    //Check
                    if (GetComponent<PhotonView>().IsMine)
                    {
                        GetComponent<PhotonView>().RPC("destroyPickUpActor", RpcTarget.AllBuffered, pickup);
                    }
                    // PhotonNetwork.Destroy(pickup.gameObject);
                }
            }
    }*/


    public override void pickupTrigger(PickUpScript pickup)
    {
        if (gameObject.GetComponent<PlayerController>().team == 1)
        {
            if (pickup != null)
            {
                // Check if it s a food component 
                if (pickup.pickupType == PickUpScript.PickUpType.Food)
                {
                    if (!picked)
                    {
                        //Check
                        //pickup.destroyThisObject();
                        picked = true;
                        RoomManager.Instance.suppliesPicked();
                    }
                    // PhotonNetwork.Destroy(pickup.gameObject);
                }
            }
        }
    }


    /*[PunRPC]
    private void destroyPickUpActor(PickUpScript pickUp)
    {
        PhotonNetwork.Destroy(pickUp.gameObject);
    }*/





    /*if(pickup.pickupType == PickUpScript.PickUpType.Supply)
    {
        var supplyPickup = cast.... < SupplyPickup >;
        switch supplyPickup.SupplyType{
            case food: food++; break;
            case fuel: fuel++; break;
            default: break;
        }
        foreach (GameObject uiElement in instancesUIElements)
        {
            if (uiElement.layer == 5 && uiElement.tag == "Health")
            {
                uiElement.GetComponent<Slider>().value += 0.5f;
                Destroy(pickup.gameObject);
            }
        }
    }*/
}

