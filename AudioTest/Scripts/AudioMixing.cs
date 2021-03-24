using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioMixing : MonoBehaviour
{
    [SerializeField] AudioMixer _audioMixer;
    // Start is called before the first frame update
    [SerializeField] string parameter;

    Slider _slider;

    void Start()
    {
        _slider = gameObject.GetComponent<Slider>();
        _slider.value = VolumeRevertChange();
    }

    public void VolumeChange()
    {
        float volume = ConvertVolume2dB(_slider.value / 100);
        _audioMixer.SetFloat(parameter, volume);
    }

    private int VolumeRevertChange()
    {
        float volume;
        _audioMixer.GetFloat(parameter, out volume);
        int value = (int) (100 * Mathf.Pow(10, volume / 20));
        return value;
    }
    private float ConvertVolume2dB(float volume) => Mathf.Clamp(20f * Mathf.Log10(Mathf.Clamp(volume, 0f, 1f)), -80f, 0f);
}
