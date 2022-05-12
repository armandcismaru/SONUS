using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class Pause : MonoBehaviourPunCallbacks
{   
    public static bool paused = false;
    private bool disconnecting = false;

    // lock cursor & display pause menu on screen
    public void TogglePause () {
        if (disconnecting) return;

        paused = !paused;

        transform.GetChild(3).gameObject.SetActive(paused);
        Cursor.lockState = (paused) ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = paused;
    }

    // DO NOT USE!!!
    // buggy function for exiting photon server
    public void Quit()
    {
        disconnecting = true;
        PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.LocalPlayer);
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene(0);
    }



}