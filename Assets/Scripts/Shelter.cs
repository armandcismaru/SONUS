using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Shelter : MonoBehaviour, IRoundFinished
{
    [SerializeField] private int neededAmountofBoxes;
    private int currentAmountofBoxes;

    private PhotonView view;


    void Awake()
    {
        currentAmountofBoxes = 0;
        view = GetComponent<PhotonView>();
    }

    // Start is called before the first frame update
    void Start()
    {
        RoomManager.Instance.addObserver<IRoundFinished>(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Notify()
    {
            currentAmountofBoxes = 0;
    }

    public void RoundFinishedAttackersWinningByTakingSuppliesToShelter()
    {

        if (PhotonNetwork.IsMasterClient)
        {
            RPC_SuppliesTakenToShelter();
        }
        else
        {
            view.RPC("RPC_SuppliesTakenToShelter", RpcTarget.MasterClient);
        }
    }

    [PunRPC]
    private void RPC_SuppliesTakenToShelter()
    {
        RoomManager.Instance.AttackersWon();
    }

    private void RPC_replicateAmountOfFoodDelivered(int value)
    {
        view.RPC("replicateAmountOfFoodDelievered", RpcTarget.All, value);
    }

    [PunRPC]
    private void replicateAmountOfFoodDelievered(int value)
    {
        currentAmountofBoxes += value;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject != null 
            && collision.gameObject.tag == "Player" 
            && collision.gameObject.GetComponent<PhotonView>().IsMine
            && collision.gameObject.GetComponent<PlayerController>().team == 1) 
        { 
            
            SupplyPickupComponent supply = collision.gameObject.GetComponent<SupplyPickupComponent>();
            RPC_replicateAmountOfFoodDelivered((int)supply.current_food / supply.supplyCharge);
            supply.current_food = 0;
            supply.dropped = true;
            supply.updateUI();
            if (neededAmountofBoxes == currentAmountofBoxes)
            {
                RoundFinishedAttackersWinningByTakingSuppliesToShelter();
            }
        }
    }
}
