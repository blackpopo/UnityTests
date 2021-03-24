using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class CR2 : MonoBehaviour
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
    WebCamDevice[] _webCamDevices;
    
    
    
    // Start is called before the first frame update
    async void Start()
    {
        RealTimeImage.texture = await getDevices();
        _camTexture.Play();
        await getGoodHW();
    }

    async UniTask<WebCamTexture> getDevices()
    {
        _webCamDevices = WebCamTexture.devices;
        while (_webCamDevices.Length < 1)
        {
            await UniTask.Yield(PlayerLoopTiming.Update);
            _webCamDevices = WebCamTexture.devices;
        }
        Debug.Log("Web Cam Devices Length: " + _webCamDevices.Length);
        _camTexture = new WebCamTexture(_webCamDevices[0].name, width, height, fps);
        _camTexture = new WebCamTexture(_webCamDevices[0].name, _camTexture.width, _camTexture.height, fps);
        return _camTexture;
    }

    async UniTask getGoodHW()
    {
        // height = _camTexture.height;
        // width = _camTexture.width;
        height = 288;
        width = 352;
        while (height <= 16 && width <= 16)
        {
            Debug.Log("Get size");
            height = _camTexture.height;
            width = _camTexture.width;
            await UniTask.Yield(PlayerLoopTiming.Update);
        }
        Debug.Log("hight : width" + height + width);
        Vector2 sizetmp = RealTimeImage.rectTransform.sizeDelta;
        if (ConstantHightOrWidth == 0)
        {
            float scale = sizetmp.y / _camTexture.height;
            sizetmp.x = scale * _camTexture.width;
        }
        else
        {
            float scale = sizetmp.x / _camTexture.width;
            sizetmp.y = scale * _camTexture.height;
        }

        RealTimeImage.rectTransform.sizeDelta = sizetmp;

        CaptureImage.rectTransform.sizeDelta = sizetmp;
    }
    
    // Update is called once per frame
    async  void Update()
    {
        Debug.Log(Time.frameCount);
        await Capture();
    }

    async UniTask Capture()
    {
        await UniTask.Delay(1000 * 5);
        Texture2D copyed = CopyImage();
        Debug.Log("Copyed");
        RealTimeImage.texture = copyed;
        Debug.Log("Exchange!");
        Destroy(_camTexture);
        Debug.Log("Destroyed");
        _camTexture = new WebCamTexture(_webCamDevices[0].name, width, height, fps);
        CaptureImage.texture = _camTexture;
        _camTexture.Play();
    }

    private Texture2D CopyImage()
    {
        Texture2D copyed = new Texture2D(width, height,TextureFormat.ARGB32, false);
        copyed.SetPixels32(_camTexture.GetPixels32());
        copyed.Apply();
        return copyed;
    }
}