using UnityEngine;
using UnityEngine.UI;

public class DisplayMessage : MonoBehaviour
{
    void Start()
    {
        gameObject.SetActive(true);
    }

    void Update()
    {
        //GameObject mng = GameObject.FindWithTag("PlayerController");
    }

    public void SetText(string msg)
    {
        GetComponent<Text>().text = msg;
    }

    public void MakeVisible(bool visible)
    {
        gameObject.SetActive(visible);
    }

    public void SetColour(string team)
    {
        if (team == "red")
            GetComponent<Text>().color = new Color(1, 0, 0, 1);
        else if (team == "blue")
            GetComponent<Text>().color = new Color(0, 0, 1, 1);
    }
}
