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

    public float suppliesX;
    public float suppliesZ;

    private GameObject supplies;

    const int maxNumOfPlayers = 6;
    private string[] offerString = new string[maxNumOfPlayers];
    [HideInInspector] public int index = 0;

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
#if UNITY_WEBGL && !UNITY_EDITOR

        string temp = VoiceChat.getIds();
        offerString = temp.Split(',');
        for (int i = 0; i < maxNumOfPlayers; i++)
        {
            Debug.Log(offerString[i]);
        }
        
#endif
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
            Timer.Instance.StartTimer(30f);
        }
        if (scene.buildIndex == 1)
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

        if (PhotonNetwork.IsMasterClient)
        {
            if (supplies != null)
            {
                PhotonNetwork.Destroy(supplies);
                supplies = null;
            }
            supplies = PhotonNetwork.Instantiate("Supplies", new Vector3(suppliesX, 0, suppliesZ), Quaternion.identity);

            Timer.Instance.StartTimer(90f);
            view.RPC("RPC_StartRound", RpcTarget.All);
        }
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
    }

    public void AttackersWon()
    {
        if (Timer.Instance.GetTimeRemaining() < 85)
        {
            view.RPC("RPC_EndRoundAndUpdateScores", RpcTarget.All, 1);
        }
    }

    [PunRPC]
    void RPC_StartRound()
    {
        int aux = bluePlayers;
        bluePlayers = redPlayers;
        redPlayers = bluePlayers;
        aliveBlue = bluePlayers;
        aliveRed = redPlayers;
        roundRunning = true;
        aux = scoreBlue;
        scoreBlue = scoreRed;
        scoreRed = aux;
        playerManager.GetComponent<PlayerManager>().SwapTeams();
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
        if (PhotonNetwork.IsMasterClient)
        {
            Timer.Instance.StopTimer();
            StartRound();
        }
    }

    [PunRPC]
    void RPC_SetWarmupEnded(bool value)
    {
        warmupEnded = value;
    }

    public void suppliesPicked()
    {
        view.RPC("RPC_suppliesPicked", RpcTarget.MasterClient);
    }

    [PunRPC]
    public void RPC_suppliesPicked()
    {
        AttackersWon();
    }

    public void StartVoiceChat()
    {

        Debug.Log("startLocal");
        if (PhotonNetwork.IsMasterClient)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            view.RPC("RPC_StartVoiceChat", RpcTarget.All);
#endif
        }
    }

    [PunRPC]
    void RPC_StartVoiceChat()
    {
        Debug.Log("receivedStart");
#if UNITY_WEBGL && !UNITY_EDITOR
        Debug.Log("startRemote");
        view.RPC("RPC_SendOfferStrings", RpcTarget.Others, index, offerString);
#endif
    }

    [PunRPC]
    void RPC_SendOfferStrings(int fromIndex, string[] offer)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        if(index > fromIndex)
        {
            Debug.Log("ans");
            Debug.Log(offer[index]);
            Debug.Log(index);
            VoiceChat.makeCall(fromIndex, offer[index]);
        }
#endif
    }
}
