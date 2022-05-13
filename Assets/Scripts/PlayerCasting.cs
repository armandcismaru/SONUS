using UnityEngine;

// Casts rays in game
public class PlayerCasting : MonoBehaviour
{
    public static float DistanceFromTarget;
    public float ToTarget;

    // Update is called once per frame
    void Update()
    {
        RaycastHit Hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out Hit)) {
            ToTarget = Hit.distance;
            DistanceFromTarget = ToTarget;
        }
    }
}
