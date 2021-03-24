using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class CameraAnalysis : MonoBehaviour
{
    
    WebCamDevice[] webCamDevices;
    WebCamTexture webCamTexture;
    RawImage rawImage;
    int camIndex;
    // Start is called before the first frame update
    async void Start()
    {
        rawImage = this.gameObject.GetComponent<RawImage>();
        await Init();
    }

    // Update is called once per frame
    async void Update()
    {
        await GetInformation();
    }

    async UniTask Init()
    {
        webCamDevices = WebCamTexture.devices;
        while (webCamDevices.Length < 1)
        {
            await UniTask.DelayFrame(1);
        }
        for (int i=0; i<webCamDevices.Length; i++ ){
            if (webCamDevices[i].isFrontFacing)
            {
                Debug.Log("Front Facing Texture");
                webCamTexture = new WebCamTexture(webCamDevices[i].name);
                camIndex = i;
                Debug.Log(webCamDevices[i].name);
                rawImage.texture = webCamTexture;
                DebugInformation(webCamTexture, webCamDevices[i]);
            }
            else
            {
                Debug.Log("Rear Facing Texture");
                DebugInformation(new WebCamTexture(webCamDevices[i].name), webCamDevices[i]);
            }
        }

        webCamTexture.Play();
    }

    async UniTask GetInformation()
    {
        while (webCamTexture.height <= 16 && webCamTexture.width <= 16)
        {
            await UniTask.DelayFrame(1);
        }
        DebugInformation(webCamTexture, webCamDevices[camIndex]);
        
    }

    private void DebugInformation(WebCamTexture webCamTexture, WebCamDevice webCamDevice)
    {
        Debug.Log("Width: Height" + webCamTexture.width + " : " + webCamTexture.height);
        Debug.Log("Kind :" + webCamDevice.kind);
        Debug.Log("Resolution :" + webCamDevice.availableResolutions);
        Debug.Log("Name :" + webCamDevice.name);
        Debug.Log("Depth Camera Name : " + webCamDevice.depthCameraName);
        Debug.Log("Mirrored :" + webCamTexture.videoVerticallyMirrored);
        Debug.Log("Is depth" + webCamTexture.isDepth);
    }

    private void CameraRotate()
    {
        
    }
}
