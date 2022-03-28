using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class Pause : MonoBehaviour
{   
    public static bool paused = false;
    private bool disconnecting = false;

    public void TogglePause (){
        if (disconnecting) return;

        paused = !paused;

        transform.GetChild(3).gameObject.SetActive(paused);
        Cursor.lockState = (paused) ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = paused;
    }

    public void Quit() {
        disconnecting = true;
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(0);
    }

}