using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseWASDPopUp : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //Pop up appears as soon as game starts
        gameObject.SetActive(true);
    }

    void DetectMovement()
    {
        if (Input.GetKey(KeyCode.W) | Input.GetKey(KeyCode.A) | Input.GetKey(KeyCode.S) | Input.GetKey(KeyCode.D))
        {
            //Pop up disappears as soon as player moves then never appears again
            gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        DetectMovement();
    }
}
