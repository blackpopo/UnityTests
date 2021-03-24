using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ChangeSky : MonoBehaviour
{
    [SerializeField] Text _buttonText;
    string[] _skyNames;
    string _skyPath;

    // Start is called before the first frame update
    void Start()
    {
        _skyPath = Application.dataPath + "/Scenes/FaceAPITest/Resources/Sky";
        _skyNames = Directory.GetFiles(_skyPath, "*.mat");
        for (int i = 0; i < _skyNames.Length; i++)
        {
            _skyNames[i] = Path.GetFileNameWithoutExtension(_skyNames[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ButtonOnClick()
    {
        var num = (int)Random.Range(0.0f, _skyNames.Length);
        _buttonText.text = _skyNames[num];
        Debug.Log("num: :" + num + " Sky name " + _skyNames[num]);
        Material skybox = (Material)Resources.Load("Sky/" + _skyNames[num]);
        RenderSettings.skybox = skybox;
    }
}
