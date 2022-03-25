using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    [SerializeField]
    Gun gun;

    public void OnShoot()
    {
        gun.Shoot();
    }
}
