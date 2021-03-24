using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DG.Tweening;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class Test2MicInput : MonoBehaviour
{

    //Variables
    int recordSeconds;
    bool setup = false;
    int head;
    string TSTFileName;
    string EMVFileName;
    float volume;
    float freqency;
    bool isSpeaking;
    bool isRecording;
    float[] waves;
    Random emVranRandom;
    int minFreq;
    float bufferTime = 1.2f;
    float timer = 0.0f;
    

    //SeriarizableFiled
    [SerializeField] Image buttonImage;
    [SerializeField] float gain = 200.0f;
    [SerializeField] float threshold = 0.1f;
    
    //CONSTANTS 
    const int EMVsamplingFrequency = 11050;
    [SerializeField] AudioSource AudioSource;
    // Start is called before the first frame update
    //データにつき取り出すサンプル数　>> recordTime ? or 1s
    readonly int SAMPLES = 16000;
    readonly int FFTSAMPLES = 2 << 10;
    //wavのヘッダーサイズ
    const int HEADER_SIZE = 44;
    //何かのスケール因子？
    private const float RESCALE_FACTOR = 32767;
    
    void Start()
    {
        if (Microphone.devices.Length > 0)
        {
            string device = Microphone.devices[0];
            int maxFreq;
            Microphone.GetDeviceCaps(null, out minFreq, out maxFreq);
            // while (Microphone.GetPosition(null) <= 0){}
            //今回のminFreqは48000, recordSeconds は　3s
            //可変の場合はminFreq が 0 になるらしい。
            Debug.Log("min frequency" + minFreq);
            minFreq = minFreq <= 16000 ? 48000 : minFreq;
            recordSeconds = minFreq / SAMPLES;
            AudioSource.loop = true;
            AudioSource.clip = Microphone.Start(device, true, recordSeconds, minFreq);
            AudioSource.Play();
            emVranRandom = new Random(); ;
            Debug.Log("Recording Seconds: " + recordSeconds + " min frequency: " + minFreq);
        }
    }

    public void ButtonOnClick()
    {
        setup = !setup;
        buttonImage.color = setup ? Color.yellow : Color.cyan;
        Debug.Log("Set UP IS OK? " + setup);
    }

    // Update is called once per frame
    void Update()
    {
        if (setup)
        {
            int position = Microphone.GetPosition(null);
            DateTime now = DateTime.Now;
            string time = now.Hour.ToString() + "_" + now.Minute.ToString() + "_" + now.Second.ToString();
            //Record Seconds は切り捨てになるから　実際は minFreqencyより少ない数
            if (position < 0 || position == head){};

            CalcVolumeAndFreq();
            // Debug.Log("Frequency vs Volume" + freqency + " : " + volume);
            if (100.0f < freqency && freqency < 1400.0f && volume > threshold)
            {
                timer = 0.0f;
                isSpeaking = true;
                if (!isRecording)
                {
                    isRecording = true;
                    // DateTime now = DateTime.Now;
                    // string time = now.Hour.ToString() + "_" + now.Minute.ToString() + "_" + now.Second.ToString();
                    TSTFileName = "TST_" + time + ".wav";
                    EMVFileName = "EMV_" + time + ".wav";
                    // TSTFileName = System.IO.Path.GetFullPath(Path.Combine(Application.temporaryCachePath, TSTFileName));
                    // EMVFileName = System.IO.Path.GetFullPath(Path.Combine(Application.temporaryCachePath, TSTFileName));
                    TSTFileName = System.IO.Path.Combine("C:\\Users\\Atsuya\\AndroidStudioProjects", "TEMPTATION",
                        TSTFileName);
                    EMVFileName = System.IO.Path.Combine("C:\\Users\\Atsuya\\AndroidStudioProjects", "TEMPTATION",
                        EMVFileName);
                    Debug.Log("TST File: " + TSTFileName);
                    Debug.Log("EMV File Name: " + EMVFileName);
                    using (var fileStream = new FileStream(TSTFileName, FileMode.Create))
                    {
                        byte[] headerSpace = new byte[HEADER_SIZE];
                        fileStream.Write(headerSpace, 0, headerSpace.Length);
                    }
                    using (var fileStream = new FileStream(EMVFileName, FileMode.Create))
                    {
                        byte[] headerSpace = new byte[HEADER_SIZE];
                        fileStream.Write(headerSpace, 0, headerSpace.Length);
                    }
                }
                buttonImage.color = Color.red;
            }else if ((volume > threshold || timer < bufferTime) && isSpeaking)
            {
                timer += Time.deltaTime;
                buttonImage.color = Color.magenta;
            }
            else
            {
                timer = 0.0f;
                buttonImage.color = Color.yellow;
                if (isRecording)
                {
                    Debug.Log("Finish Recording");
                    Debug.Log("Clip Source Information");
                    Debug.Log("frequency :" + AudioSource.clip.frequency);
                    Debug.Log("channels :" + AudioSource.clip.channels);
                    Debug.Log("Samples" + AudioSource.clip.samples);
                    isRecording = false;
                    isSpeaking = false;
                    using (var fileStream = new FileStream(TSTFileName, FileMode.Open))
                    {
                        WavHeaderWrite(fileStream, AudioSource.clip.channels, AudioSource.clip.frequency);
                    }
                    using (var fileStream = new FileStream(EMVFileName, FileMode.Open))
                    {
                        WavHeaderWrite(fileStream, AudioSource.clip.channels, EMVsamplingFrequency);
                    }
                }
            }

            
            //Corouting をつかって、recordingSecondごとにwavFileに書き出す？
            //でも、サンプルは1sになっていた。なぜ？
            if (isRecording)
            {
                if (position < 0 || head == position) {
                    return;
                }
                waves = new float[recordSeconds * minFreq];
                AudioSource.clip.GetData(waves, 0);
                //GetDataだとrecordSeconds の間のデータがすべて入っている。
                //head と　position がおかしい？
                List<float> TSTAudioData = new List<float>();
                List<float> EMVAudioData= new List<float>();
                if (head < position)
                {
                    for (int i = head; i < position; i++)
                    {
                        TSTAudioData.Add(waves[i]);
                        int rnd = emVranRandom.Next(minFreq);
                        if (0 <= rnd && rnd <= EMVsamplingFrequency)
                        {
                            EMVAudioData.Add(waves[i]);
                        }
                    }
                }
                else
                {
                    for (int i = head; i < waves.Length; i++)
                    {
                        TSTAudioData.Add(waves[i]);
                        int rnd = emVranRandom.Next(minFreq);
                        if (0 <= rnd && rnd <= EMVsamplingFrequency)
                        {
                            EMVAudioData.Add(waves[i]);
                        }
                    }
                    for (int i = 0; i < position; i++)
                    {
                        TSTAudioData.Add(waves[i]);
                        int rnd = emVranRandom.Next(minFreq);
                        if (0 <= rnd && rnd <= EMVsamplingFrequency)
                        {
                            EMVAudioData.Add(waves[i]);
                        }
                    }
                }
                using (var fileStream = new FileStream(TSTFileName, FileMode.Append))
                {
                    WavBufferWrite(fileStream, TSTAudioData);
                }
                using (var fileStream = new FileStream(EMVFileName, FileMode.Append))
                {
                    WavBufferWrite(fileStream, EMVAudioData);
                }
            }

            head = position;
        }
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

    private void CalcVolumeAndFreq()
    {
        //ここが処理の重さ的にやばいかも？
        var max_volume = 0.0f;
        var max_index = 0;
        //録音時間＊サンプリング周波数の個数のデータがほしい！
        float[] temp = new float[FFTSAMPLES];
        AudioSource.GetSpectrumData(temp, 0, FFTWindow.Blackman);
        for (int i = 0; i < temp.Length; i++)
        {
            if (max_volume < temp[i])
            {
                max_index = i;
                max_volume = temp[i];
                volume += Mathf.Abs(temp[i]);
            }
        }
        freqency = max_index * AudioSettings.outputSampleRate / 2 / temp.Length;
        volume = volume / temp.Length * gain;
    }
}
