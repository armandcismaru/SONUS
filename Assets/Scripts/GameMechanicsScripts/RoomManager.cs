using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class RoomManager : MonoBehaviourPunCallbacks
{
    //bool ok = true;
    [SerializeField] GameObject endGameSound;
    [SerializeField] GameObject inteseMusicGameObject;
    public static RoomManager Instance;
    private int currentTeam = 1;
    private PhotonView view;

    public GameObject playerManager = null;

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
    private GameObject supplies2;
    private GameObject supplies3;

    private GameObject shelter;

    private GameObject door;

    private GameObject healthBox;
    private GameObject healthBox1;
    private GameObject healthBox2;
    private GameObject healthBox3;
    private GameObject healthBox4;
    private GameObject healthBox5;

    private GameObject bullet;
    private GameObject bullet2;
    private GameObject bullet3;
    private GameObject bullet4;
    private GameObject bullet5;
    private GameObject bullet6;


    //private GameObject Door;

    const int maxNumOfPlayers = 6;
    private string[] offerString = new string[maxNumOfPlayers];
    [HideInInspector] public int index = 0;

    public static List<GameObject> collectables;

    private Dictionary<string, List<IObserver>> observers = new Dictionary<string, List<IObserver>>();

    private Shelter shelterClass;
    DisplayMessage kill;
    DisplayMessage canvasMessage;

    private float voiceChatVolume = 1f;

    public float mouseSpeed = 3f;
    public bool intenseMusicPlayed = false;
    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        // DontDestroyOnLoad(gameObject);
        Instance = this;
        view = GetComponent<PhotonView>();
#if UNITY_WEBGL && !UNITY_EDITOR

        string temp = VoiceChat.getIds();
        offerString = temp.Split(',');
       /* for (int i = 0; i < maxNumOfPlayers; i++)
        {
            Debug.Log(offerString[i]);
        }*/

#endif
    }

    private void Start()
    {
       kill = GameObject.FindWithTag("Kill").GetComponent<DisplayMessage>();
       canvasMessage = GameObject.FindWithTag("Canvas").GetComponent<DisplayMessage>();
    }

    private void Update()
    {
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
            Timer.Instance.StartTimer(30f); ///TODO 30f
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

    public int getCurrentTeam()
    {
        return currentTeam;
    }

    public float getVoiceChatVolume()
    {
        return voiceChatVolume;
    }

    public void setVoiceChatVolume(float volume)
    {
        voiceChatVolume = volume;
#if UNITY_WEBGL && !UNITY_EDITOR
        for(int i = 0; i < 6; i++){
            VoiceChat.setPlayerVolume(i, voiceChatVolume);
        }
#endif

    }

    public float getMouseSpeed()
    {
        return mouseSpeed;
    }

    public void setMouseSpeed(float volume)
    {
        mouseSpeed = volume;
        if (playerManager.GetComponent<PlayerManager>().getAvatar() != null)
        {
            if (playerManager.GetComponent<PlayerManager>().getAvatar().GetComponent<MouseController>() != null)
                playerManager.GetComponent<PlayerManager>().getAvatar().GetComponent<MouseController>().mouseSpeed = mouseSpeed;
        }
    }

    private void FixedUpdate()
    {
        if (!intenseMusicPlayed)
        {
            view.RPC("RPC_StopIntenseMusic", RpcTarget.All);
        }
        if (Timer.Instance.GetTimeRemaining() <= 31 && !intenseMusicPlayed)
        {
            intenseMusicPlayed = true;
            view.RPC("RPC_PlayIntenseMusic", RpcTarget.All);
        }

        if (!roundRunning && warmupEnded && PhotonNetwork.IsMasterClient)
        {
            StartRound();
        }

        //keep mouse sens during warmup
        if (!warmupEnded && playerManager != null && playerManager.GetComponent<PlayerManager>().getAvatar() != null)
        {
            setMouseSpeed(mouseSpeed);
        }

        //get the positions of all players relative to our player and send them to their respective js panner
#if UNITY_WEBGL && !UNITY_EDITOR
        if(playerManager != null)
        {
            GameObject avatar = playerManager.GetComponent<PlayerManager>().getAvatar();
            if(avatar != null)
            {
                Vector3 myPos = avatar.transform.position;
                Vector3 myOr = avatar.transform.forward;

                VoiceChat.setMyOrientation(-myOr.x, myOr.y, -myOr.z);
                GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
                //Debug.Log(players.Length);
                foreach(GameObject player in players)
                {
                    int playerInd = player.GetComponent<PlayerController>().index;
                    if (playerInd != index && playerInd != -1)
                    {
                        //Debug.Log(playerInd);
                        Vector3 playerPos = player.transform.position;
                        VoiceChat.setPosition(playerInd, playerPos.x - myPos.x, playerPos.y - myPos.y, playerPos.z - myPos.z);
                    }
                }
            }
        }

#endif
    }

    public void PlayerDied(int team, string name)
    {
        if (roundRunning)
        {
            if (team == 0)
            {
                view.RPC("RPC_KillAndDisplay", RpcTarget.All, team, name);
                aliveBlue--;
            }
            else
            {
                view.RPC("RPC_KillAndDisplay", RpcTarget.All, team, name);
                aliveRed--;
            }
            if (aliveBlue == 0)
            {
                if (scoreBlue + scoreRed + 1 == 4)
                {
                    view.RPC("RPC_CreateStateOutro", RpcTarget.All, scoreRed + 1, scoreBlue);
                    PhotonNetwork.LoadLevel(3);
                }
                else
                {
                    AttackersWon();
                }
            }
            else if (aliveRed == 0)
            {
                if (scoreBlue + scoreRed + 1 == 4)
                {
                    view.RPC("RPC_CreateStateOutro", RpcTarget.All, scoreRed, scoreBlue + 1);
                    PhotonNetwork.LoadLevel(3);
                }
                else
                {
                    DefendersWon();
                }
            }

        }
    }

    private void StartRound()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            supplies = PhotonNetwork.Instantiate("Supplies_Roasted_Pig", new Vector3(-29.59f, 21f, 45.9f), Quaternion.identity);
            //supplies2 = PhotonNetwork.Instantiate("Supplies_Chicken", new Vector3(suppliesX - 4, 24, suppliesZ + 4), Quaternion.identity);
            supplies3 = PhotonNetwork.Instantiate("Supplies_Roasted_Pig", new Vector3(5.2f, 24f, -14.9f), Quaternion.identity);

            //shelter = PhotonNetwork.Instantiate("Shelter", new Vector3(suppliesX - 4, 23, suppliesZ - 4), Quaternion.identity);
            shelter = PhotonNetwork.Instantiate("Shelter", new Vector3(-60.0999985f, 23f, -34.2000008f), Quaternion.identity);
            //door = PhotonNetwork.Instantiate("Door", new Vector3(-36, 23, -70), Quaternion.identity);

            healthBox = PhotonNetwork.Instantiate("HealthBox", new Vector3(-45.79f, 24.38f, -27.20f), Quaternion.identity);
            healthBox1 = PhotonNetwork.Instantiate("HealthBox", new Vector3(12.8999996f, 24.3877335f, 0.400000006f), Quaternion.identity);
            healthBox2 = PhotonNetwork.Instantiate("HealthBox", new Vector3(-49.2999992f, 24.8999996f, -19.2000008f), Quaternion.identity);

            healthBox3 = PhotonNetwork.Instantiate("HealthBox", new Vector3(-20.3700008f, 23.7f, -5.80000019f), Quaternion.identity);
            healthBox4 = PhotonNetwork.Instantiate("HealthBox", new Vector3(23.8999996f, 24.8999996f, -22.1900005f), Quaternion.identity);
            healthBox5 =  PhotonNetwork.Instantiate("HealthBox", new Vector3(-44f, 21.2999992f, 46.2999992f), Quaternion.identity);


            bullet = PhotonNetwork.Instantiate("Bullet", new Vector3(-40, 23, -70), Quaternion.identity);
            bullet2 = PhotonNetwork.Instantiate("Bullet", new Vector3(-45, 23, -80), Quaternion.identity);
            bullet3 = PhotonNetwork.Instantiate("Bullet", new Vector3(-40, 23, -70), Quaternion.identity);

            bullet4 = PhotonNetwork.Instantiate("Bullet", new Vector3(-8, 23, -10), Quaternion.identity);
            bullet5 = PhotonNetwork.Instantiate("Bullet", new Vector3(-8, 23, 20), Quaternion.identity);
            bullet6 = PhotonNetwork.Instantiate("Bullet", new Vector3(-10, 23, 17), Quaternion.identity);

            collectables = new List<GameObject>() {supplies, supplies2, supplies3, shelter, healthBox, healthBox1, healthBox2, healthBox4, 
                                                   healthBox5, bullet, bullet2, bullet3, bullet4, bullet5, bullet6};

            Timer.Instance.StartTimer(135f);
            view.RPC("RPC_StartRound", RpcTarget.All);
        }
    }

    public void TimerFinished()
    {
        if (roundRunning)
        {
            if (scoreBlue + scoreRed + 1 == 4)
            {
                view.RPC("RPC_CreateStateOutro", RpcTarget.All, scoreRed, scoreBlue + 1);
                PhotonNetwork.LoadLevel(3);
            }
            else
            {
                DefendersWon();
            }
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
        canvasMessage.MakeVisible(true);
        canvasMessage.SetText(msg);
        canvasMessage.SetColour(team);

        Time.timeScale = 0f;
        float pauseEndTime = Time.realtimeSinceStartup + pauseTime;
        while (Time.realtimeSinceStartup < pauseEndTime)
        {
            yield return 0;
        }

        canvasMessage.MakeVisible(false);
        Time.timeScale = 1f;
        
    }

    public void DefendersWon()
    {
        
        view.RPC("RPC_PauseAndDisplay", RpcTarget.All, "Campers won!", "blue");
        view.RPC("RPC_EndRoundAndUpdateScores", RpcTarget.All, 0);
    }

    public void AttackersWon()
    {
        view.RPC("RPC_PauseAndDisplay", RpcTarget.All, "Scavengers won!", "red");

        if (Timer.Instance.GetTimeRemaining() < 85)
        {
            view.RPC("RPC_EndRoundAndUpdateScores", RpcTarget.All, 1);
        }
    }

    public void StartVoiceChat()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        if (PhotonNetwork.IsMasterClient)
        {
            view.RPC("RPC_StartVoiceChat", RpcTarget.All);
        }
#endif
}

    [PunRPC]
    void RPC_CreateStateOutro(int scoreRed, int scoreBlue)
    {
        StateOutro.team = playerManager.GetComponent<PlayerManager>().team;
        StateOutro.attackers = scoreRed;
        StateOutro.defenders = scoreBlue;
    }
    [PunRPC]
    void RPC_StartVoiceChat()
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        view.RPC("RPC_SendOfferStrings", RpcTarget.Others, index, offerString);
#endif
    }

    [PunRPC]
    void RPC_SendOfferStrings(int fromIndex, string[] offer)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        if(index > fromIndex)
        {
            VoiceChat.makeCall(fromIndex, offer[index]);
        }
