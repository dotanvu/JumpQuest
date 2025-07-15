using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Quản lý việc load scene đúng khi khởi động game
/// Đảm bảo game load đúng màn chơi đã lưu
/// </summary>
public class SceneLoadManager : MonoBehaviour
{
    public static SceneLoadManager Instance { get; private set; }
    
    [Header("Scene Management")]
    [SerializeField] private string defaultScene = "Game"; // Scene mặc định khi bắt đầu game mới
    [SerializeField] private bool autoLoadCorrectScene = true;
    
    private bool hasCheckedSceneLoad = false;
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    
    void Start()
    {
        if (autoLoadCorrectScene && !hasCheckedSceneLoad)
        {
            StartCoroutine(CheckAndLoadCorrectScene());
        }
    }
    
    /// <summary>
    /// Kiểm tra và load scene đúng dựa trên save data
    /// </summary>
    private IEnumerator CheckAndLoadCorrectScene()
    {
        hasCheckedSceneLoad = true;
        
        // Đợi SaveSystem khởi tạo
        while (SaveSystem.Instance == null)
        {
            yield return null;
        }
        
        // Kiểm tra có save file không
        if (!SaveSystem.Instance.HasSaveFile())
        {
            // Không có save file, ở lại scene hiện tại
            yield break;
        }
        
        // Load save data
        GameData saveData = SaveSystem.Instance.LoadGame();
        if (saveData == null || string.IsNullOrEmpty(saveData.currentScene))
        {
            // Save data không hợp lệ, ở lại scene hiện tại
            yield break;
        }
        
        string currentScene = SceneManager.GetActiveScene().name;
        string savedScene = saveData.currentScene;
        
        // Nếu scene hiện tại khác với scene đã lưu, chuyển đến scene đúng
        if (currentScene != savedScene)
        {
            Debug.Log($"Loading correct scene: {savedScene} (current: {currentScene})");
            yield return LoadSceneAsync(savedScene);
        }
    }
    
    /// <summary>
    /// Load scene bất đồng bộ
    /// </summary>
    public IEnumerator LoadSceneAsync(string sceneName)
    {
        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogWarning("Scene name is empty!");
            yield break;
        }
        
        // Kiểm tra scene có tồn tại không
        if (!IsSceneInBuildSettings(sceneName))
        {
            Debug.LogWarning($"Scene '{sceneName}' not found in build settings!");
            yield break;
        }
        
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        
        Debug.Log($"Scene '{sceneName}' loaded successfully");
    }
    
    /// <summary>
    /// Kiểm tra scene có trong build settings không
    /// </summary>
    private bool IsSceneInBuildSettings(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneNameInBuild = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            
            if (sceneNameInBuild == sceneName)
            {
                return true;
            }
        }
        return false;
    }
    
    /// <summary>
    /// Load scene và save game trước khi chuyển
    /// </summary>
    public void LoadSceneWithSave(string sceneName)
    {
        StartCoroutine(LoadSceneWithSaveCoroutine(sceneName));
    }
    
    private IEnumerator LoadSceneWithSaveCoroutine(string sceneName)
    {
        // Save game trước khi chuyển scene
        GameManager gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager != null)
        {
            gameManager.SaveGame();
        }
        
        // Đợi một frame để đảm bảo save hoàn tất
        yield return null;
        
        // Load scene mới
        yield return LoadSceneAsync(sceneName);
    }
    
    /// <summary>
    /// Reset về scene mặc định (dùng khi New Game)
    /// </summary>
    public void LoadDefaultScene()
    {
        StartCoroutine(LoadSceneAsync(defaultScene));
    }
    
    /// <summary>
    /// Lấy tên scene hiện tại
    /// </summary>
    public string GetCurrentSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }
    
    /// <summary>
    /// Kiểm tra xem có đang ở scene mặc định không
    /// </summary>
    public bool IsInDefaultScene()
    {
        return GetCurrentSceneName() == defaultScene;
    }
}
