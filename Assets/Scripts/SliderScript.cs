using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderScript : MonoBehaviour
{
    [SerializeField] GameObject player;
    public Slider slider;
    public Text text;
    public string unit;
    public byte decimals = 2;

    void OnEnable () 
    {
        slider.onValueChanged.AddListener(ChangeValue);
        slider.value = RoomManager.Instance.getMouseSpeed();
        ChangeValue(slider.value);
    }
    void OnDisable()
    {
        slider.onValueChanged.RemoveAllListeners();
    }

    void ChangeValue(float value)
    {
        text.text = value.ToString("n"+decimals) + " " + unit;
        RoomManager.Instance.setMouseSpeed(value);
    }
}
