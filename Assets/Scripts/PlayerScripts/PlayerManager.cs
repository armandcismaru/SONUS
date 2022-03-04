using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private GameObject PlayerPrefab;
    private GameObject myAvatar = null;
    private PhotonView view;
    private object[] id;

    public float minX;
    public float maxX;
    public float minZ;
    public float maxZ;

    [HideInInspector] public int team = -1;
    [HideInInspector] public bool isReady = false;
    [HideInInspector] public bool isAlive = false;

    private void Awake()
    {
        view = GetComponent<PhotonView>();
    }
    void Start()
    {
        if (view.IsMine)
        {
            view.RPC("RPC_GetTeam", RpcTarget.MasterClient);
        }
        id = new object[] { view.ViewID };

    }

    private void SpawnPlayer()
    {
        Vector3 randomPosition = new Vector3(Random.Range(minX, maxX), 3, Random.Range(minZ, maxZ));
        if (team == 0)
        {
            myAvatar = PhotonNetwork.Instantiate(PlayerPrefab.name, randomPosition, Quaternion.identity, 0, id);
            myAvatar.GetComponent<PlayerController>().SetTeamAndUpdateMaterials(team);
        }
        else
        {
            myAvatar = PhotonNetwork.Instantiate(PlayerPrefab.name, randomPosition, Quaternion.identity, 0, id);
            myAvatar.GetComponent<PlayerController>().SetTeamAndUpdateMaterials(team);
        }
    }
    void FixedUpdate()
    {
        if (view.IsMine)
        {
            if (!RoomManager.Instance.warmupEnded && myAvatar == null && team != -1)
            {
                SpawnPlayer();
            }
        }
    }

    public void StartRound()
    {
        if (view.IsMine)
        {
            SpawnPlayer();
        }
    }

    public void Die()
    {       
        DestroyController();
        Debug.Log(team);
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("DIE CALLED BEFORE MASTER DIES!");
            RoomManager.Instance.PlayerDied(team);
            Debug.Log("DIE CALLED AFTER MASTER DIES!");
        }
        else
        {
            Debug.Log("DIE CALLED BEFORE CLIENT DIES!");
            view.RPC("RPC_PlayerDied", RpcTarget.MasterClient, team);
            Debug.Log("DIE CALLED AFTER CLIENT DIES!");
        }
    }

    public void DestroyController()
    {
        if (view.IsMine && myAvatar != null)
        {
            PhotonNetwork.Destroy(myAvatar.gameObject);
            myAvatar = null;
        }
    }


    [PunRPC]
    void RPC_GetTeam()
    {
        team = RoomManager.Instance.currentTeam % 2;
        if (myAvatar != null)
        {
            myAvatar.GetComponent<PlayerController>().SetTeamAndUpdateMaterials(team);
            isReady = true;
        }
        RoomManager.Instance.UpdateTeam();
        view.RPC("RPC_SentTeam", RpcTarget.OthersBuffered, team);
    }

    [PunRPC]
    void RPC_SentTeam(int tm)
    {
        team = tm;
        if (myAvatar != null)
        {
            myAvatar.GetComponent<PlayerController>().SetTeamAndUpdateMaterials(team);
            isReady = true;
        }
    }

    [PunRPC]
    void RPC_PlayerDied(int tm)
    {
        RoomManager.Instance.PlayerDied(tm);
    }
}
