using System.Collections.Generic;
using UnityEngine;


// Meant to create the Canvas 
public class UIScriptPlayer : MonoBehaviour
{ 
    // Instances 
    private Canvas playerCanvas;

    // List of different UI elements
    // Is obsolete if nothing is attached to the Canvas.
    private List<GameObject> uiElements;

    // Start is called before the first frame update
    void Start()
    {
        playerCanvas = GetComponentInChildren<Canvas>();
        uiElements = new List<GameObject>(); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*  Each gameplay component creates its own UI elements and has its own logic
        ex. Health Component - creates UI object, knows where it needs to be on the screen
        (after creation -> the object is passed to a UI manager/ this script, gets attached)
        => rest of the logic is controlled in the Health Component - pickup health, take damage, know how much health you have etc.
        absolutely the same principle for supplies.
        AttachUI() passes as parameters are the UI element (a prefab - which has already been initialized-),
        then the location of where the prefab is placed on the screen
    */
    public GameObject AttachUI(GameObject uiObject, GameObject parent, bool isParentCanvas)
    {
        Transform parentTransform;
        if (isParentCanvas)
        {
            parentTransform = playerCanvas.transform;
        }
        else
        {
            parentTransform = parent.transform;
        }

        Vector2 referenceResolution = uiObject.GetComponent<UIElementData>().referenceResolution;
        float screenMultiplier = Screen.currentResolution.width / referenceResolution.x;

        Vector3 localPosition = uiObject.transform.localPosition;
        Quaternion localRotation = uiObject.transform.rotation;
        Vector3 localScale = uiObject.transform.localScale;

        uiObject = Instantiate(uiObject, parentTransform);
        uiObject.transform.localPosition = localPosition; // screenMultiplier;
        uiObject.transform.localRotation = localRotation; // multiply rotation later
        uiObject.transform.localScale = localScale; // screenMultiplier;
        uiElements.Add(uiObject);

        return uiObject;
    }
}
