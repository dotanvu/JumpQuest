using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;

    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    public Transform startPoint; // Object SpawnPoint

    private bool isDead = false;
    private float invulnerableTime = 0.5f; // miễn thương 0.5s
    private float invulnerableTimer = 0f;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHearts();
        isDead = false;
        invulnerableTimer = 0f;
    }

    void Update()
    {
        if (invulnerableTimer > 0)
        {
            invulnerableTimer -= Time.deltaTime;
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead || invulnerableTimer > 0f) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHearts();

        invulnerableTimer = invulnerableTime; // bật iframe

        GameManager gm = FindFirstObjectByType<GameManager>();
        if (gm != null && gm.player != null)
        {
            if (currentHealth > 0)
            {
                if (gm.hasCheckpoint)
                {
                    gm.player.position = gm.checkpointPosition;
                }
                else
                {
                    if (startPoint != null)
                    {
                        gm.player.position = startPoint.position;
                    }
                    else
                    {
                        Debug.LogWarning("StartPoint chưa gán!");
                        gm.player.position = new Vector3(0, 0, 0);
                    }
                }
            }
            else
            {
                Die();
            }
        }
    }

    void UpdateHearts()
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < currentHealth)
            {
                hearts[i].sprite = fullHeart;
            }
            else
            {
                hearts[i].sprite = emptyHeart;
            }
        }
    }

    void Die()
    {
        if (isDead) return;

        isDead = true;
        Debug.Log("Player died");

        // Tách Player ra khỏi platform (nếu đang bị SetParent)
        if (transform.parent != null)
        {
            transform.SetParent(null);
        }

        GameManager gm = FindFirstObjectByType<GameManager>();
        if (gm != null)
        {
            gm.GameOver();

            // Dừng game
            Time.timeScale = 0;
        }
    }
}
