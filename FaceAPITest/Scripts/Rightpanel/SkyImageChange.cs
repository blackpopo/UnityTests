using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkyImageChange : MonoBehaviour
{
    [SerializeField] Material Sky;

    [SerializeField] RawImage Image;

    [SerializeField] Texture _initTextrue;

    [SerializeField] bool isImage;

    [SerializeField] Text text;
    
    // Start is called before the first frame update
    void Start()
    {
        Image.texture = _initTextrue;
        if (isImage)
        {
            Image.enabled = true;
            RenderSettings.skybox = null;
            text.text = "Image";
        }
        else
        {
            Image.enabled = false;
            RenderSettings.skybox = Sky;
            text.text = "Skybox";
        }
        
    }

    public void buttonOnClick()
    {
        //Image の状態のとき　skybox をあてはめて、rawimage　をfalseにする。
        if (RenderSettings.skybox == null)
        {
            RenderSettings.skybox = Sky;
            Image.enabled = false;
            text.text = "Skybox";
        }
        //Skybox　のときはImageを有効化した後、 skybox を保存する。
        else
        {
            Image.enabled = true;
            Sky = RenderSettings.skybox;
            RenderSettings.skybox = null;
            text.text = "Image";
        }
    }

    // Update is called once per frame

}
