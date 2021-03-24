using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserNameInput : MonoBehaviour
{
    [SerializeField] TMP_InputField _user_name_field = null;
    [SerializeField] TextMeshProUGUI display_text = null;
    string file_path; 
    SaveManager _saveManager;
    string user_name;

    [SerializeField] string file_name = null;

    public void Start()
    {
        _saveManager = GetComponent<SaveManager>();
        _saveManager.setFileName(file_name + ".json");
        _saveManager.Load();
        Debug.Log(_saveManager._saveData);
        user_name = _saveManager._saveData.user_name;
        if (user_name != "")
        {
            display_text.text = "ようこそ！\n" +  user_name + "さん！";
        }
    }

    public void OnClick()
    {
        Debug.Log(_user_name_field);
        Debug.Log("_user_name_filed: " + _user_name_field.text);
        Debug.Log("display_text: "+ display_text.text);
        if (_user_name_field.text != "" && _user_name_field.text != null)
        {
            display_text.text = "新しいユーザーネームは\n" +  _user_name_field.text + "さんですね！";
            _saveManager._saveData.user_name = _user_name_field.text;
            _saveManager.Save();
        }
        else
        {
            display_text.text = "ユーザーネームは空白なのはダメ！";
        }

    }
    
}
