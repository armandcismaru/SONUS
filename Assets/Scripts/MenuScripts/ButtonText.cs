using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ButtonText : MonoBehaviour
{
    [SerializeField] private Text buttonText;
    void Start()
    {
        
    }

    void Update()
    {

    }

    private void OnMouseOver()
    {
        Debug.Log("ddd");
        buttonText.fontSize = 4;
    }
}
