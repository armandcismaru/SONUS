using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderScript : MonoBehaviour
{
    [SerializeField] GameObject player;
    public Slider slider;
    public TMP_Text text;
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

    // Update mouse sensitivity when using ther slider
    void ChangeValue(float value)
    {
        text.text = value.ToString("n"+decimals) + " " + unit;
        RoomManager.Instance.setMouseSpeed(value);
    }
}
