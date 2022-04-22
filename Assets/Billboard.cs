using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    private GameObject cam;
    private void Update()
    {
        if(cam == null)
        {
            cam = GameObject.FindGameObjectsWithTag("Player")[0];
        }

        if(cam == null)
        {
            return;
        }
        transform.LookAt(cam.transform);
        transform.Rotate(Vector3.up * 180);
    }
}
