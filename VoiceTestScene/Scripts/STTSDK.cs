using System.Collections;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using UnityEngine;

public class STTSDK 
{
    string subscription_key = "c5a88b17adcf42c28a0948019a7d2d29";
    string region = "japanwest";
    string location = "ja-JP";
    
    //Sampling Rate 11050*2, bitRate=16, channels = 1
   public  async UniTask STT(string wavFilepath, int sampleRate, int bitRate, int channels)
    {
        var speechConfig = SpeechConfig.FromSubscription(subscription_key, region); 
        speechConfig.SpeechRecognitionLanguage = location;
        var reader = new BinaryReader(File.OpenRead(wavFilepath));
        var audioStreamFormat = AudioStreamFormat.GetWaveFormatPCM((uint)sampleRate, (byte)bitRate, (byte)channels);
        var audioInputStream = AudioInputStream.CreatePushStream(audioStreamFormat);
        var audioConfig = AudioConfig.FromStreamInput(audioInputStream);
        var recognizer = new SpeechRecognizer(speechConfig, audioConfig);

        byte[] readBytes;
        do
        {
            readBytes = reader.ReadBytes(1024);
            audioInputStream.Write(readBytes, readBytes.Length);
        } while (readBytes.Length > 0);

        var result = await recognizer.RecognizeOnceAsync();
        Debug.Log($"Recognized Line : = {result.Text}");
    }
    
    public async UniTask STTBytes(byte[] readBytes, int sampleRate, int bitRate, int channels)
    {
        var speechConfig = SpeechConfig.FromSubscription(subscription_key, region);
        speechConfig.SpeechRecognitionLanguage = location;
        var audioStreamFormat = AudioStreamFormat.GetWaveFormatPCM((uint)sampleRate, (byte)bitRate, (byte)channels);
        var audioInputStream = AudioInputStream.CreatePushStream(audioStreamFormat);
        var audioConfig = AudioConfig.FromStreamInput(audioInputStream);
        var recognizer = new SpeechRecognizer(speechConfig, audioConfig);
        
        audioInputStream.Write(readBytes, readBytes.Length);
        
        var result = await recognizer.RecognizeOnceAsync();
        Debug.Log($"Recognized Line : = {result.Text}");
    }
}
