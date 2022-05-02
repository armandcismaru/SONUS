using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class OutroController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] TMP_Text score;
    [SerializeField] TMP_Text label;

    void Start()
    {
        int attackers = StateOutro.attackers;
        int defenders = StateOutro.defenders;
        int team = StateOutro.team;
        score.text = attackers + "\n" + defenders;
        label.text = "";
        if (attackers < defenders)
        {
            if (team == 0)
            {
                label.text = "VICTORY";
            })
            else
            {
                label.text = "DEFEAT";
            }
        }
        else if (attackers > defenders)
        {
            if (team == 0)
            {
                label.text = "DEFEAT";
            }
            else
            {
                label.text = "VICTORY";
            }
        }
        else
        {
            label.text = "DRAW";
        }
    }
}
