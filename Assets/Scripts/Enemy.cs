using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float speed = 2f;
    [SerializeField] private float distance = 5f;
    [SerializeField] private float deathDelay = 0.6f;

    private Vector3 startPos;
    private bool movingRight = true;
    private bool isDead = false;
    private string enemyID;
    private bool hasCheckedKillStatus = false;

    private Animator animator;
    private Collider2D enemyCollider;
    void Awake()
    {
        // Gán ID duy nhất cho Enemy trong Awake()
        GenerateEnemyID();
    }

    void Start()
    {
        startPos = transform.position;
        animator = GetComponent<Animator>();
        enemyCollider = GetComponent<Collider2D>();

        // Bắt đầu coroutine để đợi dữ liệu save load xong
        StartCoroutine(WaitAndCheckKillStatus());
    }

    void Update()
    {
        if (isDead) return;

        float leftBound = startPos.x - distance;
        float rightBound = startPos.x + distance;
        if (movingRight)
        {
            transform.Translate(Vector2.right * speed * Time.deltaTime);
            if (transform.position.x >= rightBound)
            {
                movingRight = false;
                Flip();
            }
        }
        else
        {
            transform.Translate(Vector2.left * speed * Time.deltaTime);
            if (transform.position.x <= leftBound)
            {
                movingRight = true;
                Flip();
            }
        }
    }

    void Flip()
    {
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;
    }



    public void Die()
    {
        if (isDead) return;

        isDead = true;

        // Thêm enemy ID vào danh sách đã bị giết
        if (SaveSystem.Instance != null && !string.IsNullOrEmpty(enemyID))
        {
            SaveSystem.Instance.AddKilledEnemy(enemyID);

            // Gọi SaveGame() để lưu ngay lập tức
            GameManager gameManager = FindFirstObjectByType<GameManager>();
            if (gameManager != null)
            {
                gameManager.SaveGame();
            }
        }

        // Phát animation chết
        if (animator != null)
        {
            animator.SetTrigger("IsDead");
        }

        // Disable collider để không va chạm thêm
        if (enemyCollider != null)
        {
            enemyCollider.enabled = false;
        }

        // Destroy enemy sau delay để animation phát xong
        Destroy(gameObject, deathDelay);
    }

    private void GenerateEnemyID()
    {
        // Tạo ID duy nhất dựa vào vị trí, ví dụ "Enemy_12.3_5.8"
        float x = Mathf.Round(transform.position.x * 10f) / 10f;
        float y = Mathf.Round(transform.position.y * 10f) / 10f;
        enemyID = $"Enemy_{x:F1}_{y:F1}";
    }

    private System.Collections.IEnumerator WaitAndCheckKillStatus()
    {
        // Đợi cho đến khi SaveLoadBootstrap load xong dữ liệu
        yield return SaveLoadBootstrap.WaitForDataLoad();

        // Bây giờ mới kiểm tra trạng thái đã bị giết
        CheckIfAlreadyKilled();
    }

    private void CheckIfAlreadyKilled()
    {
        if (hasCheckedKillStatus) return;
        hasCheckedKillStatus = true;

        // Mỗi Enemy trong scene check nếu ID của mình nằm trong danh sách thì tự Destroy() ngay lập tức
        if (SaveSystem.Instance != null && !string.IsNullOrEmpty(enemyID))
        {
            if (SaveSystem.Instance.IsEnemyKilled(enemyID))
            {
                // Enemy đã bị giết, destroy ngay lập tức, không spawn
                Destroy(gameObject);
            }
        }
    }

    /// <summary>
    /// Được gọi bởi SaveLoadBootstrap khi dữ liệu đã sẵn sàng
    /// </summary>
    public void OnSaveDataReady()
    {
        if (!hasCheckedKillStatus)
        {
            CheckIfAlreadyKilled();
        }
    }


}
