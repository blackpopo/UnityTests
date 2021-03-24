using System.Collections;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class CamTest3 : MonoBehaviour
{
        // FaceAPIのテスト設計
        // 1背景画面を映す
        // 2 画面の回転
        // 2 5sごとにループを回す
        // 3 背景の画像をTexture2Dに保存
        // 4 保存したTextureを背景イメージと入れ替え
        // 5　カメラの切り替え
        // 6 Face の撮影
        // 7 撮影したcolors32を回転
        // 8 png に保存
        // 9 カメラの切り替え
        // 10 ? ここで回転の必要あるのかな？
        
    //Private
    WebCamDevice[] webCamDevices;
    int frontHeight = 1080;
    int frontWidth = 1920;
    int backHeight = 1080;
    int backWidth = 1920;
    int frontIndex;
    int backIndex;
    int frontRotate;
    int backRotate;
    int fps = 30;

    WebCamTexture frontCamTexture;
    WebCamTexture backCamTexture;

    Color32[] _color32s;

    float timer;

    float interval = 5.0f;
    
    //Serialize Fileds
    [SerializeField] RawImage frontImage;
    [SerializeField] RawImage backImage;
    [SerializeField] RawImage pngImage;
    [SerializeField] RawImage staticBackImage;
    [SerializeField, Range(0, 1)] int HeightOrWidth; 
    
    
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Init");
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
            Debug.Log(string.Format("WebCam Devices {0} th is {1}", i, webCamDevices[i].name ));
            if (webCamDevices[i].isFrontFacing)
            {
                frontIndex = i;
                frontCamTexture = new WebCamTexture(webCamDevices[i].name, frontWidth, frontHeight);
                Graphics.ConvertTexture(frontCamTexture, frontImage.texture);
            }
            else
            {
                backIndex = i;
                backCamTexture = new WebCamTexture(webCamDevices[i].name, backWidth, backHeight);
                Graphics.ConvertTexture(backCamTexture, frontImage.texture);
            }
        }
        pngImage.texture = new  Texture2D(1920, 1080);
        backImage.texture = backCamTexture;
        backCamTexture.Play();
        StartCoroutine(RotateRawImage(backCamTexture, backImage, false));
    }
    IEnumerator RotateRawImage(WebCamTexture webCamTexture, RawImage rawImage, bool isFront)
    {
        while (webCamTexture.height <= 16 && webCamTexture.height <= 16)
        {
            yield return null;
        }
        
        
        // アスペクト比、何度回転させれば正しく表示されるかがわかる
        if ((webCamTexture.videoRotationAngle == 90) || (webCamTexture.videoRotationAngle == 270)) {
            // 表示するRawImageを回転させる
            Vector3 angles = rawImage.rectTransform.eulerAngles;
            angles.z = -webCamTexture.videoRotationAngle;
            rawImage.rectTransform.localEulerAngles = angles;
        }
        
        if (isFront)
        {
            frontRotate = webCamTexture.videoRotationAngle;
            frontHeight = webCamTexture.height;
            frontWidth = webCamTexture.width;
            // Debug.Log(string.Format("Front Information width: {0} height: {1} rotate: {2}", frontWidth, frontHeight, frontRotate));
        }
        else
        {
            backRotate = webCamTexture.videoRotationAngle;
            backHeight = webCamTexture.height;
            backWidth = webCamTexture.width;
            // Debug.Log(string.Format("Back Information width: {0} height: {1} rotate: {2}",  backWidth, backHeight, backRotate));
            staticBackImage.texture = new Texture2D(backHeight, backWidth);
        }
    }

    // Update is called once per frame
    async void Update()
    {
        await IntervalTask();
    }

    float tempTimer = 0.0f;
    float tempInterval = 5.0f;
    async UniTask IntervalTask()
    {
        timer += Time.deltaTime;
        if (timer > interval)
        {

            Debug.Log("Back Image" + backImage.rectTransform.transform.localScale);
            Debug.Log("Static Image" + staticBackImage.rectTransform.transform.localScale);
            Debug.Log("Start!");
            Texture2D copyed;
            copyed = await CopyTexture(backCamTexture, false);
            
            ConvertTex(copyed, staticBackImage.texture);
            // DestroyImmediate(copyed);
            backCamTexture.Stop();
            backImage.enabled = false;
            while (tempTimer < tempInterval)
            {
                tempTimer += Time.deltaTime;
            }

            tempTimer = 0.0f;
            timer = 0.0f;
            // frontImage.texture = frontCamTexture;
            // frontCamTexture.Play();
            // RotateRawImage(frontCamTexture, frontImage, true);
            // copyed = await CopyTexture(frontCamTexture, true);
            // //もうFrontCamはいらんか。。。
            // frontCamTexture.Stop();
            backImage.enabled = true;
            //WebCamtextureだとは認識しているけど…
            backCamTexture.Play();
            // PngSaver(copyed);
            // // DestroyImmediate(copyed);
            // copyed = PngLoader(frontWidth, frontHeight);
            // pngImage.texture = copyed;
            Destroy(copyed);
        }
    }

    private void ConvertTex(Texture2D src, Texture dst)
    {
        Debug.Log(string.Format("Convertiong Info \n Src size width: {0} height :{1} \n Dst size width: {2} height: {3}",
            src.width, src.height, dst.width, dst.height));
        Graphics.ConvertTexture(src, dst);
    }
    
    async UniTask<Texture2D> CopyTexture(WebCamTexture webCamTexture, bool isFront)
    {
        while (webCamTexture.height <= 16 && webCamTexture.width <= 16)
        {
            await UniTask.DelayFrame(1);
        }

        int height;
        int width;
        int rotate;
        if (isFront)
        {
            height = frontHeight;
            width = frontWidth; 
            rotate = frontRotate;
        }
        else
        {
            height = backHeight;
            width = backWidth; 
            rotate = backRotate;
        }
        Texture2D copyed = new Texture2D(width, height);
        _color32s = new Color32[width * height];
        webCamTexture.GetPixels32(_color32s);
        // _color32s = RotateColor32s(_color32s, width, height, rotate);
        copyed.SetPixels32(_color32s);
        copyed.Apply();
        Texture2D resTex2d = RotateTexture2D(copyed, rotate); 
        return resTex2d;
    }

    private Texture2D RotateTexture2D(Texture2D originalTexture, int rotate)
    {
        if (!(rotate == 90 || rotate == 270))
        {
            return originalTexture;
        }

        bool clockwise = rotate == 90 ? true : false;
        Color32[] original = originalTexture.GetPixels32();
        Color32[] rotated = new Color32[original.Length];
        int w = originalTexture.width;
        int h = originalTexture.height;

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

    private void PngSaver(Texture2D texture)
    {
        // Encode
        byte[] bin = texture.EncodeToPNG();
        // Encodeが終わったら削除
        Object.Destroy(texture);

        // ファイルを保存
        #if UNITY_ANDROID
        File.WriteAllBytes(Application.persistentDataPath + "/test.png", bin);
        #else
        File.WriteAllBytes(Application.dataPath + "/test.jpg", bin);
        #endif
    }

    private Texture2D PngLoader(int width, int height)
    {
        string path = Application.persistentDataPath + "/test.png";
        BinaryReader bin = new BinaryReader(new FileStream(path, FileMode.Open, FileAccess.Read));
        byte[] bytes = bin.ReadBytes((int)bin.BaseStream.Length);
        bin.Close();
        Texture2D texture = new Texture2D(width, height);
        texture.LoadImage(bytes);
        return texture;
    }
    
}
