using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FileManager : MonoBehaviour
{
    [Tooltip("Path to file where KeyCodes of player inputs are stored.")]
    [SerializeField] private string playerInputPath;
    [SerializeField] private string settingsPath;
    public static FileManager instance;
    private void Awake() {
        if(instance != null)
        {
            Destroy(this);
        }
        else
            instance = this;
        DontDestroyOnLoad(this);
    }
    public string ReadFromPlayerInputFile()
    {
        return ReadFromFile(playerInputPath);
    }
    public string ReadFromSettingsFile()
    {
        return ReadFromFile(settingsPath);
    }
    public void SaveToPlayerInputFile(KeycodeManager keycodeManager)
    {
        SaveToFile(playerInputPath, keycodeManager.ToJSON());
    }
    public void SaveToSettingsFile(Settings settings)
    {
        SaveToFile(settingsPath, JsonUtility.ToJson(settings));
    }
    private string ReadFromFile(string path){
        try{
        string jsonString = System.IO.File.ReadAllText(Application.persistentDataPath + path);
        return jsonString;
        }catch(Exception e){
            // if some kind of exception is thrown return empty string so keycodeManager will know that it won't be using inputs from file
            Debug.Log(e.Message);
            return "";
        }
    }
    private void SaveToFile(string path, string textToSave){
        try{
        Debug.Log(textToSave);
        System.IO.File.WriteAllText(Application.persistentDataPath + path, textToSave);
        }catch(Exception e){
            Debug.Log(e.Message);
        }
    }
}
