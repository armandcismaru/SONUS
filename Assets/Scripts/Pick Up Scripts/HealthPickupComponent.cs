using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class HealthPickupComponent : PickUpComponent, IDamageObserver
{
    private float current_health;
    [SerializeField] private float max_health;
    [SerializeField] private float min_health;
    [SerializeField] private float start_health;

    private PlayerController playerController;
    private PlayerManager playerManager;

    public GameObject HealthUI;

    private void Start()
    {
        current_health = start_health;
    }

    private void Awake()
    {
       var playerController = GetComponent<PlayerController>();
        playerController.addObserver<IDamageObserver>(this);
    }

    /*void Start()
    {
        GameObject healthBar = Instantiate(HealthUI, transform.position, transform.rotation) as GameObject;
        healthBar.transform.SetParent(GameObject.FindGameObjectsWithTag("Health").transform, false);
    }*/

    public override List<GameObject> GetUIElements()
    {
        var elements = base.GetUIElements();
        base.setSlider(5, "Health", current_health / max_health);

        return elements;
    }


    /*public GameObject GetHealthUI()
    {
        Instantiate(HealthUI, HealthUI.transform.position, HealthUI.transform.rotation);
        var element = HealthUI;
        base.setSlider(5, "Health", 10);

        return element;
    }*/

    private void incrementHealth(float value)
    {
        if (!GetComponent<PhotonView>().IsMine)
            return;

        if (PhotonNetwork.IsMasterClient)
        {
            //Debug.Log("Before increment health from Master Client.");
            rpcIncrementHealth(value);
            //Debug.Log("After increment health from Master.");
        }
        else
        {
            //Debug.Log("Before incremente health from Client.");
            GetComponent<PhotonView>().RPC("rpcIncrementHealth", RpcTarget.MasterClient, value);
            //Debug.Log("After increment health from Client.");
        }
    }

    [PunRPC]
    private void rpcIncrementHealth(float value)
    {
        GetComponent<PhotonView>().RPC("replicateIncrementHealth", RpcTarget.AllViaServer, value); 
    }

    [PunRPC]
    private void replicateIncrementHealth(float value)
    {
        current_health = Mathf.Clamp(current_health + value, min_health, max_health);
        if (GetComponent<PhotonView>().IsMine)
        {
            base.setSlider(5, "Health", current_health / max_health);
        }
    }
    public void decrementHealth(float value)
    {
        current_health = Mathf.Clamp(current_health - value, min_health, max_health);
        if (GetComponent<PhotonView>().IsMine)
        {
            base.setSlider(5, "Health", current_health / max_health);
        }
    }

    /*public void TakeDamage(int damage)
    {
        GetComponent<PhotonView>().RPC("rpcGetDamage", RpcTarget.All, damage);
        //view.RPC("rpcGetDamage", RpcTarget.All, damage);
    }

    [PunRPC]
    void rpcGetDamage(int damage)
    {
        if (!GetComponent<PhotonView>().IsMine)
            return;
        *//*if (!view.IsMine)
            return;*//*

        decrementHealth((float)damage);
    }*/


    public void Notify()
    {

    }

    public void Notify(int damage)
    { 
        if (PhotonNetwork.IsMasterClient)
        {
            //Debug.Log("Before Get Damage, Master");
            rpcGetDamage(5);
        } else
        {
           // Debug.Log("Before Get Damage, Client");
            GetComponent<PhotonView>().RPC("rpcGetDamage", RpcTarget.MasterClient, 5);
        }
    }


    [PunRPC]
    void rpcGetDamage(int damage)
    {
        //Debug.Log("Before Decrement Health, Master");
        decrementHealth((float)5);
        GetComponent<PhotonView>().RPC("rpcGetDamageAll", RpcTarget.Others, 5);
        //Debug.Log("Current health value: ");
        //Debug.Log(current_health);
        if (current_health <= 0 && PhotonNetwork.IsMasterClient)
        {
            //Debug.Log("Before Player Dies, Master");
           // playerController.Die();
            playerManager.Die();
            //Debug.Log("After Player Dies, Master");
        } else
        {
            GetComponent<PhotonView>().RPC("RPCCallDie", RpcTarget.MasterClient);
        }
    }

    [PunRPC]
    void RPCCallDie()
    {
        playerManager.Die();
    }

    [PunRPC]
    void rpcGetDamageAll(int damage)
    {
        //Debug.Log("Before Decrement Health, Client");
        decrementHealth((float)5);
        //Debug.Log("Current health value: ");
        //Debug.Log(current_health);
        /*if (current_health <= 0 && GetComponent<PhotonView>().IsMine)
        {
            //Debug.Log("Before Player Dies, Client");
           // playerController.Die();
            playerManager.Die();
           // Debug.Log("After Player Dies, Client");
        }*/
    }

    public override void pickupTrigger(PickUpScript pickup)
    {
        if (pickup != null)
        {
            // Check if it s a health component 
            if (pickup.pickupType == PickUpScript.PickUpType.Health)
            {
                //Go through each uiElement form the list of PickUpComponents where these prefabs are declared.

                incrementHealth(5f);
                pickup.destroyThisObject();
                //Destroy(pickup.gameObject);
            }
        }
    }
}