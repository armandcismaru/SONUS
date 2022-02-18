using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoorPopUp : MonoBehaviour
{
    //Makes sure pop up only displays once
    bool AlreadyDisplayed;
    CanvasGroup canvasGroup;
    // Start is called before the first frame update
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        AlreadyDisplayed = false;
    }
    /*
    void DetectNearToDoor()
    {
        GameObject Door = GameObject.Find("Door");
        GameObject Player = GameObject.Find("PlayerCapsule");
        float Distance = (Door.transform.position - Player.transform.position).sqrMagnitude;
        //float TheDistance = PlayerCasting.DistanceFromTarget;
        if (Distance <= 4.0f && !AlreadyDisplayed)
        {
            //Pop up appears when close to door
            gameObject.SetActive(true);
            canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.alpha = 1;
            AlreadyDisplayed = true;
        }
        else if (Input.GetKey(KeyCode.E))
        {
            //Pop up disappears as soon as player opens door for the first time
            gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        DetectNearToDoor();
    }*/
}
