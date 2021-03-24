using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ChangeImages : MonoBehaviour
{
    [SerializeField] Text _buttonText;
    string[] _imageNames;
    
    string _imagePath;
    
    [SerializeField] RawImage image;

    // Start is called before the first frame update
    void Start()
    {
        _imagePath = Application.dataPath + "/Scenes/FaceAPITest/Resources/Landscape";
        _imageNames = Directory.GetFiles(_imagePath, "*.jpg");
        for (int i = 0; i < _imageNames.Length; i++)
        {
            _imageNames[i] = Path.GetFileNameWithoutExtension(_imageNames[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ButtonOnClick()
    {
        var num = (int)Random.Range(0.0f, _imageNames.Length);
        _buttonText.text = _imageNames[num];
        Debug.Log("num: :" + num + " Image name " + _imageNames[num]);
        Texture landscape = (Texture)Resources.Load("Landscape/" + _imageNames[num]);
        image.texture = landscape;
    }
}
