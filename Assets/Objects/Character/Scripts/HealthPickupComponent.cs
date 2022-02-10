using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class HealthPickupComponent : PickUpComponent
{
    private float  current_health;
    [SerializeField] private float max_health;
    [SerializeField] private float min_health;

    private void Update()
    {
        
    }

    public override List<GameObject>  GetUIElements()
    {
        var elements = base.GetUIElements();
        base.setSlider(5, "Health", current_health / max_health);
        
        return elements;
    }

    private void incrementHealth(float value)
    {
        current_health = Mathf.Clamp(current_health + value, min_health, max_health);
        base.setSlider(5, "Health", current_health / max_health);
    }


    public override void pickupTrigger(PickUpScript pickup)
    {
         if (pickup != null)
         {
            // Check if it s a health component 
            if (pickup.pickupType == PickUpScript.PickUpType.Health)
            {
                //Go through each uiElement form the list of PickUpComponents where these prefabs are declared.

                incrementHealth(5f);
                Destroy(pickup.gameObject);
            }
         } 
    }
}
