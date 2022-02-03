using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] private int damage = 50;
    [SerializeField] private float range = 100f;
    [SerializeField] Camera fpsCam;
    //public ParticleSystem muzzleFlash;
    // Start is called before the first frame update
    //PhotonView view;

    //void Awake()
    //{
    //    view = GetComponent<PhotonView>();
    //}

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetButtonDown("Fire1")) {
        //    Shoot();
        //}
    }
    public void Shoot()
    {
        //muzzleFlash.Play();
        RaycastHit hit;
        Debug.Log("Shoooteria");
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            //if (hit.collider.gameObject.name)
            //{
                //PlayerController playerController = hit.transform.GetComponent<PlayerController>();
                //playerController.TakeDamage(damage);
                hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(damage);
                //view.RPC("RPC_Shoot", RpcTarget.All, hit.point, hit.normal);

            //}
            Debug.Log(hit.transform.name);
        }
    }

    //[PunRPC]
    //void RPC_Shoot(Vector3 hitPosition, Vector3 hitNormal)
    //{
    //    Collider[] colliders = Physics.OverlapSphere(hitPosition, 0.3f);
    //    if (colliders.Length != 0)
    //    {
    //        GameObject bulletImpactObj = Instantiate(bulletImpactPrefab, hitPosition + hitNormal * 0.001f, Quaternion.LookRotation(hitNormal, Vector3.up) * bulletImpactPrefab.transform.rotation);
    //        Destroy(bulletImpactObj, 10f);
    //        bulletImpactObj.transform.SetParent(colliders[0].transform);
    //    }
    //}
}
