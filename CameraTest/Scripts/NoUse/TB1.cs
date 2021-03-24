using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TB1 : MonoBehaviour
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
    
    // Start is called before the first frame update
    void Start()
    {
        Screen.orientation = ScreenOrientation.Portrait;
        _devices = WebCamTexture.devices;
        Debug.Log(_devices.Length);
        for (int i = 0; i < _devices.Length; i++)
        {
            Debug.Log("Devices name: [" + i + "]  is " + _devices[i]);
            if (_devices[i].isFrontFacing)
            {
                frontCamTexture = new WebCamTexture(_devices[i].name, width, height, fps);
                Debug.Log("Front Camera : auto focus" + _devices[i].isAutoFocusPointSupported);
                FGImage.texture = frontCamTexture;
            }
            else
            {
                rearCamTexture = new WebCamTexture(_devices[i].name, width, height, fps);
                BGImage.texture = rearCamTexture;
                Debug.Log("REAR Camera : auto focus" + _devices[i].isAutoFocusPointSupported);
            }
        }
        // FrontTaker();
        rearCamTexture.Play();
        // 起動させて初めてvideoRotationAngle、width、heightに値が入り、
        // アスペクト比、何度回転させれば正しく表示されるかがわかる
        if ((rearCamTexture.videoRotationAngle == 90) || (rearCamTexture.videoRotationAngle == 270)) {
            // 表示するRawImageを回転させる
            Vector3 angles = BGImage.rectTransform.eulerAngles;
            angles.z = -rearCamTexture.videoRotationAngle;
            BGImage.rectTransform.eulerAngles = angles;
        }
        // 回転済みなので、widthはx、heightはyでそのままサイズ調整
        // 全体を表示させるように、大きい方のサイズを表示枠に合わせる
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

    // Update is called once per frame
    void Update()
    {
        interval += Time.deltaTime;
        if (interval > intervalTimer)
        {
            RearTaker();
            interval = 0.0f;
            rearCamTexture.Stop();
            frontCamTexture.Play();
            Debug.Log("Front Camera height : " + frontCamTexture.height +" width : " + frontCamTexture.width);
            Vector2 sizetmp = FGImage.rectTransform.sizeDelta;
            float scale;
            if (rearCamTexture.width > rearCamTexture.height) {
                scale = sizetmp.y / rearCamTexture.height;
            } else {
                scale = sizetmp.x / rearCamTexture.width;
            }
            sizetmp.x = rearCamTexture.width * scale;
            sizetmp.y = rearCamTexture.height * scale;
            FGImage.rectTransform.sizeDelta = sizetmp;
            //強制停止 frontCamera
            rearCamTexture.Play();
        }
    }

    void FrontTaker()
    {
        Debug.Log("Start Front Capture");
        if (rearCamTexture.isPlaying)
        {
            color32 = frontCamTexture.GetPixels32();
            //背面画面の貼り付け
            Texture2D _reartexture = new Texture2D(rearCamTexture.height, rearCamTexture.width);
            BGImage.texture = _reartexture;
            _reartexture.SetPixels32(color32);
            _reartexture.Apply();
            rearCamTexture.Stop();
            
        }
        FGImage.texture = rearCamTexture;
        Debug.Log("Front Information : " + _devices[0].availableResolutions);
        Debug.Log(" Is Front or Rear? " + _devices[0].isFrontFacing);
        frontCamTexture.Play();
        while(!frontCamTexture.isPlaying){}
        color32 = frontCamTexture.GetPixels32();
        //フロント画面の貼り付け
        Texture2D texture = new Texture2D(frontCamTexture.height, frontCamTexture.width);
        FGImage.texture = texture;
        texture.SetPixels32(color32);
        texture.Apply();
    }

    void RearTaker()
    {
        // Debug.Log("Start Rear Capture");
        // if (frontCamTexture.isPlaying)
        // {
        //     frontCamTexture.Stop();
        // }
        Debug.Log("Rear Information : " + rearCamTexture.deviceName);
        Debug.Log("height " + rearCamTexture.height + " width " + rearCamTexture.width  + " texel: " +rearCamTexture.texelSize);
        Debug.Log("kind: " + _devices[0].kind + " Resolution: " + _devices[0].availableResolutions);
        Debug.Log("depth Camera Name" + _devices[0].depthCameraName);
        // rearCamTexture.Play();
    }

    void TextureRotate()
    {
        
    }
}
