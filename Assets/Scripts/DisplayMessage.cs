using UnityEngine;
using TMPro;

/* Class used to display certain messages on the canvas;
 * sits on the game map */
public class DisplayMessage : MonoBehaviour
{
    public TMP_Text text_box;
    void Start()
    {
        gameObject.SetActive(false);
    }

    void Update()
    {
        //GameObject mng = GameObject.FindWithTag("PlayerController");
    }

    public void SetText(string msg)
    {
        text_box.text = msg;
    }

    public void MakeVisible(bool visible)
    {
        gameObject.SetActive(visible);
    }

    public void SetColour(string team)
    {
        if (team == "red")
            text_box.color = new Color(0.6431373f, 0.2039216f, 0.227451f, 1);
        else if (team == "blue")
            text_box.color = new Color(0.0745f, 0.1262f, 0.2941f, 1);
    }
}
