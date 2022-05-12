using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class CreateAndJoinRooms : MonoBehaviourPunCallbacks
{
    public static CreateAndJoinRooms Instance;

    [SerializeField] TMP_InputField inputWord;
    [SerializeField] GameObject roomListButtonPrefab;
    [SerializeField] Transform roomListContent;
    [SerializeField] TMP_Text displayRoomName;
    [SerializeField] GameObject startButton;
    [SerializeField] TMP_Text playerCount;

    private void Awake()
    {
        Instance = this;
    }

    //function that is called by the button to create lobbies that players can join
    public void CreateRoom()
    {
        if (!string.IsNullOrEmpty(inputWord.text))
        {
            PhotonNetwork.CreateRoom(inputWord.text, new RoomOptions { MaxPlayers = 6 });
            MenuManager.Instance.OpenMenu("loading");
        }
    }
    //function that is called to join a certain room
    public void JoinRoom(RoomInfo inf)
    {
        PhotonNetwork.JoinRoom(inf.Name);
    }

    //photon callback for when a player joins a room, it displays the number of players and the name of the room in the menu
    public override void OnJoinedRoom()
    {
        MenuManager.Instance.OpenMenu("lobby");
        displayRoomName.text = PhotonNetwork.CurrentRoom.Name;
        startButton.SetActive(PhotonNetwork.IsMasterClient);
        playerCount.text = PhotonNetwork.CurrentRoom.PlayerCount.ToString() + "/6 players";
    }

    //photon callback for when the masterclient is switched in case the old one leaves the room, it allows the new masterclient to start the game
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        startButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    //function that is called by the leave room button
    public void leaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        MenuManager.Instance.OpenMenu("loading");
    }

    public override void OnLeftRoom()
    {
        MenuManager.Instance.OpenMenu("main");
    }

    //photon callback, used to controll and display the room list
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (Transform trans in roomListContent)
        {
            Destroy(trans.gameObject);
        }
        for (int i = 0; i < roomList.Count; i++)
        {
            if(!roomList[i].RemovedFromList && roomList[i].PlayerCount < roomList[i].MaxPlayers)
                Instantiate(roomListButtonPrefab, roomListContent).GetComponent<RoomItem>().SetUp(roomList[i]);
        }
    }

    //photon callback used to update the player counts
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        playerCount.text = PhotonNetwork.CurrentRoom.PlayerCount.ToString() + "/6 players";
    }

    //photon callback used to update the player counts
    public override void OnPlayerLeftRoom(Player newPlayer)
    {
        playerCount.text = PhotonNetwork.CurrentRoom.PlayerCount.ToString() + "/6 players";
    }


    public void StartGame()
    {
        PhotonNetwork.LoadLevel(2);
    }

    //all the photon clients in the room will stay in sync their scenes with the masterclient
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }
}
