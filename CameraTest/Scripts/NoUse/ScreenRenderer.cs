using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ScreenRenderer : MonoBehaviour
{
    //private filelds
    WebCamDevice[] webCamDevices;
    int frontHeight = 1080;
    int frontWidth = 1980;
    int backHeight = 1080;
    int backWidth = 1980;
    int frontIndex;
    int backIndex;

    WebCamTexture frontCamTextrue;
    WebCamTexture backCamTexture;

    Color32[] _color32s;

    float timer;

    float interval = 5.0f;
    //CONSTANTS
    int fps = 30;
    
    
    //Serialized Fileds
    [SerializeField] RawImage frontImage;
    [SerializeField] RawImage backImage;
    [SerializeField, Range(0, 1)] int HeightOrWidth; 
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Initializing!");
        //デバイスの取得と回転まで
        StartCoroutine("Init");
    }

    IEnumerator Init()
    {
        webCamDevices = WebCamTexture.devices;
        while (webCamDevices.Length < 1)
        {
            yield return null;
        }

        for (int i = 0; i < webCamDevices.Length; i++)
        {
            Debug.Log("WebCamDevice name: " + webCamDevices[i].name);
            if (webCamDevices[i].isFrontFacing)
            {
                frontIndex = i;
                frontCamTextrue = new WebCamTexture(webCamDevices[i].name, frontWidth, frontHeight);
            }
            else
            {
                backIndex = i;
                backCamTexture = new WebCamTexture(webCamDevices[i].name, backWidth, backHeight);
                backImage.texture = new Texture2D(backWidth, backHeight);
            }
        }

        frontImage.texture = frontCamTextrue;
        frontCamTextrue.Play();
        RotateRawImage(frontCamTextrue, frontImage);
    }

    private void RotateRawImage(WebCamTexture webCamTexture, RawImage rawImage)
    {
        // アスペクト比、何度回転させれば正しく表示されるかがわかる
        if ((webCamTexture.videoRotationAngle == 90) || (webCamTexture.videoRotationAngle == 270)) {
            // 表示するRawImageを回転させる
            Vector3 angles = rawImage.rectTransform.eulerAngles;
            angles.z = -webCamTexture.videoRotationAngle;
            rawImage.rectTransform.eulerAngles = angles;
        }

        Vector2 sizetmp = rawImage.rectTransform.sizeDelta;
        //横長画像
        var x = sizetmp.x;
        sizetmp.x = sizetmp.y;
        sizetmp.y = x;
        rawImage.rectTransform.sizeDelta = sizetmp;
    }

    // Update is called once per frame
    async void Update()
    {
       await FirstTask();
    }
    
    async UniTask<Texture2D> TextureCopy(WebCamTexture webCamTexture)
    {
        while (webCamTexture.height <= 16 && webCamTexture.width <= 16)
        {
            await UniTask.DelayFrame(1);
        }
        Texture2D copyed = new Texture2D(webCamTexture.width, webCamTexture.height);
        _color32s = new Color32[webCamTexture.height * webCamTexture.width];
        webCamTexture.GetPixels32(_color32s);
        copyed.SetPixels32(_color32s);
        copyed.Apply();
        Debug.Log("copyed");
        return copyed;

    }

   async UniTask  FirstTask()
   {
       timer += Time.deltaTime;
       if(timer>interval)
       {
           timer = 0.0f;
            var copyedTexture = await TextureCopy(frontCamTextrue);
            // Destroy(frontCamTextrue);
            ConvertTex(copyedTexture, backImage.texture);
            
        }
   }

   private void ConvertTex(Texture2D src, Texture dst)
   {
       Debug.Log("converted");
       Graphics.ConvertTexture(src, dst);
   }
}