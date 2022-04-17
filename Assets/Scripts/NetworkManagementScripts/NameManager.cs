using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class NameManager : MonoBehaviour
{
    [SerializeField] TMP_InputField nameInput;
    void Start()
    {
        PhotonNetwork.NickName = "Player-" + Random.Range(0, 1000000).ToString("00000");
    }

    public void OnNameChanged()
    {
        PhotonNetwork.NickName = nameInput.text;
    }
}
