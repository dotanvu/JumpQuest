using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static int score = 0;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private GameObject gameOverUi;
    [SerializeField] private GameObject gameWinUi;
    public Transform player;

    private bool isGameOver = false;
    private bool isGameWin = false;
    public Vector3 checkpointPosition;
    public bool hasCheckpoint = false;

    [Header("Save/Load Settings")]
    [SerializeField] private bool autoLoadOnStart = true;

    void Start()
    {
        isGameOver = false;
        isGameWin = false;
        Time.timeScale = 1;

        UpdateScore();
        if (gameOverUi != null) gameOverUi.SetActive(false);
        if (gameWinUi != null) gameWinUi.SetActive(false);

        // Tìm player reference
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }

        StartCoroutine(InitializeGameAfterFrame());
    }

    public void AddScore(int points)
    {
        if (!isGameOver && !isGameWin)
        {
            score += points;
            UpdateScore();
        }
    }

    public void UpdateScore()
    {
        if (scoreText != null)
        {
            scoreText.text = score.ToString();
        }
    }

    public void SetCheckpoint(Vector3 position)
    {
        checkpointPosition = position;
        hasCheckpoint = true;
    }

    public void RespawnPlayer()
    {
        if (player != null)
        {
            if (hasCheckpoint)
            {
                player.position = checkpointPosition;
            }
            else
            {
                player.position = new Vector3(0, 0, 0); // Vị trí mặc định nếu chưa có checkpoint
            }
        }
    }

    public void GameOver()
    {
        isGameOver = true;

        if (scoreText != null)
        {
            score = 0;
            UpdateScore();
        }

        Time.timeScale = 0;

        if (gameOverUi != null)
        {
            gameOverUi.SetActive(true);
            Debug.Log("Game Over UI activated!");
        }
        else
        {
            Debug.LogError("Game Over UI is null! Please assign it in the inspector.");
        }

        // Reset coin sẽ được gọi khi restart, không cần reset ngay ở đây
    }

    public void GameWin()
    {
        isGameWin = true;
        Time.timeScale = 0;

        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == "Game")
        {
            // Auto-save trước khi chuyển sang Map2
            SaveGame();

            Time.timeScale = 1;
            LoadNextScene("Map2");
        }
        else if (currentScene == "Map2")
        {
            // Auto-save trước khi chuyển sang Map3
            SaveGame();

            Time.timeScale = 1;
            LoadNextScene("Map3");
        }
        else if (currentScene == "Map3")
        {
            // Auto-save khi hoàn thành game
            SaveGame();

            if (gameWinUi != null)
            {
                gameWinUi.SetActive(true);
            }
        }
    }

    /// <summary>
    /// Load scene tiếp theo với SceneLoadManager
    /// </summary>
    private void LoadNextScene(string sceneName)
    {
        if (SceneLoadManager.Instance != null)
        {
            SceneLoadManager.Instance.LoadSceneWithSave(sceneName);
        }
        else
        {
            // Fallback nếu không có SceneLoadManager
            SceneManager.LoadScene(sceneName);
        }
    }

    public void RestartGame()
    {
        isGameOver = false;
        isGameWin = false;
        score = 0;
        hasCheckpoint = false;
        Time.timeScale = 1;

        // Reset hoàn toàn cho restart (xóa tất cả trừ save data)
        ResetForNewGame();

        Debug.Log("RestartGame: Complete reset for new game");

        if (gameOverUi != null) gameOverUi.SetActive(false);
        if (gameWinUi != null) gameWinUi.SetActive(false);

        // Load scene sau khi đã reset
        SceneManager.LoadScene("Game");
    }

    public void GoToMenu()
    {
        isGameOver = false;
        isGameWin = false;
        score = 0;
        hasCheckpoint = false;
        Time.timeScale = 1;

        SceneManager.LoadScene("Menu");
    }

    public bool IsGameOver()
    {
        return isGameOver;
    }

    public bool IsGameWin()
    {
        return isGameWin;
    }

    private System.Collections.IEnumerator InitializeGameAfterFrame()
    {
        yield return null;

        // Đợi SaveLoadBootstrap load xong dữ liệu enemy đã giết
        yield return SaveLoadBootstrap.WaitForDataLoad();

        if (autoLoadOnStart && SaveSystem.Instance != null && SaveSystem.Instance.HasSaveFile())
        {
            LoadGame();
        }
        else
        {
            InitializeNewGame();
        }
    }

    #region Save/Load System

    private void InitializeNewGame()
    {
        score = 0;
        hasCheckpoint = false;

        if (player != null)
        {
            checkpointPosition = player.position;
        }

        UpdateScore();
    }

    public bool SaveGame()
    {
        if (SaveSystem.Instance == null)
            return false;

        GameData gameData = CollectCurrentGameData();
        return gameData != null && SaveSystem.Instance.SaveGame(gameData);
    }

    public bool LoadGame()
    {
        if (SaveSystem.Instance == null)
            return false;

        GameData gameData = SaveSystem.Instance.LoadGame();

        if (gameData == null)
        {
            InitializeNewGame();
            return false;
        }

        // Kiểm tra xem có cần chuyển scene không
        string currentScene = SceneManager.GetActiveScene().name;
        if (!string.IsNullOrEmpty(gameData.currentScene) && currentScene != gameData.currentScene)
        {
            // Scene khác nhau, SceneLoadManager sẽ xử lý việc chuyển scene
            // Không apply data ở đây, sẽ apply sau khi chuyển scene
            Debug.Log($"Scene mismatch: current={currentScene}, saved={gameData.currentScene}");
            return true;
        }

        // Cùng scene, apply data ngay
        ApplyLoadedGameData(gameData);
        return true;
    }

    /// <summary>
    /// Load game data mà không kiểm tra scene (dùng sau khi đã chuyển scene đúng)
    /// </summary>
    public bool LoadGameDataOnly()
    {
        if (SaveSystem.Instance == null)
            return false;

        GameData gameData = SaveSystem.Instance.LoadGame();

        if (gameData == null)
        {
            InitializeNewGame();
            return false;
        }

        ApplyLoadedGameData(gameData);
        return true;
    }

    private GameData CollectCurrentGameData()
    {
        try
        {
            Vector3 playerPos = player != null ? player.position : Vector3.zero;
            int playerHealth = GetPlayerHealth();
            string currentScene = SceneManager.GetActiveScene().name;
            List<string> killedEnemies = SaveSystem.Instance != null ? SaveSystem.Instance.GetEnemyKilledList() : new List<string>();

            return new GameData(playerPos, playerHealth, score, currentScene, checkpointPosition, hasCheckpoint, killedEnemies);
        }
        catch
        {
            return null;
        }
    }

    private void ApplyLoadedGameData(GameData data)
    {
        try
        {
            score = data.currentScore;
            UpdateScore();

            checkpointPosition = data.checkpointPosition;
            hasCheckpoint = data.hasCheckpoint;

            if (player != null)
            {
                player.position = data.playerPosition;
            }

            SetPlayerHealth(data.playerHealth);

            // Áp dụng danh sách Enemy đã bị giết
            if (SaveSystem.Instance != null)
            {
                SaveSystem.Instance.SetEnemyKilledList(data.enemyKilledList);
            }
        }
        catch
        {
            // Ignore errors
        }
    }

    private int GetPlayerHealth()
    {
        PlayerHealth playerHealth = FindFirstObjectByType<PlayerHealth>();
        if (playerHealth != null)
            return playerHealth.GetCurrentHealth();

        if (player != null)
        {
            PlayerHealth playerHealthOnPlayer = player.GetComponent<PlayerHealth>();
            if (playerHealthOnPlayer != null)
                return playerHealthOnPlayer.GetCurrentHealth();
        }

        return 3; // Default health
    }

    private void SetPlayerHealth(int health)
    {
        PlayerHealth playerHealth = FindFirstObjectByType<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.SetHealth(health);
            return;
        }

        if (player != null)
        {
            PlayerHealth playerHealthOnPlayer = player.GetComponent<PlayerHealth>();
            if (playerHealthOnPlayer != null)
                playerHealthOnPlayer.SetHealth(health);
        }
    }

    public void ResetForNewGame()
    {
        if (SaveSystem.Instance != null)
        {
            SaveSystem.Instance.DeleteSave();
            SaveSystem.Instance.ClearEnemyKilledList(); // Reset danh sách enemy đã giết
        }

        // Reset flag của SaveLoadBootstrap
        SaveLoadBootstrap.ResetDataLoadedFlag();

        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        InitializeNewGame();

        // Load về scene mặc định nếu cần
        if (SceneLoadManager.Instance != null && !SceneLoadManager.Instance.IsInDefaultScene())
        {
            SceneLoadManager.Instance.LoadDefaultScene();
        }
    }

    #endregion

    // Function để reset tất cả coin (chỉ dùng khi cần thiết)
    private void ResetAllCoins()
    {
        // Xóa tất cả coin keys
        GameObject[] coins = GameObject.FindGameObjectsWithTag("Coin");
        foreach (GameObject coin in coins)
        {
            string coinID = "Coin_" + coin.transform.position.x.ToString("F1") + "_" + coin.transform.position.y.ToString("F1");
            PlayerPrefs.DeleteKey(coinID);
        }

        PlayerPrefs.Save();
        Debug.Log("All coins reset");
    }


}
