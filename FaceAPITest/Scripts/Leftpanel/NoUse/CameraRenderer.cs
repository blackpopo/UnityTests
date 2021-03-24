using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class CameraRenderer : MonoBehaviour
{
    [SerializeField] int width = 1600;

    [SerializeField] int height = 900;

    [SerializeField] int fps = 30;

    [SerializeField] RawImage RealTimeImage;

    [SerializeField] RawImage CaptureImage;

    [SerializeField, Range(0, 1)] int ConstantHightOrWidth;
    
    //private variables;
    WebCamTexture _camTexture;
    WebCamTexture _camTexture2;
    Color32[] _color32;

    // Start is called before the first frame update
    void Start()
    {
        WebCamDevice[] devices = WebCamTexture.devices;
        _camTexture = new WebCamTexture(devices[0].name, width, height, fps);
        Debug.Log("_camTexture" + _camTexture.height + ":" + _camTexture.width + ":" + _camTexture.deviceName);
        _camTexture = new WebCamTexture(devices[0].name, _camTexture.width, _camTexture.height, fps);
        RealTimeImage.texture = _camTexture;
        StartCoroutine(getGoodSize(_camTexture));
        _camTexture.Play();
        //この時点でRealTimeImageに自分の顔が写っている
    }
    
    
    // Update is called once per frame
    void Update()
    {
    }

    IEnumerator  getGoodSize(WebCamTexture camTexture)
    {        
        Vector2 sizetmp = RealTimeImage.rectTransform.sizeDelta;
        while (camTexture.height <= 16 && camTexture.width <= 16)
        {
            yield return null;
        }

        height = camTexture.height;
        width = camTexture.width;
        Debug.Log("hight : width" + camTexture.height + camTexture.width);
        if (ConstantHightOrWidth == 0)
        {
            float scale = sizetmp.y / camTexture.height;
            sizetmp.x = scale * camTexture.width;
        }
        else
        {
            float scale = sizetmp.x / camTexture.width;
            sizetmp.y = scale * camTexture.height;
        }

        RealTimeImage.rectTransform.sizeDelta = sizetmp;

        CaptureImage.rectTransform.sizeDelta = sizetmp;
    }

    IEnumerator Capture()
    {
        while (true)
        {
            yield return new WaitForSeconds(10.0f);
            
        }
    }

    private void SetRealTimeImage()
    {
        Texture2D tempTex = new Texture2D(width, height);
        tempTex.SetPixels32(_camTexture.GetPixels32());
    }

    // IEnumerator Timer(float time)
    // {
    //     while (true)
    //     {
    //         yield return new WaitForSeconds(time);
    //         _color32 = _camTexture.GetPixels32();
    //         Texture2D tempTex = new Texture2D(width, height);
    //         CaptureImage.texture = tempTex;
    //         tempTex.Apply();
    //     }
    // }

    // async  Task Capture()
    // {
    //     await Task.Delay(1000 * 10);
    //     _color32 = await getPixel32();
    //     Debug.Log("Now width : height " + _camTexture.height + " : " + _camTexture.width);
    //     Texture2D tempTex =  new Texture2D(width , height);
    //     tempTex = await setPixel32(tempTex, _color32);
    //     RealTimeImage.texture = tempTex;
    //     tempTex.Apply();
    //     _camTexture.Stop();
    //     _camTexture2 = new WebCamTexture();
    //     // await getGoodSize2();
    //     Debug.Log("New Texture Kakunin: " + width + " : " + height);
    //     _camTexture2 = new WebCamTexture(width, height, fps);
    //     CaptureImage.texture = _camTexture2;
    //     _camTexture2.Play();
    // }

    // async Task<Color32[]> getPixel32()
    // {
    //     return _camTexture.GetPixels32();
    // }
    //
    // async Task<Texture2D> setPixel32(Texture2D tempTex, Color32[] _color32s)
    // {
    //     tempTex.SetPixels32(_color32s);
    //     return tempTex;
    // }
    //
    // async Task getGoodSize2()
    // {
    //     while (_camTexture2.height <= 16 && _camTexture2.width <= 16){}
    //
    //     height = _camTexture.height;
    //     width = _camTexture.width;
    //     Debug.Log("New Texture : " + width + " : " + height);
    // }
}
