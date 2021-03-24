using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class CR3 : MonoBehaviour
{
    private int width = 352;

    private int height = 288;

    private int fps = 30;

    [SerializeField] RawImage RealTimeImage;

    [SerializeField] RawImage CaptureImage;

    [SerializeField, Range(0, 1)] int ConstantHightOrWidth;

    //private variables;
    WebCamTexture _camTexture;
    WebCamTexture _camTexture2;
    Color32[] _color32;
    WebCamDevice[] _webCamDevices;

    // FaceAPI _api;
    UserLocalAPI _api;

    float interval = 10.0f;
    float timer = 0.0f;

    // Update is called once per frame
    Stopwatch _stopwatch;

    void Start()
    {
        StartCoroutine("GetDevices");
        StartCoroutine("GetGoodHW");
        // _api = new FaceAPI();
        _api = new UserLocalAPI();
        _stopwatch = new Stopwatch();
    }

    IEnumerator GetDevices()
    {
        _webCamDevices = WebCamTexture.devices;
        while (_webCamDevices.Length < 1)
        {
            yield return null;
            _webCamDevices = WebCamTexture.devices;
        }
        _camTexture = new WebCamTexture(_webCamDevices[0].name, width, height, fps);
        Debug.Log(String.Format(" Requested Width: Height = {0} : {1} \n Actual Width : Height  = {2} : {3}\n" , _camTexture.requestedWidth, _camTexture.requestedHeight, _camTexture.width, _camTexture.height));
        // _camTexture = new WebCamTexture(_webCamDevices[0].name, _camTexture.width, _camTexture.height, fps);
        RealTimeImage.texture = _camTexture;
        // RealTimeImage.rectTransform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
        _camTexture.Play();
    }

    IEnumerator GetGoodHW()
    {
        height = _camTexture.height;
        width = _camTexture.width;
        while (height <= 16 && width <= 16)
        {
            yield return null;
            height = _camTexture.height;
            width = _camTexture.width;
        }
        Debug.Log("hight : width" + height + width);
        CaptureImage.texture = new Texture2D(width, height);
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
        yield return null;
    }
    async void Update()
    {
        await Capture();
    }

    async UniTask Capture()
    {
        timer += Time.deltaTime;
        if (timer > interval)
        {
            timer = 0.0f;
            Texture2D copyed = await CopyImage();
            await PngRequest(copyed);
        }
    }
    
    async UniTask<Texture2D>  CopyImage()
    {
        while (_camTexture.height <= 16 && _camTexture.width <= 16)
        {
            await UniTask.DelayFrame(1);
        }
        Texture2D copyed = new Texture2D(width, height,TextureFormat.ARGB32, false);
        _color32 = new Color32[width * height];
        _camTexture.GetPixels32(_color32);
        copyed.SetPixels32(_color32);
        copyed.Apply();
        Graphics.ConvertTexture(copyed, CaptureImage.texture);
        return copyed;
    }

    async UniTask PngRequest(Texture2D copyed)
    {
        byte[] bytes = copyed.EncodeToPNG();
        _stopwatch.Start();
        // await _api.GetRequest(bytes);
        // await _api.GetUnityRequest(bytes);
        _stopwatch.Stop();
        Debug.Log(_stopwatch.Elapsed);
        _stopwatch.Reset();
        //IN ANDROID
        PngSaver(bytes);
    }

    private void PngSaver(byte[] copyed)
    {
        //string FileName = System.IO.Path.GetFullPath(Path.Combine(Application.temporaryCachePath, "Face.png"));

        string FileName = System.IO.Path.Combine("C:\\Users\\Atsuya\\AndroidStudioProjects", "TEMPTATION",
            "Face.png");
        File.WriteAllBytes(FileName, copyed);
    }
    
}
