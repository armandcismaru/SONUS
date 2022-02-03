using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Knife : MonoBehaviour
{
    [SerializeField] private int damage = 100;
    [SerializeField] private float range = 2f;
    [SerializeField] Camera fpsCam;
    //public ParticleSystem muzzleFlash;
    // Start is called before the first frame update

    public void UseKnife()
    {
        //muzzleFlash.Play();
        RaycastHit hit;
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range))
        {
            hit.collider.gameObject.GetComponent<IDamageable>()?.TakeDamage(damage);
            Debug.Log("KNIFE:" + hit.transform.name);
        }
    }
}
