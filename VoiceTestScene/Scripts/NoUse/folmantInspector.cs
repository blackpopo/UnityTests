using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class folmantInspector : MonoBehaviour
{

    AudioSource _audioSource;
    private readonly int SampleNum = (2 << 8); // サンプリング数は2のN乗(N=5-12)
    [SerializeField, Range(0f, 1000f)] float gain = 100.0f; // 倍率
    [SerializeField, Range(0f, 1f)] float threshold = 0.1f; //第一フォルマントの閾値
    float[] _wave;
    judgeVowel _judgeVowel;

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

        _judgeVowel = this.GetComponent<judgeVowel>();
        Debug.Log("judgeVowel" + _judgeVowel);
    }

    // Update is called once per frame
    public void LateUpdate()
    {
        _audioSource.GetSpectrumData(_wave, 0, FFTWindow.BlackmanHarris);
        float[] folmants_volume = new float[10];
        int[] folmants_indices = new int[10];
        float[] folmants = new float[10];
        for (int i = 0; i < _wave.Length; i++)
        {
            if (folmants_volume[folmants.Length - 1] < (_wave[i]))
            {
                for (int j = 0; j < folmants.Length; j++)
                {
                    if (folmants_volume[j] < _wave[i] )
                    {
                        folmants_volume[j] = _wave[i];
                        folmants_indices[j] = i;
                        break;
                    }
                }
            }
        }

        if (folmants_volume[0] >= threshold)
        {
            for (int j = 0; j < folmants.Length; j++)
            {
                folmants[j] = folmants_indices[j] * AudioSettings.outputSampleRate / 2 / _wave.Length;
                // Debug.Log(j + " th Folmant is " + folmants[j]);
            }
            _judgeVowel.Vowel(folmants);
            // if (_judgeVowel.vowel != null)
            // {
            //     Debug.Log("vowel is:" + _judgeVowel.vowel);
            // }

        }
    }
}