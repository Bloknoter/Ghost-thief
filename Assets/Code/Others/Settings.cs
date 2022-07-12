using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{

    [SerializeField]
    private AudioMixerGroup Mixer;

    [SerializeField]
    private Slider MusicSl;

    [SerializeField]
    private Slider SoundsSl;

    void Start()
    {
        object value = SaveLoadCon.LoadData("music");
        if(value == null)
        {
            value = -20f; 
        }
        Mixer.audioMixer.SetFloat("MusicVolume", (float)value);
        MusicSl.value = (float)value;

        value = SaveLoadCon.LoadData("sounds");
        if (value == null)
        {
            value = -20f;
        }
        Mixer.audioMixer.SetFloat("SoundsVolume", (float)value);
        SoundsSl.value = (float)value;
    }

    void Update()
    {
        
    }

    public void ChangeMusicValue(float newvalue)
    {
        float value = Mathf.Clamp(newvalue, -80, 20);
        Mixer.audioMixer.SetFloat("MusicVolume", value);
        SaveLoadCon.SaveData(value, "music");
    }

    public void ChangeSoundsValue(float newvalue)
    {
        float value = Mathf.Clamp(newvalue, -80, 20);
        Mixer.audioMixer.SetFloat("SoundsVolume", value);
        SaveLoadCon.SaveData(value, "sounds");
    }
}
