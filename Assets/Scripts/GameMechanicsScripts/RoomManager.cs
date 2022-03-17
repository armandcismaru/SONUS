using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.Collections;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager Instance;
    [HideInInspector] public int currentTeam = 1;
    private PhotonView view;

    public GameObject playerManager;

    [HideInInspector] public bool warmupEnded = false;
    private int bluePlayers = 1;
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
    private GameObject healthBox;
    private GameObject healthBox1;
    private GameObject healthBox2;
    private GameObject healthBox3;
    private GameObject healthBox4;
    private GameObject healthBox5;

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
                view.RPC("RPC_KillAndDisplay", RpcTarget.All, team);
                aliveBlue--;
            }
            else
            {
                view.RPC("RPC_KillAndDisplay", RpcTarget.All, team);
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
            supplies = PhotonNetwork.Instantiate("Supplies", new Vector3(suppliesX, 24, suppliesZ), Quaternion.identity);
            
            //Defenders' Spot
            healthBox = PhotonNetwork.Instantiate("HealthBox", new Vector3(- 8, 24, 8), Quaternion.identity);
            healthBox1 = PhotonNetwork.Instantiate("HealthBox", new Vector3(- 10, 24, 15), Quaternion.identity);
            healthBox2 = PhotonNetwork.Instantiate("HealthBox", new Vector3(-15, 26, -20), Quaternion.identity);

            //Attackers' Spot
            //healthBox3 = PhotonNetwork.Instantiate("HealthBox", new Vector3(-40, 24, -53), Quaternion.identity);
            healthBox4 = PhotonNetwork.Instantiate("HealthBox", new Vector3(-44, 25, -48), Quaternion.identity);
            healthBox5 =  PhotonNetwork.Instantiate("HealthBox", new Vector3(-42, 26, -55), Quaternion.identity);


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

    public void StartVoiceChat()
    {

        //Debug.Log("startLocal");
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
        //Debug.Log("receivedStart");
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

    [PunRPC]
    void RPC_KillAndDisplay(int team)
    {
        DisplayKill(team);
    }

    private void DisplayKill(int team)
    {
        DisplayMessage kill = GameObject.FindWithTag("Kill").GetComponent<DisplayMessage>();

        if (team == 0)
        {
            kill.SetText("");
            kill.SetText("              Defender died");
            kill.SetColour("blue");
        }
        else
        {
            kill.SetText("");
            kill.SetText("              Attacker died");
            kill.SetColour("red");
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
        redPlayers = aux;

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

            if (supplies != null) PhotonNetwork.Destroy(supplies);
            if (healthBox != null) PhotonNetwork.Destroy(healthBox);
            if (healthBox1 != null) PhotonNetwork.Destroy(healthBox1);
            if (healthBox2 != null) PhotonNetwork.Destroy(healthBox2);
            if (healthBox4 != null) PhotonNetwork.Destroy(healthBox4);
            if (healthBox5 != null) PhotonNetwork.Destroy(healthBox5);


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
