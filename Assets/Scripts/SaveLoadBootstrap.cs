using UnityEngine;
using System.Collections;

/// <summary>
/// Bootstrap để đảm bảo SaveSystem load dữ liệu trước khi Enemy kiểm tra
/// Chạy sớm nhất trong scene để đồng bộ hóa quá trình load
/// </summary>
public class SaveLoadBootstrap : MonoBehaviour
{
    public static bool IsDataLoaded { get; private set; } = false;
    public static SaveLoadBootstrap Instance { get; private set; }
    
    [Header("Bootstrap Settings")]
    [SerializeField] private bool autoLoadOnStart = true;
    
    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            
            // Đảm bảo chạy sớm nhất
            StartCoroutine(BootstrapSaveLoad());
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private IEnumerator BootstrapSaveLoad()
    {
        IsDataLoaded = false;

        // Đợi SaveSystem khởi tạo
        while (SaveSystem.Instance == null)
        {
            yield return null;
        }

        // Đợi SceneLoadManager kiểm tra và load scene đúng (nếu cần)
        if (SceneLoadManager.Instance != null)
        {
            // Đợi một chút để SceneLoadManager hoàn thành việc kiểm tra scene
            yield return new WaitForSeconds(0.1f);
        }

        // Load dữ liệu nếu có save file
        if (autoLoadOnStart && SaveSystem.Instance.HasSaveFile())
        {
            GameData loadedData = SaveSystem.Instance.LoadGame();
            if (loadedData != null)
            {
                // Set danh sách enemy đã giết vào SaveSystem
                SaveSystem.Instance.SetEnemyKilledList(loadedData.enemyKilledList);
            }
        }

        // Đánh dấu dữ liệu đã load xong
        IsDataLoaded = true;

        // Thông báo cho tất cả Enemy rằng có thể kiểm tra trạng thái
        NotifyEnemiesDataReady();
    }
    
    private void NotifyEnemiesDataReady()
    {
        // Tìm tất cả Enemy trong scene và thông báo dữ liệu đã sẵn sàng
        Enemy[] allEnemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
        foreach (Enemy enemy in allEnemies)
        {
            enemy.OnSaveDataReady();
        }
    }
    
    /// <summary>
    /// Đợi cho đến khi dữ liệu save được load xong
    /// </summary>
    public static IEnumerator WaitForDataLoad()
    {
        while (!IsDataLoaded)
        {
            yield return null;
        }
    }
    
    /// <summary>
    /// Reset trạng thái khi bắt đầu game mới
    /// </summary>
    public static void ResetDataLoadedFlag()
    {
        IsDataLoaded = false;
    }
}
