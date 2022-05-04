using Photon.Pun;
using System.Collections;
using UnityEngine;
using System;
using TMPro;

public class DoorOpener : MonoBehaviour
{
    [SerializeField] public float TheDistance;
    [SerializeField] public DisplayMessage Instructions;
    public GameObject TheDoor;
    public AudioSource CreakSound;

    private PhotonView view;
   
    DoorMutex dm;

    PlayerManager playerManager;

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

        clip1.AddEvent(evt);
        clip2.AddEvent(evt);

        view = TheDoor.GetComponent<PhotonView>();

        playerManager = RoomManager.Instance.playerManager.GetComponent<PlayerManager>();
    }


    // Update is called once per frame
    void Update()
    {
        if (playerManager.getAvatar() != null)
             TheDistance = Math.Abs(Vector3.Magnitude(playerManager.getAvatar().transform.position - gameObject.transform.position));
    }

     private IEnumerator DisplayInstructions()
     {
         Instructions.MakeVisible(true);
         Instructions.SetText("Press 'R' to interact with the door");
         yield return new WaitForSeconds(2);
         Instructions.MakeVisible(false);
     }


    void OnMouseOver()
    {
        if (TheDistance <= 7)
        {
            StartCoroutine(DisplayInstructions());
            if (dm.lock_access_pivot && Input.GetButtonDown("Action"))
            {

                dm.AquireAccessPivot(false);
                view.RPC("AquireAccessPivot", RpcTarget.Others, false);

                view.TransferOwnership(PhotonNetwork.LocalPlayer);

                OpenDoorAndDisableCollider();
            }
        }
    }

    private void OpenDoorAndDisableCollider()
    {   
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
