using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpener : MonoBehaviour
{
    public float TheDistance;
    public GameObject TheDoor;
    public AudioSource CreakSound;

    private PhotonView view;
    private PhotonView view_DoorTrigger;

    PlayerManager playerManager;

    bool mutex_door = true;

    DoorMutex dm;

    private void Start()
    {
        dm = gameObject.transform.parent.GetComponentInChildren<DoorMutex>();

        AnimationClip clip1;
        AnimationClip clip2;

        AnimationEvent evt = new AnimationEvent();

        evt.time = 1.2f;
        evt.functionName = "Evt_mutex";
        evt.objectReferenceParameter = this;

        clip1 = TheDoor.GetComponent<Animation>().GetClip("DoorAnimation1");
        clip2 = TheDoor.GetComponent<Animation>().GetClip("DoorAnimation1_reverse");

       /* clip1.AddEvent(evt);
        clip2.AddEvent(evt);*/

        playerManager = RoomManager.Instance.playerManager.GetComponent<PlayerManager>();
        view = TheDoor.GetComponent<PhotonView>();
        //view_DoorTrigger = GetComponent<PhotonView>();
    }

   /* public void Mutex()
    {
        mutex_door = true;
       // view_DoorTrigger.RPC("RPC_Mutex", RpcTarget.Others);
    }

    [PunRPC]
    public void RPC_Mutex()
    {
        mutex_door = true;
    }*/

    // Update is called once per frame
    void Update()
    {
        TheDistance = PlayerCasting.DistanceFromTarget;
    }

    void OnMouseOver()
    {
        Debug.Log("Mouse is over the door.");
        
        if (Input.GetButtonDown("Action") && (TheDistance <= 7))
        {
            //mutex_door = false;
            
            
            view.TransferOwnership(PhotonNetwork.LocalPlayer);
           // Debug.Log("Player is near the door and Action bashed.");
            OpenDoorAndDisableCollider();
        }
    }

    private void OpenDoorAndDisableCollider()
    {
        /*Player ownerOfView = view.IsMine ? PhotonNetwork.ViewID : view.owner;
        Debug.Log(PlayerManager.LocalPlayerInstance);*/
        
        Debug.Log("Door Opened Client side.");
        view.RPC("CallDisableCollider", RpcTarget.Others);

        if (!dm.isOpen)
        {
            TheDoor.GetComponent<Animation>().Play("DoorAnimation1");
        } else
        {
            TheDoor.GetComponent<Animation>().Play("DoorAnimation1_reverse");
        }

        dm.CallDisableCollider();

        CreakSound.Play();
    }
}
