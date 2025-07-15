using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem Instance { get; private set; }

    private const string SAVE_FILE_NAME = "JumpQuestSave.json";
    private string SaveFilePath => Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME);

    private List<string> enemyKilledList = new List<string>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeSaveSystem();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeSaveSystem()
    {
        string directory = Path.GetDirectoryName(SaveFilePath);
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        // Khởi tạo danh sách enemy đã giết
        enemyKilledList = new List<string>();
    }

    public bool SaveGame(GameData data)
    {
        if (data == null || !data.IsValid())
            return false;

        try
        {
            string jsonData = JsonUtility.ToJson(data, true);
            File.WriteAllText(SaveFilePath, jsonData);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public GameData LoadGame()
    {
        if (!HasSaveFile())
            return null;

        try
        {
            string jsonData = File.ReadAllText(SaveFilePath);
            if (string.IsNullOrEmpty(jsonData))
                return null;

            GameData data = JsonUtility.FromJson<GameData>(jsonData);
            return (data != null && data.IsValid()) ? data : null;
        }
        catch
        {
            return null;
        }
    }

    public bool DeleteSave()
    {
        try
        {
            if (File.Exists(SaveFilePath))
                File.Delete(SaveFilePath);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public bool HasSaveFile()
    {
        return File.Exists(SaveFilePath);
    }

    public void AddKilledEnemy(string enemyID)
    {
        if (!enemyKilledList.Contains(enemyID))
        {
            enemyKilledList.Add(enemyID);
        }
    }

    public bool IsEnemyKilled(string enemyID)
    {
        return enemyKilledList.Contains(enemyID);
    }

    public List<string> GetEnemyKilledList()
    {
        return new List<string>(enemyKilledList);
    }

    public void SetEnemyKilledList(List<string> killedEnemies)
    {
        enemyKilledList.Clear();
        if (killedEnemies != null)
        {
            enemyKilledList.AddRange(killedEnemies);
        }
    }

    public void ClearEnemyKilledList()
    {
        enemyKilledList.Clear();
    }
}
