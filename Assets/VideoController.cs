using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class VideoController : MonoBehaviour
{   
    [SerializeField] private double time, currentTime;

    void Update()
    { 
        currentTime = gameObject.GetComponent<VideoPlayer> ().time;
        if (currentTime >= time || Input.GetKeyDown(KeyCode.Escape)) { 
            SceneManager.LoadScene(1);
        }
    }
}
