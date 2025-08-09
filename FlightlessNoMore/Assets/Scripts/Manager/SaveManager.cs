using System.IO;
using UnityEditor.Overlays;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    public SaveData CurrentSaveData { get; private set; }

    private string SavePath => Application.persistentDataPath + "/save.json";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Load();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Load()
    {
        if (File.Exists(SavePath))
        {
            string json = File.ReadAllText(SavePath);
            CurrentSaveData = JsonUtility.FromJson<SaveData>(json);
        }
        else
        {
            CurrentSaveData = new SaveData(); //新規データを作成
        }
    }

    public void Save()
    {
        Debug.Log("保存先：" + Application.persistentDataPath);
        string json = JsonUtility.ToJson(CurrentSaveData, true);
        File.WriteAllText(SavePath, json);
    }
}
