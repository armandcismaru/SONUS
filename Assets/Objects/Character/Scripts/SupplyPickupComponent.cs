using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SupplyPickupComponent : PickUpComponent
{
    private int food;
    private int fuel;

    public override void pickupTrigger(PickUpScript pickup)
    {
        if (pickup != null)
        {
            // Check if it s a health component 
            if (pickup.pickupType == PickUpScript.PickUpType.Food || pickup.pickupType == PickUpScript.PickUpType.Fuel)
            {

                //Go through each uiElement form the list of PickUpComponents where these prefabs are declared.
                foreach (GameObject uiElement in instancesUIElements)
                {
                    //layer 5 is for UIs apparently
                    if (uiElement.layer == 5 && uiElement.tag == "Food")
                    {
                        //Find the slider for the food component and increment it.
                        uiElement.GetComponent<Slider>().value += 0.5f;
                        //Then destroy the health object.
                        Destroy(pickup.gameObject);
                    } else if (uiElement.layer == 5 && uiElement.tag == "Fuel")
                    {
                        //Find the slider for the food component and increment it.
                        uiElement.GetComponent<Slider>().value += 0.5f;
                        //Then destroy the health object.
                        Destroy(pickup.gameObject);
                    }
                }
            }
        }
    }



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

