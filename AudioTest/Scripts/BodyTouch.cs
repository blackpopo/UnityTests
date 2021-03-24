using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyTouch : MonoBehaviour
{
    [SerializeField] AudioClip FaceTouchVoice;

    [SerializeField] AudioClip[] BodyTouchClips;
    
    //private
    AudioSource AudioSource;

    private int BodyTouchCounter=0;
    // Start is called before the first frame update
    void Start()
    {
        AudioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FaceClick()
    {
        Debug.Log(string.Format("Face Touched... :{0}", FaceTouchVoice));
        AudioSource.PlayOneShot(FaceTouchVoice);
    }

    public void BodyClick()
    {
        Debug.Log("Body Touched...");
        AudioSource.PlayOneShot(BodyTouchClips[BodyTouchCounter % BodyTouchClips.Length]);
        BodyTouchCounter += 1;

    }
    
}
