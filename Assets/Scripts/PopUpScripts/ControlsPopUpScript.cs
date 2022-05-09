using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlsPopUpScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(true);
    }

    void DetectMovement()
    {
        //If WASD hasn't been pressed (so player hasn't moved), pop up shown. As soon as player moves, it disappears
        if (Input.GetKey(KeyCode.W) | Input.GetKey(KeyCode.A) | Input.GetKey(KeyCode.S) | Input.GetKey(KeyCode.D))
        {
            gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        DetectMovement();
    }
}
