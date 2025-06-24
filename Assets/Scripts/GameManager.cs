using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

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

    void Start()
    {
        isGameOver = false;
        isGameWin = false;
        Time.timeScale = 1;

        UpdateScore();
        if (gameOverUi != null) gameOverUi.SetActive(false);
        if (gameWinUi != null) gameWinUi.SetActive(false);

        if (SceneManager.GetActiveScene().name == "Game")
        {
            score = 0;
            UpdateScore();
            hasCheckpoint = false;
        }

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }

        if (player != null)
        {
            checkpointPosition = player.position;
        }
    }

    public void AddScore(int points)
    {
        if (!isGameOver && !isGameWin)
        {
            score += points;
            UpdateScore();
        }
    }

    private void UpdateScore()
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
        }
    }

    public void GameWin()
    {
        isGameWin = true;
        Time.timeScale = 0;

        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene == "Game")
        {
            Time.timeScale = 1;
            SceneManager.LoadScene("Map2");
        }
        else if (currentScene == "Map2")
        {
            Time.timeScale = 1;
            SceneManager.LoadScene("Map3");
        }
        else if (currentScene == "Map3")
        {
            if (gameWinUi != null)
            {
                gameWinUi.SetActive(true);
            }
        }
    }

    public void RestartGame()
    {
        isGameOver = false;
        isGameWin = false;
        score = 0;
        hasCheckpoint = false;
        Time.timeScale = 1;

        if (gameOverUi != null) gameOverUi.SetActive(false);
        if (gameWinUi != null) gameWinUi.SetActive(false);

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
}
