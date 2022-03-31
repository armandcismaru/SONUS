using UnityEngine;

public class Gun : MonoBehaviour
{
    private int damage = 5;
    private float range = 100f;
    [SerializeField] Camera fpsCam;

    public void Shoot()
    {
        RaycastHit hit;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            var iDamagableComponent = hit.collider.gameObject.GetComponent<IDamageable>();
            if (iDamagableComponent != null)
            {
                iDamagableComponent.TakeDamage(damage);
            } 
        }
    }
}
