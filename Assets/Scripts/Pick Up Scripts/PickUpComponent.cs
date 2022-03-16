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

    internal void setSlider(int filterLayer, string filterTag, float value)
    {
            foreach (GameObject uiElement in instancesUIElements)
            {
                if (uiElement.layer == filterLayer && uiElement.tag == filterTag)
                {
                    Slider slider = uiElement.GetComponent<Slider>();
                    slider.value = value;
                }
            }
    }

    public virtual void updateUI()
    {

    }

    public virtual void pickupTrigger(PickUpScript pickup)
    {

    }
}