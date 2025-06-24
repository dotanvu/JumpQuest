using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    private GameManager gameManager;
    private AudioManager audioManager;

    private void Awake()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        audioManager = FindAnyObjectByType<AudioManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Kiểm tra gameManager và audioManager có tồn tại không
        if (gameManager == null)
        {
            gameManager = FindAnyObjectByType<GameManager>();
        }
        if (audioManager == null)
        {
            audioManager = FindAnyObjectByType<AudioManager>();
        }

        // Kiểm tra va chạm với Coin
        if (collision.CompareTag("Coin"))
        {
            Destroy(collision.gameObject);
            if (audioManager != null) audioManager.PlayCoinSound();
            if (gameManager != null) gameManager.AddScore(1);
        }
        // Kiểm tra va chạm với Trap
        else if (collision.CompareTag("Trap"))
        {
            PlayerHealth ph = FindFirstObjectByType<PlayerHealth>();
            if (ph != null) ph.TakeDamage(1);
        }
        // Kiểm tra va chạm với Enemy
        else if (collision.CompareTag("Enemy"))
        {
            PlayerHealth ph = FindFirstObjectByType<PlayerHealth>();
            if (ph != null) ph.TakeDamage(1);
        }
        // Kiểm tra va chạm với Key
        else if (collision.CompareTag("Key"))
        {
            Destroy(collision.gameObject);
            if (gameManager != null) gameManager.GameWin();
        }
    }
}
