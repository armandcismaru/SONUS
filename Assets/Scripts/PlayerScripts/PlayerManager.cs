using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerManager : MonoBehaviour
{
    public GameObject playerPrefab;

    PhotonView view;

    public float minX;
    public float maxX;
    public float minZ;
    public float maxZ;

    [HideInInspector] public int team;

    private void Awake()
    {
        view = GetComponent<PhotonView>();
    }
    void Start()
    {
        if (view.IsMine)
        {
            Vector3 randomPosition = new Vector3(Random.Range(minX, maxX), 3, Random.Range(minZ, maxZ));
            GameObject obj = PhotonNetwork.Instantiate(playerPrefab.name, randomPosition, Quaternion.identity);
            obj.GetComponent<PlayerController>().team = team;
        }
    }
}
