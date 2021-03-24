using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class getMeanVolume : MonoBehaviour
{
    AudioSource _audioSource;

    float[] _wave = new float[1024];

    float countup = 0.0f;

    float timelimit = 3.0f;

    float volumeRate;

    float gain = 1.0f;
    
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Starting is OK...");
        _audioSource = GetComponent<AudioSource>();
        _audioSource.clip = Microphone.Start(null, false, 1000, 44100);
        while(Microphone.GetPosition(null) <=0){}
        _audioSource.Play();
    }
    
    // Update is called once per frame

    void Update ()
    {
        countup += Time.deltaTime;
        _audioSource.GetSpectrumData(_wave, 0, FFTWindow.Hamming);
        float sum = 0f;
        for (int i = 0; i < _wave.Length; ++i)
        {
            sum += _wave[i]; // データ（周波数帯ごとのパワー）を足す
        }
        // データ数で割ったものに倍率をかけて音量とする
        float volumeRate = Mathf.Clamp01(sum * gain );

        if (volumeRate > 0.1f)
        {
            Debug.Log("Time: " + countup + "volumeRate: " + volumeRate);
        }
    }
}
