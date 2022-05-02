using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class OutroController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] TMP_Text score;
    [SerializeField] TMP_Text textUp;
    [SerializeField] TMP_Text textDown;
    void Start()
    {
        score.text = StateOutro.attackers + " - " + StateOutro.defenders;
        textUp.text = "";
        textDown.text = "";
        foreach(string user in StateOutro.attackerPlayers)
        {
            textUp.text += user + " ";
        }
        foreach(string user in StateOutro.defenderPlayers)
        {
            textDown.text += user + " ";
        }
    }
}
