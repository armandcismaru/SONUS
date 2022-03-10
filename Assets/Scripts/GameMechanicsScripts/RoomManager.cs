using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.Collections;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager Instance;
    [HideInInspector] public int currentTeam = 0;
    private PhotonView view;

    public GameObject playerManager;

    [HideInInspector]
    public bool warmupEnded = false;
    private int bluePlayers = 0;
    private int redPlayers = 0;

    private int aliveBlue = 0;
    private int aliveRed = 0;

    [HideInInspector]
    public int scoreBlue = 0;
    [HideInInspector]
    public int scoreRed = 0;

    private bool roundRunning = false;
    private float suppliesX;
    private float suppliesZ;

    private GameObject supplies;

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

    public void StartPause(string msg, string team)
    {
        StartCoroutine(PauseGame(5f, msg, team));
    }

    public IEnumerator PauseGame(float pauseTime, string msg, string team)
    {
        Debug.Log("Inside PauseGame()");

        GameObject canvas = GameObject.FindWithTag("Canvas");
        canvas.GetComponent<DisplayMessage>().SetText(msg);
        canvas.GetComponent<DisplayMessage>().SetColour(team);

        Time.timeScale = 0f;
        float pauseEndTime = Time.realtimeSinceStartup + pauseTime;
        while (Time.realtimeSinceStartup < pauseEndTime)
        {
            yield return 0;
        }

        canvas.GetComponent<DisplayMessage>().SetText("");
        Time.timeScale = 1f;
        Debug.Log("Done with my pause");
    }

    public void DefendersWon()
    {
        view.RPC("RPC_PauseAndDisplay", RpcTarget.All, "Defenders won!", "blue");
        view.RPC("RPC_EndRoundAndUpdateScores", RpcTarget.All, 0);
    }

    public void AttackersWon()
    {
        view.RPC("RPC_PauseAndDisplay", RpcTarget.All, "Attackers won!", "red");
       
        if (Timer.Instance.GetTimeRemaining() < 85)
        {
            view.RPC("RPC_EndRoundAndUpdateScores", RpcTarget.All, 1);
        }
    }


    [PunRPC]
    void RPC_PauseAndDisplay(string msg, string team)
    {
        StartPause(msg, team);
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
}
