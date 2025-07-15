using UnityEngine;

public class PlayerCollision : MonoBehaviour
{
    private GameManager gameManager;
    private AudioManager audioManager;
    private Rigidbody2D playerRb;

    private void Awake()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        audioManager = FindAnyObjectByType<AudioManager>();
        playerRb = GetComponent<Rigidbody2D>();
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
            // Tạo ID đơn giản dựa trên vị trí
            string coinID = "Coin_" + collision.transform.position.x.ToString("F1") + "_" + collision.transform.position.y.ToString("F1");

            // Lưu coin đã thu thập
            PlayerPrefs.SetInt(coinID, 1);
            PlayerPrefs.Save();

            // Ẩn coin
            collision.gameObject.SetActive(false);

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
            HandleEnemyCollision(collision);
        }
        // Kiểm tra va chạm với Key
        else if (collision.CompareTag("Key"))
        {
            Destroy(collision.gameObject);
            if (gameManager != null) gameManager.GameWin();
        }
    }

    private void HandleEnemyCollision(Collider2D enemyCollider)
    {
        // Kiểm tra xem player có đang rơi xuống không
        if (playerRb != null && playerRb.linearVelocity.y < 0)
        {
            // Player đang rơi xuống - nhảy lên đầu quái
            Enemy enemy = enemyCollider.GetComponent<Enemy>();
            if (enemy != null)
            {
                // Giết quái
                enemy.Die();

                // Player bounce lên
                playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, 10f);

                // Phát âm thanh nếu có
                if (audioManager != null)
                {
                    audioManager.PlayCoinSound(); // Có thể thay bằng âm thanh giết quái
                }
            }
        }
        else
        {
            // Player không rơi xuống - nhận sát thương
            PlayerHealth playerHealth = FindFirstObjectByType<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(1);
            }
        }
    }
}
