using UnityEngine;

public class Gun : MonoBehaviour
{
    private int damage = 50;
    private float range = 100f;
    [SerializeField] Camera fpsCam;

    public void Shoot()
    {
        // Things to check as rpcs might not be the issue
        // 1. is it called: 
        // a. It is not - why? 
        // b. It is 
        // 2. Is it shooting towards the right direction?
        // Check the direction of the camera? - check the forward vector 
        // 3. Check if there is collision with anything? 
        // a. There is not - check colliders, check raycast profile 
        // b. There is
        // 4. Is take damage called ..... 

        RaycastHit hit;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
           // Debug.Log(hit.collider.gameObject.name);
           //Debug.Log();
            var iDamagableComponent = hit.collider.gameObject.GetComponent<IDamageable>();
            if (iDamagableComponent != null)
            {
               // Debug.Log(iDamagableComponent);
                iDamagableComponent.TakeDamage(damage);
            } 
        }
    }
}
