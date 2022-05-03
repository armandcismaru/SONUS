using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class VideoController : MonoBehaviour
{   
    public double time;
    public double currentTime;

    // Start is called before the first frame update
    void Start()
    {
        //time = gameObject.GetComponent<VideoPlayer> ().clip.length;
        time = 47;
    }

    // Update is called once per frame
    void Update()
    { 
        currentTime = gameObject.GetComponent<VideoPlayer> ().time;
        Debug.Log(currentTime);
        if (currentTime >= time || Input.GetKeyDown(KeyCode.Escape)) {
            SceneManager.LoadScene(1);
        }
    }
}
