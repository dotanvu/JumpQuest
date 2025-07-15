using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseController : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    private bool isPaused = false;

    void Start()
    {
        // Đảm bảo khi bắt đầu game, PauseMenu tắt
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0;
            if (pauseMenu != null) pauseMenu.SetActive(true);
        }
        else
        {
            Time.timeScale = 1;
            if (pauseMenu != null) pauseMenu.SetActive(false);
        }
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1;
        if (pauseMenu != null) pauseMenu.SetActive(false);
    }

    public void RestartGame()
    {
        isPaused = false;
        Time.timeScale = 1;

        // Reset hoàn toàn cho game mới
        ResetForNewGame();

        Debug.Log("PauseController RestartGame: Complete reset for new game");

        SceneManager.LoadScene("Game");
    }

    public void GoToMenu()
    {
        isPaused = false;
        Time.timeScale = 1;
        SceneManager.LoadScene("Menu");
    }

    public void SaveGame()
    {
        GameManager gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager != null)
        {
            gameManager.SaveGame();
        }
    }

    public void SaveAndGoToMenu()
    {
        SaveGame();
        isPaused = false;
        Time.timeScale = 1;
        SceneManager.LoadScene("Menu");
    }

    private void ResetForNewGame()
    {
        GameManager gameManager = FindFirstObjectByType<GameManager>();
        if (gameManager != null)
        {
            gameManager.ResetForNewGame();
        }
        else
        {
            if (SaveSystem.Instance != null)
            {
                SaveSystem.Instance.DeleteSave();
            }
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
        }
    }
}
