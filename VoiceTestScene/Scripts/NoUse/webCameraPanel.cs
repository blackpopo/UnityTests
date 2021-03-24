using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class webCameraPanel : MonoBehaviour
{
    RawImage _image;
    WebCamTexture _webCamTexture;
    // Start is called before the first frame update
    void Start()
    {
        _image = GetComponent<RawImage>();
        Screen.orientation = ScreenOrientation.Portrait;
        WebCamDevice[] devices = WebCamTexture.devices;
        for (var i = 0; i < devices.Length; i++)
        {
            Debug.Log("["+ i +"]"+ devices[i].name); 
        }
        _webCamTexture = new WebCamTexture(devices[0].name, 900, 1600, 30);
        _image.texture =　_webCamTexture;
        Debug.Log("Web CamTexture size: " + _webCamTexture.height + " : " + _webCamTexture.width);
        Debug.Log("Get pixels : "+ _webCamTexture.GetPixels());
        _webCamTexture.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
