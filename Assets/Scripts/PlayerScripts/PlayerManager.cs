using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

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
    private bool isDead = false;
    private List<GameObject> spectateCameras = new List<GameObject>();
    private int spectateIndex = 0;

    private void Awake()
    {
        view = GetComponent<PhotonView>();
    }

    void Start()
    {
        id = new object[] { view.ViewID, -1 };
        if (PhotonNetwork.IsMasterClient)
        {
            team = 0;
            id[1] = 0;
        }

        if (view.IsMine && !PhotonNetwork.IsMasterClient)
        {
            view.RPC("RPC_GetTeam", RpcTarget.MasterClient);
        }
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
        spectateCameras = new List<GameObject>();
        isDead = false;
    }

    private void Update()
    {
        //weird
        if(Input.GetKeyDown(KeyCode.Greater) && isDead)
        {
            spectateCameras[spectateIndex].SetActive(false);
            spectateIndex += 1;
            spectateIndex %= spectateCameras.Count;

            if (spectateCameras[spectateIndex] == null)
            {
                spectateCameras.RemoveAt(spectateIndex);
                spectateIndex %= spectateCameras.Count;
            }

            if (spectateCameras[spectateIndex] != null)
            {
                spectateCameras[spectateIndex].SetActive(true);
                spectateCameras[spectateIndex].GetComponentInParent<PlayerController>().SolveSpectateComponents();
            }
        }
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
        //dead players should not be able to communicate with the team
        int ind = myAvatar.GetComponent<PlayerController>().index;
        view.RPC("RPC_MuteDeadPlayer", RpcTarget.Others, RoomManager.Instance.index);

        //and should not hear the team either
#if UNITY_WEBGL && !UNITY_EDITOR
        for(int i = 0; i < 6; i++)
        {
        //unmutes player if he exists otherwise does nothing
            VoiceChat.setPlayerVolume(i, 0);
        }
#endif

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
        if (view.IsMine)
        {
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            //Debug.Log(players.Length);
            foreach (GameObject player in players)
            {
                int playerInd = player.GetComponent<PlayerController>().index;
                if(playerInd != playerController.index && playerInd % 2 == playerController.index % 2)
                {
                    spectateCameras.Add(player.GetComponentInChildren(typeof(Camera), true).gameObject);
                    //player.GetComponent<PlayerController>().SpectateCanv.SetActive(true);
                }
            }
            spectateCameras[0].SetActive(true);
            spectateCameras[0].GetComponentInParent<PlayerController>().SolveSpectateComponents();
            //GameObject.FindWithTag("Player").GetComponent<Camera>().gameObject.SetActive(true);
        }
        isDead = true;

    }

    [PunRPC]
    public void RPC_MuteDeadPlayer(int index)
    {
        Debug.Log("index index index index");
#if UNITY_WEBGL && !UNITY_EDITOR
        if(index != -1)
        {
            VoiceChat.setPlayerVolume(index, 0);
        }
#endif
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

    public GameObject getAvatar()
    {
        return myAvatar;
    }


    [PunRPC]
    void RPC_GetTeam()
    {
        RoomManager.Instance.UpdateTeam();
        view.RPC("RPC_SentTeam", RpcTarget.OthersBuffered, RoomManager.Instance.getCurrentTeam() - 1);
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
            id[1] = tm;
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
