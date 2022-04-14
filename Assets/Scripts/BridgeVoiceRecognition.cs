using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeVoiceRecognition : MonoBehaviour
{
    public readonly string SPELL_SOUND = "Gunshot";
    [SerializeField] Gun gun;
    PlayerController playerController;
    public float timeSpellTimer;
    public float remainingSpellTimer;
    public bool isSpellAvailable;
    // [SerializeField] GameObject playerController;
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

    void StartTimer()
    {
        isSpellAvailable = false;
        timeSpellTimer = Time.time;
    }
    void TriggerSpell(string hypseg) {
        Debug.Log("From Unity:" + hypseg);
        if (isSpellAvailable){
            int team = playerController.GetComponent<PlayerController>().team;
            if (team == 0) {
                if (hypseg == "evolve me")
                {
                    playerController.GetComponent<AudioManager>().Play(SPELL_SOUND);
                    playerController.StartFastSpeed();
                    StartTimer();
                }
                else if (hypseg == "device")
                {
                    playerController.GetComponent<AudioManager>().Play(SPELL_SOUND);
                    playerController.EmittingSpell();
                    StartTimer();
                }
            }
            else
            {
                if (hypseg == "evolve me")
                {
                    playerController.GetComponent<AudioManager>().Play(SPELL_SOUND);
                    playerController.StartInvisibilitySpell();
                    StartTimer();
                }
                else if (hypseg == "device")
                {
                    playerController.GetComponent<AudioManager>().Play(SPELL_SOUND);
                    playerController.DeployDecoy();
                    StartTimer();
                }
            }

        }
    }
}
