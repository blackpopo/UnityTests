using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    // Start is called before the first frame update
    string file_path;
    public SaveData _saveData;
    
    public void setFileName(string file_name)
    {
        _saveData = new SaveData();
        Debug.Log(_saveData);
        file_path = Application.persistentDataPath + "/." + file_name;
        Debug.Log("File_path is " + file_path);
        Debug.Log(File.Exists(file_path));
        if (!File.Exists(file_path))
        {
            Save();
        }
    }

    public void Save()
    {
        string json = JsonUtility.ToJson(_saveData);
        Debug.Log("_saveData " + _saveData);
        Debug.Log("json " + json);
        StreamWriter streamWriter = new StreamWriter(file_path);
        streamWriter.Write(json);
        streamWriter.Flush();
        streamWriter.Close();
        Debug.Log("SAVING is Finished...");
    }

    public void Load()
    {
        if (File.Exists(file_path))
        {
            StreamReader streamReader;
            streamReader = new StreamReader(file_path);
            string json = streamReader.ReadToEnd();
            Debug.Log("_saveData " + _saveData);
            Debug.Log("json " + json);
            streamReader.Close();
            _saveData = JsonUtility.FromJson<SaveData>(json);
            Debug.Log("Loading is Finished...");
        }
    }
}
