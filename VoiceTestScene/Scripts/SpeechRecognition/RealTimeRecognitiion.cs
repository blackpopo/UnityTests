using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using UnityEngine.SocialPlatforms;

public class RealTimeRecognitiion : MonoBehaviour
{
    SpeechConfig speechConfig;
    AudioConfig audioConfig;
    SpeechRecognizer recognizer;
    string subscriptionKey = "c5a88b17adcf42c28a0948019a7d2d29";
    string region = "japanwest";
    string locale = "ja-JP";

    void Start()
    {
    // #if PLATFORM_ANDROID
    //     // Request to use the microphone, cf.
    //     // https://docs.unity3d.com/Manual/android-RequestingPermissions.html
    //     message = "Waiting for mic permission";
    //     if (!Permission.HasUserAuthorizedPermission(Permission.Microphone))
    //     {
    //         Permission.RequestUserPermission(Permission.Microphone);
    //     }
    // #elif PLATFORM_IOS
    //     if (!Application.HasUserAuthorization(UserAuthorization.Microphone))
    //     {
    //         Application.RequestUserAuthorization(UserAuthorization.Microphone);
    //     }
    // #else
    Debug.Log("Script is Alive");
    Main(subscriptionKey, region, locale);
    }

    
    void Update()
    {
        // #if PLATFORM_ANDROID
        // if (!micPermissionGranted && Permission.HasUserAuthorizedPermission(Permission.Microphone))
        // {
        //     micPermissionGranted = true;
        //     message = "Click button to recognize speech";
        // }
        // #elif PLATFORM_IOS
        // if (!micPermissionGranted && Application.HasUserAuthorization(UserAuthorization.Microphone))
        // {
        //     micPermissionGranted = true;
        //     message = "Click button to recognize speech";
        // }
        // #endif
    }
    
    async static Task FromMic(SpeechConfig speechConfig)
    {
        var audioConfig = AudioConfig.FromDefaultMicrophoneInput();
        var recognizer = new SpeechRecognizer(speechConfig, audioConfig);

        Console.WriteLine("Speak into your microphone.");
        var result = await recognizer.RecognizeOnceAsync();
        Debug.Log(result.Text);
    }

    async static Task Main(string subscriptionKey, string region, string locale)
    {
        var speechConfig = SpeechConfig.FromSubscription(subscriptionKey, region);
        speechConfig.SpeechRecognitionLanguage = locale;
        await FromMic(speechConfig);
    }
}
