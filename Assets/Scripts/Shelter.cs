using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shelter : MonoBehaviour, IRoundFinished
{
    [SerializeField] private int neededAmountofBoxes;
    private int currentAmountofBoxes;

    void Awake()
    {
        currentAmountofBoxes = 0;
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject != null) 
        { 
            if (collision.gameObject.tag == "Player" && collision.gameObject.GetComponent<PlayerController>().team == 1)
            {
                SupplyPickupComponent supply = collision.gameObject.GetComponent<SupplyPickupComponent>();
                currentAmountofBoxes += (int)supply.current_food / supply.supplyCharge;
                supply.current_food = 0;
                if (neededAmountofBoxes == currentAmountofBoxes)
                {
                    RoomManager.Instance.AttackersWon();
                }
            }
        }
    }
}
