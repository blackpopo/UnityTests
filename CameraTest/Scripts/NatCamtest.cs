using System.Collections;
using System.Collections.Generic;
using NatSuite.Devices;
using UnityEngine;
using UnityEngine.UI;

//予想通りクラッシュしましたｗｗｗ
//もういいや

public class NatCamtest : MonoBehaviour
{
    [SerializeField] RawImage BGImage;
    [SerializeField] RawImage CaptureImgage;
    [SerializeField] AspectRatioFitter aspectRatioFitter;

    CameraDevice frontDevice;
    CameraDevice rearDevice;


    // Start is called before the first frame update
    async void Start()
    {
        var Fquery = new MediaDeviceQuery(MediaDeviceQuery.Criteria.FrontFacing);
        frontDevice = (CameraDevice)Fquery.currentDevice;
        var Rquery = new MediaDeviceQuery(MediaDeviceQuery.Criteria.RearFacing);
        rearDevice = (CameraDevice)Rquery.currentDevice;

        var previewTexture = await rearDevice.StartRunning();
        BGImage.texture = previewTexture;
    }

    // 5秒ごとにCapture
    float timer = 0.0f;

    float interval = 5.0f;
    // Update is called once per frame
    async void Update()
    {
        timer += Time.deltaTime;
        if (timer > interval)
        {
            timer = 0.0f;
            var capturePhoto= await rearDevice.CapturePhoto();
            BGImage.texture = capturePhoto;
            var previewTexture = await frontDevice.CapturePhoto();
            CaptureImgage.texture = previewTexture;
        }
    }
}
