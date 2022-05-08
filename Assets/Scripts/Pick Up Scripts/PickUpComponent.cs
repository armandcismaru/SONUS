using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*Base class where the prefabs are being called*/
public class PickUpComponent : MonoBehaviour
{
    [SerializeField] private List<GameObject> UIElements; //references to the UI componenets that can be attached.
    public List<GameObject> instancesUIElements;

    public List<GameObject> getUIElements()
    {
        return UIElements;
    }

    //Called in PickUpManager
    public void setInstancesUI(Component component, List<GameObject> instancesUI)
    {
        if (component.gameObject == this.gameObject)
        {
            instancesUIElements = instancesUI;
        }
    }

    internal void SetSlider(int filterLayer, string filterTag, float value)
    {
        foreach (GameObject uiElement in instancesUIElements)
            if (uiElement.layer == filterLayer && uiElement.tag == filterTag)
            {        
                Slider slider = uiElement.GetComponent<Slider>();
                slider.value = value;                     
            }
    }

    public virtual void updateUI()
    {
        
    }

    public virtual void updateUIText(int filterLayer, string filterTag, int value)
    {
        foreach (GameObject uiElement in instancesUIElements)
        {
            if (uiElement.layer == filterLayer && uiElement.tag == filterTag)
            {
                Text displayedText = uiElement.GetComponent<Text>();
                displayedText.text = (value + 1).ToString();
            }
        }
    }

    /*Method used by all "picking up collectables" classes 
     * in which mechanic for collecting the respective item is being described
     * (incrementing the resource, destroying the item in the scene, notifing the user via the HUD)
     */ 
    public virtual void pickupTrigger(PickUpScript pickup)
    {

    }
}