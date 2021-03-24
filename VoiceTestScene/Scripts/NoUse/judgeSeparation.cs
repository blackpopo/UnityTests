using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class judgeSeparation : MonoBehaviour
{
    AudioSource _audioSource;
    private readonly int SampleNum = (2 << 9); // サンプリング数は2のN乗(N=5-12)
    [SerializeField, Range(0f, 1000f)] float gain = 200f; // 倍率

    [SerializeField] float threshold = 1.0f;
    [SerializeField] Button _button;
    float[] _wave;
    bool isSpeaking = false;
    
    float notSpeakingTime =0.0f;
    float totalSpeakingTime = 0.0f;

    //単語・音節間の時間
    float bufferTime = 0.0f;
    
    //bufferTime をどこまで許すのか？
    float intermidiateTime = 1.0f;
    //Speaking Time中のしゃっべてない時間;

    int betweenCount = 1; 
    // Start is called before the first frame update
    bool aizuchi = false;
    bool kakunin = false;
    
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
        float[] wave = new float[1024];
        AudioListener.GetSpectrumData(wave, 0, FFTWindow.Rectangular);
        _audioSource.GetOutputData(wave, 0);
        var max_value = 0.0f;
        var max_index = 0;
        var volume = 0.0f;
        for (int i = 0; i < wave.Length; i++)
        {
            if (max_value < wave[i])
            {
                max_index = i;
                max_value = wave[i];
                volume += Mathf.Abs(wave[i]);
            }
        }
        var freq = max_index * AudioSettings.outputSampleRate / 2 / wave.Length;
        volume = volume / wave.Length * gain;

        
        //発音したとき
        if (100.0f < freq && freq < 1400.0f && volume > threshold)
        {
            //初期化
            if (!isSpeaking)
            {
                isSpeaking = true;
                aizuchi = true;
                kakunin = true;
            }
            Debug.Log("Speaking time" + totalSpeakingTime);
            bufferTime = 0.0f;
            totalSpeakingTime += Time.deltaTime;
            ColorChange(Color.red);
            betweenCount = 1;
        }
        //単語・音節の間
        else if ((bufferTime < intermidiateTime || totalSpeakingTime < 1.0f) && isSpeaking)
        {
            totalSpeakingTime += Time.deltaTime/ betweenCount;
            bufferTime += Time.deltaTime;
            betweenCount += 1;
            ColorChange(Color.yellow);
        }
        else
        {
            //初期化
            if (isSpeaking)
            {
                Debug.Log("betweenTime : " + bufferTime);
                Debug.Log("IntermiditateTime: "+ intermidiateTime);
                Debug.Log("SpeakingTime before Not Speaking: " + (totalSpeakingTime - intermidiateTime));
                isSpeaking = false;
                notSpeakingTime = 0.0f;
                Debug.Log("BetweenCount while Speaking" + betweenCount);
                totalSpeakingTime = 0.0f;
            }
            
            //話していない時間
            notSpeakingTime += Time.deltaTime;
            ColorChange(Color.blue);
            if (notSpeakingTime > 0.5f && aizuchi)
            {
                Debug.Log("うん");
                aizuchi = false;
            }

            if (notSpeakingTime > 10.0f && kakunin)
            {
                Debug.Log("どうしたの？");
                kakunin = false;
            }
            
        }
    }

    void ColorChange(Color color)
    {
        _button.image.color = color;
    }
}
