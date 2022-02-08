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
    public void CreateRoom()
    {
        if (!string.IsNullOrEmpty(inputWord.text))
        {
            PhotonNetwork.CreateRoom(inputWord.text, new RoomOptions { MaxPlayers = 6 });
            MenuManager.Instance.OpenMenu("loading");
        }
    }

    public void JoinRoom(RoomInfo inf)
    {
        PhotonNetwork.JoinRoom(inf.Name);
    }
    public override void OnJoinedRoom()
    {
        MenuManager.Instance.OpenMenu("lobby");
        displayRoomName.text = PhotonNetwork.CurrentRoom.Name;
        startButton.SetActive(PhotonNetwork.IsMasterClient);
        playerCount.text = PhotonNetwork.CurrentRoom.PlayerCount.ToString() + "/6 players";
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        startButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public void leaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        MenuManager.Instance.OpenMenu("loading");
    }

    public override void OnLeftRoom()
    {
        MenuManager.Instance.OpenMenu("main");
    }

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

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        playerCount.text = PhotonNetwork.CurrentRoom.PlayerCount.ToString() + "/6 players";
    }

    public override void OnPlayerLeftRoom(Player newPlayer)
    {
        playerCount.text = PhotonNetwork.CurrentRoom.PlayerCount.ToString() + "/6 players";
    }

    public void StartGame()
    {
        PhotonNetwork.LoadLevel("Map 2.0");
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }
}
