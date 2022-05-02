using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using Photon.Pun;
using Photon.Realtime;

public class VideoController : MonoBehaviour
{   
    public double time;
    public double currentTime;

    // Start is called before the first frame update
    void Start()
    {
        time = gameObject.GetComponent<VideoPlayer> ().clip.length;
    }

    // Update is called once per frame
    void Update()
    { 
        currentTime = gameObject.GetComponent<VideoPlayer> ().time;
        if (currentTime >= time) {
            PhotonNetwork.LoadLevel(1);
        }
    }
}
