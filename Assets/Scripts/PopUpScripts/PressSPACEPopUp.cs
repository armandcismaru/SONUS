using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressSPACEPopUp : MonoBehaviour
{
    //Makes sure pop up only displays once
    private bool AlreadyDisplayed;
    CanvasGroup canvasGroup;
    // Start is called before the first frame update
    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        AlreadyDisplayed = false;
    }

    void DetectMovement()
    {
        if ((Input.GetKey(KeyCode.W) | Input.GetKey(KeyCode.A) | Input.GetKey(KeyCode.S) | Input.GetKey(KeyCode.D)) && !AlreadyDisplayed)
        {
            //Pop up appears right after WASD pop up
            gameObject.SetActive(true);
            canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.alpha = 1;
            AlreadyDisplayed = true;
        }
        else if (Input.GetKey(KeyCode.Space) | Input.GetKey(KeyCode.LeftShift))
        {
            //Pop up disappears as soon as player jumps or sprints for the first time
            gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        DetectMovement();
    }
}
