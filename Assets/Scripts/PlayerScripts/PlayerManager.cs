using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerManager : MonoBehaviour
{
    [SerializeField] private GameObject RedPlayerPrefab;
    [SerializeField] private GameObject BluePlayerPrefab;



    private GameObject myAvatar = null;
    PhotonView view;

    public float minX;
    public float maxX;
    public float minZ;
    public float maxZ;

    public int team = -1;

    private void Awake()
    {
        view = GetComponent<PhotonView>();
    }
    void Start()
    {
        if (view.IsMine)
        {
            view.RPC("RPC_GetTeam", RpcTarget.MasterClient);
            /*Vector3 randomPosition = new Vector3(Random.Range(minX, maxX), 3, Random.Range(minZ, maxZ));
            GameObject obj = PhotonNetwork.Instantiate(playerPrefab.name, randomPosition, Quaternion.identity);
            obj.GetComponent<PlayerController>().team = team;*/
        }
    }

    void FixedUpdate()
    {
        if (view.IsMine)
        {
            if (myAvatar == null && team != -1)
            {
                Vector3 randomPosition = new Vector3(Random.Range(minX, maxX), 3, Random.Range(minZ, maxZ));
                if (team == 0)
                {
                    myAvatar = PhotonNetwork.Instantiate(BluePlayerPrefab.name, randomPosition, Quaternion.identity);
                    myAvatar.GetComponent<PlayerController>().SetTeamAndUpdateMaterials(team);
                }
                else
                {
                    myAvatar = PhotonNetwork.Instantiate(RedPlayerPrefab.name, randomPosition, Quaternion.identity);
                    myAvatar.GetComponent<PlayerController>().SetTeamAndUpdateMaterials(team);
                }
            }
        }
    }
    [PunRPC]
    void RPC_GetTeam()
    {
        team = RoomManager.Instance.currentTeam % 2;
        if (myAvatar != null)
        {
            myAvatar.GetComponent<PlayerController>().SetTeamAndUpdateMaterials(team);
        }
        RoomManager.Instance.UpdateTeam();
        view.RPC("RPC_SentTeam", RpcTarget.Others, team);
    }

    [PunRPC]
    void RPC_SentTeam(int tm)
    {
        team = tm;
        if (myAvatar != null)
        {
            myAvatar.GetComponent<PlayerController>().SetTeamAndUpdateMaterials(team);
        }
    }
}
