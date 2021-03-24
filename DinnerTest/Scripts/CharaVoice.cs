using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CharaVoice : MonoBehaviour
{
    [SerializeField] Image hiddenImage;
    
    [SerializeField] AudioSource Mic;

    [SerializeField] AudioSource characterVoice;

    [SerializeField] AudioClip itadakimasu;

    [SerializeField] AudioClip gotisousama;

    [SerializeField] AudioClip goodby;

    [SerializeField] AudioClip[] lines;

    [SerializeField] AudioClip un;

    [SerializeField] AudioClip he;

    float interval = 85.0f;
    float timer = 0.0f;
    List<int> already_voices;
    void Start()
    {
        var devices = Microphone.devices;
        for (int i = 0; i < devices.Length; i++)
        {
            Debug.Log("Device [" + i +"] Name : " + devices[i]);
        }
        var deviceName = Microphone.devices[0];
        Mic.clip = Microphone.Start(deviceName, true, 5, 24000); // ループ再生にしておく
        Mic.loop = true;
        Mic.Play();
        already_voices = new List<int> { };
    }


    public void ButtonClick()
    {
        if (hiddenImage.color == Color.blue)
        {
            characterVoice.PlayOneShot(itadakimasu); //いただきます
            var i = Random.Range(0, lines.Length);
            already_voices.Add(i);
            var clip = lines[i];
            StartCoroutine(DelayMethod(2.5f, () =>
            {
                if (hiddenImage.color == Color.blue)
                {
                    characterVoice.PlayOneShot(clip);
                }
            }));
            
        }else if (hiddenImage.color == Color.cyan)
        {
            characterVoice.PlayOneShot(gotisousama);//ごちそうさまでした
            StartCoroutine(DelayMethod(2.5f, () =>
            {
                characterVoice.PlayOneShot(goodby);
            }));
        }
    }
    
    private IEnumerator DelayMethod(float waitTime, Action action)
    {
        yield return new WaitForSeconds(waitTime);
        action();
    }
    
    // Update is called once per frame
    async void Update()
    {
        if (timer > interval && hiddenImage.color == Color.blue)
        {
            timer = 0.0f;
            var volume = GetVolume();
            volume *= 1000;
            if (4.0 < volume && volume < 10.0)
            {
                characterVoice.PlayOneShot(un);
            }  
            else if (10.0 <= volume)
            {
                characterVoice.PlayOneShot(he);
            }
            else
            {
                var i = Random.Range(0, lines.Length);
                while (already_voices.Count != 0 && already_voices.Contains(i))
                {
                    i = Random.Range(0, lines.Length);
                }
                already_voices.Add(i);
                if (already_voices.Count == lines.Length)
                {
                    already_voices = new List<int>{};
                }
                var clip = lines[i];
                characterVoice.PlayOneShot(clip);
            }

        }
        else
        {
            timer += Time.deltaTime;
        }
    }

    float GetVolume()
    {
        var length = 256;
        float[] data = new float[length];
        float a = 0;
        Mic.GetOutputData(data,0);
        foreach(float s in data)
        {
            a += Mathf.Abs(s);
        }
        return a/256.0f;
    }
    
}
