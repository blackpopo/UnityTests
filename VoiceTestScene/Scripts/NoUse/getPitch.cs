using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class getPitch : MonoBehaviour
{
    AudioSource _audioSource;
    private readonly int SampleNum = (2 << 10); // サンプリング数は2のN乗(N=5-12)
    [SerializeField, Range(0f, 1000f)] float gain = 100.0f; // 倍率
    [SerializeField, Range(0f, 1f)] float threshold = 0.1f; //第一フォルマントの閾値
    float[] _wave;

    float volumeRate;
    // Start is called before the first frame update
    
    void Start () {
        _audioSource = GetComponent<AudioSource>();
        _wave = new float[SampleNum];
        if ((_audioSource != null) && (Microphone.devices.Length > 0)) // オーディオソースとマイクがある
        {
            string devName = Microphone.devices[0]; // 複数見つかってもとりあえず0番目のマイクを使用
            int minFreq, maxFreq;
            Microphone.GetDeviceCaps(devName, out minFreq, out maxFreq); // 最大最小サンプリング数を得る
            int ms = minFreq / SampleNum; // サンプリング時間を適切に取る
            _audioSource.loop = true; // ループにする
            _audioSource.clip = Microphone.Start(devName, true, ms, minFreq); // clipをマイクに設定
            while (!(Microphone.GetPosition(devName) > 0)) { } // きちんと値をとるために待つ
            Microphone.GetPosition(null);
            _audioSource.Play();
        }
    }

    // Update is called once per frame
    public void Update()
    {
        _audioSource.GetSpectrumData(_wave, 0, FFTWindow.BlackmanHarris);
        var first_folmant = 0.0f;
        var second_folmant = 0.0f;
        var first_index = 0;
        var second_index = 0;
        var FFF = 0.0f;
        var SFF = 0.0f; ;
        for (int i = 0 ;i < _wave.Length; i++)
        {
            if (_wave[i] > first_folmant)
            {
                second_folmant = first_folmant;
                first_folmant = _wave[i];
                second_index = first_index;
                first_index = i;
            } else if ((_wave[i] > second_folmant) && Mathf.Abs(i - first_index) > 3)
            {
                second_folmant = _wave[i];
                second_index = i;
            }
        }

        if (first_folmant >= threshold)
        {
            Debug.Log("First Step: Volume is over threshold: " + threshold + " Volume: " + first_folmant);
            FFF = first_index * AudioSettings.outputSampleRate / 2 / _wave.Length;
            SFF = second_index * AudioSettings.outputSampleRate / 2 / _wave.Length;
            Debug.Log("FFF :" + FFF + " SFF :"+ SFF);
            if ((100.0f <= FFF && FFF <= 1400.0f) &&
                (500.0f <= SFF && SFF <= 2000.0f))
            {
                Debug.Log("UOA");
            } else if ((100.0f <= FFF && FFF <= 800.0f) &&
                       (200.0f <= SFF && SFF <= 3500.0f))
            {
                Debug.Log("IE");
            }
        }
    }
}
