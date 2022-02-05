using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireAnimations : MonoBehaviour
{
    public int FireState;
    public GameObject FireLight;


    // Update is called once per frame
    void Update()
    {
        if (FireState == 0)
        {
            StartCoroutine(AnimateFire());
        }
    }

    IEnumerator AnimateFire()
    {
        FireState = Random.Range(1, 4); //I have created 3 animations 

        switch(FireState)
        {
            case 0: FireLight.GetComponent<Animation>().Play("FireAnimation1"); break;
            case 1: FireLight.GetComponent<Animation>().Play("FireAnimation2"); break;
            case 2: FireLight.GetComponent<Animation>().Play("FireAnimation3"); break;
        }

        //make the script wait for just under one sec so that full animation can complete
        yield return new WaitForSeconds(0.99f);

        //reset back to initial state
        FireState = 0;

        //One scene has 60 s and there are 60 samples per second.
        //I have created these animations by playing with the point light intensity associated to the fire light on wall.
    }
}
