using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* The moment you start the game, the component from the PickUpManager is asked if the Canvas exists in the Start() routine.*/
public class PickUpManager : MonoBehaviour {
    //PickUpComponent is the camera, thus has the Canvas
    //Each component from the ones that are attached to the player (e.g: Health, Supply etc.) are added to the "PickUpComponent" list.
    [SerializeField] private List<PickUpComponent> instancePickupComponents;  
    private UIScriptPlayer uiComponent; // pick up component that can be attached
    private bool uiInitialized;
    // Start is called before the first frame update
    //Creates the camera and the canvas for the person itself, the character
    void Start()
    {
        //If the canvas exists, it asks form the uiComponent (if the UIScriptPlayer) acctually exists!
        uiComponent = this.gameObject.GetComponentInParent<UIScriptPlayer>();
        if (uiComponent == null) throw new MissingComponentException("UI Script missing from parent");

    }

    // Update is called once per frame
    //
    void Update()
    {
        if(!uiInitialized)
        {
            bool setUiInitialized = true;
            foreach (PickUpComponent pickupComponent in instancePickupComponents)
            {
                bool toBreak = false;
                foreach (GameObject uiElement in pickupComponent.GetUIElements())
                { 
                    try {
                        //Gets called based on how many pick up components it passes.
                        //Attach components to the screen based on how many they are according to each player.
                        uiComponent.AttachUI(uiElement, uiElement.transform.position, uiElement.transform.rotation, uiElement.transform.localScale);
                    }
                    catch (System.Exception e) { 
                        setUiInitialized = false;
                        toBreak = true; 
                    }
                }
                if (toBreak) break;
            }
            uiInitialized = setUiInitialized;
        }
    }

    //Every time you collision with sth that is a pick up, checks if it has a pick up script, if it does, passes the pick up script to 
    //all instances of pick up components that are already attached to the pick up manager.
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "PickUp")
        {
            //pass pick up component and script to Trigger; the logic from the respective pick up item is being passed
            //could change the Pick Up Type form inside Unity (Supply/Health/Harmour)
            var pickUp = collision.gameObject.GetComponent<PickUpScript>();
            foreach (PickUpComponent instance in instancePickupComponents)
            {
                if(collision.gameObject != null)
                {
                    instance.pickupTrigger(pickUp);

                } else { break; }
            }
        }
    }
}
