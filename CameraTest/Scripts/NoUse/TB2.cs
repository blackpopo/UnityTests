using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TB2 : MonoBehaviour
{
    [SerializeField] RawImage BGImage;

    [SerializeField] RawImage FGImage;

    WebCamTexture frontCamTexture;
    WebCamTexture rearCamTexture;
    

    public Color32[] color32;
    float intervalTimer = 10.0f;
    float interval = 0.0f;
    
    WebCamDevice[] _devices;

    int width = 1920;
    int height = 1080;
    int fps = 60;
    byte[] _bytes;
    
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Transparent Background 2!");
        Screen.orientation = ScreenOrientation.Portrait;
        _devices = WebCamTexture.devices;
        Debug.Log(_devices.Length);
        for (int i = 0; i < _devices.Length; i++)
        {
            Debug.Log("Devices name: [" + i + "]  is " + _devices[i]);
            if (_devices[i].isFrontFacing)
            {
                frontCamTexture = new WebCamTexture(_devices[i].name, width, height, fps);
            }
            else
            {
                rearCamTexture = new WebCamTexture(_devices[i].name, width, height, fps);
                BGImage.texture = rearCamTexture;
            }
        }
        rearCamTexture.Play();
        RearCameraRotate();
    }

    // Update is called once per frame
    void Update()
    {
        interval += Time.deltaTime;
        if (interval > intervalTimer)
        {
            interval = 0.0f;
            RearCameraSaver();
            CameraChange();
            FrontCameraRotate();
            FrontCamSaver();
            CameraChange();
            Debug.Log("One interval is finished...");
        }
    }

    void RearCameraRotate()
    {
        // アスペクト比、何度回転させれば正しく表示されるかがわかる
        if ((rearCamTexture.videoRotationAngle == 90) || (rearCamTexture.videoRotationAngle == 270)) {
            // 表示するRawImageを回転させる
            Vector3 angles = BGImage.rectTransform.eulerAngles;
            angles.z = -rearCamTexture.videoRotationAngle;
            BGImage.rectTransform.eulerAngles = angles;
        }
        // 回転済みなので、widthはx、heightはyでそのままサイズ調整
        // 全体を表示させるように、
        //Raw Image が回転してるからRawImageを元に戻すのでは？
        Vector2 sizetmp = BGImage.rectTransform.sizeDelta;
        float scale;
        if (rearCamTexture.width > rearCamTexture.height) {
            scale = sizetmp.y / rearCamTexture.height;
        } else {
            scale = sizetmp.x / rearCamTexture.width;
        }
        sizetmp.x = rearCamTexture.width * scale;
        sizetmp.y = rearCamTexture.height * scale;
        BGImage.rectTransform.sizeDelta = sizetmp;
    }

    void FrontCameraRotate()
    {
        // アスペクト比、何度回転させれば正しく表示されるかがわかる
        if ((frontCamTexture.videoRotationAngle == 90) || (frontCamTexture.videoRotationAngle == 270)) {
            // 表示するRawImageを回転させる
            Vector3 angles = FGImage.rectTransform.eulerAngles;
            angles.z = -frontCamTexture.videoRotationAngle;
            FGImage.rectTransform.eulerAngles = angles;
        }
        // 回転済みなので、widthはx、heightはyでそのままサイズ調整
        // 全体を表示させるように、大きい方のサイズを表示枠に合わせる
        Vector2 sizetmp = FGImage.rectTransform.sizeDelta;
        float scale;
        if (frontCamTexture.width > frontCamTexture.height) {
            scale = sizetmp.x / frontCamTexture.width;
        } else {
            scale = sizetmp.y / frontCamTexture.height;
        }
        sizetmp.x = frontCamTexture.width * scale;
        sizetmp.y = frontCamTexture.height * scale;
        FGImage.rectTransform.sizeDelta = sizetmp;
    }
    void RearCameraSaver()
    {
        Texture2D _reartexture;
        Debug.Log("Rear Camera Saving");
        //背面画面の貼り付け
        while (true)
        {
            if (rearCamTexture.height >16 && rearCamTexture.width >16)
            {
                _reartexture = new Texture2D(rearCamTexture.height, rearCamTexture.width, TextureFormat.ARGB32, false);
                BGImage.texture = _reartexture;
                break;
            }
        }
        _reartexture.SetPixels32(rearCamTexture.GetPixels32());
        _reartexture.Apply();
    }
    
    void FrontCamSaver()
    {
        Texture2D _fronttexture;
        Debug.Log("Front Camera Saving");
        //前面画面の貼り付け
        while (true)
        {
            if (frontCamTexture.height > 16 && rearCamTexture.width > 16)
            {
                _fronttexture = new Texture2D(frontCamTexture.height, frontCamTexture.width);
                FGImage.texture = _fronttexture;
                break;
            }
        }
        _fronttexture.SetPixels32(frontCamTexture.GetPixels32());
        _fronttexture.Apply();
    }

    void CameraChange()
    {
        if (rearCamTexture.isPlaying)
        {
            rearCamTexture.Stop();
            frontCamTexture.Play();
            while (!frontCamTexture.isPlaying) { }
        }
        else
        {
            frontCamTexture.Stop();
            BGImage.texture = rearCamTexture;
            rearCamTexture.Play();
            while (!rearCamTexture.isPlaying) { }
        }
    }


}
