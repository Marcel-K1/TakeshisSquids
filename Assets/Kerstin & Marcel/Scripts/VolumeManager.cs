using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

//Made by Marcel

public class VolumeManager : MonoBehaviour
{
    [SerializeField]
    private AudioMixer m_masterMixer = null;

    [SerializeField]
    private Slider m_masterVolumeSlider = null;

    private float m_masterVolume = 0.5f;

    public void Start()
    {
        SetMasterVolumeSlider();
    }

    //Button methods and saving the values in player prefs
    public void ChangeMasterVolume(float _value)
    {
        PlayerPrefs.SetFloat("PlayerMasterVolume", _value);
        PlayerPrefs.Save();
        m_masterMixer.SetFloat("MasterVolume", PlayerPrefs.GetFloat("PlayerMasterVolume", 0f));
    }
    public void SetMasterVolumeSlider()
    {
        m_masterVolumeSlider.value = PlayerPrefs.GetFloat("PlayerMasterVolume", 0f);
    }
    public void MuteMaster(bool _mute)
    {
        if (_mute)
        {
            if (m_masterMixer.GetFloat("MasterVolume", out float volume))
            {
                m_masterVolume = volume;
            }
            m_masterMixer.SetFloat("MasterVolume", -80.0f);
        }
        else
        {
            m_masterMixer.SetFloat("MasterVolume", m_masterVolume);
        }
    }
    
}
