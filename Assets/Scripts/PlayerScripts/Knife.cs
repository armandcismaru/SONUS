using UnityEngine;

public class Knife : MonoBehaviour
{
    private int damage = 100;
    private float range = 1.5f;
    [SerializeField] Camera fpsCam;
    
    // Logic for using the knife
    public void UseKnife()
    {
        RaycastHit hit;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(damage);
            GetComponentInParent<PlayerController>().KnifeKillingSound();
        }
        else
        {
            GetComponentInParent<PlayerController>().KnifeSound();
        }
    }
}
