using UnityEngine;
using Photon.Pun;

/* Script attached to the player
 * Governs the logic for collecting health packs 
 * Communicates with player controller for knowing when the player gets hurt
 * Calls 'die' from player manager when life is zero
 */
public class HealthPickupComponent : PickUpComponent, IDamageObserver
{
    [SerializeField] private float max_health;
    [SerializeField] private float min_health;
    [SerializeField] private float start_health;
    private float current_health;
    private PlayerManager playerManager;
    private PhotonView view;
    [SerializeField] PlayerController playerController;

    private void Awake()
    {
        view = GetComponent<PhotonView>();
        playerController = GetComponent<PlayerController>();
    }

    private void Start()
    {
        /* Adding this class as an IDamageObserver to player controller
         * The health component is asynchroniously updated when the player is shot or cut
         * This is needed as the health resource is manipulated from two perspectives
         * The one taking damage from guns and the one collecting health packs (here, in this class) 
         */ 
        playerController.addObserver<IDamageObserver>(this);
        current_health = start_health;

        if (view.IsMine)
        {
            // Player Manager is needed if the player runs out of life and dies
            playerManager = PhotonView.Find((int)view.InstantiationData[0]).GetComponent<PlayerManager>();
        }
    }

    public override void updateUI()
    {
        base.SetSlider(5, "Health", current_health / max_health);
    }

    private void IncrementHealth(float value)
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
        if (view.IsMine)
            playerController.GotHurt();

        GetDamage(5);
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

    /* Governs the mechanic for picking up health 
     * and destroying the health pack after it has been collected
     */
    public override void pickupTrigger(PickUpScript pickup)
    {
        if (pickup != null)
        {
            if (pickup.pickupType == PickUpScript.PickUpType.Health)
            {
                IncrementHealth(pickup.amount);
                pickup.destroyThisObject();
            }
        }
    }
}


