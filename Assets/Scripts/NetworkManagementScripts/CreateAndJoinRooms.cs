using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class CreateAndJoinRooms : MonoBehaviourPunCallbacks
{
    public InputField inputWord;
    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(inputWord.text);
    }

    public void JoinRoom()
    {
        PhotonNetwork.JoinRoom(inputWord.text);
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.LoadLevel("Map 2.0");
    }
}
