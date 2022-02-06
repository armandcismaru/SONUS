using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpener : MonoBehaviour
{
    public float TheDistance;
    public GameObject TheDoor;
    public AudioSource CreakSound;

    // Update is called once per frame
    void Update()
    {
        TheDistance = PlayerCasting.DistanceFromTarget;
    }

    void OnMouseOver()
    {
        if (Input.GetButtonDown("Action") && (TheDistance <= 2))
        {
            this.GetComponent<BoxCollider>().enabled = false;
            TheDoor.GetComponent<Animation>().Play("DoorAnimation1");
            CreakSound.Play();
        }

    }
}
