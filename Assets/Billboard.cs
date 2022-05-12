using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//class for the player names, it orients the names to be facing the camera of the player
public class Billboard : MonoBehaviour
{
    private GameObject cam;

    //searches for the camera of the player and orients the object it is attached to towards it
    private void Update()
    {
        if(cam == null)
        {
            var players = GameObject.FindGameObjectsWithTag("Player");
            foreach(var player in players)
            {
                if (player.GetComponent<PlayerController>().mainCamera.gameObject.activeSelf)
                {
                    cam = player;
                    break;
                }
            }
        }

        if(cam == null)
        {
            return;
        }

        transform.LookAt(cam.transform);
        transform.Rotate(Vector3.up * 180);
    }
}
