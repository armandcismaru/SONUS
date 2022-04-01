using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class Pause : MonoBehaviourPunCallbacks
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

    // public override void OnLeftRoom()
    // {
    //     Debug.Log("AICI");
    //     SceneManager.LoadScene(0);
    // }

    public void Quit()
    {
        disconnecting = true;
        // PhotonNetwork.LeaveRoom();
        // PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.Player);
        // PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.LocalPlayer);
        // PhotonNetwork.LeaveRoom();
        // PhotonNetwork.LeaveLobby();
        // PhotonNetwork.Disconnect();
        // while ( PhotonNetwork.NetworkClientState.ToString() == "Disconnecting") {
        //     continue;
        // }
        // SceneManager.LoadScene(0);
        PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.LocalPlayer);
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene(0);
    }



}