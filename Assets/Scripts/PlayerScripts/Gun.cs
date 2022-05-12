using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class Gun : MonoBehaviour
{
    private int damage = 5;
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
    private Transform GunTip;
    [SerializeField]
    private GameObject HitMarker;

    private void Awake()
    {
        //Animator = GetComponent<Animator>();
    }

    public void Shoot()
    {
        // Use an object pool instead for these! To keep this tutorial focused, we'll skip implementing one.
        // For more details you can see: https://youtu.be/fsDE_mO4RZM or if using Unity 2021+: https://youtu.be/zyzqA_CPz2E

        ShootingSystem.Play();
        Vector3 direction = GetDirection();

        /* If the shooting ray hits a collider of any kind an animated trail
         * will be spawned as well as shooting fire effect and bullet hitmark.
         * If the bullets hits an enemy the crosshair will become red */
        if (Physics.Raycast(BulletSpawnPoint.position, direction, out RaycastHit hit, range, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
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

    /* Returns direction forward of the camera, if used for shooting it will 
     * add random bullet spread in a predefined range of gun variance */
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

    // Activates hitmarker and keeps it on for a set amount of time
    private IEnumerator ActivateHitmarker()
    {
        HitMarker.GetComponent<Image>().color = Color.red;
        yield return new WaitForSeconds(0.35f);

        HitMarker.GetComponent<Image>().color = Color.white;
    }

    // Spawns gun trail and impact effect and particle effect
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
 
        Trail.transform.position = Hit.point;
        Instantiate(ImpactParticleSystem, Hit.point, Quaternion.LookRotation(Hit.normal));

        Destroy(Trail.gameObject, Trail.time);
    }
}
