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

    public float minBlueX;
    public float maxBlueX;
    public float minBlueZ;
    public float maxBlueZ; 

    public float minRedX;
    public float maxRedX;
    public float minRedZ;
    public float maxRedZ;

    public int team = -1;
    [HideInInspector] public bool isReady = false;
    [HideInInspector] public bool isAlive = false;

    PlayerController playerController;

    private void Awake()
    {
        view = GetComponent<PhotonView>();
    }

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            team = 0;
        }

        if (view.IsMine && !PhotonNetwork.IsMasterClient)
        {
            view.RPC("RPC_GetTeam", RpcTarget.MasterClient);
        }
        id = new object[] { view.ViewID };

    }

    Vector3 getRandomPosition()
    {
        if (team == 0)
        {
            return new Vector3(Random.Range(minBlueX, maxBlueX), 30, Random.Range(minBlueZ, maxBlueZ));
        }
        return new Vector3(Random.Range(minRedX, maxRedX), 30, Random.Range(minRedZ, maxRedZ));
    }

    private void SpawnPlayer()
    {
        Vector3 randomPosition = getRandomPosition();
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

        playerController = myAvatar.gameObject.GetComponent<PlayerController>();
    }

    void FixedUpdate()
    {
        KillYourself();
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

    public void KillYourself()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            Die();
        }
    }

    public void Die()
    {
        playerController.Die();
        DestroyMyAvatar();
        if (PhotonNetwork.IsMasterClient)
        {
            RoomManager.Instance.PlayerDied(team);
        }
        else
        {
            view.RPC("RPC_PlayerDied", RpcTarget.MasterClient, team);
        }
    }

    public void DestroyMyAvatar()
    {
        if (view.IsMine && myAvatar != null)
        {
            PhotonNetwork.Destroy(myAvatar.gameObject);
            myAvatar = null;
        }
    }

    public void SwapTeams()
    {
        if (view.IsMine)
        {
            team = 1 - team;
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
        if (view.IsMine)
        {
            RoomManager.Instance.index = tm;
            Debug.Log("team ->");
            Debug.Log(tm);
            team = tm % 2;
            if (myAvatar != null)
            {
                myAvatar.GetComponent<PlayerController>().SetTeamAndUpdateMaterials(team);
                isReady = true;
            }
        }


    }

    [PunRPC]
    void RPC_PlayerDied(int tm)
    {
        RoomManager.Instance.PlayerDied(tm);
    }

    [PunRPC]
    void RPC_SwapTeams()
    {
        if (view.IsMine)
        {
            team = 1 - team;
            myAvatar.GetComponent<PlayerController>().team = team;
        }
    }
}
