using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class HealthPickupComponent : PickUpComponent, IDamageObserver
{
    [SerializeField] private float max_health;
    [SerializeField] private float min_health;
    [SerializeField] private float start_health;
    private float current_health;
    private PlayerManager playerManager;
    private PhotonView view;

    private void Awake()
    {
        view = GetComponent<PhotonView>();
    }

    private void Start()
    {
        var playerController = GetComponent<PlayerController>();
        playerController.addObserver<IDamageObserver>(this);
        current_health = start_health;

        if (view.IsMine)
        {
            playerManager = PhotonView.Find((int)view.InstantiationData[0]).GetComponent<PlayerManager>();
        }
    }

    public override void updateUI()
    {
        base.setSlider(5, "Health", current_health / max_health);
    }

    private async void incrementHealth(float value)
    {
        if (!view.IsMine)
            return;

        replicateIncrementHealth(value);
    }

    private void replicateIncrementHealth(float value)
    {
        current_health = Mathf.Clamp(current_health + value, min_health, max_health);
        if (view.IsMine)
        {
            updateUI();
        }
    }
    public void decrementHealth(float value)
    {
        current_health = Mathf.Clamp(current_health - value, min_health, max_health);
        if (view.IsMine)
        {
           updateUI();
        }
    }

    public void Notify()
    {

    }

    public void Notify(int damage)
    {
        GetDamage(damage);  
    }

    void GetDamage(int damage)
    {
        decrementHealth(damage);

        if (current_health == 0)
        {
            if (playerManager != null && view.IsMine)
            {
                playerManager.Die();
            }
        }
    }

    public override void pickupTrigger(PickUpScript pickup)
    {
        if (pickup != null)
        {
            if (pickup.pickupType == PickUpScript.PickUpType.Health)
            {
                incrementHealth(pickup.amount);
                pickup.destroyThisObject();
            }
        }
    }
}


