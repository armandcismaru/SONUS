using UnityEngine;

public class Gun : MonoBehaviour
{
    private int damage = 50;
    private float range = 100f;
    [SerializeField] Camera fpsCam;

    public void Shoot()
    {
        RaycastHit hit;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(damage);
        }
    }
}
