using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SupplyPickupComponent : PickUpComponent, IDieObserver
{
    public float current_food = 0;

    [Tooltip("Only one supply can be picked by an attacker. Leave value as 1 by default.")]
    [ReadOnly]
    [SerializeField] private float max_food = 1;
    [SerializeField] private float min_food;

    [SerializeField] private GameObject prefabType;

    [SerializeField] public int supplyCharge;

    private PhotonView view;

    private void Awake()
    {
        view = GetComponent<PhotonView>();
    }

    private void Start()
    {
        var playerController = GetComponent<PlayerController>();
        playerController.addObserver<IDieObserver>(this);

        supplyCharge = prefabType.GetComponent<PickUpScript>().amount;
    }

    public void Notify()
    {
        if (view.IsMine)
             DropSupplies();
    }

    public void DropSupplies()
    {
        PickUpScript pickup = prefabType.GetComponent<PickUpScript>();
        if (pickup != null && pickup.pickupType == PickUpScript.PickUpType.Food)
        {
            for (int i = 0; i < current_food / supplyCharge; i++)
            {
                
                GameObject supply =  PhotonNetwork.Instantiate(prefabType.name, gameObject.transform.position, Quaternion.identity);
                RoomManager.collectables.Add(supply);
            }
        } else
        {
            throw new System.Exception("Incorrect prefab type");
        }
    }

    public override void updateUI()
    {
        if (view.IsMine)
            base.setSlider(5, "Food", current_food / (max_food * supplyCharge));
    }

    private async void incrementFood(float value)
    {
        if (!view.IsMine)
            return;

        replicateIncrementFood(value);
    }
    
    private void replicateIncrementFood(float value)
    {
        current_food = Mathf.Clamp(current_food + value, min_food, max_food * supplyCharge);
        if (view.IsMine)
            updateUI(); 
    }
    
    public override void pickupTrigger(PickUpScript pickup)
    {
        if (!view.IsMine)
            return;

        if (gameObject.GetComponent<PlayerController>().team == 1)
        {
            if (pickup.pickupType == PickUpScript.PickUpType.Food && current_food < max_food * supplyCharge)
            {
                float value = pickup.amount;
                incrementFood(value);
                pickup.destroyThisObject();
            }
        }
    } 
}

