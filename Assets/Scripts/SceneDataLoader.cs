using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Component để load game data sau khi scene đã được load
/// Đảm bảo data được apply đúng trong scene đã lưu
/// </summary>
public class SceneDataLoader : MonoBehaviour
{
    [Header("Scene Data Loading")]
    [SerializeField] private bool autoLoadDataOnStart = true;
    [SerializeField] private float loadDelay = 0.1f; // Delay để đảm bảo scene đã setup xong
    
    void Start()
    {
        if (autoLoadDataOnStart)
        {
            StartCoroutine(LoadSceneDataAfterDelay());
        }
    }
    
    /// <summary>
    /// Load game data sau một khoảng delay
    /// </summary>
    private IEnumerator LoadSceneDataAfterDelay()
    {
        // Đợi một chút để scene setup hoàn tất
        yield return new WaitForSeconds(loadDelay);
        
        // Đợi SaveSystem khởi tạo
        while (SaveSystem.Instance == null)
        {
            yield return null;
        }
        
        // Kiểm tra có save file không
        if (!SaveSystem.Instance.HasSaveFile())
        {
            yield break;
        }
        
        // Load save data
        GameData saveData = SaveSystem.Instance.LoadGame();
        if (saveData == null)
        {
            yield break;
        }
        
        string currentScene = SceneManager.GetActiveScene().name;
        
        // Chỉ load data nếu đang ở đúng scene đã lưu
        if (currentScene == saveData.currentScene)
        {
            LoadGameDataInCurrentScene();
        }
    }
    
    /// <summary>
    /// Load game data trong scene hiện tại
    /// </summary>
    public void LoadGameDataInCurrentScene()
    {
        GameManager gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager != null)
        {
            gameManager.LoadGameDataOnly();
            Debug.Log($"Game data loaded in scene: {SceneManager.GetActiveScene().name}");
        }
        else
        {
            Debug.LogWarning("GameManager not found in scene!");
        }
    }
    
    /// <summary>
    /// Kiểm tra xem scene hiện tại có khớp với save data không
    /// </summary>
    public bool IsCurrentSceneMatchingSaveData()
    {
        if (SaveSystem.Instance == null || !SaveSystem.Instance.HasSaveFile())
        {
            return false;
        }
        
        GameData saveData = SaveSystem.Instance.LoadGame();
        if (saveData == null)
        {
            return false;
        }
        
        string currentScene = SceneManager.GetActiveScene().name;
        return currentScene == saveData.currentScene;
    }
    
    /// <summary>
    /// Force load game data (dùng khi cần thiết)
    /// </summary>
    [ContextMenu("Force Load Game Data")]
    public void ForceLoadGameData()
    {
        LoadGameDataInCurrentScene();
    }
}
