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
                if (filterTag == "Food")
                {
                    //Image image = uiElement.GetComponentInChildren<Image>();
                    //image.color = new Color32(255, 255, 255, 100);
                }
                else
                {
                    Slider slider = uiElement.GetComponent<Slider>();
                    slider.value = value;
                }                      
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

    public virtual void pickupTrigger(PickUpScript pickup)
    {

    }
}