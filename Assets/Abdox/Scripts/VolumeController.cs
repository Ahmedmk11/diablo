using UnityEngine;
using UnityEngine.UI;

public class VolumeController : MonoBehaviour
{
    public Slider volumeSlider;
    public Slider sfxSlider;

    void Start()
    {
        if (volumeSlider != null)
        {
            volumeSlider.value = FindObjectOfType<audiomanager>().GetMusicSource().volume;
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }
        if (sfxSlider != null)
        {
            sfxSlider.value = FindObjectOfType<audiomanager>().GetSFXSource().volume;
            sfxSlider.onValueChanged.AddListener(OnSFXChanged);
        }
    }

    void OnVolumeChanged(float value)
    {
        FindObjectOfType<audiomanager>().SetMusicVolume(value);
    }

    void OnSFXChanged(float value)
    {
        FindObjectOfType<audiomanager>().SetSFXVolume(value);
    }
}
