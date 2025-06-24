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
        SceneManager.LoadScene("Game");
    }

    public void GoToMenu()
    {
        isPaused = false;
        Time.timeScale = 1;
        SceneManager.LoadScene("Menu");
    }
}