#endif
    }

    [PunRPC]
    void RPC_KillAndDisplay(int team, string name)
    {
        DisplayKill(team, name);
    }


    private IEnumerator DisplayKillAndFade(int team, DisplayMessage kill, string name)
    {
        if (team == 0)
        {
            kill.MakeVisible(true);
            kill.SetText(name + " died");
            kill.SetColour("blue");
        }
        else
        {
            kill.MakeVisible(true);
            kill.SetText(name + " died");
            kill.SetColour("red");
        }
        yield return new WaitForSeconds(2);
        kill.MakeVisible(false);
    }
    private void DisplayKill(int team, string name)
    {
        StartCoroutine(DisplayKillAndFade(team, kill, name));
    }

    [PunRPC]
    void RPC_PauseAndDisplay(string msg, string team)
    {
        endGameSound.GetComponent<AudioSource>().Play();
        if (intenseMusicPlayed)
        {
            inteseMusicGameObject.GetComponent<AudioSource>().Stop();
            intenseMusicPlayed = false;
        }
        StartPause(msg, team);
    }

    [PunRPC]
    void RPC_PlayIntenseMusic()
    {
        inteseMusicGameObject.GetComponent<AudioSource>().Play();
    }

    [PunRPC]
    void RPC_StopIntenseMusic()
    {
        inteseMusicGameObject.GetComponent<AudioSource>().Stop();
    }

    [PunRPC]
    void RPC_StartRound()
    {
        intenseMusicPlayed = false;
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
        playerManager.GetComponent<PlayerManager>().DestroyMyAvatar();
        playerManager.GetComponent<PlayerManager>().StartRound();


#if UNITY_WEBGL && !UNITY_EDITOR
        for(int i = 0; i < 6; i++)
        {
        //unmutes player if he exists otherwise does nothing
            VoiceChat.setPlayerVolume(i, voiceChatVolume);
        }
#endif
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

            foreach (GameObject collectable in collectables)
            {
                if (collectable != null)
                {
                    PhotonNetwork.Destroy(collectable);
                }
            }

            StartRound();
        }
    }

    public void AddCollectable(GameObject gameObject)
    {
        if (!PhotonNetwork.IsMasterClient)
            view.RPC("RPC_AddCollectables", RpcTarget.MasterClient, gameObject);
        else
        {
            collectables.Add(gameObject);
        }
    }

    [PunRPC]
    public void RPC_AddCollectables(GameObject gameObject)
    {
        collectables.Add(gameObject);
    }

    public void RemoveCollectable(GameObject gameObject)
    {
        if (!PhotonNetwork.IsMasterClient)
               view.RPC("RPC_RemoveCollectable", RpcTarget.MasterClient, gameObject);
        else
        {
            collectables.Remove(gameObject);
        }
    }

    [PunRPC]
    public void RPC_RemoveCollectable(GameObject gameObject)
    {
        collectables.Remove(gameObject);
    }


    [PunRPC]
    void RPC_SetWarmupEnded(bool value)
    {
        warmupEnded = value;
    }

    /*public void suppliesPicked()
    {
        view.RPC("RPC_suppliesPicked", RpcTarget.MasterClient);
    }

    [PunRPC]
    public void RPC_suppliesPicked()
    {
        AttackersWon();
    }*/

    public void suppliesPicked()
    {
        view.RPC("RPC_suppliesPicked", RpcTarget.MasterClient);
    }

    [PunRPC]
    public void RPC_suppliesPicked()
    {
        shelterClass.RoundFinishedAttackersWinningByTakingSuppliesToShelter();
    }

    public void SuppliesTakenSuccesfullyToShelter()
    {
        view.RPC("RPC_AttackersWonByTakingSuppliesToShelter", RpcTarget.All);
    }

    [PunRPC]
    public void RPC_AttackersWonByTakingSuppliesToShelter()
    {
        foreach (string subscriberType in observers.Keys)
        {
            if (subscriberType.Equals("IRoundFinished"))
            {
                foreach (IObserver subscriber in observers[subscriberType])
                {
                    (subscriber as IRoundFinished).Notify();
                }
            }
        }
    }

    public void addObserver<T>(IObserver observer)
    {
        // If the key element exists in observers.keys
        foreach (string observerType in observers.Keys)
        {
            if (typeof(T).Name == observerType)
            {
                observers[typeof(T).Name].Add(observer);
                return;
            }
        }
        observers.Add(typeof(T).Name, new List<IObserver>());
        observers[typeof(T).Name].Add(observer);
    }

    public void unsubscribeObserver<T>(IObserver observer)
    {
        foreach (string subscriberType in observers.Keys)
        {
            if (typeof(T).Equals(subscriberType))
            {
                observers[subscriberType].Remove(observer);
            }
        }
    }
}
