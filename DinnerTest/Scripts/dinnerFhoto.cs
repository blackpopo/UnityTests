using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using NatSuite.Devices;
using UnityEngine;
using UnityEngine.UI;

public class dinnerFhoto : MonoBehaviour
{

    [SerializeField] Button decisionButton;
    [SerializeField] Button oneMoreButton;
    [SerializeField] Button captureButton;
    
    [Header("Camera Preview")]
    public RawImage previewPanel;
    public AspectRatioFitter previewAspectFitter;

    [Header("Photo Capture")]
    public RawImage photoPanel;
    public AspectRatioFitter photoAspectFitter;
    MediaDeviceQuery query;

    public Texture2D photoTexture = null;

    CameraDevice device;
    #region --Setup--

    async void Start ()
    {
        //initialization
        photoPanel.gameObject.SetActive(false);
        captureButton.gameObject.SetActive(true);
        decisionButton.gameObject.SetActive(false);
        oneMoreButton.gameObject.SetActive(false);
        
        // Request camera permissions
        // if (!await MediaDeviceQuery.RequestPermissions<CameraDevice>()) {
        //     Debug.LogError("User did not grant camera permissions");
        //     return;
        // }
        // Create a device query for device cameras
        // Use `GenericCameraDevice` so we also capture WebCamTexture cameras
        var criterion = MediaDeviceQuery.Criteria.RearFacing;
        var query = new MediaDeviceQuery(criterion);
        device = query.currentDevice as CameraDevice;
        var previewTexture = await device.StartRunning();
        Debug.Log($"Started camera preview with resolution {previewTexture.width}x{previewTexture.height}");
        // Display preview texture
        previewPanel.texture = previewTexture;
        previewAspectFitter.aspectRatio = previewTexture.width / (float)previewTexture.height;
        // Set UI state
  }
    #endregion

    public void DecideButton()
    {
        decisionButton.gameObject.SetActive(false);
        oneMoreButton.gameObject.SetActive(false);
        captureButton.gameObject.SetActive(false);
    }

    public void OneMoreButton()
    {
        photoPanel.gameObject.SetActive(false);
        captureButton.gameObject.SetActive(true);
        decisionButton.gameObject.SetActive(false);
        oneMoreButton.gameObject.SetActive(false);
        Destroy(photoTexture);
    }

    public void CaptureButton()
    {
        // if (photoTexture != null)
        // {
        //     Destroy(photoTexture);
        // }
        CapturePhoto();
        captureButton.gameObject.SetActive(false);
        decisionButton.gameObject.SetActive(true);
        oneMoreButton.gameObject.SetActive(true);

    }
    
    public async void CapturePhoto () {
        // Only `CameraDevice` supports capturing photos, not `ICameraDevice`
        // if (query.currentDevice is CameraDevice device) {
        //     // Capture photo
            photoTexture = await device.CapturePhoto();
            Debug.Log($"Captured photo with resolution {photoTexture.width}x{photoTexture.height}");
            // Display photo texture for a few seconds
            photoPanel.gameObject.SetActive(true);
            photoPanel.texture = photoTexture;
            photoAspectFitter.aspectRatio = photoTexture.width / (float)photoTexture.height;
        // }
    }

}
