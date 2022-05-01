using Photon.Pun;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

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

    public void Update()
    {
        Manualsupplies();
    }

    public void DropSupplies()
    {
        PickUpScript pickup = prefabType.GetComponent<PickUpScript>();
        if (pickup != null && pickup.pickupType == PickUpScript.PickUpType.Food)
        {
            CreateFoodBox((int)current_food / supplyCharge);
        } else
        {
            throw new System.Exception("Incorrect prefab type");
        }
    }

    public void CreateFoodBox(int amount)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            view.RPC("RPC_CreateFoodBox", RpcTarget.MasterClient, amount);
        } else
        {
            RPC_CreateFoodBox(amount);
        }
    }

    [PunRPC]
    public void RPC_CreateFoodBox(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
           GameObject supply = PhotonNetwork.Instantiate(prefabType.name, gameObject.transform.position, Quaternion.identity);
           RoomManager.Instance.AddCollectable(supply);
        }
    }

    public override void updateUI()
    {
        if (view.IsMine)
            base.SetSlider(5, "Food", current_food / (max_food * supplyCharge));
    }

    // ------------------------------------
    private void Manualsupplies()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            IncrementFood(1);
        }
    }

    private void IncrementFood(float value)
    {
        if (!view.IsMine)
            return;

        current_food = Mathf.Clamp(current_food + value, min_food, max_food * supplyCharge);
        if (view.IsMine)
        {
            foreach (GameObject uiElement in instancesUIElements)
                if (uiElement.tag == "Food")
                {
                    Image image = uiElement.GetComponentInChildren<Image>();
                    TMP_Text text = uiElement.GetComponentInChildren<TMP_Text>();
                    text.text = "Supplies collected";
                    image.color = new Color32(255, 255, 255, 255);
                }
            //base.SetSlider(5, "Food", current_food / (max_food * supplyCharge));
        }
        //replicateIncrementFood(value);
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


                IncrementFood(value);
                pickup.destroyThisObject();
                GetComponent<PlayerController>().PickUpSupplySound();
            }
        }
    } 
}

