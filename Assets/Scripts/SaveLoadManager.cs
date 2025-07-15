using UnityEngine;

/// <summary>
/// Manager để đảm bảo SaveLoadBootstrap luôn có trong scene
/// Tự động tạo nếu chưa có
/// </summary>
public class SaveLoadManager : MonoBehaviour
{
    [Header("Auto Setup")]
    [SerializeField] private bool autoCreateBootstrap = true;
    [SerializeField] private bool autoCreateSaveSystem = true;
    [SerializeField] private bool autoCreateSceneLoadManager = true;

    void Awake()
    {
        // Đảm bảo có SaveSystem
        if (autoCreateSaveSystem && SaveSystem.Instance == null)
        {
            CreateSaveSystem();
        }

        // Đảm bảo có SceneLoadManager
        if (autoCreateSceneLoadManager && SceneLoadManager.Instance == null)
        {
            CreateSceneLoadManager();
        }

        // Đảm bảo có SaveLoadBootstrap
        if (autoCreateBootstrap && SaveLoadBootstrap.Instance == null)
        {
            CreateSaveLoadBootstrap();
        }
    }
    
    private void CreateSaveSystem()
    {
        GameObject saveSystemObj = new GameObject("SaveSystem");
        saveSystemObj.AddComponent<SaveSystem>();
        DontDestroyOnLoad(saveSystemObj);
    }
    
    private void CreateSceneLoadManager()
    {
        GameObject sceneLoadObj = new GameObject("SceneLoadManager");
        sceneLoadObj.AddComponent<SceneLoadManager>();
        DontDestroyOnLoad(sceneLoadObj);
    }

    private void CreateSaveLoadBootstrap()
    {
        GameObject bootstrapObj = new GameObject("SaveLoadBootstrap");
        bootstrapObj.AddComponent<SaveLoadBootstrap>();
    }
    
    /// <summary>
    /// Gọi để đảm bảo hệ thống save/load hoạt động
    /// </summary>
    [ContextMenu("Setup Save Load System")]
    public void SetupSaveLoadSystem()
    {
        if (SaveSystem.Instance == null)
        {
            CreateSaveSystem();
        }

        if (SceneLoadManager.Instance == null)
        {
            CreateSceneLoadManager();
        }

        if (SaveLoadBootstrap.Instance == null)
        {
            CreateSaveLoadBootstrap();
        }

        Debug.Log("Save/Load system setup completed!");
    }
}
