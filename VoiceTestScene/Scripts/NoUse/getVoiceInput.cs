using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class getVoiceInput : MonoBehaviour
{
    AudioSource _audio;
    [SerializeField]Button _button;

    bool _isRecording = false;

    string _micName;
    // Start is called before the first frame update
    void Start()
    {
        _audio = GetComponent<AudioSource>();
        foreach (string device in Microphone.devices)
        {
            Debug.Log("Name: " + device);
        }
    }

    public void ButtonOnClick()
    {
        if (_isRecording)
        {
            Debug.Log("Recording is Ended...");
            _isRecording = false;
            Microphone.End(_micName);
            _audio.Play();
            _button.image.color = Color.Lerp(Color.magenta, Color.white, 5f);

        }
        else
        {
            Debug.Log("Recording is Started...");
            _isRecording = true;
            _audio.clip = Microphone.Start(null, true, 10, 44100);
            _button.image.color = Color.Lerp(Color.white, Color.magenta, 5f);
        }

    }
    
    // Update is called once per frame
    void Update()
    { 
        float[] wave = new float[1024];
        AudioListener.GetSpectrumData(wave, 0, FFTWindow.Rectangular);
        _audio.GetOutputData(wave, 0);
        var max_value = 0.0f;
        var max_index = 0;
        for (int i = 0; i < wave.Length; i++)
        {
            if (max_value < wave[i])
            {
                max_index = i;
                max_value = wave[i];
            }
        }
        var freq = max_index * AudioSettings.outputSampleRate / 2 / wave.Length;
        Debug.Log("Volume: " + max_value + " freq: " + freq);
        
    }
}
