using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Timer : MonoBehaviour
{

    public static Timer Instance;

    private PhotonView view;
    private bool timerIsRunning = false;
    public const float roundTime = 90f;
    private float timeRemaining = roundTime;

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

    void FixedUpdate()
    {
        if (timerIsRunning && PhotonNetwork.IsMasterClient)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.fixedDeltaTime;
                view.RPC("RPC_UpdateTime", RpcTarget.Others, timeRemaining);

            }
            else
            {
                timerIsRunning = false;
                RoomManager.Instance.TimerFinished();
                view.RPC("RPC_TimerFinished", RpcTarget.Others);
            }
        }
    }
    public float GetTimeRemaining()
    {
        return timeRemaining;
    }

    public void StartTimer(float time)
    {
        timeRemaining = time;
        timerIsRunning = true;
        view.RPC("RPC_StartTimer", RpcTarget.Others, time);
    }

    public void StopTimer()
    {
        timerIsRunning = false;
        view.RPC("RPC_StopTimer", RpcTarget.Others);
    }

    public float GetTimerSeconds()
    {
        return (float)System.Math.Floor(timeRemaining % 60);
    }
    public float GetTimerMinutes()
    {
        return (float)System.Math.Floor(timeRemaining/60);
    }

    public bool IsRunning()
    {
        return timerIsRunning;
    }

    [PunRPC]
    void RPC_StartTimer(float time)
    {
        timeRemaining = time;
        timerIsRunning = true;
    }

    [PunRPC]
    void RPC_StopTimer()
    {
        timerIsRunning = false;
    }

    [PunRPC]
    void RPC_UpdateTime(float time)
    {
        timeRemaining = time;
    }

    [PunRPC]
    void RPC_TimerFinished()
    {
        timerIsRunning = false;
    }
}
