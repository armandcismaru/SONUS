using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class Gun : MonoBehaviour
{
    private int damage = 50;
    private float range = 100f;
    [SerializeField] Camera fpsCam;

    [SerializeField]
    private bool AddBulletSpread = true;
    [SerializeField]
    private Vector3 BulletSpreadVariance = new Vector3(0.1f, 0.1f, 0.1f);
    [SerializeField]
    private ParticleSystem ShootingSystem;
    [SerializeField]
    private Transform BulletSpawnPoint;
    [SerializeField]
    private ParticleSystem ImpactParticleSystem;
    [SerializeField]
    private TrailRenderer BulletTrail;
    [SerializeField]
    private LayerMask Mask;
    [SerializeField]
    private Transform GunTip;
    [SerializeField]
    private GameObject HitMarker;

    private Animator Animator;

    private void Awake()
    {
        Animator = GetComponent<Animator>();
    }

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

    public void Shoot()
    {
        // Use an object pool instead for these! To keep this tutorial focused, we'll skip implementing one.
        // For more details you can see: https://youtu.be/fsDE_mO4RZM or if using Unity 2021+: https://youtu.be/zyzqA_CPz2E

        Animator.SetBool("IsShooting", true);      
        ShootingSystem.Play();
        Vector3 direction = GetDirection();

        if (Physics.Raycast(BulletSpawnPoint.position, direction, out RaycastHit hit, range))
        {
            TrailRenderer trail = Instantiate(BulletTrail, GunTip.position, Quaternion.identity);

            StartCoroutine(SpawnTrail(trail, hit));

            var iDamagableComponent = hit.collider.gameObject.GetComponent<IDamageable>();
     
            if (iDamagableComponent != null)
            {
                StartCoroutine(ActivateHitmarker());
                iDamagableComponent.TakeDamage(damage);
            }
        }
    }

    private Vector3 GetDirection()
    {
        Vector3 direction = fpsCam.transform.forward;

        if (AddBulletSpread)
        {
            direction += new Vector3(
                Random.Range(-BulletSpreadVariance.x, BulletSpreadVariance.x),
                Random.Range(-BulletSpreadVariance.y, BulletSpreadVariance.y),
                Random.Range(-BulletSpreadVariance.z, BulletSpreadVariance.z)
            );

            direction.Normalize();
        }

        return direction;
    }

    private IEnumerator ActivateHitmarker()
    {
        HitMarker.GetComponent<Image>().color = Color.red;

        yield return new WaitForSeconds(0.35f);

        HitMarker.GetComponent<Image>().color = Color.white;
    }

    private IEnumerator SpawnTrail(TrailRenderer Trail, RaycastHit Hit)
    {
        float time = 0;
        Vector3 startPosition = Trail.transform.position;

        while (time < 1)
        {
            Trail.transform.position = Vector3.Lerp(startPosition, Hit.point, time);
            time += Time.deltaTime / Trail.time;

            yield return null;
        }
        Animator.SetBool("IsShooting", false);
        Trail.transform.position = Hit.point;
        Instantiate(ImpactParticleSystem, Hit.point, Quaternion.LookRotation(Hit.normal));

        Destroy(Trail.gameObject, Trail.time);
    }
}
