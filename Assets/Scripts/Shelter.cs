using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/* Class for making the round finish if food supplies are taken 
 * successully to the shelter
 */
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
        /* Add This class as an IRoundFinished observer 
         * to RoomManager to bridge the logic between taking the supplies to shelter and finishing the round
         * with attackers winning it
         */
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

    /* Replicate logic for delievering the supplies and making the round finish
     * using RPC calls
     */
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

    /* Delievering the food items by colliding with the shelter
     * Call "AttackersWinning" if the items delievered coincide with the 
     * needed amount of boxes
     * All logic - replicated accross the browser
     */
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
