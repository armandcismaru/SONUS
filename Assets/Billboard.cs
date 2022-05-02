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
