using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Timers;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class SDKFlushing : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] AudioSource listenSource;
    // Start is called before the first frame update
    [SerializeField] float gain = 200.0f;
    
    //private fields
    float volume = 0.0f;
    float frequency = 0.0f;
    bool isSpeaking;
    bool isRecording;
    float emvTimer;
    float bufferTimer;
    float speakingTimer;
    bool isEMVend = false;
    int head;
    string STTFileName;
    string EMVFileName;
    byte[] STTBytes;
 
    //COSNTANTS
    //Emonet の　sampling_frequency は 11025Hz で STT の sampling_frequency は　16000Hz
    //音質の劣化を考慮して2250で取って、emonetは半分に変換る。MSのSTTはSDKの機能で変換する。
    const int RECORD_SECONDS = 2;
    const int SAMPLING_FREQUENCY = 44100 / 2;
    const int FFTSAMPLES = 2 << 8;
    const int HEADER_SIZE = 44;
    const float RESCALE_FACTOR = 32767;
    const float MIN_FREQ = 100.0f;
    const float MAX_FREQ = 1400.0f;
    const float MIN_VOLUME = 0.1f;
    const float EMV_INTERVAL = 5.0f;
    const float BUFFER_TIME = 1.1f;
    const float TOTAL_SPEAKING_TIME = 12.0f;
    
    
    void Start()
    {
        GetMic();
    }

     private void GetMic()
    {
        while (Microphone.devices.Length< 1) { }
        string device = Microphone.devices[0];
        listenSource.loop = true;
        listenSource.clip = Microphone.Start(device, true, RECORD_SECONDS, SAMPLING_FREQUENCY);
        while (!(Microphone.GetPosition(device) > 0)) { }
        listenSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        Waiting();
    }

    private void Waiting()
    {
        DateTime now = DateTime.Now;
        var time = now.Hour.ToString() + "_" + now.Minute.ToString() + "_" + now.Second.ToString();
        CalculateVowel();
        if (MIN_FREQ < frequency && frequency < MAX_FREQ && MIN_VOLUME < volume)
        {
            isSpeaking = true;
            emvTimer += Time.deltaTime;
            bufferTimer = 0.0f;
            speakingTimer += Time.deltaTime;
            //始めてレコーディングを開始したとき
            if (!isRecording)
            {
                isRecording = true;
                emvTimer = 0.0f;
                isEMVend = false;
                speakingTimer = 0.0f;
                // EMVFileName = System.IO.Path.GetFullPath(Path.Combine(Application.temporaryCachePath, STTFileName));
                EMVFileName = "EMV_" + time + ".wav";
                // STTFileName = System.IO.Path.Combine("C:\\Users\\Atsuya\\AndroidStudioProjects", "TEMPTATION",
                //     STTFileName);
                EMVFileName = System.IO.Path.Combine("C:\\Users\\Atsuya\\AndroidStudioProjects", "TEMPTATION",
                    EMVFileName);
                //ここでbytesの初期化
                STTBytes = new byte[HEADER_SIZE];
                    using (var fileStream = new FileStream(EMVFileName, FileMode.Create))
                {
                    byte[] headerSpace = new byte[HEADER_SIZE];
                    fileStream.Write(headerSpace, 0, headerSpace.Length);
                }
            }
            image.color = Color.red;
        }
        else if (isSpeaking && volume > MIN_VOLUME)
        {
            //子音字をしゃべっていると判定
            bufferTimer = 0.0f;
            emvTimer += Time.deltaTime;
            speakingTimer += Time.deltaTime;
            isEMVend = emvTimer > EMV_INTERVAL ? true : false;
            image.color = Color.magenta;
        }
        else if (isSpeaking && bufferTimer < BUFFER_TIME)
        {
            emvTimer += Time.deltaTime;
            bufferTimer += Time.deltaTime;
            speakingTimer += Time.deltaTime;
            isEMVend = emvTimer > EMV_INTERVAL ? true : false;
            image.color = Color.cyan;
        }
        else
        {
            image.color = Color.blue;
            isSpeaking = false;
            if (isRecording)
            {
                isRecording = false;
                isEMVend = false;
                using (var fileStream = new FileStream(STTFileName, FileMode.Open))
                {
                    WavHeaderWrite(fileStream, listenSource.clip.channels, SAMPLING_FREQUENCY);
                }
                using (var fileStream = new FileStream(EMVFileName, FileMode.Open))
                {
                    WavHeaderWrite(fileStream, listenSource.clip.channels, SAMPLING_FREQUENCY/2);
                }

                if (speakingTimer > TOTAL_SPEAKING_TIME)
                {
                    //Send wav file to API
                    Debug.Log("SEND Wav File!");
                }
            }
        }

        int position = Microphone.GetPosition(null);
        if (position < 0 || head == position)
        {
            return;
        }

        if (isRecording)
        {
            var waves = new float[listenSource.clip.samples * listenSource.clip.channels];
            listenSource.clip.GetData(waves, 0);
            //GetDataだとrecordSeconds の間のデータがすべて入っている。
            //head と　position がおかしい？
            List<float> STTAudioData = new List<float>();
            if (head < position)
            {
                for (int i = head; i < position; i++)
                {
                    STTAudioData.Add(waves[i]);
                }
            }
            else
            {
                for (int i = head; i < waves.Length; i++)
                {
                    STTAudioData.Add(waves[i]);
                }

                for (int i = 0; i < position; i++)
                {
                    STTAudioData.Add(waves[i]);
                }
            }

            foreach (float data in STTAudioData)
            {
                Byte[] buffer = BitConverter.GetBytes((short)(data * RESCALE_FACTOR));
                //STTBYTESと結合！
                byte[] temp = new byte[buffer.Length + STTBytes.Length];
                Array.Copy(STTBytes, temp, STTBytes.Length);
                Array.Copy(buffer, 0, temp, STTBytes.Length, buffer.Length);
                STTBytes = temp;
            }

            if (!isEMVend)
            {
                List<float> EMVAudioData = new List<float>();
                if (head < position)
                {
                    for (int i = head; i < position; i++)
                    {
                        if (i % 2 == 0) EMVAudioData.Add(waves[i]);
                    }
                }
                else
                {
                    for (int i = head; i < waves.Length; i++)
                    {
                        if (i % 2 == 0) EMVAudioData.Add(waves[i]);
                    }

                    for (int i = 0; i < position; i++)
                    {
                        if (i % 2 == 0) EMVAudioData.Add(waves[i]);
                    }
                }

                using (var fileStream = new FileStream(EMVFileName, FileMode.Append))
                {
                    WavBufferWrite(fileStream, EMVAudioData);
                }
            }
        }

        head = position;
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
    private void STTHeaderWrite(byte[] STTbytes, int channels, int samplingFrequency)
    {
        //サンプリング数を計算
        var samples = ((int)STTbytes.Length - HEADER_SIZE) / 2;
        var position = 0;
        Byte[] riff = System.Text.Encoding.UTF8.GetBytes("RIFF");
        for (int i = 0; i<riff.Length; i++ )
        {
            STTbytes[position] = riff[i];
            position++;
        }
        Byte[] chunkSize = BitConverter.GetBytes(STTbytes.Length - 8);
        for (int i = 0; i<chunkSize.Length; i++ )
        {
            STTbytes[position] = chunkSize[i];
            position++;
        }
        Byte[] wave = System.Text.Encoding.UTF8.GetBytes("WAVE");
        for (int i = 0; i<wave.Length; i++ )
        {
            STTbytes[position] = wave[i];
            position++;
        }
        Byte[] fmt = System.Text.Encoding.UTF8.GetBytes("fmt ");
        for (int i = 0; i<fmt.Length; i++ )
        {
            STTbytes[position] = fmt[i];
            position++;
        }
        Byte[] subChunk1 = BitConverter.GetBytes(16);
        for (int i = 0; i<subChunk1.Length; i++ )
        {
            STTbytes[position] = subChunk1[i];
            position++;
        }
        UInt16 _one = 1;
        Byte[] audioFormat = BitConverter.GetBytes(_one);
        for (int i = 0; i<audioFormat.Length; i++ )
        {
            STTbytes[position] = audioFormat[i];
            position++;
        }
        Byte[] numChannels = BitConverter.GetBytes(channels);
        for (int i = 0; i<numChannels.Length; i++ )
        {
            STTbytes[position] = numChannels[i];
            position++;
        }
        Byte[] sampleRate = BitConverter.GetBytes(samplingFrequency);
        for (int i = 0; i<sampleRate.Length; i++ )
        {
            STTbytes[position] = sampleRate[i];
            position++;
        }
        Byte[] byteRate = BitConverter.GetBytes(samplingFrequency * channels * 2);
        for (int i = 0; i<byteRate.Length; i++ )
        {
            STTbytes[position] = byteRate[i];
            position++;
        }
        Byte[] blockAlign = BitConverter.GetBytes((ushort)(channels * 2));
        for (int i = 0; i<blockAlign.Length; i++ )
        {
            STTbytes[position] = blockAlign[i];
            position++;
        }
        UInt16 bps = 16;
        Byte[] bitsPerSample = BitConverter.GetBytes(bps);
        for (int i = 0; i<bitsPerSample.Length; i++ )
        {
            STTbytes[position] = bitsPerSample[i];
            position++;
        }
        Byte[] datastring = System.Text.Encoding.UTF8.GetBytes("data");
        for (int i = 0; i<datastring.Length; i++ )
        {
            STTbytes[position] = datastring[i];
            position++;
        }
        Byte[] subChunk2 = BitConverter.GetBytes(samples * channels * 2);
        for (int i = 0; i<subChunk2.Length; i++ )
        {
            STTbytes[position] = subChunk2[i];
            position++;
        }
        
    }


    private void CalculateVowel()
    {
        {
            //ここが処理の重さ的にやばいかも？
            var max_volume = 0.0f;
            var max_index = 0;
            var total_volume = 0.0f;
            //録音時間＊サンプリング周波数の個数のデータがほしい！
            float[] temp = new float[FFTSAMPLES];
            listenSource.GetSpectrumData(temp, 0, FFTWindow.Blackman);
            for (int i = 0; i < temp.Length; i++)
            {
                if (max_volume < temp[i])
                {
                    max_index = i;
                    max_volume = temp[i];
                    total_volume += Mathf.Abs(temp[i]);
                }
            }

            if (temp.Length > 0)
            {
                frequency = max_index * AudioSettings.outputSampleRate / 2 / temp.Length;
                volume = total_volume / temp.Length * gain;
            }
        }
    }
}
