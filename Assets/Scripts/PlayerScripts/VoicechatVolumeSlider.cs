using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class VoicechatVolumeSlider : MonoBehaviour
{
    public Slider slider;
    public TMP_Text text;
    public string unit;
    public byte decimals = 2;

    void OnEnable()
    {
        slider.onValueChanged.AddListener(ChangeValue);
        slider.value = RoomManager.Instance.getVoiceChatVolume();
        ChangeValue(slider.value);
    }
    void OnDisable()
    {
        slider.onValueChanged.RemoveAllListeners();
    }

    void ChangeValue(float value)
    {
        text.text = value.ToString("n" + decimals) + " " + unit;
        RoomManager.Instance.setVoiceChatVolume(value);
    }

}
