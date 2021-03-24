using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class CameraTest4 : MonoBehaviour
{
    //Private
    WebCamDevice[] webCamDevices;
    int frontHeight = 1080;
    int frontWidth = 1920;
    int backHeight = 1080;
    int backWidth = 1920;
    int fps = 30;
    int frontRotate;
    int backRotate;
    int frontIndex;
    int backIndex;

    WebCamTexture frontCamTexture;
    WebCamTexture backCamTexture;

    Color32[] _color32s;
    Stopwatch sw;

    //Serialize Fileds
    [SerializeField] RawImage frontImage;
    [SerializeField] RawImage pngImage;
    [SerializeField] RawImage staticBackImage;
    
    float timer = 0.0f;

    float interval = 5.0f;
    // Start is called before the first frame update
    async void Start()
    {
        sw = new Stopwatch();
        await Initialize();
    } 
    async UniTask Initialize()
    {
        webCamDevices = WebCamTexture.devices;
        while (webCamDevices.Length < 1)
        {
            await UniTask.DelayFrame(1);
        }

        for (int i = 0; i < webCamDevices.Length; i++)
        {
            Debug.Log(string.Format("WebCam Devices {0} th is {1}", i, webCamDevices[i].name));
            if (webCamDevices[i].isFrontFacing)
            {
                // frontCamTexture = new WebCamTexture(webCamDevices[i].name, frontWidth, frontHeight, fps);
                frontIndex = i;
            }
            else
            {
                backCamTexture = new WebCamTexture(webCamDevices[i].name, backWidth, backHeight, fps);
                backIndex = i;
            }
        }
        pngImage.texture = new  Texture2D(1920, 1080);
        backCamTexture.Play();
        // frontCamTexture.Play();
    }
    // Update is called once per frame
    void Update()
    {
        CaptureBackground();
    }

    void CaptureBackground()
    {
        timer += Time.deltaTime;
        CopyTexture(backCamTexture, staticBackImage);
        if (timer > interval)
        {
            timer = 0.0f;
            ChangeCamera(true);
            CopyTexture(frontCamTexture, frontImage);
            ChangeCamera(false);
        }
    }

    private void CopyTexture(WebCamTexture webCamTexture, RawImage dstImage)
    {
        // while (!webCamTexture.isPlaying)
        // {
        //     yield return null;
        // }
        // while (webCamTexture.height <= 16 && webCamTexture.width <= 16)
        // {
        //     yield return null;
        // }
        dstImage.texture =  RotateTexture2D(webCamTexture);
    }
    private Texture2D RotateTexture2D(WebCamTexture originalTexture)
    {
        Color32[] original = originalTexture.GetPixels32();
        Color32[] rotated = new Color32[original.Length];
        int w = originalTexture.width;
        int h = originalTexture.height;
        int r = originalTexture.videoRotationAngle;
        bool clockwise = (r == 90) ? true : false;
        
        if (!(r == 90 || r == 270))
        {
            Texture2D notRotatedTexture = new Texture2D(w, h);
            notRotatedTexture.SetPixels32(original);
            notRotatedTexture.Apply();
            return notRotatedTexture;
        }
        
        int iRotated, iOriginal;

        for (int j = 0; j < h; ++j)
        {
            for (int i = 0; i < w; ++i)
            {
                iRotated = (i + 1) * h - j - 1;
                iOriginal = clockwise ? original.Length - 1 - (j * w + i) : j * w + i;
                rotated[iRotated] = original[iOriginal];
            }
        }
        Texture2D rotatedTexture = new Texture2D(h, w);
        rotatedTexture.SetPixels32(rotated);
        rotatedTexture.Apply();
        return rotatedTexture;
    }

    private void ChangeCamera(bool ToFront)
    {
        if (ToFront)
        {
            backCamTexture.Stop();
            // while (backCamTexture.isPlaying)
            // {
            //     yield return null;
            // }
            Destroy(backCamTexture);
            frontCamTexture = new WebCamTexture(webCamDevices[frontIndex].name, frontWidth, frontHeight, fps);
            frontCamTexture.Play();
            // while (!frontCamTexture.isPlaying)
            // {
            //     yield return null;
            // }
            new WaitForSeconds(3);
        }
        else
        {
            frontCamTexture.Stop();
            // while (frontCamTexture.isPlaying)
            // {
            //     yield return null;
            // }
            Destroy(frontCamTexture);
            backCamTexture = new WebCamTexture(webCamDevices[backIndex].name, backWidth, backHeight, fps);
            backCamTexture.Play();
            // while (!backCamTexture.isPlaying)
            // {
            //     yield return null;
            // }
        }
    }
}
