using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*Base class where the prefabs are being called*/
public class PickUpComponent : MonoBehaviour
{
    [SerializeField] private List<GameObject> UIElements; //references to the UI componenets that can be attached.
    internal List<GameObject> instancesUIElements;
    //Called in PickUpManager
    public virtual List<GameObject> GetUIElements()
    {
        if (instancesUIElements == null)
        {
            instancesUIElements = new List<GameObject>();
            //Initialize the UI components
            //Create instances for the prefabs (object templates)
            foreach (GameObject uiElement in UIElements)
            {
                instancesUIElements.Add(Instantiate(uiElement, uiElement.transform.position, uiElement.transform.rotation));
            }
        }
        return instancesUIElements;
    }

    internal void setSlider(int filterLayer, string filterTag, float value)
    {
        foreach (GameObject uiElement in instancesUIElements)
        {
            if (uiElement.layer == filterLayer && uiElement.tag == filterTag)
            {
                uiElement.GetComponent<Slider>().value = value;
            }
        }
    }

    public virtual void pickupTrigger(PickUpScript pickup)
    {

    }
}