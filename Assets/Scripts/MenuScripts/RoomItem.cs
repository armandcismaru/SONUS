using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomItem : MonoBehaviour
{
    [SerializeField] private Text text;

    RoomInfo info;
    public void SetUp(RoomInfo inf)
    {
        info = inf;
        text.text = inf.Name;
    }

    public void OnPressed()
    {
        CreateAndJoinRooms.Instance.JoinRoom(info);
    }
}
