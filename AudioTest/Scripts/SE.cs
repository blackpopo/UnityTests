using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SE : MonoBehaviour
{
    [SerializeField] AudioClip ConfigEffect;

    [SerializeField] AudioClip FaceEffect;

    [SerializeField] AudioClip BodyEffect;
    // Start is called before the first frame update
    AudioSource AudioSource;

    void Start()
    {
        AudioSource = GetComponent<AudioSource>();
    }

    public  void ConfigClick()
    {
        AudioSource.PlayOneShot(ConfigEffect);
    }
    public void FaceClick()
    {
        AudioSource.PlayOneShot(FaceEffect);
    }
    public void BodyClick()
    {
        AudioSource.PlayOneShot(BodyEffect);
    }
}
