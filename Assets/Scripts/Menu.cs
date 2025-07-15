using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public void PlayGame()
    {
        // Reset HOÀN TOÀN cho game mới (xóa save file)
        if (SaveSystem.Instance != null)
        {
            SaveSystem.Instance.DeleteSave();
        }

        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        Debug.Log("PlayGame: Complete reset for new game");

        SceneManager.LoadScene("Game");
    }

    public void ContinueGame()
    {
        // Kiểm tra xem có save file không
        if (SaveSystem.Instance != null && SaveSystem.Instance.HasSaveFile())
        {
            Debug.Log("Continue game: Loading saved game");
            SceneManager.LoadScene("Game"); // GameManager sẽ tự động load
        }
        else
        {
            Debug.Log("No save file found! Starting new game...");
            SceneManager.LoadScene("Game");
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    // Function để reset coin (cực nhanh)
    private void ResetAllCoins()
    {
        // Cách siêu nhanh: tạo session ID mới
        int newSessionID = Random.Range(1000, 9999);
        PlayerPrefs.SetInt("GameSessionID", newSessionID);
        PlayerPrefs.Save();
        Debug.Log("New game session started: " + newSessionID);
    }
}
