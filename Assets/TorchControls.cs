using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TorchControls : MonoBehaviour
{
    bool TorchOn;
    const int MAXTORCHLIFE = 30;
    const int MAXLIGHTINTENSITY = 7;
    private float LifeRemaining = MAXTORCHLIFE;
    
    // Start is called before the first frame update
    void Start()
    {
        TorchOn = false;
    }

    // Update is called once per frame
    void Update()
    {    
        if (Input.GetKey(KeyCode.T))
        {
            TorchOn = !TorchOn;
            LifeRemaining = MAXTORCHLIFE;
        }

        if (TorchOn)
        {
            if (LifeRemaining > 0)
            {
                LifeRemaining -= Time.deltaTime;
            }
            else
            {
                LifeRemaining = 0;
            }
            this.GetComponent<Light>().intensity = MAXLIGHTINTENSITY * (LifeRemaining / MAXTORCHLIFE);
        }
        else
        {
            this.GetComponent<Light>().intensity = 0;
        }
    }
}
