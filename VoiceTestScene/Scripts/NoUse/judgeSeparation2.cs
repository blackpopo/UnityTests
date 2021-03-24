using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class judgeSeparation2 : MonoBehaviour
{
    [SerializeField]AudioSource speakingAudioSource;
    [SerializeField] AudioSource listenAudioSource;
    //このSampleNumってなにもの？
    //音量を計算するためのサンプル数？
    private readonly int SampleNum = (2 << 9); // サンプリング数は2のN乗(N=5-12)
    [SerializeField, Range(0f, 1000f)] float gain = 200f;
    int ms;// 倍率

    [SerializeField] float threshold = 1.0f;
    [SerializeField] Image _image;
    float[] _wave;
    bool isSpeaking = false;
    bool isListen = true;
    
    float speakingTime = 0.0f;

    float consonantBufferTime = 1.6f;
    float intermidiateBufferTime = 1.2f;
    
    private const float RESCALE_FACTOR = 32767;
    
    //しゃべり方の速さの判定
    //sumBetweelVowlが母音と母音との間の時間の合計
    //無音状態が続いたら0.0fになる。
    float sumBetweenVowel = 0.0f;
    //VowelCntが母音と母音の間と判定した回数
    int betweenVowelCnt = 0;
    //単純に母音と母音との間の時間
    float betweenVowel = 0.0f;
    //bufferTime　はおなじでいいのかな？

    float CBT = 0.0f;
    float IBT = 0.0f;

    bool unazuki = false;
    bool aizuchi = false;

    // Start is called before the first frame update

    hukidashiText _hukidashiText;
    
    //Recording Settings
    bool isRec;
    const int HEADER_SIZE = 44;
    string recFileName;

    int head;
    
    
    
    
    void Start () {
        speakingAudioSource = GetComponent<AudioSource>();
        if ((speakingAudioSource != null) && (Microphone.devices.Length > 0)) // オーディオソースとマイクがある
        {
            string devName = Microphone.devices[0]; // 複数見つかってもとりあえず0番目のマイクを使用
            int minFreq, maxFreq;
            Microphone.GetDeviceCaps(devName, out minFreq, out maxFreq); // 最大最小サンプリング数を得る
            ms = minFreq / SampleNum; // サンプリング時間を適切に取る
            speakingAudioSource.loop = true; // ループにする
            speakingAudioSource.clip = Microphone.Start(devName, true, ms, minFreq); // clipをマイクに設定
            while (!(Microphone.GetPosition(devName) > 0)) { } // きちんと値をとるために待つ
            speakingAudioSource.Play();
        }

        _hukidashiText = GetComponent<hukidashiText>();
    }

    // Update is called once per frame
    void Update()
    {
        float[] wave = new float[SampleNum];
        AudioListener.GetSpectrumData(wave, 0, FFTWindow.Rectangular);
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
        
        //母音の発音
        if (100.0f < freq && freq < 1400.0f && volume > threshold)
        {
            ColorChange(Color.red);
            isSpeaking = true;
            CBT = 0.0f;
            IBT = 0.0f;
            // Debug.Log("Between Vowel" + betweenVowel);
            sumBetweenVowel += betweenVowel;
            betweenVowelCnt += 1;
            intermidiateBufferTime = sumBetweenVowel / betweenVowelCnt > 1.0f ? 1.6f : 1.4f;
            intermidiateBufferTime = sumBetweenVowel > 10.0f ? 1.2f : intermidiateBufferTime;
            if (isListen)
            {
                recFileName = Path.Combine(Application.temporaryCachePath, "rec.wav");
                Debug.Log("Recording Start!: "+ recFileName);                    
                using (var fileStream = new FileStream(recFileName, FileMode.Create))
                {
                    byte[] headerSpace = new byte[HEADER_SIZE];
                    fileStream.Write(headerSpace, 0, headerSpace.Length);
                }


                isListen = false;
            }

            if (sumBetweenVowel > 10.0f && !unazuki)
            {
                _hukidashiText.Line("うん");
                unazuki = true;
            }
            betweenVowel = 0.0f;
        }//小さな母音の発音(結構 volume の条件判定よりも甘いからIBTは 0.0fにしない。)
        else if (isSpeaking　&& volume >= threshold)
        {
            betweenVowel += Time.deltaTime;
            ColorChange(Color.yellow);
            if (consonantBufferTime >= CBT)
            {
                CBT += Time.deltaTime;
                IBT = 0.0f;
            }
            else
            {
                isSpeaking = false;
            }
        }
        else if (isSpeaking)
        {
            betweenVowel += Time.deltaTime;
            ColorChange(Color.green);
            if (intermidiateBufferTime >= IBT)
            {
                IBT += Time.deltaTime;
            }
            else
            {
                isSpeaking = false;
            }
        }
        //isSpeaking がFalseのとき
        else
        {
            if (!isListen)
            {
                isListen = true;
                Debug.Log("Finish Recording");                    
                using (var fileStream = new FileStream(recFileName, FileMode.Open))
                {
                    WavHeaderWrite(fileStream, listenAudioSource.clip.channels, listenAudioSource.clip.frequency);
                }
                isListen = true;
                isRec = false;

                // string sentence = stt.Listen(recFileName);

            }
            ColorChange(Color.blue);
            sumBetweenVowel = 0.0f;
            betweenVowelCnt = 0;
            if (unazuki)
            {
                _hukidashiText.Line("うん。");
                unazuki = false;
            }
        }

        if (isRec)
        {
            int position = Microphone.GetPosition(null);
            Debug.Log(head + ":" +  position);
            float[] tmp = new float[SampleNum* ms];
            {
                List<float> audioData = new List<float>();
 
                if (head < position)
                {
                    for (int i = head; i < position; i++)
                    {
                        audioData.Add(tmp[i]);
                    }
                }
                else
                {
                    for (int i = head; i < tmp.Length; i++)
                    {
                        audioData.Add(tmp[i]);
                    }
                    for (int i = 0; i < position; i++)
                    {
                        audioData.Add(tmp[i]);
                    }
                }
                using (var fileStream = new FileStream(recFileName, FileMode.Append))
                {
                    WavBufferWrite(fileStream, audioData);
                }
            }
            head = position;
        }
    }
    
    private void WavHeaderWrite(FileStream fileStream, int channels, int samplingFrequency)
    {
        //サンプリング数を計算
        var samples = ((int)fileStream.Length - HEADER_SIZE) / 2;
 
        fileStream.Seek(0, SeekOrigin.Begin);
 
        Byte[] riff = System.Text.Encoding.UTF8.GetBytes("RIFF");
        fileStream.Write(riff, 0, 4);
        Byte[] chunkSize = BitConverter.GetBytes(fileStream.Length - 8);
        fileStream.Write(chunkSize, 0, 4);
        Byte[] wave = System.Text.Encoding.UTF8.GetBytes("WAVE");
        fileStream.Write(wave, 0, 4);
        Byte[] fmt = System.Text.Encoding.UTF8.GetBytes("fmt ");
        fileStream.Write(fmt, 0, 4);
        Byte[] subChunk1 = BitConverter.GetBytes(16);
        fileStream.Write(subChunk1, 0, 4);
        UInt16 _one = 1;
        Byte[] audioFormat = BitConverter.GetBytes(_one);
        fileStream.Write(audioFormat, 0, 2);
        Byte[] numChannels = BitConverter.GetBytes(channels);
        fileStream.Write(numChannels, 0, 2);
        Byte[] sampleRate = BitConverter.GetBytes(samplingFrequency);
        fileStream.Write(sampleRate, 0, 4);
        Byte[] byteRate = BitConverter.GetBytes(samplingFrequency * channels * 2);
        fileStream.Write(byteRate, 0, 4);
        UInt16 blockAlign = (ushort)(channels * 2);
        fileStream.Write(BitConverter.GetBytes(blockAlign), 0, 2);
        UInt16 bps = 16;
        Byte[] bitsPerSample = BitConverter.GetBytes(bps);
        fileStream.Write(bitsPerSample, 0, 2);
        Byte[] datastring = System.Text.Encoding.UTF8.GetBytes("data");
        fileStream.Write(datastring, 0, 4);
        Byte[] subChunk2 = BitConverter.GetBytes(samples * channels * 2);
        fileStream.Write(subChunk2, 0, 4);
 
        fileStream.Flush();
    }
    
    private void WavBufferWrite(FileStream fileStream, List<float> dataList)
    {
        foreach (float data in dataList)
        {
            Byte[] buffer = BitConverter.GetBytes((short)(data * RESCALE_FACTOR));
            fileStream.Write(buffer, 0, 2);
        }
        fileStream.Flush();
    }
    void ColorChange(Color color)
    {
        _image.color = color;
    }
}
