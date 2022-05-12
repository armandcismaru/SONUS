using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeVoiceRecognition : MonoBehaviour
{
    [SerializeField] Gun gun;
    [SerializeField] TorchControls torchControls;
    PlayerController playerController;
    public float timeSpellTimer;
    public float remainingSpellTimer;
    public bool isSpellAvailable; // countdown for spell
    // Start is called before the first frame update
    void Start()
    {
        playerController = this.gameObject.transform.parent.gameObject.GetComponent<PlayerController>();
        isSpellAvailable = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isSpellAvailable)
        {
            UpdateSpellTimer();
        }
    }

    // Update mechanism + HUD of spell countdown 
    void UpdateSpellTimer()
    {
        remainingSpellTimer = timeSpellTimer + 5f - Time.time;
        playerController.UpdateTimerSpell(remainingSpellTimer.ToString("0.0000"));

        if(remainingSpellTimer <= 0)
        {
            isSpellAvailable = true;
            playerController.UpdateTimerSpell("SPELL ACTIVATED");
        }
    }

    // Countdown for spell activation
    void StartTimer()
    {
        isSpellAvailable = false;
        timeSpellTimer = Time.time;
    }

    // Function called from javascript when a word is recognised
    // hypseg represent the recognised word
    void TriggerSpell(string hypseg) {
        if (hypseg == "torch")
        {
            torchControls.TriggerTorch();
            return;
        }
        if (isSpellAvailable){
            int team = playerController.GetComponent<PlayerController>().team;
            if (team == 0) {
                if (hypseg == "listen")
                {
                    playerController.SpellTransformSound();
                    playerController.EmittingSpell();
                    StartTimer();
                }
            }
            else
            {
                if (hypseg == "decoy")
                {
                    playerController.SpellTransformSound();
                    playerController.DeployDecoy();
                    StartTimer();
                }
            }

        }
    }
}
