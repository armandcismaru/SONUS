using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager Instance;
    [HideInInspector] public int currentTeam = 0;
    private PhotonView view;

    public GameObject playerManager;
    public bool warmupEnded = false;
    public int bluePlayers = 0;
    public int redPlayers = 0;

    public int aliveBlue = 0;
    public int aliveRed = 0;

    public int scoreBlue = 0;
    public int scoreRed = 0;
    public bool roundRunning = false;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        Instance = this;
        view = GetComponent<PhotonView>();
    }

    public override void OnEnable()
    {
        base.OnEnable();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    public override void OnDisable()
    {
        base.OnDisable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (!Timer.Instance.IsRunning() && PhotonNetwork.IsMasterClient)
        {
            Timer.Instance.StartTimer(20f);
        }
        if (scene.buildIndex == 2)
        {
            playerManager = PhotonNetwork.Instantiate("PlayerManager", Vector3.zero, Quaternion.identity);
        }
    }

    public void UpdateTeam()
    {
        if(currentTeam % 2 == 0)
        {
            bluePlayers++;
        }
        else
        {
            redPlayers++;
        }
        currentTeam++;
    }

    private void FixedUpdate()
    {
        if (!roundRunning && warmupEnded && PhotonNetwork.IsMasterClient)
        {
            StartRound();
        }
    }

    public void PlayerDied(int team)
    {
        if (roundRunning)
        {
            if (team == 0)
            {
                aliveBlue--;
            }
            else
            {
                aliveRed--;
            }
            if (aliveBlue == 0)
            {
                AttackersWon();
            }
            else if (aliveRed == 0)
            {
                DefendersWon();
            }
        }
    }

    private void StartRound()
    {
        Timer.Instance.StartTimer(90f);
        view.RPC("RPC_StartRound", RpcTarget.All);
    }

    public void TimerFinished()
    {
        if (roundRunning)
        {
            DefendersWon();
        }
        else
        {
            warmupEnded = true;
            view.RPC("RPC_SetWarmupEnded", RpcTarget.OthersBuffered, true);
        }

    }

    public void DefendersWon()
    {
        view.RPC("RPC_EndRoundAndUpdateScores", RpcTarget.All, 0);
        if (PhotonNetwork.IsMasterClient)
        {
            Timer.Instance.StopTimer();
            StartRound();
        }
    }

    public void AttackersWon()
    {
        view.RPC("RPC_EndRoundAndUpdateScores", RpcTarget.All, 1);
        if (PhotonNetwork.IsMasterClient)
        {
            Timer.Instance.StopTimer();
            StartRound();
        }
    }


    [PunRPC]
    void RPC_StartRound()
    {
        aliveBlue = bluePlayers;
        aliveRed = redPlayers;
        roundRunning = true;
        playerManager.GetComponent<PlayerManager>().DestroyController();
        playerManager.GetComponent<PlayerManager>().StartRound();
    }

    [PunRPC]
    void RPC_EndRoundAndUpdateScores(int team)
    {
        roundRunning = false;
        if (team == 0)
        {
            scoreBlue++;
        } 
        else
        {
            scoreRed++;
        }
    }

    [PunRPC]
    void RPC_SetWarmupEnded(bool value)
    {
        warmupEnded = value;
    }

}
